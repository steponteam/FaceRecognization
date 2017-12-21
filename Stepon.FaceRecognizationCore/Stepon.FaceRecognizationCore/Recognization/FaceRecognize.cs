/*********************************************************
 * author：Gavin
 * created：2017-8-15
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Stepon.FaceRecognizationCore.Common;
using Stepon.FaceRecognizationCore.Exceptions;
using Stepon.FaceRecognizationCore.Interfaces;
using Stepon.FaceRecognizationCore.Recognization.Wrapper;

namespace Stepon.FaceRecognizationCore.Recognization
{
    /// <summary>
    ///     人脸识别
    /// </summary>
    public class FaceRecognize : FaceBase
    {
        /// <summary>
        ///     初始化人脸识别
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="sdkKey">应用Key</param>
        /// <param name="preAllocMemSize">缓存区内存大小（byte）</param>
        public FaceRecognize(string appId, string sdkKey, int preAllocMemSize = 41943040) : base(appId, sdkKey,
            preAllocMemSize)
        {
            var retCode =
                (ErrorCode)RecognizeWrapper.AFR_FSDK_InitialEngine(AppId, SdkKey, Buffer, PreAllocMemSize, out Engine);

            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);

            IsIntialized = true;
        }

        /// <summary>
        ///     抽取人脸特征，此函数不包含额外信息（性别和年龄）
        /// </summary>
        /// <param name="image">要抽取的人脸特征图像数据</param>
        /// <param name="faceLocation">人脸位置</param>
        /// <param name="orient">人脸朝向</param>
        /// <exception cref="Exception">如果执行失败，则抛出异常，并给出失败结果</exception>
        /// <returns>如果执行成功，返回特征数据，如果为null，则表示faceLocation的位置不存在人脸</returns>
        public Feature ExtractFeature(ImageData image, FaceRect faceLocation, OrientCode orient)
        {
            var faceRes = new AFR_FSDK_FACEINPUT
            {
                rcFace = faceLocation,
                lOrient = (int)orient
            };

            var result =
                (ErrorCode)RecognizeWrapper.AFR_FSDK_ExtractFRFeature(Engine, ref image, ref faceRes,
                    out var faceModel);

            if (result == ErrorCode.Ok)
            {
                var data = new byte[faceModel.lFeatureSize];
                Marshal.Copy(faceModel.pbFeature, data, 0, faceModel.lFeatureSize);
                var feature = new Feature
                {
                    FeatureData = data,
                    Rect = faceLocation
                };

                return feature;
            }

            //采用Tracking定位时，可能出现识别错误的位置，此时返回的错误为Unsupported，遇到这样的情况直接返回null
            if (result == ErrorCode.Unsupported)
                return null;

            throw new FaceException(result);
        }

        /// <summary>
        ///     根据人脸识别结果返回特征数据数组，如果位置信息中包含额外信息（性别和年龄），则会同时返回额外信息到特征中
        /// </summary>
        /// <param name="faces">人脸识别结果</param>
        /// <param name="autoDispose">是否自动释放人脸识别结果</param>
        /// <exception cref="FaceNotFoundException">如果识别结果中未包含人脸，则抛出此异常</exception>
        /// <returns>脸部特征数据数组</returns>
        public Feature[] ExtractFeatures(LocateResult faces, bool autoDispose = true)
        {
            if (faces.HasFace)
            {
                var features = new List<Feature>();
                for (var faceIndex = 0; faceIndex < faces.FaceCount; faceIndex++)
                {
                    var faceLocation = faces.Faces[faceIndex];
                    var orient = faces.FacesOrient[faceIndex];

                    var feature = ExtractFeature(faces.OffInput, faceLocation, orient);
                    if (feature != null)
                    {
                        if (faces.Ages != null)
                            feature.Age = faces.Ages[faceIndex];
                        if (faces.Genders != null)
                            feature.Gender = faces.Genders[faceIndex];
                        features.Add(feature);
                    }

                }

                if (autoDispose)
                    faces.Dispose();

                return features.ToArray();
            }

            if (autoDispose)
                faces.Dispose();

            throw new FaceNotFoundException();
        }

        /// <summary>
        ///     对两个特征数据进行比较
        /// </summary>
        /// <param name="origin">原始特征数据，一般为实时监测到的特征数据</param>
        /// <param name="toMatch">要比较的特征数据，一般为人脸库中的特征数据</param>
        /// <param name="autoFreeOrigin">是否自动释放原始特征数据，一般情况下，用于1:N的比较，原始特征数据不进行自动释放，以提升处理效率，在循环比较完成后，进行释放</param>
        /// <param name="autoFreeMatch">是否自动释放待比较的特征数据，一般情况下，可以直接释放</param>
        /// <returns>两个特征数据的相似度，一般大于0.5可以视为相似</returns>
        public float Match(FaceModel origin, FaceModel toMatch, bool autoFreeOrigin = false, bool autoFreeMatch = true)
        {
            try
            {
                var originModel = origin.MatchFaceModel;
                var matchModel = toMatch.MatchFaceModel;

                float result = 0;

                var retCode =
                    (ErrorCode)RecognizeWrapper.AFR_FSDK_FacePairMatching(Engine, ref originModel, ref matchModel,
                        ref result);

                if (retCode != ErrorCode.Ok)
                    throw new FaceException(retCode);

                return result;
            }
            finally
            {
                if (autoFreeOrigin)
                    origin.Dispose();

                if (autoFreeMatch)
                    toMatch.Dispose();
            }
        }

        /// <summary>
        ///     获取识别库版本
        /// </summary>
        /// <returns>版本信息</returns>
        public SdkVersion GetVersion()
        {
            var retPtr = RecognizeWrapper.AFR_FSDK_GetVersion(Engine);
            var ret = Marshal.PtrToStructure<AFR_FSDK_Version>(retPtr);

            return new SdkVersion
            {
                Codebase = ret.codebase,
                Major = ret.major,
                Minor = ret.minor,
                Build = ret.build,
                FeatureLevel = ret.lFeatureLevel,
                Version = ret.version,
                BuildDate = ret.buildDate,
                Copyright = ret.copyright
            };
        }

        /// <summary>
        ///     释放相关资源
        /// </summary>
        public override void Dispose()
        {
            var retCode = (ErrorCode)RecognizeWrapper.AFR_FSDK_UninitialEngine(Engine);
            if (retCode != ErrorCode.Ok)
                throw new FaceException(retCode);
        }
    }
}