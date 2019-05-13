namespace PhysFS.IO.Stream {
    public interface IPhysFSStream {
        uint Version { get; }
        bool CanRead { get; }
        bool CanWrite { get; }

        long Read(byte[] buffer, ulong len);
        long Write(byte[] buffer, ulong len);
        bool Seek(ulong offset);
        long Tell();
        long Length();
        IPhysFSStream Duplicate();
        bool Flush();
        void Destroy();
    }
}
