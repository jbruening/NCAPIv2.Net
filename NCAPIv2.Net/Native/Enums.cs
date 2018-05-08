//based on https://github.com/movidius/ncsdk/blob/f0e4de8c3ec3be5c7d6fc8f715bc09f41725f3d1/api/include/mvnc.h
#pragma warning disable 1591
namespace NCAPIv2.Native
{
    public enum ncStatus_t : int
    {
        NC_OK = 0,
        NC_BUSY = -1,                     // Device is busy, retry later
        NC_ERROR = -2,                    // Error communicating with the device
        NC_OUT_OF_MEMORY = -3,            // Out of memory
        NC_DEVICE_NOT_FOUND = -4,         // No device at the given index or name
        NC_INVALID_PARAMETERS = -5,       // At least one of the given parameters is wrong
        NC_TIMEOUT = -6,                  // Timeout in the communication with the device
        NC_MVCMD_NOT_FOUND = -7,          // The file to boot Myriad was not found
        NC_NOT_ALLOCATED = -8,            // The graph or device has been closed during the operation
        NC_UNAUTHORIZED = -9,             // Unauthorized operation
        NC_UNSUPPORTED_GRAPH_FILE = -10,  // The graph file version is not supported
        NC_UNSUPPORTED_CONFIGURATION_FILE = -11, // The configuration file version is not supported
        NC_UNSUPPORTED_FEATURE = -12,     // Not supported by this FW version
        NC_MYRIAD_ERROR = -13,            // An error has been reported by the device
                                          // use  NC_DEVICE_DEBUG_INFO or NC_GRAPH_DEBUG_INFO
        NC_INVALID_DATA_LENGTH = -14,      // invalid data length has been passed when get/set option
        NC_INVALID_HANDLE = -15 // handle to object that is invalid
    }

    public enum ncGlobalOption_t : int
    {
        NC_RW_LOG_LEVEL = 0,    // Log level, int, MVLOG_DEBUG = 0, debug and above (full verbosity)
                                // MVLOG_INFO = 1, info and above
                                // MVLOG_WARN = 2, warnings and above
                                // MVLOG_ERROR = 3, errors and above
                                // MVLOG_FATAL = 4, fatal only
        NC_RO_API_VERSION = 1, // retruns API Version. array of unsigned int of size 4
    }

    public enum ncGraphOption_t : int
    {
        NC_RO_GRAPH_STATE = 1000,           // Returns graph state: CREATED, ALLOCATED, WAITING_FOR_BUFFERS, RUNNING, DESTROYED
        NC_RO_GRAPH_TIME_TAKEN = 1001,      // Return time taken for last inference (float *)
        NC_RO_GRAPH_INPUT_COUNT = 1002,     // Returns number of inputs, size of array returned
                                            // by NC_RO_INPUT_TENSOR_DESCRIPTORS, int
        NC_RO_GRAPH_OUTPUT_COUNT = 1003,    // Returns number of outputs, size of array returned
                                            // by NC_RO_OUTPUT_TENSOR_DESCRIPTORS,int
        NC_RO_GRAPH_INPUT_TENSOR_DESCRIPTORS = 1004,  // Return a tensorDescriptor pointer array
                                                      // which describes the graph inputs in order.
                                                      // Can be used for fifo creation.
                                                      // The length of the array can be retrieved
                                                      // using the NC_RO_INPUT_COUNT option

        NC_RO_GRAPH_OUTPUT_TENSOR_DESCRIPTORS = 1005, // Return a tensorDescriptor pointer
                                                      // array which describes the graph
                                                      // outputs in order. Can be used for
                                                      // fifo creation. The length of the
                                                      // array can be retrieved using the
                                                      // NC_RO_OUTPUT_COUNT option

        NC_RO_GRAPH_DEBUG_INFO = 1006,          // Return debug info, string
        NC_RO_GRAPH_NAME = 1007,                // Returns name of the graph, string
        NC_RO_GRAPH_OPTION_CLASS_LIMIT = 1008,  // return the highest option class supported
        NC_RO_GRAPH_VERSION = 1009,             // returns graph version, string
        NC_RO_GRAPH_TIME_TAKEN_ARRAY_SIZE = 1011, // Return size of array for time taken option, int
        NC_RW_GRAPH_EXECUTORS_NUM = 1110,
    }

    public enum ncDeviceState_t : int
    {
        NC_DEVICE_CREATED = 0,
        NC_DEVICE_OPENED = 1,
        NC_DEVICE_CLOSED = 2,
    }

