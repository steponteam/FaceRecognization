using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Stepon.FaceRecognization;
using Stepon.FaceRecognization.Common;
using Stepon.FaceRecognization.Detection;
using Stepon.FaceRecognization.Recognization;
using Stepon.FaceRecognization.Tracking;

namespace FaceDemo
{
    public partial class EmguDemo : Form
    {
        private const string AppId = "";
        private const string FtKey = "";
        private const string FdKey = "";
        private const string FrKey = "";

        private const string FaceLibraryPath = "faces";

        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        private VideoCapture _capture;

        private FaceDetection _detection; //用于静态图片抽取特征
        private readonly long _fps = 1000 / 30;
        private bool _isRunning = true;
        private FaceProcessor _processor;

        private Mat _receivedImage = new Mat();
        private FaceRecognize _recognize;

        private Task _run;

        private bool _shouldShot;
        private FaceTracking _traking; //用于视频流检测人脸
        private readonly Stopwatch _watch = new Stopwatch();

        public EmguDemo()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        ///     获取RTSP流视频
        /// </summary>
        /// <returns></returns>
        private VideoCapture GetRtspStream()
        {
            return new VideoCapture("rtsp://user:password@192.168.1.64:554/h264/ch1/main/av_stream");
        }

        /// <summary>
        ///     从USB摄像头获取视频流
        /// </summary>
        /// <returns></returns>
        private VideoCapture GetWebCamera()
        {
            return new VideoCapture(0) {FlipHorizontal = true};
        }

        private void Init()
        {
            _traking = LocatorFactory.GetTrackingLocator(AppId, FtKey) as FaceTracking;
            _detection = LocatorFactory.GetDetectionLocator(AppId, FdKey) as FaceDetection;
            _recognize = new FaceRecognize(AppId, FrKey);
            _processor = new FaceProcessor(_traking, _recognize);

            //init cache
            if (Directory.Exists(FaceLibraryPath))
            {
                var files = Directory.GetFiles(FaceLibraryPath);
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    _cache.Add(info.Name.Replace(info.Extension, ""), File.ReadAllBytes(file));
                }
            }

            CvInvoke.UseOpenCL = false;

            _capture = GetWebCamera();
            _capture.Start();

            //Application.Idle += VideoCaptured; //可以采用此方式捕获视频，则无需单独开线程
            //_capture.ImageGrabbed += VideoCaptured; //不要采用此方式
            _run = new Task(VideoCaptured);
            _run.Start();
        }

        private void VideoCaptured() //object sender, EventArgs e
        {
            while (_isRunning)
            {
                _watch.Restart();
                if (_capture != null && _capture.Ptr != IntPtr.Zero)
                    try
                    {
                        if (_receivedImage == null || _receivedImage.Ptr == IntPtr.Zero)
                            _receivedImage = new Mat();
                        if (_capture.Retrieve(_receivedImage))
                        {
                            if (_shouldShot)
                            {
                                WriteFeature(_receivedImage.Bitmap);
                                _shouldShot = false;
                            }

                            //CvInvoke.Resize(_receivedImage, _resizedImage,new Size(_receivedImage.Width / 2, _receivedImage.Height / 2));

                            Verify(_receivedImage.Bitmap);
                            //videoImage.Image?.Dispose();
                            //videoImage.Image = resizedImage.Bitmap;
                            videoImage.Image = _receivedImage.Bitmap;
                        }
                    }
                    catch (Exception error)
                    {
                        // 需要处理网络异常，内存异常等
                        // ignored
                        File.AppendAllText("log.txt", $"{error.Message}{Environment.NewLine}");
                    }

                _watch.Stop();
                var runtime = _watch.ElapsedMilliseconds;
                if (runtime < _fps)
                    Thread.Sleep((int) (_fps - runtime));
            }
        }

        private void Verify(Bitmap bitmap)
        {
            var features = _processor.LocateExtract(bitmap);
            if (features != null)
            {
                var names = MatchAll(features);
                var index = 0;

                using (var g = Graphics.FromImage(bitmap))
                {
                    foreach (var feature in features)
                    {
                        g.DrawRectangle(new Pen(Color.Crimson, 2), feature.FaceLoaction);
                        g.DrawString(names[index], new Font(new FontFamily("SimSun"), 12),
                            new SolidBrush(Color.Crimson), feature.FaceLoaction.Right, feature.FaceLoaction.Bottom);

                        feature.Dispose(); //feature中的特征数据从托管内存复制到非托管，必须释放，否则内存泄露

                        index++;
                    }
                }
            }
        }

        private string[] MatchAll(Feature[] features)
        {
            var result = Enumerable.Repeat("陌生人", features.Length).ToArray();

            if (_cache.Count == 0)
                return result;

            //依次处理找到的人脸
            var max = new float[features.Length];

            try
            {
                _cacheLock.EnterReadLock();

                foreach (var single in _cache)
                    for (var i = 0; i < features.Length; i++)
                    {
                        var sim = _processor.Match(features[i].FeatureData,
                            single.Value); //此方法默认保留采集到的特征（非托管内存），并自动释放被比较（特征库）的特征数据，所以无需担心内存泄露
                        if (sim > 0.5)
                            if (sim > max[i])
                            {
                                max[i] = sim;
                                result[i] = single.Key;
                            }
                    }
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }


            return result;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _isRunning = false;
            _run.Wait();

            _traking.Dispose();
            _detection.Dispose();
            _recognize.Dispose();

            if (_capture.IsOpened)
                _capture.Stop();

            _capture.Dispose(); //这里执行不时出现异常，还未找到解决方案

            _receivedImage.Dispose();

            base.OnClosing(e);
        }

        private void WriteFeature(Bitmap bitmap)
        {
            var code = _detection.Detect(bitmap, out var locate);
            try
            {
                if (code == ErrorCode.Ok)
                {
                    if (locate.HasFace)
                    {
                        if (locate.FaceCount > 1)
                        {
                            MessageBox.Show("选定的图片检测到多余一张的人脸，请重新选择");
                            return;
                        }

                        var name = userIdentity.Text;

                        using (var feature =
                            _recognize.ExtractFeature(locate.OffInput, locate.Faces[0], locate.FacesOrient[0]))
                        {
                            if (!Directory.Exists(FaceLibraryPath))
                                Directory.CreateDirectory(FaceLibraryPath);

                            File.WriteAllBytes(Path.Combine(FaceLibraryPath, $"{name}.dat"), feature.FeatureData);

                            try
                            {
                                _cacheLock.EnterWriteLock();
                                _cache.Add(name, feature.FeatureData);
                            }
                            finally
                            {
                                _cacheLock.ExitWriteLock();
                            }
                        }

                        userIdentity.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show(code.ToString());
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            finally
            {
                locate.Dispose();
            }

            MessageBox.Show("建立特征成功");
        }

        /// <summary>
        ///     选择静态照片进行特征抽取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImageExtractClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(userIdentity.Text))
            {
                MessageBox.Show("请先输入用户标识");
                return;
            }

            var openFile = new OpenFileDialog {Multiselect = false};
            var result = openFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                var file = openFile.FileName;
                WriteFeature(new Bitmap(Image.FromFile(file)));
            }
        }

        /// <summary>
        ///     从视频流中抓取照片进行特征抽取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShotExtractClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(userIdentity.Text))
            {
                MessageBox.Show("请先输入用户标识");
                return;
            }
            _shouldShot = true;
        }
    }
}