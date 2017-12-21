using System;
using System.Runtime.InteropServices;

namespace Stepon.FaceRecognizationCore.Common
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

    /// <summary>
    ///     人脸在图片中的位置
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FaceRect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    ///     待检测的图像信息，一般在使用时无需关心此结构
    /// </summary>
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
    ///     定义人脸检测结果中的人脸角度，Orient后面的数字表示角度
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
        /// <summary>
        ///     检测 0 度（±45 度）方向
        /// </summary>
        Orient0Only = 0x1,

        /// <summary>
        ///     检测 90 度（±45 度）方向
        /// </summary>
        Orient90Only = 0x2,

        /// <summary>
        ///     检测 270 度（±45 度）方向
        /// </summary>
        Orient270Only = 0x3,

        /// <summary>
        ///     检测 180 度（±45 度）方向
        /// </summary>
        Orient180Only = 0x4,

        /// <summary>
        ///     检测 0， 90， 180， 270 四个方向,0 度更优先
        /// </summary>
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

    #region age and gender data structure

    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtraFaceInput
    {
        public IntPtr pFaceRectArray;
        public IntPtr pFaceOrientArray;
        public int lFaceNumber;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtraFaceResult
    {
        public IntPtr pResult;
        public int lFaceNumber;
    }

    #endregion


    /// <summary>
    ///     人脸定位的操作定义
    /// </summary>
    [Flags]
    public enum LocateOperation
    {
        /// <summary>
        ///     一般操作
        /// </summary>
        None = 0,

        /// <summary>
        ///     包含年龄
        /// </summary>
        IncludeAge = 1,

        /// <summary>
        ///     包含性别
        /// </summary>
        IncludeGender = 2
    }

    /// <summary>
    ///     错误代码
    /// </summary>
    public enum ErrorCode
    {
        Ok = 0,

        /// <summary>
        ///     通用错误类型
        /// </summary>
        BasicBase = 0x0001,

        /// <summary>
        ///     错误原因不明
        /// </summary>
        Unknown = BasicBase,

        /// <summary>
        ///     无效的参数
        /// </summary>
        InvalidParam = BasicBase + 1,

        /// <summary>
        ///     引擎不支持
        /// </summary>
        Unsupported = BasicBase + 2,

        /// <summary>
        ///     内存不足
        /// </summary>
        NoMemory = BasicBase + 3,

        /// <summary>
        ///     状态错误
        /// </summary>
        BadState = BasicBase + 4,

        /// <summary>
        ///     用户取消相关操作
        /// </summary>
        UserCancel = BasicBase + 5,

        /// <summary>
        ///     操作时间过期
        /// </summary>
        Expired = BasicBase + 6,

        /// <summary>
        ///     用户暂停操作
        /// </summary>
        UserPause = BasicBase + 7,

        /// <summary>
        ///     缓冲上溢
        /// </summary>
        BufferOverflow = BasicBase + 8,

        /// <summary>
        ///     缓冲下溢
        /// </summary>
        BufferUnderflow = BasicBase + 9,

        /// <summary>
        ///     存贮空间不足
        /// </summary>
        NoDiskspace = BasicBase + 10,

        /// <summary>
        ///     组件不存在
        /// </summary>
        ComponentNotExist = BasicBase + 11,

        /// <summary>
        ///     全局数据不存在
        /// </summary>
        GlobalDataNotExist = BasicBase + 12,

        /// <summary>
        ///     Free SDK通用错误类型
        /// </summary>
        SdkBase = 0x7000,

        /// <summary>
        ///     无效的App Id
        /// </summary>
        InvalidAppId = SdkBase + 1,

        /// <summary>
        ///     无效的SDK key
        /// </summary>
        InvalidSdkId = SdkBase + 2,

        /// <summary>
        ///     AppId和SDKKey不匹配
        /// </summary>
        InvalidIdPair = SdkBase + 3,

        /// <summary>
        ///     SDKKey 和使用的SDK 不匹配
        /// </summary>
        MismatchIdAndSdk = SdkBase + 4,

        /// <summary>
        ///     系统版本不被当前SDK所支持
        /// </summary>
        SystemVersionUnsupported = SdkBase + 5,

        /// <summary>
        ///     SDK有效期过期，需要重新下载更新
        /// </summary>
        LicenceExpired = SdkBase + 6,

        /// <summary>
        ///     Face Recognition错误类型
        /// </summary>
        FaceRecognitionBase = 0x12000,

        /// <summary>
        ///     无效的输入内存
        /// </summary>
        InvalidMemoryInfo = FaceRecognitionBase + 1,

        /// <summary>
        ///     无效的输入图像参数
        /// </summary>
        InvalidImageInfo = FaceRecognitionBase + 2,

        /// <summary>
        ///     无效的脸部信息
        /// </summary>
        InvalidFaceInfo = FaceRecognitionBase + 3,

        /// <summary>
        ///     当前设备无GPU可用
        /// </summary>
        NoGpuAvailable = FaceRecognitionBase + 4,

        /// <summary>
        ///     待比较的两个人脸特征的版本不一致
        /// </summary>
        MismatchedFeatureLevel = FaceRecognitionBase + 5
    }
}