/*********************************************************
 * author：Gavin
 * created：2017-8-9
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Stepon.FaceRecognization.Extensions
{
    internal static class BitmapExtensions
    {
        /// <summary>
        ///     将图像转换为RGB图像
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Bitmap ConvertToRgb24(this Bitmap self)
        {
            if (self.PixelFormat != PixelFormat.Format24bppRgb)
            {
                var convertImage = new Bitmap(self.Width, self.Height, PixelFormat.Format24bppRgb);
                using (var g = Graphics.FromImage(self))
                {
                    g.DrawImage(self, 0, 0);
                }

                return convertImage;
            }
            return self;
        }

        /// <summary>
        ///     获取位图数据的像素数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="useNativePixelFormat"></param>
        /// <param name="pixelSize"></param>
        /// <returns></returns>
        public static byte[] GetBitmapData(this Bitmap self, out int pixelSize, bool useNativePixelFormat = false)
        {
            var rect = new Rectangle(0, 0, self.Width, self.Height);
            var bmpData = self.LockBits(rect, ImageLockMode.ReadOnly,
                useNativePixelFormat ? self.PixelFormat : PixelFormat.Format24bppRgb);
            var dataPtr = bmpData.Scan0;
            var bytesCount = Math.Abs(bmpData.Stride) * self.Height;

            var data = new byte[bytesCount];
            Marshal.Copy(dataPtr, data, 0, bytesCount);

            self.UnlockBits(bmpData);

            pixelSize = bmpData.Stride / bmpData.Width;

            return data;
        }
    }
}