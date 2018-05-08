//based on https://github.com/movidius/ncsdk/blob/f0e4de8c3ec3be5c7d6fc8f715bc09f41725f3d1/api/include/mvnc.h

using System;
using System.Runtime.InteropServices;

namespace NCAPIv2.Native
{
    using fifoPrivate_t = IntPtr;
    using graphPrivate_t = IntPtr;
    using devicePrivate_t = IntPtr;
    using unsigned_int = UInt32;

    [StructLayout(LayoutKind.Sequential)]
    public struct ncFifoHandle_t
    {
        // keep place for public data here
        fifoPrivate_t private_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ncGraphHandle_t
    {
        // keep place for public data here
        graphPrivate_t private_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ncDeviceHandle_t
    {
        // keep place for public data here
        devicePrivate_t private_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ncTensorDescriptor_t
    {
        public unsigned_int n;
        public unsigned_int c;
        public unsigned_int w;
        public unsigned_int h;
        public unsigned_int totalSize;
    }

    public static class Sizes
    {
        public const int NC_MAX_NAME_SIZE = 28;
        public const int NC_THERMAL_BUFFER_SIZE = 100;
        public const int NC_DEBUG_BUFFER_SIZE = 120;
        public const int NC_VERSION_MAX_SIZE = 4;
    }
}
