using System;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Common;

namespace Stepon.FaceRecognizationCore.Gender.Wrapper
{
    internal class GenderWrapper
    {
        private const string DllPosition = "libs/libarcsoft_fsdk_gender_estimation.so";

        [DllImport(DllPosition, EntryPoint = "ASGE_FSDK_InitGenderEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASGE_FSDK_InitGenderEngine(string appId, string sdkKey, byte[] memory, int memroySize,
            out IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "ASGE_FSDK_GenderEstimation_StaticImage",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASGE_FSDK_GenderEstimation_StaticImage(IntPtr engine, ref ImageData pImgData,
            ref ExtraFaceInput pFaceRes, out ExtraFaceResult pGenderRes);

        [DllImport(DllPosition, EntryPoint = "ASGE_FSDK_GenderEstimation_Preview",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASGE_FSDK_GenderEstimation_Preview(IntPtr engine, ref ImageData pImgData,
            ref ExtraFaceInput pFaceRes, out ExtraFaceResult pGenderRes);

        [DllImport(DllPosition, EntryPoint = "ASGE_FSDK_UninitGenderEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ASGE_FSDK_UninitGenderEngine(IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "ASGE_FSDK_GetVersion", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern IntPtr ASGE_FSDK_GetVersion(IntPtr engine);
    }
}
