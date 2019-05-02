﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PhysFS {
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

        public string BaseDirectory { get { return Marshal.PtrToStringAnsi(Interop.PHYSFS_getBaseDir()); } }
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

                IntPtr ptr = Marshal.StringToHGlobalAnsi(value);
                int ret = Interop.PHYSFS_setWriteDir(ptr);
                Marshal.FreeHGlobal(ptr);

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

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return true;
        }

        public bool SetSaneConfig(string organization, string appName, string archiveExt, bool includeCdRoms, bool archivesFirst) {
            IntPtr organizationStrPtr = Marshal.StringToHGlobalAnsi(organization),
                   appNameStrPtr = Marshal.StringToHGlobalAnsi(appName),
                   archiveExtStrPtr = Marshal.StringToHGlobalAnsi(archiveExt);

            int ret = Interop.PHYSFS_setSaneConfig(organizationStrPtr, appNameStrPtr, archiveExtStrPtr, Convert.ToInt32(includeCdRoms), Convert.ToInt32(archivesFirst));

            Marshal.FreeHGlobal(organizationStrPtr);
            Marshal.FreeHGlobal(appNameStrPtr);
            Marshal.FreeHGlobal(archiveExtStrPtr);

            CheckReturnValue(ret);
            return true;
        }

        public bool Mkdir(string dirName) {
            IntPtr dirNameStrPtr = Marshal.StringToHGlobalAnsi(dirName);
            int ret = Interop.PHYSFS_mkdir(dirNameStrPtr);
            Marshal.FreeHGlobal(dirNameStrPtr);

            CheckReturnValue(ret);
            return true;
        }

        public bool Delete(string filename) {
            IntPtr filenameStrPtr = Marshal.StringToHGlobalAnsi(filename);
            int ret = Interop.PHYSFS_delete(filenameStrPtr);
            Marshal.FreeHGlobal(filenameStrPtr);

            CheckReturnValue(ret);
            return true;
        }

        public bool Exists(string filename) {
            IntPtr filenameStrPtr = Marshal.StringToHGlobalAnsi(filename);
            int ret = Interop.PHYSFS_exists(filenameStrPtr);
            Marshal.FreeHGlobal(filenameStrPtr);
            return ret != 0;
        }

        public bool Mount(string newDir, string mountPoint, bool appendToPath) {
            IntPtr newDirStrPtr = Marshal.StringToHGlobalAnsi(newDir),
                   mountPointStrPtr = Marshal.StringToHGlobalAnsi(mountPoint);

            int ret = Interop.PHYSFS_mount(newDirStrPtr, mountPointStrPtr, Convert.ToInt32(appendToPath));

            CheckReturnValue(ret);
            return ret != 0;
        }

        public bool Unmount(string oldDir) {
            IntPtr oldDirStrPtr = Marshal.StringToHGlobalAnsi(oldDir);
            int ret = Interop.PHYSFS_unmount(oldDirStrPtr);
            CheckReturnValue(ret);
            return ret != 0;
        }

        public string GetRealDirectory(string filename) {
            IntPtr filenameStrPtr = Marshal.StringToHGlobalAnsi(filename);
            IntPtr realDirStrPtr = Interop.PHYSFS_getRealDir(filenameStrPtr);
            Marshal.FreeHGlobal(filenameStrPtr);

            if (realDirStrPtr == IntPtr.Zero) {
                return null;
            }

            string realDir = Marshal.PtrToStringAnsi(realDirStrPtr);
            Marshal.FreeHGlobal(realDirStrPtr);

            return realDir;
        }

        public IEnumerator<string> EnumerateFiles(string dirName) {
            IntPtr dirStrPtr = Marshal.StringToHGlobalAnsi(dirName);
            IntPtr enumerateFilesPtr = Interop.PHYSFS_enumerateFiles(dirStrPtr);

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
            Marshal.FreeHGlobal(dirStrPtr);
        }

        #endregion Public Methods

        #region Private Methdods

        private void ParseReturnCodeValue(int ret) {
            PHYSFS_ErrorCode errorCode = (PHYSFS_ErrorCode) ret;
            if (errorCode != PHYSFS_ErrorCode.PHYSFS_ERR_OK) {
                throw new PhysFSException(errorCode);
            }
        }

        private void CheckReturnValue(int ret) {
            if (ret != 0) {
                return;
            }

            throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
        }

        #endregion Private Methdods
    }
}