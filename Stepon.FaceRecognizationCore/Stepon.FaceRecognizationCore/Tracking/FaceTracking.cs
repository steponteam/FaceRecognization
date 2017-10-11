/*********************************************************
 * author：Gavin
 * created：2017-8-14
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Common;
using Stepon.FaceRecognizationCore.Exceptions;
using Stepon.FaceRecognizationCore.Extensions;
using Stepon.FaceRecognizationCore.Interfaces;
using Stepon.FaceRecognizationCore.Tracking.Wrapper;

namespace Stepon.FaceRecognizationCore.Tracking
{
    public class FaceTracking : FaceLocator
    {
        /// <summary>
        ///     初始化人脸跟踪
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="sdkKey">应用Key</param>
        /// <param name="autoInitWithDefaultParameter">如果设置为True，将自动采用默认参数初始化引擎</param>
        /// <param name="preAllocMemSize">缓存区内存大小（byte）</param>
        public FaceTracking(string appId, string sdkKey, bool autoInitWithDefaultParameter = true,
            int preAllocMemSize = 41943040) : base(appId, sdkKey, autoInitWithDefaultParameter, preAllocMemSize)
        {
        }

        /// <summary>
        ///     初始化引擎
        /// </summary>
        /// <param name="orientPriority">期望的脸部检测角度范围</param>
        /// <param name="scale">
        ///     用于数值表示的最小人脸尺寸有效值范围[2,16] 推荐值 16。该尺寸是人脸相对于所在图片的长边的占比。例如，如果用户想检测到的最小人脸尺寸是图片长度的 1/8，那么这个 nScale 就应该设置为
        ///     8
        /// </param>
        /// <param name="maxFaceNumber">用户期望引擎最多能检测出的人脸数有效值范围[1,20]</param>
        /// <returns>成功返回 MOK，否则返回失败 code</returns>
        public override ErrorCode Initialize(OrientPriority orientPriority = OrientPriority.OrientHigherExt,
            int scale = 16, int maxFaceNumber = 10)
        {
            if (scale < 2 || scale > 16)
                throw new ArgumentOutOfRangeException(nameof(scale), "有效的取值范围为2到16");

            if (maxFaceNumber < 1 || maxFaceNumber > 20)
                throw new ArgumentOutOfRangeException(nameof(maxFaceNumber), "有效的取值范围为1到20");

            IsIntialized = true;

            return (ErrorCode) TrackingWrapper.AFT_FSDK_InitialFaceEngine(AppId, SdkKey, Buffer, PreAllocMemSize,
                out Engine, (int) orientPriority, scale, maxFaceNumber);
        }

        /// <summary>
        ///     进行检测
        /// </summary>
        /// <param name="image">待检测的图像数据</param>
        /// <param name="result">检测结果</param>
        /// <returns>成功返回 MOK，否则返回失败 code</returns>
        public override ErrorCode Detect(Bitmap image, out LocateResult result)
        {
            var ret = ErrorCode.Ok;
            result = CommonOperation.OffInputOperation(image,
                (offInput, pImageData) => Detect(offInput, pImageData, out ret));

            return ret;
        }

        public override ErrorCode Detect(byte[] imageData, int width, int height, out LocateResult result)
        {
            var ret = ErrorCode.Ok;
            result = CommonOperation.OffInputOperation(imageData, width, height,
                (offInput, pImageData) => Detect(offInput, pImageData, out ret));

            return ret;
        }

        private LocateResult Detect(ImageData offInput, IntPtr pImageData, out ErrorCode ret)
        {
            var retCode =
                TrackingWrapper.AFT_FSDK_FaceFeatureDetect(Engine, ref offInput, out var pDetectResult);
            ret = (ErrorCode) retCode;
            if (ret == ErrorCode.Ok)
            {
                var nativeResult = pDetectResult.ToStruct<AFT_FSDK_FACERES>();
                var resolveResult = new LocateResult
                {
                    FaceCount = nativeResult.nFace,
                    FacesOrient = Enumerable.Repeat((OrientCode) nativeResult.lfaceOrient, nativeResult.nFace)
                        .ToArray()
                };

                resolveResult.Faces = nativeResult.rcFace.ToStructArray<FaceRect>(resolveResult.FaceCount);
                resolveResult.OffInput = offInput;
                resolveResult.ImageDataPtr = pImageData;

                return resolveResult;
            }

            return default(LocateResult);
        }

        /// <summary>
        ///     获得识别引擎版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        public override SdkVersion GetVersion()
        {
            var retPtr = TrackingWrapper.AFT_FSDK_GetVersion(Engine);
            var ret = Marshal.PtrToStructure<AFDT_FSDK_Version>(retPtr); //这里无法调用DestroyStructure
            var result = new SdkVersion
            {
                Codebase = ret.codebase,
                Major = ret.major,
                Minor = ret.minor,
                Build = ret.build,
                Version = ret.version,
                BuildDate = ret.buildDate,
                Copyright = ret.copyright
            };

            return result;
        }

        /// <summary>
        ///     释放所有资源
        /// </summary>
        public override void Dispose()
        {
            var retCode = (ErrorCode) TrackingWrapper.AFT_FSDK_UninitialFaceEngine(Engine);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
        }
    }
}