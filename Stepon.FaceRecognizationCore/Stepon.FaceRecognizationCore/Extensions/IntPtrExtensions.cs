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