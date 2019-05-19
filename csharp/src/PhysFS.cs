using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using PhysFS.Core;
using PhysFS.IO;
using PhysFS.IO.Stream;
using PhysFS.Util;

namespace PhysFS {
    #region Delegates

    public delegate PHYSFS_EnumerateCallbackResult EnumerateCallback(string dir, string filename);
    public delegate IEnumerator EnumerateEnumeratorCallback(string dir, string filename);

    public delegate void EnumerateSearchPathCallback(string path);
    public delegate void EnumerateCdRomDirsCallback(string path);

    #endregion Delegates

    public class PhysFS {
        #region Private Members

        private static IntPtr _currentAllocatorPtr;

        #endregion Private Members

        #region Constructors

        private PhysFS() {
        }

        #endregion Constructors

        #region Destructors

        ~PhysFS() {
            Deinitialize();
        }

        #endregion Destructors

        #region Public Properties


        public static PhysFS Instance { get; private set; }
        public static bool IsInitialized { get { return Interop.PHYSFS_isInit() != 0; } }

        public static PHYSFS_Version LinkedVersion {
            get {
                int size = Marshal.SizeOf<PHYSFS_Version>();
                IntPtr ptr = Marshal.AllocHGlobal(size);

                Interop.PHYSFS_getLinkedVersion(ptr);

                PHYSFS_Version version = Marshal.PtrToStructure<PHYSFS_Version>(ptr);
                Marshal.FreeHGlobal(ptr);

                return version;
            }
        }

        public static IPhysFSAllocator Allocator {
            get {
                IntPtr physfsAllocatorPtr = Interop.PHYSFS_getAllocator();
                PHYSFS_Allocator physfsAllocator = Marshal.PtrToStructure<PHYSFS_Allocator>(physfsAllocatorPtr);

                PhysFSInternalAllocator internalAllocator = new PhysFSInternalAllocator {
                    InitializeFunction = () => {
                        return physfsAllocator.Init() != 0;
                    },
                    DeinitializeFunction = () => {
                        physfsAllocator.Deinit();
                    },
                    MallocFunction = (ulong bytes) => {
                        return physfsAllocator.Malloc(bytes);
                    },
                    ReallocFunction = (IntPtr ptr, ulong newSize) => {
                        return physfsAllocator.Realloc(ptr, newSize);
                    },
                    FreeFunction = (IntPtr ptr) => {
                        physfsAllocator.Free(ptr);
                    }
                };

                return internalAllocator;
            }

            set {
                if (_currentAllocatorPtr != IntPtr.Zero) {
                    Marshal.FreeHGlobal(_currentAllocatorPtr);
                }

                IPhysFSAllocator allocator = value;

                PHYSFS_Allocator physfsAllocator = new PHYSFS_Allocator {
                    Init = () => {
                        return allocator.Initialize() ? 1 : 0;
                    },
                    Deinit = allocator.Deinitialize,
                    Malloc = allocator.Malloc,
                    Realloc = allocator.Realloc,
                    Free = allocator.Free
                };

                IntPtr physfsAllocatorPtr = Marshal.AllocHGlobal(Marshal.SizeOf<PHYSFS_Allocator>());
                Marshal.StructureToPtr(physfsAllocator, physfsAllocatorPtr, fDeleteOld: false);

                Interop.PHYSFS_setAllocator(physfsAllocatorPtr);
                _currentAllocatorPtr = physfsAllocatorPtr;
            }
        }

        public static string DirSeparator { get { return Marshal.PtrToStringAnsi(Interop.PHYSFS_getDirSeparator()); } }

        public string BaseDirectory { get { return Marshal.PtrToStringAnsi(Interop.PHYSFS_getBaseDir()); } }

        [Obsolete("As of PhysicsFS 2.1, you probably want PhysFS.GetPrefDirectory().")]
        public string UserDirectory { get { return Marshal.PtrToStringAnsi(Interop.PHYSFS_getUserDir()); } }

