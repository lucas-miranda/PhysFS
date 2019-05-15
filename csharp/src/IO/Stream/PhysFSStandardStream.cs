using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PhysFS.IO.Stream {
    public class PhysFSStandardStream : IPhysFSStream {
        #region Private Members

        private byte[] _buffer;
        private long _pos;

        #endregion Private Members

        #region Constructors

        public PhysFSStandardStream(string filepath) {
            Filepath = filepath;
            _buffer = System.IO.File.ReadAllBytes(filepath);
        }

        public PhysFSStandardStream(byte[] buffer) {
            _buffer = new byte[buffer.LongLength];

            for (long i = 0; i < buffer.LongLength; i++) {
                _buffer[i] = buffer[i];
            }
        }

        public PhysFSStandardStream(IntPtr ptr, int length) {
            _buffer = new byte[length];
            Marshal.Copy(ptr, _buffer, 0, length);
        }

        public PhysFSStandardStream(IntPtr ptr, ulong length) {
            _buffer = new byte[length];
            Util.Helper.MarshalCopy(ptr, _buffer, 0UL, length);
        }

        #endregion Constructors

        #region Public Properties

        public string Filepath { get; }

        public uint Version {
            get {
                return 0U;
            }
        }

        public bool CanRead {
            get {
                return true;
            }
        }

        public bool CanWrite {
            get {
                return true;
            }
        }

        public bool IsDebugInfoEnabled { get; set; }

        #endregion Public Properties

        #region Public Methods

        public long Read(byte[] buffer, ulong len) {
            if (len == 0UL) {
                return -1L;
            }

            if (_pos >= _buffer.LongLength) {
                return 0L;
            } else if (_pos + (long) len > _buffer.LongLength) {
                len = (ulong) (_buffer.LongLength - _pos);
            }

            Debug.WriteLineIf(IsDebugInfoEnabled, $"Read {len} bytes");

            Array.Copy(_buffer, _pos, buffer, 0L, (long) len);
            _pos += (long) len;
            return (long) len;
        }

        public long Write(byte[] buffer, ulong len) {
            if (len == 0UL) {
                return -1L;
            }

            if (_pos + (long) len > _buffer.LongLength) {
                len = (ulong) (_buffer.LongLength - _pos);
            }

            Debug.WriteLineIf(IsDebugInfoEnabled, $"Writing {len} bytes");

            Array.Copy(buffer, 0, _buffer, _pos, (long) len);
            _pos += (long) len;
            return (long) len;
        }

        public bool Seek(ulong offset) {
            Debug.WriteLineIf(IsDebugInfoEnabled, $"Seek to {offset} byte");

            if (offset > (ulong) _buffer.LongLength) {
                return false;
            }

            _pos = (long) offset;
            return true;
        }

        public long Tell() {
            Debug.WriteLineIf(IsDebugInfoEnabled, $"Current pos: {_pos}");
            return _pos;
        }

        public long Length() {
            Debug.WriteLineIf(IsDebugInfoEnabled, $"Total length: {_buffer.LongLength}");
            return _buffer.LongLength;
        }

        public IPhysFSStream Duplicate() {
            Debug.WriteLineIf(IsDebugInfoEnabled, $"Duplicating");
            return new PhysFSStandardStream(_buffer);
        }

        public bool Flush() {
            Debug.WriteLineIf(IsDebugInfoEnabled, $"Data flushed");
            return true;
        }

        public void Destroy() {
            Debug.WriteLineIf(IsDebugInfoEnabled, $"Destroy");
            _buffer = null;
        }

        #endregion Public Methods
    }
}
