using System;
using System.Drawing;
using Stepon.FaceRecognizationCore.Common;
using Stepon.FaceRecognizationCore.Extensions;

namespace Stepon.FaceRecognizationCore.Recognization
{
    /// <summary>
    ///     特征数据信息
    /// </summary>
    public class Feature : IDisposable
    {
        /// <summary>
        ///     特征数据
        /// </summary>
        public FaceModel FeatureData { get; set; }

        /// <summary>
        ///     在图片中的位置
        /// </summary>
        public FaceRect Rect { get; set; }

        /// <summary>
        ///     特征对应人脸的年龄，如果进行了检测则存在
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        ///     特征对应人脸的性别，如果进行了检测则存在,1表示女，0表示男，-1表示未知
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        ///     位置的另外的表达方式，一般用作GDI+的绘制
        /// </summary>
        public Rectangle FaceLoaction => Rect.ToRectangle();

        public void Dispose()
        {
            FeatureData?.Dispose();
        }
    }
}