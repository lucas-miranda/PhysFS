using PhysFS.IO.Stream;

namespace PhysFS.Core {
    public interface IPhysFSArchiver<ArchiveDataType> {
        uint Version { get; }
        string Extension { get; }
        string Description { get; }
        string Author { get; }
        string Url { get; }
        bool SupportSymLinks { get; }

        ArchiveDataType OpenArchive(IPhysFSStream stream, string name, bool forWrite, out bool claimed);
        PHYSFS_EnumerateCallbackResult Enumerate(ref ArchiveDataType data, string dirname, EnumerateCallback callback, string origDir);
        IPhysFSStream OpenRead(ref ArchiveDataType data, string filename);
        IPhysFSStream OpenWrite(ref ArchiveDataType data, string filename);
        IPhysFSStream OpenAppend(ref ArchiveDataType data, string filename);
        bool Remove(ref ArchiveDataType data, string filename);
        bool Mkdir(ref ArchiveDataType data, string filename);
        bool Stat(ref ArchiveDataType data, string filename, out PHYSFS_Stat stat);
        void CloseArchive(ref ArchiveDataType data);
    }
}
