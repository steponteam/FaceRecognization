/*********************************************************
 * author：Gavin
 * created：2017-8-9
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Runtime.InteropServices;

namespace Stepon.FaceRecognization.Common
{
    internal class PixelFormatCode
    {
        public const uint Rgb24 = 0x201;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AFDT_FSDK_Version
    {
        public int codebase;
        public int major;
        public int minor;
        public int build;
        public string version;
        public string buildDate;
        public string copyright;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FaceRect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageData
    {
        public uint u32PixelArrayFormat;
        public int i32Width;
        public int i32Height;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public IntPtr[] ppu8Plane;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I4)] public int[] pi32Pitch;
    }

    /// <summary>
    ///     用于检测结果的角度范围编码
    /// </summary>
    public enum OrientCode
    {
        Orient0 = 0x1,
        Orient90 = 0x2,
        Orient270 = 0x3,
        Orient180 = 0x4,
        Orient30 = 0x5,
        Orient60 = 0x6,
        Orient120 = 0x7,
        Orient150 = 0x8,
        Orient210 = 0x9,
        Orient240 = 0xa,
        Orient300 = 0xb,
        Orient330 = 0xc
    }

    /// <summary>
    ///     脸部角度的检测范围
    /// </summary>
    public enum OrientPriority
    {
        Orient0Only = 0x1,
        Orient90Only = 0x2,
        Orient270Only = 0x3,
        Orient180Only = 0x4,
        OrientHigherExt = 0x5
    }

    /// <summary>
    ///     人脸检测库版本信息
    /// </summary>
    public struct SdkVersion
    {
        public int Codebase;
        public int Major;
        public int Minor;
        public int Build;
        public int FeatureLevel; //人脸识别才有该项
        public string Version;
        public string BuildDate;
        public string Copyright;
    }

    /// <summary>
    ///     错误代码
    /// </summary>
    public enum ErrorCode
    {
        Ok = 0,
        BasicBase = 0x0001,
        Unknown = BasicBase,
        InvalidParam = BasicBase + 1,
        Unsupported = BasicBase + 2,
        NoMemory = BasicBase + 3,
        BadState = BasicBase + 4,
        UserCancel = BasicBase + 5,
        Expired = BasicBase + 6,
        UserPause = BasicBase + 7,
        BufferOverflow = BasicBase + 8,
        BufferUnderflow = BasicBase + 9,
        NoDiskspace = BasicBase + 10,
        ComponentNotExist = BasicBase + 11,
        GlobalDataNotExist = BasicBase + 12,
        SdkBase = 0x7000,
        InvalidAppId = SdkBase + 1,
        InvalidSdkId = SdkBase + 2,
        InvalidIdPair = SdkBase + 3,
        MismatchIdAndSdk = SdkBase + 4,
        SystemVersionUnsupported = SdkBase + 5,
        LicenceExpired = SdkBase + 6,
        FaceRecognitionBase = 0x12000,
        InvalidMemoryInfo = FaceRecognitionBase + 1,
        InvalidImageInfo = FaceRecognitionBase + 2,
        InvalidFaceInfo = FaceRecognitionBase + 3,
        NoGpuAvailable = FaceRecognitionBase + 4,
        MismatchedFeatureLevel = FaceRecognitionBase + 5
    }
}