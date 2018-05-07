using NCAPIv2.Native;
using System;

namespace NCAPIv2.Managed
{
    [Serializable]
    internal class StatusException : Exception
    {
        public StatusException(ncStatus_t status)
            :base(FigureMessage(status))
        {
            Status = status;

        }

        private static string FigureMessage(ncStatus_t status)
        {
            //TODO: a nice message depending on status
            switch(status)
            {
                case ncStatus_t.NC_BUSY: return "Device is busy, retry later";
                case ncStatus_t.NC_ERROR: return "Error communicating with the device";
                case ncStatus_t.NC_OUT_OF_MEMORY: return "Out of memory";
                case ncStatus_t.NC_DEVICE_NOT_FOUND: return "No device at the given index or name";
                case ncStatus_t.NC_INVALID_PARAMETERS: return "At least one of the given parameters is wrong";
                case ncStatus_t.NC_TIMEOUT: return "Timeout in the communication with the device";
                case ncStatus_t.NC_MVCMD_NOT_FOUND: return "The file to boot Myriad was not found";
                case ncStatus_t.NC_NOT_ALLOCATED: return "The graph or device has been closed during the operation";
                case ncStatus_t.NC_UNAUTHORIZED: return "Unauthorized operation";
                case ncStatus_t.NC_UNSUPPORTED_GRAPH_FILE: return "The graph file version is not supported";
                case ncStatus_t.NC_UNSUPPORTED_CONFIGURATION_FILE: return "The configuration file version is not supported";
                case ncStatus_t.NC_UNSUPPORTED_FEATURE: return "Not supported by this FW version";
                case ncStatus_t.NC_MYRIAD_ERROR: return "An error has been reported by the device";
                // use  NC_DEVICE_DEBUG_INFO or NC_GRAPH_DEBUG_INFO
                case ncStatus_t.NC_INVALID_DATA_LENGTH: return "invalid data length has been passed when get/set option";
                case ncStatus_t.NC_INVALID_HANDLE: return "handle to object that is invalid";
                default: return status.ToString();
            }
        }

        public ncStatus_t Status { get; }
    }
}