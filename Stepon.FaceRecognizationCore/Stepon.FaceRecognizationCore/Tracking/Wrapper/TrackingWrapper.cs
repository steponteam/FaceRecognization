using System;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Common;

namespace Stepon.FaceRecognizationCore.Tracking.Wrapper
{
    internal class TrackingWrapper
    {
        private const string DllPosition = "libs/libarcsoft_fsdk_face_tracking.so";

        [DllImport(DllPosition, EntryPoint = "AFT_FSDK_InitialFaceEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFT_FSDK_InitialFaceEngine(string appId, string sdkKey, byte[] buffer, int bufferSize,
            out IntPtr engine, int orientPriority, int scale, int faceNumber);

        [DllImport(DllPosition, EntryPoint = "AFT_FSDK_FaceFeatureDetect",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFT_FSDK_FaceFeatureDetect(IntPtr engine, ref ImageData pImgData,
            out IntPtr pFaceRes);

        [DllImport(DllPosition, EntryPoint = "AFT_FSDK_UninitialFaceEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFT_FSDK_UninitialFaceEngine(IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "AFT_FSDK_GetVersion", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern IntPtr AFT_FSDK_GetVersion(IntPtr engine);
    }

    #region Tracking data structure

    [StructLayout(LayoutKind.Sequential)]
    internal struct AFT_FSDK_FACERES
    {
        [MarshalAs(UnmanagedType.I4)] public int nFace;
        [MarshalAs(UnmanagedType.I4)] public int lfaceOrient;
        public IntPtr rcFace;
    }

    #endregion
}