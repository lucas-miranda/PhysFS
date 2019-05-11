using System;

namespace PhysFS.Util {
    public static class Extensions {
        public static unsafe byte ToInt8(this IntPtr ptr) {
            return *((byte*) ptr.ToPointer());
        }

        public static unsafe short ToInt16(this IntPtr ptr) {
            return *((short*) ptr.ToPointer());
        }
    }
}
