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
    /// a neural compute device
    /// </summary>
    public unsafe class Device : UnmanagedObject
    {
        private readonly List<Graph> _graphs = new List<Graph>();

        #region Properties
        /// <summary>
        /// the native handle
        /// </summary>
        public ncDeviceHandle_t* Handle
        {
            get => (ncDeviceHandle_t*)_ptr;
            private set => _ptr = (IntPtr)value;
        }

        /// <summary>
        /// The state of the device
        /// </summary>
        public ncDeviceState_t State => GetProperty<ncDeviceState_t>(ncDeviceOption_t.NC_RO_DEVICE_STATE);
        /// <summary>
        /// The current memory in use on the device in bytes
        /// </summary>
        public int MemoryUsed => GetProperty<int>(ncDeviceOption_t.NC_RO_DEVICE_CURRENT_MEMORY_USED);
        /// <summary>
        /// The total memory available on the device in bytes.
        /// </summary>
        public int MemorySize => GetProperty<int>(ncDeviceOption_t.NC_RO_DEVICE_MEMORY_SIZE);
        /// <summary>
        /// The maximum number of FIFOs that can be allocated for the device.
        /// </summary>
        public int MaxFIFOCount => GetProperty<int>(ncDeviceOption_t.NC_RO_DEVICE_MAX_FIFO_NUM);
        /// <summary>
        /// The hardware version of the device.
        /// </summary>
        public ncDeviceHwVersion_t HardwareVersion => GetProperty<ncDeviceHwVersion_t>(ncDeviceOption_t.NC_RO_DEVICE_HW_VERSION);
        /// <summary>
        /// The number of graphs currently allocated for the device.
        /// </summary>
        public int AllocatedGraphCount => GetProperty<int>(ncDeviceOption_t.NC_RO_DEVICE_ALLOCATED_GRAPH_NUM);
        /// <summary>
        /// The version of the firmware currently running on the device.
        /// </summary>
        public int[] FirmwareVersion => GetPropertyArray<int>(ncDeviceOption_t.NC_RO_DEVICE_FW_VERSION, Sizes.NC_VERSION_MAX_SIZE);
        /// <summary>
        /// the internal name of the device.
        /// </summary>
        public string DeviceName => Encoding.ASCII.GetString(GetPropertyArray<byte>(ncDeviceOption_t.NC_RO_DEVICE_NAME, Sizes.NC_MAX_NAME_SIZE));
        /// <summary>
        /// Device temperatures in degrees Celsius. 
        /// </summary>
        public float[] ThermalStats => GetPropertyArray<float>(ncDeviceOption_t.NC_RO_DEVICE_THERMAL_STATS, Sizes.NC_THERMAL_BUFFER_SIZE);

        /// <summary>
        /// Thermal throttling currently taking place
        /// </summary>
        public ThermalThrottling ThermalThrottling => GetProperty<ThermalThrottling>(ncDeviceOption_t.NC_RO_DEVICE_THERMAL_THROTTLING_LEVEL);
        #endregion

        /// <summary>
        /// Get all available devices plugged in. Only supports ushort.MaxValue devices.
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<ncStatus_t, Device>> GetDevices()
        {
            var ret = new List<Tuple<ncStatus_t, Device>>();

            //I don't imagine someone can plug in more than 65k devices.
            for (var i = 0; i < ushort.MaxValue; i++)
                try
                {
                    ret.Add(Tuple.Create(ncStatus_t.NC_OK, new Device(i)));
                }
                catch (StatusException se)
                {
                    //no more devices
                    if (se.Status == ncStatus_t.NC_DEVICE_NOT_FOUND)
                        return ret;
                    else
                        ret.Add(Tuple.Create(se.Status, (Device)null));
                }

            return ret;
        }

        /// <summary>
        /// create an ncDevice.
        /// Typical multi-device usage is to call this function repeatedly, starting with index = 0 and incrementing the index each time until an error is returned.
        /// </summary>
        /// <param name="index"></param>
        public Device(int index)
        {
            var handle = (ncDeviceHandle_t*)IntPtr.Zero;
            Ensure(ncDeviceCreate(index, &handle));
            Handle = handle;
        }

        #region public methods
        /// <summary>
        /// initializes a neural compute device and opens communication.
        /// </summary>
        public void Open()
        {
            Ensure(ncDeviceOpen(Handle));
        }

        /// <summary>
        /// closes communication with a neural compute device that has been opened
        /// </summary>
        public void Close()
        {
            Ensure(ncDeviceClose(Handle));
        }
        #endregion

        #region protected/private methods
        internal void AddGraph(Graph graph)
        {
            _graphs.Add(graph);
        }

        private T GetProperty<T>(ncDeviceOption_t option)
            where T : struct
        {
            var data = new T();
            //uint size = System.Runtime.CompilerServices.Unsafe
            uint size = (uint)Unsafe.SizeOf<T>();
            Ensure(ncDeviceGetOption(Handle, option, Unsafe.AsPointer(ref data), &size));
            return data;
        }

        private T[] GetPropertyArray<T>(ncDeviceOption_t option, int length)
            where T: struct
        {
            var data = new T[length];
            uint size = (uint)Unsafe.SizeOf<T>() * (uint)length;
            ref byte dptr = ref Unsafe.As<T, byte>(ref data[0]);
            fixed (byte* ptr = &dptr)
            {
                Ensure(ncDeviceGetOption(Handle, option, ptr, &size));
            }

            Array.Resize(ref data, (int)size);

            return data;
        }

        /// <summary>
        /// close (if necessary) and destroy the unmanaged pointer to the device
        /// </summary>
        protected override void DisposeObject()
        {
            foreach (var g in _graphs)
                g.Dispose();
            _graphs.Clear();

            if (State == ncDeviceState_t.NC_DEVICE_OPENED)
                Close();

            var ptr = Handle;
            Ensure(ncDeviceDestroy(&ptr));
            Handle = ptr;
        }
        #endregion
    }
}
