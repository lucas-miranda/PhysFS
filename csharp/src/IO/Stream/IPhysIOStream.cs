namespace PhysFS.IO.Stream {
    public interface IPhysIOStream {
        uint Version { get; }
        bool CanRead { get; }
        bool CanWrite { get; }

        long Read(byte[] buffer, ulong len);
        long Write(byte[] buffer, ulong len);
        bool Seek(ulong offset);
        long Tell();
        long Length();
        IPhysIOStream Duplicate();
        bool Flush();
        void Destroy();
    }
}
