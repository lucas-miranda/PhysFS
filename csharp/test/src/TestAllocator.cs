using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using PhysFS.Core;

namespace PhysFS.Test {
    public class TestAllocator : IPhysFSAllocator {
        public bool ShowDebugInfo { get; set; }

        public bool Initialize() {
            if (ShowDebugInfo) {
                Debug.WriteLine("Initialize TestAllocator");
            }

            return true;
        }

        public void Deinitialize() {
            if (ShowDebugInfo) {
                Debug.WriteLine("Deinitialize TestAllocator");
            }
        }

        public IntPtr Malloc(ulong bytes) {
            if (bytes == 0UL) {
                return IntPtr.Zero;
            }

            if (ShowDebugInfo) {
                Debug.WriteLine($"Malloc({bytes} bytes)");
            }

            return Marshal.AllocHGlobal((int) bytes);
        }

        public IntPtr Realloc(IntPtr ptr, ulong newSize) {
            if (ptr == IntPtr.Zero) {
                if (ShowDebugInfo) {
                    Debug.WriteLine($"Realloc zero pointer");
                }

                return Malloc(newSize);
            }

            if (newSize == 0UL) {
                if (ShowDebugInfo) {
                    Debug.WriteLine($"Realloc to 0 bytes, freeing pointer");
                }

                Free(ptr);
                return IntPtr.Zero;
            }

            if (ShowDebugInfo) {
                Debug.WriteLine($"Realloc({newSize} bytes)");
            }

            IntPtr size = (IntPtr) newSize;
            return Marshal.ReAllocHGlobal(ptr, size);
        }

        public void Free(IntPtr ptr) {
            if (ptr == IntPtr.Zero) {
                return;
            }

            if (ShowDebugInfo) {
                Debug.WriteLine($"Free");
            }

            Marshal.FreeHGlobal(ptr);
        }
    }
}
