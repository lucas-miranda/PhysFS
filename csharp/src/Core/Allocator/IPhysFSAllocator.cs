using System;

namespace PhysFS.Core {
    public interface IPhysFSAllocator {
        bool Initialize();
        void Deinitialize();
        IntPtr Malloc(ulong bytes);
        IntPtr Realloc(IntPtr ptr, ulong newSize);
        void Free(IntPtr ptr);
    }
}
