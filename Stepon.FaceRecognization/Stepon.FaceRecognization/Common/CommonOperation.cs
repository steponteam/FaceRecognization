/*********************************************************
 * author：Gavin
 * created：2017-8-9
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Stepon.FaceRecognization.Extensions;

namespace Stepon.FaceRecognization.Common
{
    internal class CommonOperation
    {
        public static T SafeOffInputOperation<T>(Bitmap image, Func<ImageData, T> operation)
        {
            var imageData = image.GetBitmapData();

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

        public static T OffInputOperation<T>(Bitmap image, Func<ImageData, IntPtr, T> operation) //IntPtr为图像数据指针
        {
            var imageData = image.GetBitmapData();

            return OffInputOperation(imageData, image.Width, image.Height, operation);
        }

        public static T OffInputOperation<T>(byte[] imageData, int width, int height,
            Func<ImageData, IntPtr, T> operation)
        {
            var offInput = new ImageData
            {
                u32PixelArrayFormat = PixelFormatCode.Rgb24,
                i32Width = width,
                i32Height = height,
                pi32Pitch = new int[4],
                ppu8Plane = new IntPtr[4]
            };
            offInput.pi32Pitch[0] = width * 3; //正常情况下，应该获取图形Stride长度，这里以图像格式都为Rgb24来进行计算

            //由于ASVLOFFSCREEN还要用于人脸识别，所以不释放该资源，当识别完成后释放，或未检测到人脸时释放
            var pImageData = Marshal.AllocHGlobal(imageData.Length); //未释放

            Marshal.Copy(imageData, 0, pImageData, imageData.Length);
            offInput.ppu8Plane[0] = pImageData;

            return operation(offInput, pImageData);
        }
    }
}