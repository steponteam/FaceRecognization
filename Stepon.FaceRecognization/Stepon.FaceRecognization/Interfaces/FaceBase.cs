using System;

namespace Stepon.FaceRecognization.Interfaces
{
    /// <summary>
    ///     识别库核心基类
    /// </summary>
    public abstract class FaceBase : IDisposable
    {
        protected readonly string AppId;

        protected readonly byte[] Buffer; //buffer必须声明为全局变量，否则将会被回收，识别将出现异常
        protected readonly int PreAllocMemSize;
        protected readonly string SdkKey;
        protected IntPtr Engine;

        /// <summary>
        ///     是否进行了初始化
        /// </summary>
        public bool IsIntialized = false;

        protected FaceBase(string appId, string sdkKey, int preAllocMemSize = 41943040)
        {
            AppId = appId;
            SdkKey = sdkKey;
            PreAllocMemSize = preAllocMemSize;
            Buffer = new byte[PreAllocMemSize];
        }

        public abstract void Dispose();
    }
}