        public string WriteDirectory {
            get {
                if (!IsInitialized) {
                    throw new PhysFSException(PHYSFS_ErrorCode.PHYSFS_ERR_NOT_INITIALIZED);
                }

                return Marshal.PtrToStringAnsi(Interop.PHYSFS_getWriteDir());
            }

            set {
                if (!IsInitialized) {
                    throw new PhysFSException(PHYSFS_ErrorCode.PHYSFS_ERR_NOT_INITIALIZED);
                }

                int ret = Interop.PHYSFS_setWriteDir(value);
                CheckReturnValue(ret);
            }
        }

        public string[] SearchPaths {
            get {
                IntPtr searchPathArrPtr = Interop.PHYSFS_getSearchPath();
                IntPtr searchPathValuePtr = Marshal.ReadIntPtr(searchPathArrPtr);

                List<string> searchPaths = new List<string>();

                while (searchPathArrPtr != IntPtr.Zero) {
                    string searchPath = Marshal.PtrToStringAnsi(searchPathValuePtr);
                    searchPaths.Add(searchPath);

                    searchPathArrPtr += IntPtr.Size;
                    searchPathValuePtr = Marshal.ReadIntPtr(searchPathArrPtr);
                }

                Interop.PHYSFS_freeList(searchPathArrPtr);

                return searchPaths.ToArray();
            }
        }

        public string[] CdRomDirectories {
            get {
                List<string> cdromDirs = new List<string>();

                IntPtr ptr = Interop.PHYSFS_getCdRomDirs();
                IntPtr valuePtr = Marshal.ReadIntPtr(ptr);

                while (valuePtr != IntPtr.Zero) {
                    cdromDirs.Add(Marshal.PtrToStringAnsi(valuePtr));
                }

                Interop.PHYSFS_freeList(ptr);

                return cdromDirs.ToArray();
            }
        }

        public PHYSFS_ArchiveInfo[] SupportedArchiveTypes {
            get {
                IntPtr archiveTypesPtr = Interop.PHYSFS_supportedArchiveTypes();
                IntPtr valuePtr = Marshal.ReadIntPtr(archiveTypesPtr);

                List<PHYSFS_ArchiveInfo> archiveTypes = new List<PHYSFS_ArchiveInfo>();

                while (valuePtr != IntPtr.Zero) {
                    PHYSFS_ArchiveInfo archiveInfo = Marshal.PtrToStructure<PHYSFS_ArchiveInfo>(valuePtr);
                    archiveTypes.Add(archiveInfo);

                    archiveTypesPtr += IntPtr.Size;
                    valuePtr = Marshal.ReadIntPtr(archiveTypesPtr);
                }

                return archiveTypes.ToArray();
            }
        }

        public bool IsSymbolicLinksPermitted { get { return Interop.PHYSFS_symbolicLinksPermitted() != 0; } set { Interop.PHYSFS_permitSymbolicLink(value ? 1 : 0); } }

        #endregion Public Properties

        #region Public Methods

        public static bool Initialize(string argv0 = null) {
            if (Instance == null) {
                Instance = new PhysFS();
            }

            return Interop.PHYSFS_init(argv0) != 0;
        }

        public static bool Deinitialize() {
            if (Instance == null || !IsInitialized) {
                return false;
            }

            int ret = Interop.PHYSFS_deinit();
            CheckReturnValue(ret);
            Instance = null;
            return true;
        }

