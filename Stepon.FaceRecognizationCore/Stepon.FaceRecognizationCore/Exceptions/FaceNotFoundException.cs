using System;

namespace Stepon.FaceRecognizationCore.Exceptions
{
    /// <summary>
    /// 抽取特征时未找到人脸，抛出此异常
    /// </summary>
    public class FaceNotFoundException : Exception
    {
        public FaceNotFoundException() : base("face not found")
        {
        }
    }
}