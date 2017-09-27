/*********************************************************
 * author：Gavin
 * created：2017-8-9
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Runtime.InteropServices;
using Stepon.FaceRecognization.Common;

namespace Stepon.FaceRecognization.Detection.Wrapper
{
    internal class DetectionWrapper
    {
        private const string DllPosition = "libs/libarcsoft_fsdk_face_detection.dll";

        [DllImport(DllPosition, EntryPoint = "AFD_FSDK_InitialFaceEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFD_FSDK_InitialFaceEngine(string appId, string sdkKey, byte[] memory, int memroySize,
            out IntPtr engine, int orientPriority, int scale, int maxFaceNumber);

        [DllImport(DllPosition, EntryPoint = "AFD_FSDK_StillImageFaceDetection",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFD_FSDK_StillImageFaceDetection(IntPtr engine, ref ImageData pImgData,
            out IntPtr pFaceRes);

        [DllImport(DllPosition, EntryPoint = "AFD_FSDK_UninitialFaceEngine",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int AFD_FSDK_UninitialFaceEngine(IntPtr engine);

        [DllImport(DllPosition, EntryPoint = "AFD_FSDK_GetVersion", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern IntPtr AFD_FSDK_GetVersion(IntPtr engine);
    }


    #region Detection data structure

    [StructLayout(LayoutKind.Sequential)]
    internal struct AFD_FSDK_FACERES
    {
        public int nFace;
        public IntPtr rcFace;
        public IntPtr lfaceOrient;
    }

    #endregion
}