using System;
using Stepon.FaceRecognizationCore.Common;

namespace Stepon.FaceRecognizationCore.Exceptions
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