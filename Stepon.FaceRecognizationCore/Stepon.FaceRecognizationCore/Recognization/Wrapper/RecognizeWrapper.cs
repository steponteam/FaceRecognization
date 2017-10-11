using System;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Common;

namespace Stepon.FaceRecognizationCore.Recognization.Wrapper
{
    internal class RecognizeWrapper
    {
        private const string DllPosition = "libs/libarcsoft_fsdk_face_recognition.so";

        [DllImport(DllPosition, EntryPoint = "AFR_FSDK_InitialEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFR_FSDK_InitialEngine(string appId, string sdkKey, byte[] buffer, int bufferSize,
            out IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "AFR_FSDK_ExtractFRFeature",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFR_FSDK_ExtractFRFeature(IntPtr engine, ref ImageData pInputImage,
            ref AFR_FSDK_FACEINPUT pFaceRes, out AFR_FSDK_FACEMODEL pFaceModels);

        [DllImport(DllPosition, EntryPoint = "AFR_FSDK_FacePairMatching",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFR_FSDK_FacePairMatching(IntPtr engine, ref AFR_FSDK_FACEMODEL reffeature,
            ref AFR_FSDK_FACEMODEL probefeature, ref float pfSimilScore);

        [DllImport(DllPosition, EntryPoint = "AFR_FSDK_UninitialEngine", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern int AFR_FSDK_UninitialEngine(IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "AFR_FSDK_GetVersion", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern IntPtr AFR_FSDK_GetVersion(IntPtr engine);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AFR_FSDK_FACEINPUT
    {
        public FaceRect rcFace;
        public int lOrient;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AFR_FSDK_FACEMODEL
    {
        public IntPtr pbFeature;

        [MarshalAs(UnmanagedType.I4)] public int lFeatureSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AFR_FSDK_Version
    {
        public int codebase;
        public int major;
        public int minor;
        public int build;
        public int lFeatureLevel;
        public string version;
        public string buildDate;
        public string copyright;
    }
}