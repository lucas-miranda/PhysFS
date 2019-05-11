using System;

namespace PhysFS.Util {
    public static class Helper {
        public static unsafe void MarshalCopy(byte[] source, uint startIndex, IntPtr destination, uint length) {
            byte* data = (byte*) destination.ToPointer();

            for (uint i = startIndex; i < length; i++) {
                data[i] = source[i];
            }
        }

        public static unsafe void MarshalCopy(IntPtr source, byte[] destination, uint startIndex, uint length) {
            byte* data = (byte*) source.ToPointer();

            for (uint i = 0U; i < length; i++) {
                destination[startIndex + i] = data[i];
            }
        }

        public static unsafe void MarshalCopy(byte[] source, ulong startIndex, IntPtr destination, ulong length) {
            byte* data = (byte*) destination.ToPointer();

            for (ulong i = startIndex; i < length; i++) {
                data[i] = source[i];
            }
        }

        public static unsafe void MarshalCopy(IntPtr source, byte[] destination, ulong startIndex, ulong length) {
            byte* data = (byte*) source.ToPointer();

            for (ulong i = 0UL; i < length; i++) {
                destination[startIndex + i] = data[i];
            }
        }
    }
}
