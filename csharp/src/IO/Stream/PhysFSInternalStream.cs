namespace PhysFS.IO.Stream {
    public class PhysFSInternalStream : IPhysFSStream {
        internal delegate long ReadDelegate(byte[] buffer, ulong len);
        internal delegate long WriteDelegate(byte[] buffer, ulong len);
        internal delegate bool SeekDelegate(ulong offset);
        internal delegate long TellDelegate();
        internal delegate long LengthDelegate();
        internal delegate IPhysFSStream DuplicateDelegate();
        internal delegate bool FlushDelegate();
        internal delegate void DestroyDelegate();

        public uint Version {
            get {
                return 0U;
            }
        }

        public bool CanRead { get; internal set; }
        public bool CanWrite { get; internal set; }

        internal ReadDelegate ReadFunction { get; set; }
        internal WriteDelegate WriteFunction { get; set; }
        internal SeekDelegate SeekFunction { get; set; }
        internal TellDelegate TellFunction { get; set; }
        internal LengthDelegate LengthFunction { get; set; }
        internal DuplicateDelegate DuplicateFunction { get; set; }
        internal FlushDelegate FlushFunction { get; set; }
        internal DestroyDelegate DestroyFunction { get; set; }


        public long Read(byte[] buffer, ulong len) {
            return ReadFunction(buffer, len);
        }

        public long Write(byte[] buffer, ulong len) {
            return WriteFunction(buffer, len);
        }

        public bool Seek(ulong offset) {
            return SeekFunction(offset);
        }

        public long Tell() {
            return TellFunction();
        }

        public long Length() {
            return LengthFunction();
        }

        public IPhysFSStream Duplicate() {
            return DuplicateFunction();
        }

        public bool Flush() {
            return FlushFunction();
        }

        public void Destroy() {
            DestroyFunction();
        }
    }
}
