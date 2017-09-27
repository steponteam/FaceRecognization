/*********************************************************
 * author：Gavin
 * created：2017-8-9
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System.Drawing;
using Stepon.FaceRecognization.Common;

namespace Stepon.FaceRecognization.Extensions
{
    public static class RectangleExtensions
    {
        /// <summary>
        ///     将MRECT转换为Rectangle
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Rectangle ToRectangle(this FaceRect self)
        {
            return new Rectangle(self.left, self.top, self.right - self.left, self.bottom - self.top);
        }
    }
}