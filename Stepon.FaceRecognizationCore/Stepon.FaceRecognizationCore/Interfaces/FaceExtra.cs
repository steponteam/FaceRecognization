/*********************************************************
 * author：Gavin
 * created：2017-12-21
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Common;
using Stepon.FaceRecognizationCore.Exceptions;
using Stepon.FaceRecognizationCore.Extensions;
using Stepon.FaceRecognizationCore.Tracking;

namespace Stepon.FaceRecognizationCore.Interfaces
{
    public abstract class FaceExtra : FaceBase
    {
        protected FaceExtra(string appId, string sdkKey, int preAllocMemSize = 31457280) : base(appId, sdkKey,
            preAllocMemSize)
        {
        }

        /// <summary>
        ///     根据定位器自动进行选择评估
        /// </summary>
        /// <param name="locator">人脸定位器</param>
        /// <param name="image">要识别的图片</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含额外信息的位置</returns>
        public LocateResult Estimation(FaceLocator locator, Bitmap image, bool autoDispose = true)
        {
            if (locator is FaceTracking)
                return PreviewEstimation(locator, image, autoDispose);

            return StaticEstimation(locator, image, autoDispose);
        }

        /// <summary>
        ///     根据定位器自动进行选择评估
        /// </summary>
        /// <param name="locator">人脸定位器</param>
        /// <param name="imageData">图片原始数据（不包含头信息）</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含额外信息的位置</returns>
        public LocateResult Estimation(FaceLocator locator, byte[] imageData, int width, int height,
            bool autoDispose = true)
        {
            if (locator is FaceTracking)
                return PreviewEstimation(locator, imageData, width, height, autoDispose);

            return StaticEstimation(locator, imageData, width, height, autoDispose);
        }

        /// <summary>
        ///     通用的评估函数
        /// </summary>
        /// <param name="locate"></param>
        /// <param name="estimation"></param>
        /// <param name="autoDispose"></param>
        /// <returns>包含额外信息的位置</returns>
        internal LocateResult Estimation(LocateResult locate, Action<ExtraFaceInput> estimation, bool autoDispose = true)
        {
            if (!locate.HasFace)
                return locate;

            var faceInput = new ExtraFaceInput
            {
                lFaceNumber = locate.FaceCount,
                pFaceRectArray = IntPtrExtensions.StructArrayToPtr(locate.Faces),
                pFaceOrientArray =
                    IntPtrExtensions.StructArrayToPtr(locate.FacesOrient.Select(code => (int)code).ToArray())
            };

            try
            {
                estimation(faceInput);

                return locate;
            }
            finally
            {
                if (autoDispose)
                    locate.Dispose();

                Marshal.FreeHGlobal(faceInput.pFaceRectArray);
                Marshal.FreeHGlobal(faceInput.pFaceOrientArray);
            }
        }

        /// <summary>
        ///     获得识别引擎版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        public abstract SdkVersion GetVersion();

        internal delegate ExtraFaceResult Estimate(ExtraFaceInput input, out ErrorCode code);

        #region 静态图片检测

        /// <summary>
        ///     静态图片的评估
        /// </summary>
        /// <param name="locator">人脸定位器，请采用FaceDetection</param>
        /// <param name="image">要识别的图片</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含额外信息的位置</returns>
        public LocateResult StaticEstimation(FaceLocator locator, Bitmap image, bool autoDispose = true)
        {
            var retCode = locator.Detect(image, out var result);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
            return StaticEstimation(result, autoDispose);
        }

        /// <summary>
        ///     静态图片的评估
        /// </summary>
        /// <param name="locator">人脸定位器，请采用FaceDetection</param>
        /// <param name="imageData">图片原始数据（不包含头信息）</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <param name="pixelSize">像素大小</param>
        /// <returns>包含额外信息的位置</returns>
        public LocateResult StaticEstimation(FaceLocator locator, byte[] imageData, int width, int height,
            bool autoDispose = true, int pixelSize = 3)
        {
            var retCode = locator.Detect(imageData, width, height, out var result, pixelSize);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
            return StaticEstimation(result, autoDispose);
        }

        /// <summary>
        ///     静态图片的评估
        /// </summary>
        /// <param name="locate">已经识别的人脸位置信息（图片数据未释放，如果释放，将出现异常）</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含年龄的位置信息</returns>
        public abstract LocateResult StaticEstimation(LocateResult locate, bool autoDispose = true);

        #endregion

        #region 序列帧图片检测

        /// <summary>
        ///     序列帧图片的评估
        /// </summary>
        /// <param name="locator">人脸定位器，请采用FaceTracking</param>
        /// <param name="image">要识别的图片</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含额外信息的位置</returns>
        public LocateResult PreviewEstimation(FaceLocator locator, Bitmap image, bool autoDispose = true)
        {
            var retCode = locator.Detect(image, out var result);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
            return PreviewEstimation(result, autoDispose);
        }

        /// <summary>
        ///     序列帧图片的评估
        /// </summary>
        /// <param name="locator">人脸定位器，请采用FaceTracking</param>
        /// <param name="imageData">图片原始数据（不包含头信息）</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <param name="pixelSize"></param>
        /// <returns>包含额外信息的位置</returns>
        public LocateResult PreviewEstimation(FaceLocator locator, byte[] imageData, int width, int height,
            bool autoDispose = true, int pixelSize = 3)
        {
            var retCode = locator.Detect(imageData, width, height, out var result, pixelSize);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
            return PreviewEstimation(result, autoDispose);
        }

        /// <summary>
        ///     序列帧图片的评估
        /// </summary>
        /// <param name="locate">已经识别的人脸位置信息（图片数据未释放，如果释放，将出现异常）</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含额外信息的位置</returns>
        public abstract LocateResult PreviewEstimation(LocateResult locate, bool autoDispose = true);

        #endregion
    }
}