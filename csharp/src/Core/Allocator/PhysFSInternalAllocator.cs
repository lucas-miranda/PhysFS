using System;

namespace PhysFS.Core {
    public sealed class PhysFSInternalAllocator : IPhysFSAllocator {
        #region Internal Delegates

        internal delegate bool InitializeDelegate();
        internal delegate void DeinitializeDelegate();
        internal delegate IntPtr MallocDelegate(ulong bytes);
        internal delegate IntPtr ReallocDelegate(IntPtr ptr, ulong newSize);
        internal delegate void FreeDelegate(IntPtr ptr);

        #endregion Internal Delegates

        #region Internal Properties

        internal InitializeDelegate InitializeFunction { get; set; }
        internal DeinitializeDelegate DeinitializeFunction { get; set; }
        internal MallocDelegate MallocFunction { get; set; }
        internal ReallocDelegate ReallocFunction { get; set; }
        internal FreeDelegate FreeFunction { get; set; }

        #endregion Internal Properties

        #region Public Methods

        public bool Initialize() {
            return InitializeFunction();
        }

        public void Deinitialize() {
            DeinitializeFunction();
        }

        public IntPtr Malloc(ulong bytes) {
            return MallocFunction(bytes);
        }

        public IntPtr Realloc(IntPtr ptr, ulong newSize) {
            return ReallocFunction(ptr, newSize);
        }

        public void Free(IntPtr ptr) {
            FreeFunction(ptr);
        }

        #endregion Public Methods
    }
}
