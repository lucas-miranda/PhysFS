using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PhysFS.IO.Stream {
    public class PhysFSStream : System.IO.Stream {
        #region Private Members

        private readonly bool _canRead, _canWrite;

        #endregion Private Members

        #region Constructors

        public PhysFSStream(PhysFSFile file, bool autoClose = true) {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Handle == IntPtr.Zero) {
                throw new ArgumentException("File contains an invalid Handle or it's already closed.", nameof(file));
            }

            File = file;
            AutoClose = autoClose;

            switch (file.Mode) {
                case FileMode.Read:
                    _canRead = true;
                    break;

                case FileMode.Write:
                case FileMode.Append:
                    _canWrite = true;
                    break;

                default:
                    throw new NotSupportedException($"Unsupported FileMode '{file.Mode}'");
            }
        }

        #endregion Constructors

        #region Public Properties

        public PhysFSFile File { get; private set; }
        public override bool CanRead { get { return _canRead; } }
        public override bool CanWrite { get { return _canWrite; } }
        public override bool CanSeek { get { return true; } }
        public bool AutoClose { get; }
        public override long Length { get { return Interop.PHYSFS_fileLength(File.Handle); } }

        public override long Position {
            get {
                long offsetBytes = Interop.PHYSFS_tell(File.Handle);

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

        public override void Flush() {
            int ret = Interop.PHYSFS_flush(File.Handle);

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr bufferPtr = Marshal.AllocHGlobal(count);
            long bytesRead = Interop.PHYSFS_readBytes(File.Handle, bufferPtr, (ulong) count);

            // Maybe only throw exception if bytesRead == -1, since it can be just an EOF "error"
            if (bytesRead < count) {
                Marshal.FreeHGlobal(bufferPtr);
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            Marshal.Copy(bufferPtr, buffer, offset, count);
            Marshal.FreeHGlobal(bufferPtr);

            return (int) bytesRead;
        }

        public bool ReadSLE16(ref short value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<short>());
            int ret = Interop.PHYSFS_readSLE16(File.Handle, valuePtr);
            value = Marshal.ReadInt16(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadULE16(ref ushort value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<ushort>());
            int ret = Interop.PHYSFS_readULE16(File.Handle, valuePtr);
            value = (ushort) Marshal.ReadInt16(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadSLE32(ref int value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<int>());
            int ret = Interop.PHYSFS_readSLE32(File.Handle, valuePtr);
            value = Marshal.ReadInt32(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadULE32(ref uint value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<uint>());
            int ret = Interop.PHYSFS_readSLE32(File.Handle, valuePtr);
            value = (uint) Marshal.ReadInt32(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadSLE64(ref long value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<long>());
            int ret = Interop.PHYSFS_readSLE32(File.Handle, valuePtr);
            value = Marshal.ReadInt64(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadULE64(ref ulong value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<ulong>());
            int ret = Interop.PHYSFS_readSLE32(File.Handle, valuePtr);
            value = (ulong) Marshal.ReadInt64(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadSBE16(ref short value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<short>());
            int ret = Interop.PHYSFS_readSBE16(File.Handle, valuePtr);
            value = Marshal.ReadInt16(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadUBE16(ref ushort value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<ushort>());
            int ret = Interop.PHYSFS_readUBE16(File.Handle, valuePtr);
            value = (ushort) Marshal.ReadInt16(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadSBE32(ref int value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<int>());
            int ret = Interop.PHYSFS_readSBE32(File.Handle, valuePtr);
            value = Marshal.ReadInt32(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadUBE32(ref uint value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<uint>());
            int ret = Interop.PHYSFS_readSBE32(File.Handle, valuePtr);
            value = (uint) Marshal.ReadInt32(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadSBE64(ref long value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<long>());
            int ret = Interop.PHYSFS_readSBE32(File.Handle, valuePtr);
            value = Marshal.ReadInt64(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public bool ReadUBE64(ref ulong value) {
            if (!CanRead) {
                throw new NotSupportedException("Can't read, File is oppened to write only.");
            }

            IntPtr valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<ulong>());
            int ret = Interop.PHYSFS_readSBE32(File.Handle, valuePtr);
            value = (ulong) Marshal.ReadInt64(valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            return ret != 0;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int bufferSize = count * sizeof(byte);
            IntPtr bufferPtr = Marshal.AllocHGlobal(bufferSize);
            Marshal.Copy(buffer, offset, bufferPtr, count);

            long writtenBytes = Interop.PHYSFS_writeBytes(File.Handle, bufferPtr, (ulong) bufferSize);

            Marshal.FreeHGlobal(bufferPtr);

            if (writtenBytes < count) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }
        }

        public bool WriteSLE16(short value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeSLE16(File.Handle, value);
            return ret != 0;
        }

        public bool WriteULE16(ushort value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeULE16(File.Handle, value);
            return ret != 0;
        }

        public bool WriteSBE16(short value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeSBE16(File.Handle, value);
            return ret != 0;
        }

        public bool WriteUBE16(ushort value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeUBE16(File.Handle, value);
            return ret != 0;
        }

        public bool WriteSLE32(short value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeSLE32(File.Handle, value);
            return ret != 0;
        }

        public bool WriteULE32(ushort value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeULE32(File.Handle, value);
            return ret != 0;
        }

        public bool WriteSBE32(short value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeSBE32(File.Handle, value);
            return ret != 0;
        }

        public bool WriteUBE32(ushort value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeUBE32(File.Handle, value);
            return ret != 0;
        }

        public bool WriteSLE64(short value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeSLE64(File.Handle, value);
            return ret != 0;
        }

        public bool WriteULE64(ushort value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeULE64(File.Handle, value);
            return ret != 0;
        }

        public bool WriteSBE64(short value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeSBE64(File.Handle, value);
            return ret != 0;
        }

        public bool WriteUBE64(ushort value) {
            if (!CanWrite) {
                throw new NotSupportedException("Can't write, File is oppened to read only.");
            }

            int ret = Interop.PHYSFS_writeUBE64(File.Handle, value);
            return ret != 0;
        }

        public override long Seek(long offset, SeekOrigin origin) {
            ulong pos;

            switch (origin) {
                case SeekOrigin.Begin:
                    pos = (ulong) offset;
                    break;

                case SeekOrigin.Current:
                    pos = (ulong) (Position + offset);
                    break;

                case SeekOrigin.End:
                    pos = (ulong) (Length + offset);
                    break;

                default:
                    throw new NotImplementedException($"Invalid SeekOrigin '{origin}'");
            }

            int ret = Interop.PHYSFS_seek(File.Handle, pos);

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return Position;
        }

        public override void SetLength(long value) {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (AutoClose) {
                File.Close();
            }
        }

        #endregion Protected Methods
    }
}
