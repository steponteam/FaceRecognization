using System;
using Stepon.FaceRecognization.Common;

namespace Stepon.FaceRecognization.Exceptions
{
    public class FaceException : Exception
    {
        public FaceException(ErrorCode code) : base(code.ToString())
        {
            Code = code;
        }

        public ErrorCode Code { get; }
    }
}