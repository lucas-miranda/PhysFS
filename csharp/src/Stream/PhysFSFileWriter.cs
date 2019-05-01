using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PhysFS.Stream {
    public class PhysFSFileWriter : System.IO.Stream {
        #region Constructors

        public PhysFSFileWriter(string filename) {
            IntPtr filenamePtr = Marshal.StringToHGlobalAnsi(filename);
            Handle = Interop.PHYSFS_openWrite(filenamePtr);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            Marshal.FreeHGlobal(filenamePtr);
        }

        #endregion Constructors

        #region Public Properties

        public IntPtr Handle { get; private set; } // IntPtr => PHYSFS_File*
        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }
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

        #endregion Public Properties

        #region Public Methods

        public override void Close() {
            base.Close();

            int ret = Interop.PHYSFS_close(Handle);

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        public override void Flush() {
            int ret = Interop.PHYSFS_flush(Handle);

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotSupportedException("Can't read from a PhysFSFileWriter stream.");
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
            int bufferSize = count * sizeof(byte);
            IntPtr bufferPtr = Marshal.AllocHGlobal(bufferSize);
            Marshal.Copy(buffer, offset, bufferPtr, count);

            long writtenBytes = Interop.PHYSFS_writeBytes(Handle, bufferPtr, (ulong) bufferSize);

            Marshal.FreeHGlobal(bufferPtr);

            if (writtenBytes < count) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (Handle != IntPtr.Zero) {
                int ret = Interop.PHYSFS_close(Handle);

                if (ret == 0) {
                    throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
                }

                Handle = IntPtr.Zero;
            }
        }

        #endregion Protected Methods
    }
}
