using System;
using Stepon.FaceRecognization.Common;

namespace Stepon.FaceRecognization.Exceptions
{
    /// <summary>
    ///     人脸识别异常
    /// </summary>
    public class FaceException : Exception
    {
        public FaceException(ErrorCode code) : base(code.ToString())
        {
            Code = code;
        }

        /// <summary>
        ///     错误代码
        /// </summary>
        public ErrorCode Code { get; }
    }
}