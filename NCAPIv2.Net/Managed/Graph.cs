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
    /// data and resources corresponding to network graph.
    /// </summary>
    public unsafe class Graph : UnmanagedObject
    {
        /// <summary>
        /// the native handle
        /// </summary>
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

        /// <summary>
        /// the device this graph has been allocated to
        /// </summary>
        public Device Device { get; private set; }
        /// <summary>
        /// the input fifo
        /// </summary>
        public Fifo Input { get; private set; }
        /// <summary>
        /// the output fifo
        /// </summary>
        public Fifo Output { get; private set; }

        /// <summary>
        /// create a new graph
        /// </summary>
        /// <param name="name"></param>
        public Graph(string name)
        {
            var handle = (ncGraphHandle_t*)IntPtr.Zero;
            fixed(byte* nptr = name.GetCStr(Sizes.NC_MAX_NAME_SIZE))
                Ensure(ncGraphCreate(nptr, &handle));
            Handle = handle;
        }

        /// <summary>
        /// allocate this graph to the specified device, with the specified graph from memory
        /// </summary>
        /// <param name="device"></param>
        /// <param name="graphBuffer"></param>
        public void Allocate(Device device, byte[] graphBuffer)
        {
            if (Device != null)
                throw new InvalidOperationException("Already allocated");

            fixed (byte* gptr = graphBuffer)
                Ensure(ncGraphAllocate(device.Handle, Handle, gptr, (uint)graphBuffer.Length));

            device.AddGraph(this);
            Device = device;
        }

        /// <summary>
        /// set up up the fifo input queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxElements"></param>
        /// <returns></returns>
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


        /// <summary>
        /// set up the fifo output queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxElements"></param>
        /// <returns></returns>
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

        /// <summary>
        /// allocate to the device with the specified graph, as well as setting up the input and output fifos
        /// </summary>
        /// <param name="device"></param>
        /// <param name="graphBuffer"></param>
        public void AllocateWithFifos(Device device, byte[] graphBuffer)
        {
            if (Device != null)
                throw new InvalidOperationException("Already allocated");

            ncFifoHandle_t* inputFifo = (ncFifoHandle_t*)IntPtr.Zero;
            ncFifoHandle_t* outputFifo = (ncFifoHandle_t*)IntPtr.Zero;

            fixed (byte* gptr = graphBuffer)
                Ensure(ncGraphAllocateWithFifos(device.Handle, Handle, gptr, (uint)graphBuffer.Length, &inputFifo, &outputFifo));

            Device = device;
            Input = new Fifo(inputFifo);
            Output = new Fifo(outputFifo);
        }

        /// <summary>
        /// Queue inference of the specified data
        /// </summary>
        /// <param name="buffer"></param>
        public void QueueInference(byte[] buffer)
        {
            var length = (uint)buffer.Length;
            fixed (byte* iptr = buffer)
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

        /// <summary>
        /// dispose of the unmanaged data
        /// </summary>
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
