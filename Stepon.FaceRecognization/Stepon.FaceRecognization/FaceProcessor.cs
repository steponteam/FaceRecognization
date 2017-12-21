/*********************************************************
 * author：Gavin
 * created：2017-8-15
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Drawing;
using Stepon.FaceRecognization.Common;
using Stepon.FaceRecognization.Exceptions;
using Stepon.FaceRecognization.Interfaces;
using Stepon.FaceRecognization.Recognization;

namespace Stepon.FaceRecognization
{
    /// <summary>
    ///     面部识别的综合处理器，主要用于检测加特征抽取，便于快速安全的操作数据
    /// </summary>
    public class FaceProcessor : IDisposable
    {
        private readonly FaceLocator _locator;
        private readonly FaceRecognize _recognize;

        /// <summary>
        ///     采用指定的定位和识别初始化
        /// </summary>
        /// <param name="locator">定位器</param>
        /// <param name="recognize">识别器</param>
        public FaceProcessor(FaceLocator locator, FaceRecognize recognize)
        {
            _locator = locator;
            _recognize = recognize;

            if (!_locator.IsIntialized)
                _locator.Initialize();
        }

        /// <summary>
        ///     采用默认参数的跟踪定位和识别进行初始化
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="locatorKey">定位Key</param>
        /// <param name="recognizeKey">识别Key</param>
        /// <param name="useTracking">是否使用跟踪</param>
        public FaceProcessor(string appId, string locatorKey, string recognizeKey, bool useTracking = false)
        {
            _locator = useTracking
                ? LocatorFactory.GetTrackingLocator(appId, locatorKey)
                : LocatorFactory.GetDetectionLocator(appId, locatorKey);
            _recognize = new FaceRecognize(appId, recognizeKey);
        }

        /// <summary>
        ///     释放相关资源
        /// </summary>
        public void Dispose()
        {
            _locator?.Dispose();
            _recognize?.Dispose();
        }

        /// <summary>
        ///     定位并抽取特征数据
        /// </summary>
        /// <param name="image">待处理的图像</param>
        /// <param name="operation">抽取操作</param>
        /// <returns>特征数据数组</returns>
        public Feature[] LocateExtract(Bitmap image, LocateOperation operation = LocateOperation.None)
        {
            var code = _locator.Detect(image, out var location, operation);
            if (code != ErrorCode.Ok)
                throw new FaceException(code);

            if (!location.HasFace)
            {
                //释放资源
                location.Dispose();
                return null;
            }
            ;

            var features = _recognize.ExtractFeatures(location);
            return features;
        }

        /// <summary>
        ///     定位并抽取特征数据
        /// </summary>
        /// <param name="imageData">待处理的图像数据</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="pixelSize">像素大小</param>
        /// <param name="operation">抽取操作</param>
        /// <returns>特征数据数组</returns>
        public Feature[] LocateExtract(byte[] imageData, int width, int height, int pixelSize = 3, LocateOperation operation = LocateOperation.None)
        {
            var code = _locator.Detect(imageData, width, height, out var location, pixelSize, operation);
            if (code != ErrorCode.Ok)
                throw new FaceException(code);

            if (!location.HasFace)
            {
                //释放资源
                location.Dispose();
                return null;
            }
            ;

            var features = _recognize.ExtractFeatures(location);
            return features;
        }

        /// <summary>
        ///     对两个特征数据进行比较
        /// </summary>
        /// <param name="origin">原始特征数据，一般为实时监测到的特征数据</param>
        /// <param name="toMatch">要比较的特征数据，一般为人脸库中的特征数据</param>
        /// <param name="autoFreeOrigin">是否自动释放原始特征数据，一般情况下，用于1:N的比较，原始特征数据不进行自动释放，以提升处理效率，在循环比较完成后，进行释放</param>
        /// <param name="autoFreeMatch">是否自动释放待比较的特征数据，一般情况下，可以直接释放</param>
        /// <returns>两个特征数据的相似度，一般大于0.5可以视为相似</returns>
        public float Match(FaceModel origin, FaceModel toMatch, bool autoFreeOrigin = false, bool autoFreeMatch = true)
        {
            return _recognize.Match(origin, toMatch, autoFreeOrigin, autoFreeMatch);
        }
    }
}