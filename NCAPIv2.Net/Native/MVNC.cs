//based on https://github.com/movidius/ncsdk/blob/f0e4de8c3ec3be5c7d6fc8f715bc09f41725f3d1/api/include/mvnc.h

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NCAPIv2.Native
{
    using unsigned_int = UInt32;
    using _char = Byte;

    public unsafe static class MVNC
    {
        #region Global
        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGlobalSetOption(int option, void* data, 
            unsigned_int dataLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGlobalGetOption(int option, void* data, 
            unsigned_int* dataLength);
        #endregion

        #region Device
        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncDeviceSetOption(ncDeviceHandle_t* deviceHandle,
                        int option, void* data,
                        unsigned_int dataLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncDeviceGetOption(ncDeviceHandle_t* deviceHandle,
            int option, void* data, unsigned_int* dataLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncDeviceCreate(int index, ncDeviceHandle_t** deviceHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncDeviceOpen(ncDeviceHandle_t* deviceHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncDeviceClose(ncDeviceHandle_t* deviceHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncDeviceDestroy(ncDeviceHandle_t **deviceHandle);
        #endregion

        #region Graph
        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphCreate(_char* name, ncGraphHandle_t **graphHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphAllocate(ncDeviceHandle_t *deviceHandle,
                        ncGraphHandle_t *graphHandle,
                        void* graphBuffer, unsigned_int graphBufferLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphDestroy(ncGraphHandle_t **graphHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphSetOption(ncGraphHandle_t *graphHandle,
            int option, void* data, unsigned_int dataLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphGetOption(ncGraphHandle_t *graphHandle,
                        int option, void* data,
                        unsigned_int* dataLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphQueueInference(ncGraphHandle_t *graphHandle,
                        ncFifoHandle_t** fifoIn, unsigned_int inFifoCount,
                        ncFifoHandle_t** fifoOut, unsigned_int outFifoCount);
        #endregion

        #region Helper functions
        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphQueueInferenceWithFifoElem(ncGraphHandle_t *graphHandle,
                        ncFifoHandle_t* fifoIn,
                        ncFifoHandle_t* fifoOut, void* inputTensor,
                        unsigned_int* inputTensorLength, void* userParam);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphAllocateWithFifos(ncDeviceHandle_t* deviceHandle,
                        ncGraphHandle_t* graphHandle,
                        void* graphBuffer, unsigned_int graphBufferLength,
                        ncFifoHandle_t ** inFifoHandle,
                        ncFifoHandle_t ** outFifoHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncGraphAllocateWithFifosEx(ncDeviceHandle_t* deviceHandle,
                        ncGraphHandle_t* graphHandle,
                        void* graphBuffer, unsigned_int graphBufferLength,
                        ncFifoHandle_t ** inFifoHandle, ncFifoType_t inFifoType,
                        int inNumElem, ncFifoDataType_t inDataType,
                        ncFifoHandle_t ** outFifoHandle,  ncFifoType_t outFifoType,
                        int outNumElem, ncFifoDataType_t outDataType);
        #endregion

        #region Fifo
        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoCreate(_char* name, ncFifoType_t type,
            ncFifoHandle_t** fifoHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoAllocate(ncFifoHandle_t* fifoHandle,
                        ncDeviceHandle_t* device,
                        ncTensorDescriptor_t* tensorDesc,
                        unsigned_int numElem);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoSetOption(ncFifoHandle_t* fifoHandle, int option,
            void* data, unsigned_int dataLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoGetOption(ncFifoHandle_t* fifoHandle, int option,
            void* data, unsigned_int* dataLength);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoDestroy(ncFifoHandle_t** fifoHandle);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoWriteElem(ncFifoHandle_t* fifoHandle, void* inputTensor,
            unsigned_int* inputTensorLength, void* userParam);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoReadElem(ncFifoHandle_t* fifoHandle, void* outputData,
            unsigned_int* outputDataLen, void** userParam);

        [DllImport(Invoke.Dll, CallingConvention = Invoke.Convention)]
        public static extern ncStatus_t ncFifoRemoveElem(ncFifoHandle_t* fifoHandle); //not supported yet
        #endregion
    }
}
