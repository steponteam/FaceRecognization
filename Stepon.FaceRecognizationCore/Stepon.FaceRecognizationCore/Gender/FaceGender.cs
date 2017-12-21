/*********************************************************
 * author：Gavin
 * created：2017-12-21
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Common;
using Stepon.FaceRecognizationCore.Exceptions;
using Stepon.FaceRecognizationCore.Extensions;
using Stepon.FaceRecognizationCore.Gender.Wrapper;
using Stepon.FaceRecognizationCore.Interfaces;

namespace Stepon.FaceRecognizationCore.Gender
{
    /// <summary>
    ///     性别识别
    /// </summary>
    public class FaceGender : FaceExtra
    {
        /// <summary>
        ///     初始化性别识别
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="sdkKey">应用Key</param>
        /// <param name="preAllocMemSize">缓存区内存大小（byte）</param>
        public FaceGender(string appId, string sdkKey, int preAllocMemSize = 31457280) : base(appId, sdkKey,
            preAllocMemSize)
        {
            var retCode =
                (ErrorCode) GenderWrapper.ASGE_FSDK_InitGenderEngine(AppId, SdkKey, Buffer, PreAllocMemSize,
                    out Engine);

            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);

            IsIntialized = true;
        }

        /// <summary>
        ///     静态图片的性别评估
        /// </summary>
        /// <param name="locate">已经识别的人脸位置信息（图片数据未释放，如果释放，将出现异常）</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含性别的位置信息</returns>
        public override LocateResult StaticEstimation(LocateResult locate, bool autoDispose = true)
        {
            return Estimation(locate, input =>
            {
                var code = (ErrorCode) GenderWrapper.ASGE_FSDK_GenderEstimation_StaticImage(Engine, ref locate.OffInput,
                    ref input,
                    out var result);
                if (code != ErrorCode.Ok)
                    throw new FaceException(code);

                locate.Genders = result.pResult.ToStructArray<int>(result.lFaceNumber);

            }, autoDispose);
        }

        /// <summary>
        ///     序列帧图片的性别评估
        /// </summary>
        /// <param name="locate">已经识别的人脸位置信息（图片数据未释放，如果释放，将出现异常）</param>
        /// <param name="autoDispose">是否自动释放检测的人脸资源</param>
        /// <returns>包含性别的位置信息</returns>
        public override LocateResult PreviewEstimation(LocateResult locate, bool autoDispose = true)
        {
            return Estimation(locate, input =>
            {
                var code = (ErrorCode) GenderWrapper.ASGE_FSDK_GenderEstimation_Preview(Engine, ref locate.OffInput,
                    ref input,
                    out var result);

                if (code != ErrorCode.Ok)
                    throw new FaceException(code);

                locate.Genders = result.pResult.ToStructArray<int>(result.lFaceNumber);
            }, autoDispose);
        }

        /// <summary>
        ///     获得识别引擎版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        public override SdkVersion GetVersion()
        {
            var retPtr = GenderWrapper.ASGE_FSDK_GetVersion(Engine);
            var ret = Marshal.PtrToStructure<AFDT_FSDK_Version>(retPtr);
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
            var retCode = (ErrorCode) GenderWrapper.ASGE_FSDK_UninitGenderEngine(Engine);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
        }
    }
}