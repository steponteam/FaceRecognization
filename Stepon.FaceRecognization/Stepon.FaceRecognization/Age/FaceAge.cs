/*********************************************************
 * author：Gavin
 * created：2017-12-21
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System.Runtime.InteropServices;
using Stepon.FaceRecognization.Age.Wrapper;
using Stepon.FaceRecognization.Common;
using Stepon.FaceRecognization.Exceptions;
using Stepon.FaceRecognization.Extensions;
using Stepon.FaceRecognization.Interfaces;

namespace Stepon.FaceRecognization.Age
{
    /// <summary>
    ///     年龄识别
    /// </summary>
    public class FaceAge : FaceExtra
    {
        /// <summary>
        ///     初始化年龄识别
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="sdkKey">应用Key</param>
        /// <param name="preAllocMemSize">缓存区内存大小（byte）</param>
        public FaceAge(string appId, string sdkKey, int preAllocMemSize = 31457280) : base(appId, sdkKey,
            preAllocMemSize)
        {
            var retCode =
                (ErrorCode) AgeWrapper.ASAE_FSDK_InitAgeEngine(AppId, SdkKey, Buffer, PreAllocMemSize, out Engine);

            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);

            IsIntialized = true;
        }

        /// <summary>
        ///     静态图片的年龄评估
        /// </summary>
        /// <param name="locate">已经识别的人脸位置信息（图片数据未释放，如果释放，将出现异常）</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含年龄的位置信息</returns>
        public override LocateResult StaticEstimation(LocateResult locate, bool autoDispose = true)
        {
            return Estimation(locate,  input =>
            {
                var code = (ErrorCode) AgeWrapper.ASAE_FSDK_AgeEstimation_StaticImage(Engine, ref locate.OffInput,
                    ref input,
                    out var result);

                if (code != ErrorCode.Ok)
                    throw new FaceException(code);

                locate.Ages = result.pResult.ToStructArray<int>(result.lFaceNumber);
            }, autoDispose);
        }

        /// <summary>
        ///     序列帧图片的年龄评估
        /// </summary>
        /// <param name="locate">已经识别的人脸位置信息（图片数据未释放，如果释放，将出现异常）</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含年龄的位置信息</returns>
        public override LocateResult PreviewEstimation(LocateResult locate, bool autoDispose = true)
        {
            return Estimation(locate, input =>
            {
                var code = (ErrorCode) AgeWrapper.ASAE_FSDK_AgeEstimation_Preview(Engine, ref locate.OffInput,
                    ref input,
                    out var result);

                if (code != ErrorCode.Ok)
                    throw new FaceException(code);

                locate.Ages = result.pResult.ToStructArray<int>(result.lFaceNumber);
            }, autoDispose);
        }

        /// <summary>
        ///     获得识别引擎版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        public override SdkVersion GetVersion()
        {
            var retPtr = AgeWrapper.ASAE_FSDK_GetVersion(Engine);
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
        ///     释放相关资源
        /// </summary>
        public override void Dispose()
        {
            var retCode = (ErrorCode) AgeWrapper.ASAE_FSDK_UninitAgeEngine(Engine);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
        }
    }
}