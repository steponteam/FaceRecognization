using System;
using System.Runtime.InteropServices;

namespace Stepon.FaceRecognization.Common
{
    /// <summary>
    ///     人脸检测结果
    /// </summary>
    public class LocateResult : IDisposable
    {
        /// <summary>
        ///     年龄，如果未启动年龄检测，则为空
        /// </summary>
        public int[] Ages;

        /// <summary>
        ///     人脸数量
        /// </summary>
        public int FaceCount;

        /// <summary>
        ///     人脸位置信息
        /// </summary>
        public FaceRect[] Faces;

        /// <summary>
        ///     人脸的角度信息
        /// </summary>
        public OrientCode[] FacesOrient;

        /// <summary>
        ///     性别，如果未启动性别检测，则为空。1表示女，0表示男，-1表示未知。
        /// </summary>
        public int[] Genders;

        /// <summary>
        ///     指向图形数据的指针，保留用于释放
        /// </summary>
        internal IntPtr ImageDataPtr;

        /// <summary>
        ///     图像描述结构，如果识别到人脸，可以利用此进行识别处理
        /// </summary>
        public ImageData OffInput;

        /// <summary>
        ///     结果中是否包含人脸
        /// </summary>
        public bool HasFace => FaceCount > 0;

        public void Dispose()
        {
            Marshal.FreeHGlobal(ImageDataPtr);
        }
    }
}