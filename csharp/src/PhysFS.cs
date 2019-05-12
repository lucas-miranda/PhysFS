using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using PhysFS.Stream;
using PhysFS.Util;

namespace PhysFS {
    #region Delegates

    public delegate PHYSFS_EnumerateCallbackResult EnumerateCallback(string dir, string filename);
    public delegate IEnumerator EnumerateEnumeratorCallback(string dir, string filename);

    public delegate void EnumerateSearchPathCallback(string path);
    public delegate void EnumerateCdRomDirsCallback(string path);

    #endregion Delegates

    public class PhysFS {
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

        public void MountIOStream(IPhysIOStream ioStream, string newDir, string mountPoint, bool appendToPath) {
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
            PHYSFS_EnumerateCallbackResult enumerateCallback(IntPtr data, IntPtr origDir, IntPtr fname) {
                string dir = Marshal.PtrToStringAnsi(origDir),
                       filename = Marshal.PtrToStringAnsi(fname);

                return callback(dir, filename);
            }

            int ret = Interop.PHYSFS_enumerate(dirName, enumerateCallback, d: IntPtr.Zero);
            CheckReturnValue(ret);
        }

        public void Enumerate(string dirName, EnumerateEnumeratorCallback callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            // Interop callback wrapper
            PHYSFS_EnumerateCallbackResult enumerateCallback(IntPtr data, IntPtr origDir, IntPtr fname) {
                string dir = Marshal.PtrToStringAnsi(origDir),
                       filename = Marshal.PtrToStringAnsi(fname);

                PHYSFS_EnumerateCallbackResult result = PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_ERROR;

                IEnumerator e = callback(dir, filename);
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

            void searchPathCallback(IntPtr data, IntPtr str) {
                string path = Marshal.PtrToStringAnsi(str);
                callback(path);
            }

            Interop.PHYSFS_getSearchPathCallback(searchPathCallback, IntPtr.Zero);
        }

        public void EnumerateCdRomDirs(EnumerateCdRomDirsCallback callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            void cdRomDirsCallback(IntPtr data, IntPtr str) {
                string path = Marshal.PtrToStringAnsi(str);
                callback(path);
            }

            Interop.PHYSFS_getCdRomDirsCallback(cdRomDirsCallback, IntPtr.Zero);
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

        private static IntPtr PrepareIOStruct(IPhysIOStream ioStream) {
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
                    IPhysIOStream ioStreamClone = ioStream.Duplicate();
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

        #endregion Private Methdods
    }
}
