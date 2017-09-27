using System;

namespace Stepon.FaceRecognization.Exceptions
{
    public class FaceNotFoundException : Exception
    {
        public FaceNotFoundException() : base("face not found")
        {
        }
    }
}