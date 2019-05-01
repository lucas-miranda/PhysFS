using System;
using System.Runtime.InteropServices;

namespace PhysFS {
    public class PhysFSException : Exception {
        public PhysFSException(PHYSFS_ErrorCode errorCode) : base(Marshal.PtrToStringAnsi(Interop.PHYSFS_getErrorByCode(errorCode))) {
            ErrorCode = errorCode;
        }

        public PHYSFS_ErrorCode ErrorCode { get; private set; }
    }
}
