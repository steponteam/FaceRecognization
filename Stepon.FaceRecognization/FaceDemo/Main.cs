using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NReco.VideoConverter;
using Stepon.FaceRecognization;
using Stepon.FaceRecognization.Common;
using Stepon.FaceRecognization.Detection;
using Stepon.FaceRecognization.Recognization;
using Stepon.FaceRecognization.Tracking;

namespace FaceDemo
{
    public partial class Main : Form
    {
        private const string AppId = "";
        private const string FtKey = "";
        private const string FdKey = "";
        private const string FrKey = "";

        private const string FaceLibraryPath = "faces";

        private static readonly object _imageLock = new object();

        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        private FaceDetection _detection; //用于静态图片抽取特征

        private Bitmap _image;
        private IntPtr _pImage;
        private FaceProcessor _processor;
        private FaceRecognize _recognize;
        private bool _renderRunning = true;

        private Task _renderTask;

        private bool _shouldShot;
        private FaceTracking _traking; //用于视频流检测人脸

        private MemoryStream outputStream;
        private ConvertLiveMediaTask task;

        //视频图形信息
        private int width = 1920;
        private int height = 1080;
        private int pixelSize = 3;
        private int stride;
        private int bufferSize;

        public Main()
        {
            InitializeComponent();
            Init();
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

            stride = width * pixelSize;
            bufferSize = stride * height;

            _pImage = Marshal.AllocHGlobal(bufferSize);
            _image = new Bitmap(width, height, stride, PixelFormat.Format24bppRgb, _pImage);

            var ffmpeg = new FFMpegConverter();
            outputStream = new MemoryStream();

            var setting =
                new ConvertSettings
                {
                    CustomOutputArgs = "-an -r 15 -pix_fmt bgr24 -updatefirst 1" //根据业务需求-r参数可以调整，取决于摄像机的FPS
                }; //-s 1920x1080 -q:v 2 -b:v 64k

            task = ffmpeg.ConvertLiveMedia("rtsp://user:password@192.168.1.64:554/h264/ch1/main/av_stream", null,
                outputStream, Format.raw_video, setting);

            /*
             * USB摄像头捕获
             * 通过ffmpeg可以捕获USB摄像，如下代码所示。
             * 首先通过：ffmpeg -list_devices true -f dshow -i dummy命令，可以列出系统中存在的USB摄像设备（或通过控制面板的设备管理工具查看设备名称），例如在我电脑中叫USB2.0 PC CAMERA。
             * 然后根据捕获的分辨率，修改视频图形信息，包括width和height，一般像素大小不用修改，如果要参考设备支持的分辨率，可以使用：
             * ffmpeg -list_options true -f dshow -i video="USB2.0 PC CAMERA"命令
             */
            //task = ffmpeg.ConvertLiveMedia("video=USB2.0 PC CAMERA", "dshow",
            //    outputStream, Format.raw_video, setting);

            task.OutputDataReceived += DataReceived;
            task.Start();

            _renderTask = new Task(Render);
            _renderTask.Start();
        }

        private void DataReceived(object sender, EventArgs e)
        {
            if (outputStream.Position == bufferSize) //1920*1080*3,stride * width,取决于图片的大小和像素格式
                lock (_imageLock)
                {
                    var data = outputStream.ToArray();

                    Marshal.Copy(data, 0, _pImage, data.Length);//直接替换在内存中的位图数据，以提升处理效率

                    outputStream.Seek(0, SeekOrigin.Begin);
                }
        }

        private void Render()
        {
            while (_renderRunning)
            {
                if (_image == null)
                    continue;

                Bitmap image;

                lock (_imageLock)
                {
                    image = (Bitmap)_image.Clone();
                }

                if (_shouldShot)
                {
                    WriteFeature(image);
                    _shouldShot = false;
                }

                Verify(image);

                if (videoImage.InvokeRequired)
                    videoImage.Invoke(new Action(() => { videoImage.Image = image; }));
                else
                    videoImage.Image = image;
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
            _renderRunning = false;

            task.Stop(true);

            _traking.Dispose();
            _detection.Dispose();
            _recognize.Dispose();

            Marshal.FreeHGlobal(_pImage);

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

            var openFile = new OpenFileDialog { Multiselect = false };
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