using System;
using System.Diagnostics;
using PhysFS.Stream;

namespace PhysFS.Test {
    public class TestStream : IPhysIOStream {
        private byte[] _buffer;
        private long _pos;

        public TestStream() {
            _buffer = new byte[100];

            for (int i = 0; i < _buffer.Length; i++) {
                _buffer[i] = (byte) (_buffer.Length - i);
            }
        }

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

        public long Read(byte[] buffer, ulong len) {
            if (len == 0UL) {
                return -1L;
            }

            if (_pos >= _buffer.LongLength) {
                return 0L;
            } else if (_pos + (long) len > _buffer.LongLength) {
                len = (ulong) (_buffer.LongLength - _pos);
            }

            Debug.WriteLine($"Read {len} bytes");

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

            Debug.WriteLine($"Writing {len} bytes");

            Array.Copy(buffer, 0, _buffer, _pos, (long) len);
            _pos += (long) len;
            return (long) len;
        }

        public bool Seek(ulong offset) {
            Debug.WriteLine($"Seek to {offset} byte");

            if (offset > (ulong) _buffer.LongLength) {
                return false;
            }

            _pos = (long) offset;
            return true;
        }

        public long Tell() {
            Debug.WriteLine($"Current pos: {_pos}");
            return _pos;
        }

        public long Length() {
            Debug.WriteLine($"Total length: {_buffer.LongLength}");
            return _buffer.LongLength;
        }

        public IPhysIOStream Duplicate() {
            Debug.WriteLine($"Duplicating");
            return new TestStream();
        }

        public bool Flush() {
            Debug.WriteLine($"Data flushed");
            return true;
        }

        public void Destroy() {
            Debug.WriteLine($"Destroy");
            _buffer = null;
        }
    }
}
