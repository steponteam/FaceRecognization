using System;

namespace Stepon.FaceRecognizationCore.Exceptions
{
    public class FaceNotFoundException : Exception
    {
        public FaceNotFoundException() : base("face not found")
        {
        }
    }
}