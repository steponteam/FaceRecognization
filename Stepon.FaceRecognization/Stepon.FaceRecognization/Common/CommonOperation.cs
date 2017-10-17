/*********************************************************
 * author：Gavin
 * created：2017-8-9
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 * fix：
 * 2017-10-17：
 * 1.修复填充字节对齐的问题
 *********************************************************/

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Stepon.FaceRecognization.Extensions;

namespace Stepon.FaceRecognization.Common
{
    internal class CommonOperation
    {
        /// <summary>
        ///     将图像数据转换为识别需要用到的结构体，该操作将自动释放非托管资源
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="image">要处理的图像</param>
        /// <param name="operation">转换后要执行的操作</param>
        /// <returns>执行操作的返回数据</returns>
        [Obsolete("使用OffInputOperation")]
        public static T SafeOffInputOperation<T>(Bitmap image, Func<ImageData, T> operation)
        {
            var imageData = image.GetBitmapData(out int _);

            var offInput = new ImageData
            {
                u32PixelArrayFormat = PixelFormatCode.Rgb24,
                i32Width = image.Width,
                i32Height = image.Height,
                pi32Pitch = new int[4],
                ppu8Plane = new IntPtr[4]
            };
            offInput.pi32Pitch[0] = image.Width * 3;

            var pImageData = Marshal.AllocHGlobal(imageData.Length);

            try
            {
                Marshal.Copy(imageData, 0, pImageData, imageData.Length);
                offInput.ppu8Plane[0] = pImageData;

                return operation(offInput);
            }
            finally
            {
                Marshal.FreeHGlobal(pImageData);
            }
        }

        /// <summary>
        ///     将图像数据转换为识别需要用到的结构体
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="image">要处理的图像</param>
        /// <param name="operation">转换后要执行的操作</param>
        /// <returns>执行操作的返回数据</returns>
        public static T OffInputOperation<T>(Bitmap image, Func<ImageData, IntPtr, T> operation) //IntPtr为图像数据指针
        {
            var imageData = image.GetBitmapData(out int pixelSize);

            return OffInputOperation(imageData, image.Width, image.Height, operation, pixelSize);
        }

        /// <summary>
        ///     将图像数据转换为识别需要用到的结构体，此函数主要用于直接获取图像原始数据来进行处理，一般情况下，如果捕获的图像数据不需要显示（例如做报警等），可以使用此函数
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="imageData">图像原始数据，不包含图形的其他附加头信息</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="operation">转换后要执行的操作</param>
        /// <param name="pixelSize">每像素大小</param>
        /// <returns>执行操作的返回数据</returns>
        public static T OffInputOperation<T>(byte[] imageData, int width, int height,
            Func<ImageData, IntPtr, T> operation, int pixelSize = 3)
        {
            var offInput = new ImageData
            {
                u32PixelArrayFormat = PixelFormatCode.Rgb24,
                i32Width = width,
                i32Height = height,
                pi32Pitch = new int[4],
                ppu8Plane = new IntPtr[4]
            };
            offInput.pi32Pitch[0] = (int)((width * pixelSize + pixelSize) & 0xfffffffc); //正常情况下，应该获取图形Stride长度

            //由于ASVLOFFSCREEN还要用于人脸识别，所以不释放该资源，当识别完成后释放，或未检测到人脸时释放
            var pImageData = Marshal.AllocHGlobal(imageData.Length); //未释放

            Marshal.Copy(imageData, 0, pImageData, imageData.Length);
            offInput.ppu8Plane[0] = pImageData;

            return operation(offInput, pImageData);
        }
    }
}