/*********************************************************
 * author：Gavin
 * created：2017-8-9
 * copyright：Copyright © 2016-2017 Stepon Technology CO.,LTD
 *********************************************************/

using System;
using System.Runtime.InteropServices;

namespace Stepon.FaceRecognizationCore.Extensions
{
    internal static class IntPtrExtensions
    {
        /// <summary>
        ///     将指针转换为结构体数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="self">指针</param>
        /// <param name="length">数组长度</param>
        /// <returns>结构体数组</returns>
        public static T[] ToStructArray<T>(this IntPtr self, int length)
        {
            var size = Marshal.SizeOf<T>();
            var array = new T[length];

            for (var i = 0; i < length; i++)
            {
                var iPtr = new IntPtr(self.ToInt64() + i * size);
                array[i] = iPtr.ToStruct<T>();
            }
            return array;
        }

        /// <summary>
        ///     将指针转换为结构体
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="self">指针</param>
        /// <returns>结构体实例</returns>
        public static T ToStruct<T>(this IntPtr self)
        {
            try
            {
                var result = Marshal.PtrToStructure<T>(self);
                return result;
            }
            finally
            {
                Marshal.DestroyStructure<T>(self);
            }
        }
    }
}