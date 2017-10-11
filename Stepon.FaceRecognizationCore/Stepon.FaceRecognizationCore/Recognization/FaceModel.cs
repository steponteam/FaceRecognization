using System;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Recognization.Wrapper;

namespace Stepon.FaceRecognizationCore.Recognization
{
    /// <summary>
    ///     人脸特征数据
    /// </summary>
    public class FaceModel : IDisposable
    {
        public FaceModel(byte[] data)
        {
            Data = data;

            MatchFaceModel = new AFR_FSDK_FACEMODEL
            {
                lFeatureSize = data.Length,
                pbFeature = Marshal.AllocHGlobal(data.Length)
            };

            Marshal.Copy(data, 0, MatchFaceModel.pbFeature, data.Length);
        }

        /// <summary>
        ///     模型的特征数据
        /// </summary>
        public byte[] Data { get; }

        internal AFR_FSDK_FACEMODEL MatchFaceModel { get; }

        public void Dispose()
        {
            Marshal.FreeHGlobal(MatchFaceModel.pbFeature);
        }

        public static implicit operator byte[](FaceModel model)
        {
            return model.Data;
        }

        public static implicit operator FaceModel(byte[] data)
        {
            return new FaceModel(data);
        }
    }
}