        public static PHYSFS_Stat Stat(string filename) {
            IntPtr statPtr = Marshal.AllocHGlobal(Marshal.SizeOf<PHYSFS_Stat>());

            int ret = Interop.PHYSFS_stat(filename, statPtr);

            if (ret == 0) {
                Marshal.FreeHGlobal(statPtr);
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            PHYSFS_Stat stat = Marshal.PtrToStructure<PHYSFS_Stat>(statPtr);
            Marshal.FreeHGlobal(statPtr);

            return stat;
        }

        public static string Utf8ToUtf16(string strUtf8) {
            byte[] nullTerminator = System.Text.Encoding.UTF8.GetBytes("\0");

            // copy utf8Bytes to unmanaged memory
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(strUtf8);
            IntPtr utf8BytesPtr = Marshal.AllocHGlobal((utf8Bytes.Length + nullTerminator.Length) * Marshal.SizeOf<byte>());
            Marshal.Copy(utf8Bytes, 0, utf8BytesPtr, utf8Bytes.Length);

            // append null terminator
            IntPtr nullTerminatorPtr = IntPtr.Add(utf8BytesPtr, utf8Bytes.Length * Marshal.SizeOf<byte>());
            Marshal.Copy(nullTerminator, 0, nullTerminatorPtr, nullTerminator.Length);

            // prepare utf16 string memory space
            int utf16StrSize = 2 * (utf8Bytes.Length + nullTerminator.Length);
            IntPtr utf16BytesPtr = Marshal.AllocHGlobal(utf16StrSize * Marshal.SizeOf<byte>());

            Interop.PHYSFS_utf8ToUtf16(utf8BytesPtr, utf16BytesPtr, (ulong) utf16StrSize);

            // get managed utf16 string
            byte[] utf16Bytes = new byte[utf16StrSize];
            Marshal.Copy(utf16BytesPtr, utf16Bytes, 0, utf16StrSize);
            string stringUtf16 = System.Text.Encoding.Unicode.GetString(utf16Bytes);

            Marshal.FreeHGlobal(utf16BytesPtr);
            Marshal.FreeHGlobal(utf8BytesPtr);

            return stringUtf16;
        }

        public static string Utf8FromUtf16(string strUtf16) {
            byte[] nullTerminator = System.Text.Encoding.Unicode.GetBytes("\0");

            // copy utf16Bytes to unmanaged memory
            byte[] utf16Bytes = System.Text.Encoding.Unicode.GetBytes(strUtf16);
            IntPtr utf16BytesPtr = Marshal.AllocHGlobal((utf16Bytes.Length + nullTerminator.Length) * Marshal.SizeOf<byte>());
            Marshal.Copy(utf16Bytes, 0, utf16BytesPtr, utf16Bytes.Length);

            // append null terminator
            IntPtr nullTerminatorPtr = IntPtr.Add(utf16BytesPtr, utf16Bytes.Length * Marshal.SizeOf<byte>());
            Marshal.Copy(nullTerminator, 0, nullTerminatorPtr, nullTerminator.Length);

            // prepare utf8 string memory space
            int utf8StrSize = utf16Bytes.Length;
            IntPtr utf8BytesPtr = Marshal.AllocHGlobal(utf8StrSize * Marshal.SizeOf<byte>());

            Interop.PHYSFS_utf8FromUtf16(utf16BytesPtr, utf8BytesPtr, (ulong) utf8StrSize);

            // get managed utf8 string
            byte[] utf8Bytes = new byte[utf8StrSize];
            Marshal.Copy(utf8BytesPtr, utf8Bytes, 0, utf8StrSize);
            string stringUtf8 = System.Text.Encoding.UTF8.GetString(utf8Bytes);

            Marshal.FreeHGlobal(utf8BytesPtr);
            Marshal.FreeHGlobal(utf16BytesPtr);

            return stringUtf8;
        }

        //

        public string GetPrefDirectory(string org, string app) {
            IntPtr prefDirPtr = Interop.PHYSFS_getPrefDir(org, app);

            if (prefDirPtr == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            string prefDir = Marshal.PtrToStringAnsi(prefDirPtr);
            Marshal.FreeHGlobal(prefDirPtr);
            return prefDir;
        }

        public bool SetSaneConfig(string organization, string appName, string archiveExt, bool includeCdRoms, bool archivesFirst) {
            int ret = Interop.PHYSFS_setSaneConfig(organization, appName, archiveExt, includeCdRoms ? 1 : 0, archivesFirst ? 1 : 0);

            CheckReturnValue(ret);
            return true;
        }

        public bool Mkdir(string dirName) {
            int ret = Interop.PHYSFS_mkdir(dirName);
            CheckReturnValue(ret);
            return true;
        }

        public bool Delete(string filename) {
            int ret = Interop.PHYSFS_delete(filename);
            CheckReturnValue(ret);
            return true;
        }

        public bool Exists(string filename) {
            return Interop.PHYSFS_exists(filename) != 0;
        }

        public bool Mount(string newDir, string mountPoint, bool appendToPath) {
            int ret = Interop.PHYSFS_mount(newDir, mountPoint, appendToPath ? 1 : 0);

            CheckReturnValue(ret);
            return ret != 0;
        }

        public void MountIOStream(IPhysFSStream ioStream, string newDir, string mountPoint, bool appendToPath) {
            IntPtr ioStructPtr = PrepareIOStruct(ioStream);

            int ret = Interop.PHYSFS_mountIo(ioStructPtr, newDir, mountPoint, appendToPath ? 1 : 0);

            CheckReturnValue(ret);
        }

        public void MountMemory(byte[] buffer, string newDir, string mountPoint, bool appendToPath) {
            IntPtr bufPtr = Marshal.AllocHGlobal(buffer.Length * Marshal.SizeOf<byte>());
            Marshal.Copy(buffer, 0, bufPtr, buffer.Length);

            int ret = Interop.PHYSFS_mountMemory(bufPtr, (ulong) buffer.LongLength, UnmountCallback, newDir, mountPoint, appendToPath ? 1 : 0);

            CheckReturnValue(ret);
            return;

            void UnmountCallback(IntPtr buf) {
                Marshal.FreeHGlobal(buf);
            }
        }

        /// <summary>
        /// Add an archive, contained in a <see cref="PhysFSFile"/>, to the search path.
        /// This method will automatically handle file <see cref="PhysFSFile.Close"/> when archive is unmounted, so don't worry, and *don't* use the <see cref="PhysFSFile"/> reference after unmounting, as it will be already closed. 
        /// If you need to keep <see cref="PhysFSFile"/> alive after unmount, please use <see cref="PhysFS.MountIOStream"/>.
        /// </summary>
        /// <param name="file">The file containing archive data.</param>
        /// <param name="newDir">Filename that can represent this stream.</param>
        /// <param name="mountPoint">Location in the interpoled tree that this archive will be "mounted", in platform-independent notation. null or "" is equivalent to "/".</param>
        /// <param name="appendToPath">True to append to search path, False to prepend.</param>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null or <paramref name="file.Handle"/> is IntPtr.Zero, caused by error at file opening or it's already closed.</exception>
        public void MountHandle(PhysFSFile file, string newDir, string mountPoint, bool appendToPath) {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Handle == IntPtr.Zero) {
                throw new ArgumentNullException(nameof(file.Handle), "Caused by an error at file opening or it's already closed.");
            }

            int ret = Interop.PHYSFS_mountHandle(file.Handle, newDir, mountPoint, appendToPath ? 1 : 0);
            CheckReturnValue(ret);
        }

        public bool Unmount(string oldDir) {
            int ret = Interop.PHYSFS_unmount(oldDir);
            CheckReturnValue(ret);
            return ret != 0;
        }

        public string GetMountPoint(string realPath) {
            IntPtr mountPointStrPtr = Interop.PHYSFS_getMountPoint(realPath);

            if (mountPointStrPtr == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            string mountPoint = Marshal.PtrToStringAnsi(mountPointStrPtr);
            Marshal.FreeHGlobal(mountPointStrPtr);
            return mountPoint;
        }

        public string GetRealDirectory(string filename) {
            IntPtr realDirStrPtr = Interop.PHYSFS_getRealDir(filename);

            if (realDirStrPtr == IntPtr.Zero) {
                return null;
            }

            string realDir = Marshal.PtrToStringAnsi(realDirStrPtr);
            Marshal.FreeHGlobal(realDirStrPtr);
            return realDir;
        }

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS.Stat() instead. This function just wraps it anyhow.", error: false)]
        public long GetLastModTime(string filename) {
            long lastModTime = Interop.PHYSFS_getLastModTime(filename);

            if (lastModTime == -1L) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return lastModTime;
        }

        public IEnumerator<string> EnumerateFiles(string dirName) {
            IntPtr enumerateFilesPtr = Interop.PHYSFS_enumerateFiles(dirName);

            if (enumerateFilesPtr == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            IntPtr enumeratePtr = enumerateFilesPtr;
            IntPtr valuePtr = Marshal.ReadIntPtr(enumeratePtr);
            while (valuePtr != IntPtr.Zero) {
                string value = Marshal.PtrToStringAnsi(valuePtr);

                yield return value;

                enumeratePtr += IntPtr.Size;
                valuePtr = Marshal.ReadIntPtr(enumeratePtr);
            }

            Interop.PHYSFS_freeList(enumerateFilesPtr);
        }

        public void Enumerate(string dirName, EnumerateCallback callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            // Interop callback wrapper
            PHYSFS_EnumerateCallbackResult enumerateCallback(IntPtr data, string origDir, string fname) {
                return callback(origDir, fname);
            }

            int ret = Interop.PHYSFS_enumerate(dirName, enumerateCallback, d: IntPtr.Zero);
            CheckReturnValue(ret);
        }

        public void Enumerate(string dirName, EnumerateEnumeratorCallback callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            // Interop callback wrapper
            PHYSFS_EnumerateCallbackResult enumerateCallback(IntPtr data, string origDir, string fname) {
                PHYSFS_EnumerateCallbackResult result = PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_ERROR;

                IEnumerator e = callback(origDir, fname);
                while (e.MoveNext()) {
                    if (e.Current is bool callbackRet) {
                        if (callbackRet) {
                            result = PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_OK;
                        } else {
                            result = PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_STOP;
                        }
                    }
                }

                return result;
            }

            int ret = Interop.PHYSFS_enumerate(dirName, enumerateCallback, d: IntPtr.Zero);
            CheckReturnValue(ret);
        }

        public void EnumerateSearchPath(EnumerateSearchPathCallback callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            void searchPathCallback(IntPtr data, string str) {
                callback(str);
            }

            Interop.PHYSFS_getSearchPathCallback(searchPathCallback, IntPtr.Zero);
        }

        public void EnumerateCdRomDirs(EnumerateCdRomDirsCallback callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            void cdRomDirsCallback(IntPtr data, string str) {
                callback(str);
            }

            Interop.PHYSFS_getCdRomDirsCallback(cdRomDirsCallback, IntPtr.Zero);
        }

        public void RegisterArchiver<ArchiveDataType>(IPhysFSArchiver<ArchiveDataType> archiver) {
            PHYSFS_Archiver physfsArchiver = new PHYSFS_Archiver {
                Version = archiver.Version,
                Info = new PHYSFS_ArchiveInfo {
                        Extension = archiver.Extension,
                        Description = archiver.Description,
                        Author = archiver.Author,
                        Url = archiver.Url,
                        SupportsSymlinks = archiver.SupportSymLinks ? 1 : 0
                    },
                OpenArchive = (IntPtr io, string name, int forWrite, IntPtr claimed) => {
                    IPhysFSStream internalStream = PrepareIOStream(io);
                    ArchiveDataType data = archiver.OpenArchive(internalStream, name, forWrite != 0, out bool isClaimed);

                    if (!isClaimed) {
                        return IntPtr.Zero;
                    }

                    Marshal.WriteInt32(claimed, 1);
                    IntPtr opaque = Marshal.AllocHGlobal(Marshal.SizeOf<ArchiveDataType>());
                    Marshal.StructureToPtr(data, opaque, fDeleteOld: false);

                    return opaque;
                },
                Enumerate = (IntPtr opaque, string dirname, Interop.PHYSFS_FP_EnumerateCallback callback, string origDir, IntPtr callbackData) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);

                    PHYSFS_EnumerateCallbackResult enumerateCallback(string dir, string filename) {
                        return callback(callbackData, dir, filename);
                    }

                    PHYSFS_EnumerateCallbackResult result = archiver.Enumerate(ref data, dirname, enumerateCallback, origDir);
                    Marshal.StructureToPtr(data, opaque, fDeleteOld: true); // update opaque data
                    return result;
                },
                OpenRead = (IntPtr opaque, string fnm) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);
                    IPhysFSStream stream = archiver.OpenRead(ref data, fnm);
                    Marshal.StructureToPtr(data, opaque, fDeleteOld: true); // update opaque data
                    IntPtr io = PrepareIOStruct(stream);
                    return io;
                },
                OpenWrite = (IntPtr opaque, string filename) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);
                    IPhysFSStream stream = archiver.OpenWrite(ref data, filename);
                    Marshal.StructureToPtr(data, opaque, fDeleteOld: true); // update opaque data
                    IntPtr io = PrepareIOStruct(stream);
                    return io;
                },
                OpenAppend = (IntPtr opaque, string fnm) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);
                    IPhysFSStream stream = archiver.OpenAppend(ref data, fnm);
                    Marshal.StructureToPtr(data, opaque, fDeleteOld: true); // update opaque data
                    IntPtr io = PrepareIOStruct(stream);
                    return io;
                },
                Remove = (IntPtr opaque, string filename) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);
                    bool result = archiver.Remove(ref data, filename);
                    Marshal.StructureToPtr(data, opaque, fDeleteOld: true); // update opaque data
                    return result ? 1 : 0;
                },
                Mkdir = (IntPtr opaque, string filename) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);
                    bool result = archiver.Mkdir(ref data, filename);
                    Marshal.StructureToPtr(data, opaque, fDeleteOld: true); // update opaque data
                    return result ? 1 : 0;
                },
                Stat = (IntPtr opaque, string fn, IntPtr stat) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);
                    if (archiver.Stat(ref data, fn, out PHYSFS_Stat statData)) {
                        Marshal.StructureToPtr(statData, stat, fDeleteOld: false);
                        return 1;
                    }

                    Marshal.StructureToPtr(data, opaque, fDeleteOld: true); // update opaque data
                    return 0;
                },
                CloseArchive = (IntPtr opaque) => {
                    ArchiveDataType data = Marshal.PtrToStructure<ArchiveDataType>(opaque);
                    archiver.CloseArchive(ref data);
                }
            };

            IntPtr physfsArchiverPtr = Marshal.AllocHGlobal(Marshal.SizeOf<PHYSFS_Archiver>());
            Marshal.StructureToPtr(physfsArchiver, physfsArchiverPtr, fDeleteOld: false);

            int ret = Interop.PHYSFS_registerArchiver(physfsArchiverPtr);
            CheckReturnValue(ret);
        }

        public bool DeregisterArchiver(string ext) {
            return Interop.PHYSFS_deregisterArchiver(ext) != 0;
        }

        #endregion Public Methods

        #region Private Methdods

        private static void ParseReturnCodeValue(int ret) {
            PHYSFS_ErrorCode errorCode = (PHYSFS_ErrorCode) ret;
            if (errorCode != PHYSFS_ErrorCode.PHYSFS_ERR_OK) {
                throw new PhysFSException(errorCode);
            }
        }

        private static void CheckReturnValue(int ret) {
            if (ret != 0) {
                return;
            }

            throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
        }

        private static IntPtr PrepareIOStruct(IPhysFSStream ioStream) {
            PHYSFS_Io ioStruct = new PHYSFS_Io {
                Version = ioStream.Version,
                Opaque = IntPtr.Zero,
                Seek = (IntPtr io, ulong offset) => {
                    return ioStream.Seek(offset) ? 1 : 0;
                },
                Tell = (IntPtr io) => {
                    return ioStream.Tell();
                },
                Length = (IntPtr io) => {
                    return ioStream.Length();
                },
                Duplicate = (IntPtr io) => {
                    IPhysFSStream ioStreamClone = ioStream.Duplicate();
                    IntPtr ioStructClonePtr = PrepareIOStruct(ioStreamClone);
                    return ioStructClonePtr;
                },
                Flush = (IntPtr io) => {
                    return ioStream.Flush() ? 1 : 0;
                }
            };

            if (ioStream.CanRead) {
                ioStruct.Read = (IntPtr io, IntPtr buf, ulong len) => {
                    byte[] buffer = new byte[len];
                    long bytesRead = ioStream.Read(buffer, len);

                    if (bytesRead > 0L) {
                        Helper.MarshalCopy(buffer, 0UL, buf, len);
                    }

                    return bytesRead;
                };
            }

            if (ioStream.CanWrite) {
                ioStruct.Write = (IntPtr io, IntPtr buf, ulong len) => {
                    byte[] buffer = new byte[len];
                    Helper.MarshalCopy(buf, buffer, 0UL, len);
                    long bytesWritten = ioStream.Write(buffer, len);
                    return bytesWritten;
                };
            }

            IntPtr ioPtr = Marshal.AllocHGlobal(Marshal.SizeOf<PHYSFS_Io>());

            ioStruct.Destroy = (IntPtr io) => {
                ioStream.Destroy();
                Marshal.FreeHGlobal(ioPtr); // we must free this, otherwise no one will do
            };

            Marshal.StructureToPtr(ioStruct, ioPtr, fDeleteOld: false);

            return ioPtr;
        }

        private static IPhysFSStream PrepareIOStream(IntPtr io) {
            PHYSFS_Io physfsIo = Marshal.PtrToStructure<PHYSFS_Io>(io);

            PhysFSInternalStream internalStream = new PhysFSInternalStream {
                ReadFunction = (byte[] buffer, ulong len) => {
                    IntPtr buf = Marshal.AllocHGlobal(buffer.Length * Marshal.SizeOf<byte>());
                    long bytesRead = physfsIo.Read(io, buf, len);
                    Helper.MarshalCopy(buf, buffer, 0UL, len);
                    Marshal.FreeHGlobal(buf);
                    return bytesRead;
                },
                WriteFunction = (byte[] buffer, ulong len) => {
                    IntPtr buf = Marshal.AllocHGlobal(buffer.Length * Marshal.SizeOf<byte>());
                    Helper.MarshalCopy(buffer, 0UL, buf, len);
                    long bytesWritten = physfsIo.Write(io, buf, len);
                    Marshal.FreeHGlobal(buf);
                    return bytesWritten;
                },
                SeekFunction = (ulong offset) => {
                    int seekRet = physfsIo.Seek(io, offset);
                    return seekRet != 0;
                },
                TellFunction = () => {
                    return physfsIo.Tell(io);
                },
                LengthFunction = () => {
                    return physfsIo.Length(io);
                },
                DuplicateFunction = () => {
                    IntPtr ioClone = physfsIo.Duplicate(io);
                    return PrepareIOStream(ioClone);
                },
                FlushFunction = () => {
                    return physfsIo.Flush(io) != 0;
                },
                DestroyFunction = () => {
                    physfsIo.Destroy(io);
                }
            };

            return internalStream;
        }

        #endregion Private Methdods
    }
}