    public enum ncGraphState_t : int
    {
        NC_GRAPH_CREATED = 0,
        NC_GRAPH_ALLOCATED = 1,
        NC_GRAPH_WAITING_FOR_BUFFERS = 2,
        NC_GRAPH_RUNNING = 3,
    }

    public enum ncFifoState_t : int
    {
        NC_FIFO_CREATED = 0,
        NC_FIFO_ALLOCATED = 1,
    }

    public enum ncDeviceHwVersion_t : int
    {
        NC_MA2450 = 0,
        NC_MA2480 = 1,
    }

    public enum ncDeviceOption_t : int
    {
        NC_RO_DEVICE_THERMAL_STATS = 2000,          // Return temperatures, float *, not for general use
        NC_RO_DEVICE_THERMAL_THROTTLING_LEVEL = 2001,   // 1=TEMP_LIM_LOWER reached, 2=TEMP_LIM_HIGHER reached
        NC_RO_DEVICE_STATE = 2002,                  // Returns device state: CREATED, OPENED, CLOSED, DESTROYED
        NC_RO_DEVICE_CURRENT_MEMORY_USED = 2003,    // Returns current device memory usage
        NC_RO_DEVICE_MEMORY_SIZE = 2004,            // Returns device memory size
        NC_RO_DEVICE_MAX_FIFO_NUM = 2005,           // return the maximum number of fifos supported
        NC_RO_DEVICE_ALLOCATED_FIFO_NUM = 2006,     // return the number of currently allocated fifos
        NC_RO_DEVICE_MAX_GRAPH_NUM = 2007,          // return the maximum number of graphs supported
        NC_RO_DEVICE_ALLOCATED_GRAPH_NUM = 2008,    //  return the number of currently allocated graphs
        NC_RO_DEVICE_OPTION_CLASS_LIMIT = 2009,     //  return the highest option class supported
        NC_RO_DEVICE_FW_VERSION = 2010,             // return device firmware version, array of unsigned int of size 4
                                                    //major.minor.hwtype.buildnumber
        NC_RO_DEVICE_DEBUG_INFO = 2011,             // Return debug info, string, not supported yet
        NC_RO_DEVICE_MVTENSOR_VERSION = 2012,       // returns mv tensor version, array of unsigned int of size 2
                                                    //major.minor
        NC_RO_DEVICE_NAME = 2013,                   // returns device name as generated internally
        NC_RO_DEVICE_MAX_EXECUTORS_NUM = 2014,      //Maximum number of executers per graph
        NC_RO_DEVICE_HW_VERSION = 2015, //returns HW Version, enum
    }

    public enum ncFifoType_t : int
    {
        NC_FIFO_HOST_RO = 0, // fifo can be read through the API but can not be
                             // written ( graphs can read and write data )
        NC_FIFO_HOST_WO = 1, // fifo can be written through the API but can not be
                             // read (graphs can read but can not write)
    }

    public enum ncFifoDataType_t : int
    {
        NC_FIFO_FP16 = 0,
        NC_FIFO_FP32 = 1,
    }

    public enum ncFifoOption_t : int
    {
        NC_RW_FIFO_TYPE = 0,            // configure the fifo type to one type from ncFifoType_t
        NC_RW_FIFO_CONSUMER_COUNT = 1,  // The number of consumers of elements
                                        // (the number of times data must be read by
                                        // a graph or host before the element is removed.
                                        // Defaults to 1. Host can read only once always.
        NC_RW_FIFO_DATA_TYPE = 2,       // 0 for fp16, 1 for fp32. If configured to fp32,
                                        // the API will convert the data to the internal
                                        // fp16 format automatically
        NC_RW_FIFO_DONT_BLOCK = 3,      // WriteTensor will return NC_OUT_OF_MEMORY instead
                                        // of blocking, GetResult will return NO_DATA, not supported yet
        NC_RO_FIFO_CAPACITY = 4,        // return number of maximum elements in the buffer
        NC_RO_FIFO_READ_FILL_LEVEL = 5,     // return number of tensors in the read buffer
        NC_RO_FIFO_WRITE_FILL_LEVEL = 6,    // return number of tensors in a write buffer
        NC_RO_FIFO_TENSOR_DESCRIPTOR = 7,   // return the tensor descriptor of the FIFO
        NC_RO_FIFO_STATE = 8,               // return the fifo state, returns CREATED, ALLOCATED,DESTROYED
        NC_RO_FIFO_NAME = 9,                // return fifo name
        NC_RO_FIFO_ELEMENT_DATA_SIZE = 10,  //element data size in bytes, int
    }
}
#pragma warning restore 1591