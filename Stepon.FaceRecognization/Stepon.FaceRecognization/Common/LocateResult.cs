using System;
using System.Runtime.InteropServices;

namespace Stepon.FaceRecognization.Common
{
    /// <summary>
    ///     人脸检测结果
    /// </summary>
    public class LocateResult : IDisposable
    {
        public int FaceCount;
        public FaceRect[] Faces;
        public OrientCode[] FacesOrient;

        /// <summary>
        ///     指向图形数据的指针，保留用于释放
        /// </summary>
        internal IntPtr ImageDataPtr;

        /// <summary>
        ///     图像描述结构，如果识别到人脸，可以利用此进行识别处理
        /// </summary>
        public ImageData OffInput;

        public bool HasFace => FaceCount > 0;

        public void Dispose()
        {
            Marshal.FreeHGlobal(ImageDataPtr);
        }
    }
}