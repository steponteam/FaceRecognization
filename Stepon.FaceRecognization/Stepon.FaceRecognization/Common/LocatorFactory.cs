using Stepon.FaceRecognization.Detection;
using Stepon.FaceRecognization.Exceptions;
using Stepon.FaceRecognization.Interfaces;
using Stepon.FaceRecognization.Tracking;

namespace Stepon.FaceRecognization.Common
{
    public class LocatorFactory
    {
        /// <summary>
        ///     获取面部检测的人脸定位，此定位方式支持更精确的人脸定位，但需要更高的系统运算资源，性能较低，适合急于静态图像的定位，一般可以用于人脸特征码抽取
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="sdkKey">应用Key</param>
        /// <param name="preAllocMemSize">缓存区内存大小（byte）</param>
        /// <param name="orientPriority">期望的脸部检测角度范围</param>
        /// <param name="scale">
        ///     用于数值表示的最小人脸尺寸有效值范围[2,50] 推荐值 16。该尺寸是人脸相对于所在图片的长边的占比。例如，如果用户想检测到的最小人脸尺寸是图片长度的 1/8，那么这个 nScale 就应该设置为
        ///     8
        /// </param>
        /// <param name="maxFaceNumber">用户期望引擎最多能检测出的人脸数有效值范围[1,50]</param>
        /// <returns></returns>
        public static FaceLocator GetDetectionLocator(string appId, string sdkKey, int preAllocMemSize = 41943040,
            OrientPriority orientPriority = OrientPriority.OrientHigherExt,
            int scale = 16, int maxFaceNumber = 10)
        {
            return GetLocator(appId, sdkKey, false, preAllocMemSize, orientPriority, scale, maxFaceNumber);
        }

        /// <summary>
        ///     获取面部检测的人脸跟踪，此定位方式速度较快，适合视频类的快速定位，但是精度相对较低(目前该定位器存在问题，官方的DEMO都有问题)
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="sdkKey">应用Key</param>
        /// <param name="preAllocMemSize">缓存区内存大小（byte）</param>
        /// <param name="orientPriority">期望的脸部检测角度范围</param>
        /// <param name="scale">
        ///     用于数值表示的最小人脸尺寸有效值范围[2,16] 推荐值 16。该尺寸是人脸相对于所在图片的长边的占比。例如，如果用户想检测到的最小人脸尺寸是图片长度的 1/8，那么这个 nScale 就应该设置为
        ///     8
        /// </param>
        /// <param name="maxFaceNumber">用户期望引擎最多能检测出的人脸数有效值范围[1,20]</param>
        /// <returns></returns>
        public static FaceLocator GetTrackingLocator(string appId, string sdkKey, int preAllocMemSize = 41943040,
            OrientPriority orientPriority = OrientPriority.OrientHigherExt,
            int scale = 16, int maxFaceNumber = 10)
        {
            return GetLocator(appId, sdkKey, true, preAllocMemSize, orientPriority, scale, maxFaceNumber);
        }

        /// <summary>
        ///     获取面部检测
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="sdkKey">应用Key</param>
        /// <param name="useTracking">如果设置为True，则启用面部跟踪，如果False，则启用面部检测</param>
        /// <param name="preAllocMemSize">缓存区内存大小（byte）</param>
        /// <param name="orientPriority">期望的脸部检测角度范围</param>
        /// <param name="scale">
        ///     用于数值表示的最小人脸尺寸有效值范围[2,50] 推荐值 16（面部跟踪的取值范围为[2,16]）。该尺寸是人脸相对于所在图片的长边的占比。例如，如果用户想检测到的最小人脸尺寸是图片长度的
        ///     1/8，那么这个 nScale 就应该设置为 8
        /// </param>
        /// <param name="maxFaceNumber">用户期望引擎最多能检测出的人脸数有效值范围[1,50]（面部跟踪的取值范围为[1,20]）</param>
        /// <returns></returns>
        public static FaceLocator GetLocator(string appId, string sdkKey, bool useTracking = true,
            int preAllocMemSize = 41943040,
            OrientPriority orientPriority = OrientPriority.OrientHigherExt,
            int scale = 16, int maxFaceNumber = 10)
        {
            FaceLocator locator;

            if (useTracking)
                locator = new FaceTracking(appId, sdkKey, false, preAllocMemSize);
            else
                locator = new FaceDetection(appId, sdkKey, false, preAllocMemSize);

            var code = locator.Initialize(orientPriority, scale, maxFaceNumber);
            if (code != ErrorCode.Ok)
                throw new FaceException(code);

            return locator;
        }
    }
}