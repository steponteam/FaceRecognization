using System;
using System.Drawing;
using Stepon.FaceRecognization.Common;
using Stepon.FaceRecognization.Extensions;

namespace Stepon.FaceRecognization.Recognization
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
        ///     位置的另外的表达方式，一般用作GDI+的绘制
        /// </summary>
        public Rectangle FaceLoaction => Rect.ToRectangle();

        public void Dispose()
        {
            FeatureData?.Dispose();
        }
    }
}