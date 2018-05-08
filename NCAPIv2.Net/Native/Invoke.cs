using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NCAPIv2.Native
{
    static class Invoke
    {
        public const string Dll = "libmvnc";
        public const CallingConvention Convention = CallingConvention.Cdecl;

        /// <summary>
        /// get a null-terminated string, that is no longer than maxSize
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static byte[] GetCStr(this string str, int maxSize)
        {
            str = str.Substring(0, Math.Min(str.Length, maxSize - 1));
            var bytes = new byte[str.Length + 1];
            Encoding.ASCII.GetBytes(str, 0, str.Length, bytes, 0);
            bytes[str.Length] = 0;
            return bytes;
        }
    }
}
