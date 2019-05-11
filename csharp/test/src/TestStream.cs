using System;
using System.Diagnostics;
using PhysFS.Stream;

namespace PhysFS.Test {
    public class TestStream : IPhysIOStream {
        #region Private Members

        private byte[] _buffer;
        private long _pos;

        #endregion Private Members

        #region Constructors

        public TestStream(string filepath) {
            Filepath = filepath;
            _buffer = System.IO.File.ReadAllBytes(filepath);
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

        public IPhysIOStream Duplicate() {
            Debug.WriteLineIf(IsDebugInfoEnabled, $"Duplicating");
            return new TestStream(Filepath);
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
