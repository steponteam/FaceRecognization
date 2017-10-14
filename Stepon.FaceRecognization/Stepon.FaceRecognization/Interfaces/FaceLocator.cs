using System.Drawing;
using Stepon.FaceRecognization.Common;

namespace Stepon.FaceRecognization.Interfaces
{
    /// <summary>
    ///     识别库检测和跟踪基类
    /// </summary>
    public abstract class FaceLocator : FaceBase
    {
        protected FaceLocator(string appId, string sdkKey, bool autoInitWithDefaultParameter = true,
            int preAllocMemSize = 41943040) : base(appId, sdkKey, preAllocMemSize)
        {
            if (autoInitWithDefaultParameter)
                Initialize();
        }

        public abstract ErrorCode Initialize(OrientPriority orientPriority = OrientPriority.OrientHigherExt,
            int scale = 16, int maxFaceNumber = 10);

        public abstract ErrorCode Detect(Bitmap image, out LocateResult result);

        public abstract ErrorCode Detect(byte[] imageData, int width, int height, out LocateResult result, int pixelSize = 3);

        public abstract SdkVersion GetVersion();
    }
}