using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PhysFS.Stream {
    public class PhysFSFileReader : System.IO.Stream {
        public PhysFSFileReader(string filename) {
            Handle = Interop.PHYSFS_openRead(filename);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        public IntPtr Handle { get; private set; } // IntPtr => PHYSFS_File*
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanSeek { get { return true; } }
        public override long Length { get { return Interop.PHYSFS_fileLength(Handle); } }

        public override long Position {
            get {
                long offsetBytes = Interop.PHYSFS_tell(Handle);

                if (offsetBytes == -1L) {
                    throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
                }

                return offsetBytes;
            }

            set {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush() {
            int ret = Interop.PHYSFS_flush(Handle);

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            IntPtr bufferPtr = Marshal.AllocHGlobal(count);
            long bytesRead = Interop.PHYSFS_readBytes(Handle, bufferPtr, (ulong) count);

            // Maybe only throw exception if bytesRead == -1, since it can be just an EOF "error"
            if (bytesRead < count) {
                Marshal.FreeHGlobal(bufferPtr);
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            Marshal.Copy(bufferPtr, buffer, offset, count);
            Marshal.FreeHGlobal(bufferPtr);

            return (int) bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin) {
            int ret;

            switch (origin) {
                case SeekOrigin.Begin:
                    ret = Interop.PHYSFS_seek(Handle, (ulong) offset);
                    break;

                case SeekOrigin.Current:
                    long currentPosition = Position;
                    ret = Interop.PHYSFS_seek(Handle, (ulong) (currentPosition + offset));
                    break;

                case SeekOrigin.End:
                    long fileLength = Length;
                    ret = Interop.PHYSFS_seek(Handle, (ulong) (fileLength + offset));
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return Position;
        }

        public override void SetLength(long value) {
            int ret = Interop.PHYSFS_setBuffer(Handle, (ulong) value);

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotImplementedException("Can't write from a PhysFSFileReader stream.");
        }
    }
}
