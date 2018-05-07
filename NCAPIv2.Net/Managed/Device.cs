using System;
using System.Collections.Generic;
using System.Text;
using NCAPIv2.Native;
using static NCAPIv2.Native.MVNC;
using static NCAPIv2.Managed.StatusThrows;

namespace NCAPIv2.Managed
{
    public unsafe class Device : UnmanagedObject
    {
        private ncDeviceHandle_t* _handle
        {
            get => (ncDeviceHandle_t*)_ptr;
            set => _ptr = (IntPtr)value;
        }

        public Device(int index)
        {
            var handle = (ncDeviceHandle_t*)IntPtr.Zero;
            Ensure(ncDeviceCreate(index, &handle));
            _ptr = (IntPtr)handle;
        }

        public void Close()
        {
            Ensure(ncDeviceClose(_handle));
        }

        protected override void DisposeObject()
        {
            Close();

            var ptr = _handle;
            Ensure(ncDeviceDestroy(&ptr));
            _handle = ptr;
        }
    }
}
