using System;
using System.Collections.Generic;
using System.Text;
using NCAPIv2.Native;
using static NCAPIv2.Native.MVNC;
using static NCAPIv2.Managed.StatusThrows;
using System.Runtime.CompilerServices;

namespace NCAPIv2.Managed
{
    /// <summary>
    /// data and resources corresponding to a FIFO queue.
    /// </summary>
    public unsafe class Fifo : UnmanagedObject
    {
        /// <summary>
        /// the native handle
        /// </summary>
        public ncFifoHandle_t* Handle
        {
            get => (ncFifoHandle_t*)_ptr;
            private set => _ptr = (IntPtr)value;
        }

        /// <summary>
        /// The state of the FIFO
        /// </summary>
        public ncFifoState_t State => GetProperty<ncFifoState_t>(ncFifoOption_t.NC_RO_FIFO_STATE);
        /// <summary>
        /// The data size of one FIFO element in bytes.
        /// </summary>
        public int ElementSize => GetProperty<int>(ncFifoOption_t.NC_RO_FIFO_ELEMENT_DATA_SIZE);
        /// <summary>
        /// If the (write)/(read) will block when (input is full)/(output is empty)
        /// </summary>
        public bool DontBlock
        {
            get => GetProperty<int>(ncFifoOption_t.NC_RW_FIFO_DONT_BLOCK) == 0 ? false : true;
            set => SetProperty<int>(ncFifoOption_t.NC_RW_FIFO_DONT_BLOCK, value ? 1 : 0);
        }
        /// <summary>
        /// The maximum number of elements the FIFO queue can hold
        /// </summary>
        public int Capacity => GetProperty<int>(ncFifoOption_t.NC_RO_FIFO_CAPACITY);
        /// <summary>
        /// The number of tensors (FIFO elements) in the queue for a readable FIFO.
        /// </summary>
        public int ReadFill => GetProperty<int>(ncFifoOption_t.NC_RO_FIFO_READ_FILL_LEVEL);
        /// <summary>
        /// The number of tensors (FIFO elements) in the queue for a writable FIFO.
        /// </summary>
        public int WriteFill => GetProperty<int>(ncFifoOption_t.NC_RO_FIFO_WRITE_FILL_LEVEL);
        /// <summary>
        /// A pointer to the descriptor that describes the shape of tensors that this FIFO will hold
        /// </summary>
        public ncTensorDescriptor_t Descriptor => *(ncTensorDescriptor_t*)GetProperty<IntPtr>(ncFifoOption_t.NC_RO_FIFO_TENSOR_DESCRIPTOR);
        /// <summary>
        /// The type of data that will be placed in the FIFO
        /// </summary>
        public ncFifoDataType_t DataType
        {
            get => GetProperty<ncFifoDataType_t>(ncFifoOption_t.NC_RW_FIFO_DATA_TYPE);
            set => SetProperty<ncFifoDataType_t>(ncFifoOption_t.NC_RW_FIFO_DATA_TYPE, value);
        }
        /// <summary>
        /// The type of FIFO 
        /// </summary>
        public ncFifoType_t Type
        {
            get => GetProperty<ncFifoType_t>(ncFifoOption_t.NC_RW_FIFO_TYPE);
            set => SetProperty<ncFifoType_t>(ncFifoOption_t.NC_RW_FIFO_TYPE, value);
        }

        /// <summary>
        /// construct an empty
        /// </summary>
        /// <param name="name"></param>
        public Fifo(string name)
        {
            ncFifoHandle_t* handle = (ncFifoHandle_t*)IntPtr.Zero;
            fixed (byte* nptr = name.GetCStr(Sizes.NC_MAX_NAME_SIZE))
                ncFifoCreate(nptr, ncFifoType_t.NC_FIFO_HOST_RO, &handle);
        }

        internal Fifo(ncFifoHandle_t* handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// get the results from the queued inference (to managed memory)
        /// </summary>
        /// <param name="outputData"></param>
        public void ReadElement(byte[] outputData)
        {
            var outputElementSize = (uint)ElementSize;
            if (outputData.Length != outputElementSize)
                throw new ArgumentOutOfRangeException(nameof(outputData), $"not the correct size. Expected {outputElementSize}, given {outputData.Length}");
            fixed (byte* optr = outputData)
                Ensure(ncFifoReadElem(Handle, optr, &outputElementSize, (void**)IntPtr.Zero));
        }

        /// <summary>
        /// get the results from the queued inference (to unmanaged memory)
        /// </summary>
        /// <param name="outputData"></param>
        /// <param name="length"></param>
        public void ReadElement(byte* outputData, uint length)
        {
            Ensure(ncFifoReadElem(Handle, outputData, &length, (void**)IntPtr.Zero));
        }

        private T GetProperty<T>(ncFifoOption_t option)
            where T : struct
        {
            var data = new T();
            //uint size = System.Runtime.CompilerServices.Unsafe
            uint size = (uint)Unsafe.SizeOf<T>();
            Ensure(ncFifoGetOption(Handle, option, Unsafe.AsPointer(ref data), &size));
            return data;
        }

        private void SetProperty<T>(ncFifoOption_t option, T data)
            where T : struct
        {
            Ensure(ncFifoSetOption(Handle, option, Unsafe.AsPointer(ref data), (uint)Unsafe.SizeOf<T>()));
        }

        /// <summary>
        /// dispose of the unmanaged data
        /// </summary>
        protected override void DisposeObject()
        {
            var ptr = Handle;
            Ensure(ncFifoDestroy(&ptr));
            Handle = ptr;
        }
    }
}
