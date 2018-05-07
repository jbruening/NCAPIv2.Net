using NCAPIv2.Native;
using System;

namespace NCAPIv2.Managed
{
    static class StatusThrows
    {
        public static void Ensure(ncStatus_t status)
        {
            if (status == ncStatus_t.NC_OK)
                return;

            throw new StatusException(status);
        }
    }
}
