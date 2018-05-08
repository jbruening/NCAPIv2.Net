using System;
using System.Collections.Generic;
using System.Text;
using NCAPIv2.Native;
using static NCAPIv2.Native.MVNC;
using static NCAPIv2.Managed.StatusThrows;
using System.Runtime.CompilerServices;

namespace NCAPIv2.Managed
{
    public unsafe class Graph : UnmanagedObject
    {
        public ncGraphHandle_t* Handle
        {
            get => (ncGraphHandle_t*)_ptr;
            private set => _ptr = (IntPtr)value;
        }

        /// <summary>
        /// The current state of the graph
        /// </summary>
        public ncGraphState_t State => GetProperty<ncGraphState_t>(ncGraphOption_t.NC_RO_GRAPH_STATE);
        /// <summary>
        /// The number of inputs expected by the graph.
        /// </summary>
        public int InputCount => GetProperty<int>(ncGraphOption_t.NC_RO_GRAPH_INPUT_COUNT);
        /// <summary>
        /// The number of outputs expected by the graph.
        /// </summary>
        public int OutputCount => GetProperty<int>(ncGraphOption_t.NC_RO_GRAPH_OUTPUT_COUNT);
        /// <summary>
        /// describe the graph inputs in order.
        /// </summary>
        public ncTensorDescriptor_t[] InputDescriptors => GetPropertyArray<ncTensorDescriptor_t>(ncGraphOption_t.NC_RO_GRAPH_INPUT_TENSOR_DESCRIPTORS, InputCount);
        /// <summary>
        /// describe the graph outputs in order.
        /// </summary>
        public ncTensorDescriptor_t[] OutputDescriptors => GetPropertyArray<ncTensorDescriptor_t>(ncGraphOption_t.NC_RO_GRAPH_OUTPUT_TENSOR_DESCRIPTORS, OutputCount);
        /// <summary>
        /// The highest option class supported.
        /// </summary>
        public int ClassLimit => GetProperty<int>(ncGraphOption_t.NC_RO_GRAPH_OPTION_CLASS_LIMIT);

        public Device Device { get; private set; }
        public Fifo Input { get; private set; }
        public Fifo Output { get; private set; }

        public Graph(string name)
        {
            var handle = (ncGraphHandle_t*)IntPtr.Zero;
            fixed(byte* nptr = name.GetCStr(Sizes.NC_MAX_NAME_SIZE))
                Ensure(ncGraphCreate(nptr, &handle));
            Handle = handle;
        }

        public void Allocate(Device device, byte[] graphBuffer)
        {
            if (Device != null)
                throw new InvalidOperationException("Already allocated");

            fixed (byte* gptr = graphBuffer)
                Ensure(ncGraphAllocate(device.Handle, Handle, gptr, (uint)graphBuffer.Length));

            device.AddGraph(this);
            Device = device;
        }

        public Fifo CreateInput(string name = "input", uint maxElements = 2)
        {
            if (Input != null)
                throw new InvalidOperationException("cannot create input, already created");

            ncFifoHandle_t* handle = (ncFifoHandle_t*)IntPtr.Zero;
            fixed (byte* nptr = name.GetCStr(Sizes.NC_MAX_NAME_SIZE))
                ncFifoCreate(nptr, ncFifoType_t.NC_FIFO_HOST_WO, &handle);

            var desc = InputDescriptors[0];
            Ensure(ncFifoAllocate(handle, Device.Handle, &desc, maxElements));

            return Input = new Fifo(handle);
        }

        public Fifo CreateOutput(string name = "output", uint maxElements = 2)
        {
            if (Output != null)
                throw new InvalidOperationException("cannot create output, already created");

            ncFifoHandle_t* handle = (ncFifoHandle_t*)IntPtr.Zero;
            fixed (byte* nptr = name.GetCStr(Sizes.NC_MAX_NAME_SIZE))
                ncFifoCreate(nptr, ncFifoType_t.NC_FIFO_HOST_RO, &handle);

            var desc = OutputDescriptors[0];
            Ensure(ncFifoAllocate(handle, Device.Handle, &desc, maxElements));

            return Output = new Fifo(handle);
        }

        public void Allocate(Device device, byte[] graphBuffer, out Fifo input, out Fifo output)
        {
            if (Device != null)
                throw new InvalidOperationException("Already allocated");

            ncFifoHandle_t* inputFifo = (ncFifoHandle_t*)IntPtr.Zero;
            ncFifoHandle_t* outputFifo = (ncFifoHandle_t*)IntPtr.Zero;

            fixed (byte* gptr = graphBuffer)
                Ensure(ncGraphAllocateWithFifos(device.Handle, Handle, gptr, (uint)graphBuffer.Length, &inputFifo, &outputFifo));

            Device = device;
            Input = input = new Fifo(inputFifo);
            Output = output = new Fifo(outputFifo);
        }

        public void QueueInference(byte[] imageBuffer)
        {
            var length = (uint)imageBuffer.Length;
            fixed (byte* iptr = imageBuffer)
                Ensure(ncGraphQueueInferenceWithFifoElem(Handle, Input.Handle, Output.Handle, iptr, &length, (void*)IntPtr.Zero));
        }

        private T GetProperty<T>(ncGraphOption_t option)
            where T : struct
        {
            var data = new T();
            //uint size = System.Runtime.CompilerServices.Unsafe
            uint size = (uint)Unsafe.SizeOf<T>();
            Ensure(ncGraphGetOption(Handle, option, Unsafe.AsPointer(ref data), &size));
            return data;
        }

        private T[] GetPropertyArray<T>(ncGraphOption_t option, int length)
            where T : struct
        {
            var data = new T[length];
            uint size = (uint)Unsafe.SizeOf<T>() * (uint)length;
            ref byte dptr = ref Unsafe.As<T, byte>(ref data[0]);
            fixed (byte* ptr = &dptr)
            {
                Ensure(ncGraphGetOption(Handle, option, ptr, &size));
            }

            Array.Resize(ref data, (int)size);

            return data;
        }

        protected override void DisposeObject()
        {
            if (Input != null)
                Input.Dispose();
            if (Output != null)
                Output.Dispose();

            var ptr = Handle;
            Ensure(ncGraphDestroy(&ptr));
            Handle = ptr;
        }
    }
}
