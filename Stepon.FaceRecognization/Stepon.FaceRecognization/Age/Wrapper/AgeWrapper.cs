/*********************************************************
 * author：Gavin
 * created：2017-12-21
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Runtime.InteropServices;
using Stepon.FaceRecognization.Common;

namespace Stepon.FaceRecognization.Age.Wrapper
{
    internal class AgeWrapper
    {
        private const string DllPosition = "libs/libarcsoft_fsdk_age_estimation.dll";

        [DllImport(DllPosition, EntryPoint = "ASAE_FSDK_InitAgeEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASAE_FSDK_InitAgeEngine(string appId, string sdkKey, byte[] memory, int memroySize,
            out IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "ASAE_FSDK_AgeEstimation_StaticImage",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASAE_FSDK_AgeEstimation_StaticImage(IntPtr engine, ref ImageData pImgData,
            ref ExtraFaceInput pFaceRes, out ExtraFaceResult pAgeRes);

        [DllImport(DllPosition, EntryPoint = "ASAE_FSDK_AgeEstimation_Preview",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASAE_FSDK_AgeEstimation_Preview(IntPtr engine, ref ImageData pImgData,
            ref ExtraFaceInput pFaceRes, out ExtraFaceResult pAgeRes);

        [DllImport(DllPosition, EntryPoint = "ASAE_FSDK_UninitAgeEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASAE_FSDK_UninitAgeEngine(IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "ASAE_FSDK_GetVersion", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern IntPtr ASAE_FSDK_GetVersion(IntPtr engine);
    }
}