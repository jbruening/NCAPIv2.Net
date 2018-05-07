//based on https://github.com/movidius/ncsdk/commit/f0e4de8c3ec3be5c7d6fc8f715bc09f41725f3d1

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
        unsigned_int n;
        unsigned_int c;
        unsigned_int w;
        unsigned_int h;
        unsigned_int totalSize;
    }
}
