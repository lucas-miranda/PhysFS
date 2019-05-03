using System;
using System.Runtime.InteropServices;

namespace PhysFS {
    #region Enums
    
    public enum PHYSFS_EnumerateCallbackResult {
        PHYSFS_ENUM_ERROR = -1,
        PHYSFS_ENUM_STOP = 0,
        PHYSFS_ENUM_OK = 1
    }

    public enum PHYSFS_ErrorCode {
        PHYSFS_ERR_OK,               
        PHYSFS_ERR_OTHER_ERROR,      
        PHYSFS_ERR_OUT_OF_MEMORY,    
        PHYSFS_ERR_NOT_INITIALIZED,  
        PHYSFS_ERR_IS_INITIALIZED,   
        PHYSFS_ERR_ARGV0_IS_NULL,    
        PHYSFS_ERR_UNSUPPORTED,      
        PHYSFS_ERR_PAST_EOF,         
        PHYSFS_ERR_FILES_STILL_OPEN, 
        PHYSFS_ERR_INVALID_ARGUMENT, 
        PHYSFS_ERR_NOT_MOUNTED,      
        PHYSFS_ERR_NOT_FOUND,        
        PHYSFS_ERR_SYMLINK_FORBIDDEN,
        PHYSFS_ERR_NO_WRITE_DIR,     
        PHYSFS_ERR_OPEN_FOR_READING, 
        PHYSFS_ERR_OPEN_FOR_WRITING, 
        PHYSFS_ERR_NOT_A_FILE,       
        PHYSFS_ERR_READ_ONLY,        
        PHYSFS_ERR_CORRUPT,          
        PHYSFS_ERR_SYMLINK_LOOP,     
        PHYSFS_ERR_IO,               
        PHYSFS_ERR_PERMISSION,       
        PHYSFS_ERR_NO_SPACE,         
        PHYSFS_ERR_BAD_FILENAME,     
        PHYSFS_ERR_BUSY,             
        PHYSFS_ERR_DIR_NOT_EMPTY,    
        PHYSFS_ERR_OS_ERROR,         
        PHYSFS_ERR_DUPLICATE,        
        PHYSFS_ERR_BAD_PASSWORD,     
        PHYSFS_ERR_APP_CALLBACK 
    }

    public enum PHYSFS_FileType {
        PHYSFS_FILETYPE_REGULAR, 
        PHYSFS_FILETYPE_DIRECTORY, 
        PHYSFS_FILETYPE_SYMLINK, 
        PHYSFS_FILETYPE_OTHER 
    }

    #endregion Enums

    #region Function Pointer Types

    // Callbacks

    // PHYSFS_EnumerateCallbackResult (*PHYSFS_EnumerateCallback)(void *data, const char *origdir, const char *fname);
    public delegate PHYSFS_EnumerateCallbackResult PHYSFS_FP_EnumerateCallback(IntPtr data, string origDir, string fname);

    // void (*PHYSFS_StringCallback)(void *data, const char *str);
    public delegate void PHYSFS_FP_StringCallback(IntPtr data, string str);

    // void (*PHYSFS_EnumFilesCallback)(void *data, const char *origdir, const char *fname);
    public delegate void PHYSFS_FP_EnumFilesCallback(IntPtr data, string origdir, string fname);

    // PHYSFS_Allocator
    public delegate int PHYSFS_FP_Init();
    public delegate void PHYSFS_FP_Deinit();
    public delegate IntPtr PHYSFS_FP_Malloc(ulong size);
    public delegate IntPtr PHYSFS_FP_Realloc(IntPtr handle, ulong newSize);
    public delegate void PHYSFS_FP_Free(IntPtr handle);

    // PHYSFS_Archiver
    public delegate IntPtr PHYSFS_FP_openArchive(IntPtr io, string name, int forWrite, IntPtr claimed);
    public delegate PHYSFS_EnumerateCallbackResult PHYSFS_FP_enumerate(IntPtr opaque, string dirname, PHYSFS_FP_EnumerateCallback cb, string origDir, IntPtr callbackdata);
    public delegate IntPtr PHYSFS_FP_openRead(IntPtr opaque, string fnm);
    public delegate IntPtr PHYSFS_FP_openAppend(IntPtr opaque, string fnm);
    public delegate int PHYSFS_FP_remove(IntPtr opaque, string filename);
    public delegate int PHYSFS_FP_mkdir(IntPtr opaque, string filename);
    public delegate int PHYSFS_FP_stat(IntPtr opaque, string fn, IntPtr stat);
    public delegate void PHYSFS_FP_closeArchive(IntPtr opaque);

    // PHYSFS_Io
    public delegate long PHYSFS_FP_read(IntPtr io, IntPtr buf, uint len);
    public delegate long PHYSFS_FP_write(IntPtr io, IntPtr buf, uint len);
    public delegate int PHYSFS_FP_seek(IntPtr io, uint offset);
    public delegate long PHYSFS_FP_tell(IntPtr io);
    public delegate long PHYSFS_FP_length(IntPtr io);
    public delegate IntPtr PHYSFS_FP_duplicate(IntPtr io);
    public delegate int PHYSFS_FP_flush(IntPtr io);
    public delegate void PHYSFS_FP_destroy(IntPtr io);

    #endregion Function Pointer Types

    #region Data Structures

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_Allocator {
        public PHYSFS_FP_Init Init;         // int   (*Init)(void)
        public PHYSFS_FP_Deinit Deinit;     // void  (*Deinit)(void)
        public PHYSFS_FP_Malloc Malloc;     // void* (*Malloc)(PHYSFS_uint64)
        public PHYSFS_FP_Realloc Realloc;   // void* (*Realloc)(void*, PHYSFS_uint64)
        public PHYSFS_FP_Free Free;         // void  (*Free)(void*)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_ArchiveInfo {
        public string Extension;
        public string Description;
        public string Author;
        public string Url;
        public int SupportsSymlinks;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_Archiver {
        public uint Version;
        public PHYSFS_ArchiveInfo Info;
        public PHYSFS_FP_openArchive OpenArchive;       // void* (*openArchive) (PHYSFS_Io *io, const char *name, int forWrite, int *claimed)
        public PHYSFS_FP_enumerate Enumerate;           // PHYSFS_EnumerateCallbackResult (*enumerate) (void *opaque, const char *dirname, PHYSFS_EnumerateCallback cb, const char *origdir, void *callbackdata)
        public PHYSFS_FP_openRead OpenRead;             // PHYSFS_Io* (*openRead) (void *opaque, const char *fnm)
        public PHYSFS_FP_openAppend OpenAppend;         // PHYSFS_Io* (*openAppend) (void *opaque, const char *fnm)
        public PHYSFS_FP_remove Remove;                 // int (*remove) (void *opaque, const char *filename)
        public PHYSFS_FP_mkdir Mkdir;                   // int (*mkdir) (void *opaque, const char *filename)
        public PHYSFS_FP_stat Stat;                     // int (*stat) (void *opaque, const char *fn, PHYSFS_Stat *stat)
        public PHYSFS_FP_closeArchive CloseArchive;     // void (*closeArchive) (void *opaque)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_File {
        public IntPtr Opaque; // void*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_Io {
        public uint Version;
        public IntPtr Opaque;                   // void*
        public PHYSFS_FP_read Read;             // PHYSFS_sint64 (*read) (struct PHYSFS_Io *io, void *buf, PHYSFS_uin64 len)
        public PHYSFS_FP_write Write;           // PHYSFS_sint64 (*write) (struct PHYSFS_Io *io, const void *buf, PHYSFS_uin64 len)
        public PHYSFS_FP_seek Seek;             // int (*seek) (struct PHYSFS_Io *io, PHYSFS_uint64 offset)
        public PHYSFS_FP_tell Tell;             // PHYSFS_sint64 (*tell) (struct PHYSFS_Io *io)
        public PHYSFS_FP_length Length;         // PHYSFS_sint64 (*length) (struct PHYSFS_Io *io)
        public PHYSFS_FP_duplicate Duplicate;   // struct PHYSFS_Io* (*duplicate) (struct PHYSFS_Io *io)
        public PHYSFS_FP_flush Flush;           // int (*flush) (struct PHYSFS_Io *io)
        public PHYSFS_FP_destroy Destroy;       // void (*destroy) (struct PHYSFS_Io *io)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_Stat {
        public long FileSize;
        public long ModTime;
        public long CreateTime;
        public long AccessTime;
        public PHYSFS_FileType FileType;
        public int IsReadonly;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_Version {
        public byte Major;
        public byte Minor;
        public byte Patch;
    }

    #endregion Data Structures

    public static class Interop {
        private const string nativeLibName = "physfs";

        #region Public Methods

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_getLinkedVersion(IntPtr ver); // IntPtr => PHYSFS_Version*

        /// <summary>
        /// Initialize the PhysicsFS library.
        ///
        /// This must be called before any other PhysicsFS function.
        ///
        /// This should be called prior to any attempts to change your process's current working directory.
        /// </summary>
        /// <param name="argv0">The argv[0] string passed to your program's mainline. This may be NULL on most platforms (such as ones without a standard main() function), but you should always try to pass something in here. Unix-like systems such as Linux need to pass argv[0] from main() in here.</param>
        /// <returns>Nonzero on success, zero on error. Specifics of the error can be gleaned from PHYSFS_getLastError().</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_init(string argv0); //

        /// <summary>
        /// Deinitialize the PhysicsFS library.
        /// </summary>
        /// <remarks>
        /// This closes any files opened via PhysicsFS, blanks the search/write paths, frees memory, and invalidates all of your file handles.
        /// 
        /// Note that this call can FAIL if there's a file open for writing that refuses to close (for example, the underlying operating system was buffering writes to network filesystem, and the fileserver has crashed, or a hard drive has failed, etc). It is usually best to close all write handles yourself before calling this function, so that you can gracefully handle a specific failure.
        /// 
        /// Once successfully deinitialized, <see cref="PHYSFS_init(string)"/> can be called again to restart the subsystem. All default API states are restored at this point, with the exception of any custom allocator you might have specified, which survives between initializations.
        /// </remarks>
        /// <returns>Nonzero on success, zero on error. Specifics of the error can be gleaned from PHYSFS_getLastError(). If failure, state of PhysFS is undefined, and probably badly screwed up.</returns>
        /// <seealso cref="PHYSFS_init"/>
        /// <seealso cref="PHYSFS_isInit"/>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_deinit(); //

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_supportedArchiveTypes(); // IntPtr => const PHYSFS_ArchiveInfo**

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_freeList(IntPtr listVar); // IntPtr => void*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getLastError(); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getDirSeparator(); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_permitSymbolicLink(int allow);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getCdRomDirs(); // IntPtr => char**

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getBaseDir(); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getUserDir(); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getWriteDir(); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_setWriteDir(IntPtr newDir); // IntPtr => const char*

        /// <summary>
        /// Add an archive or directory to the search path.
        /// </summary>
        /// <remarks>
        /// You must use this and not PHYSFS_mount if binary compatibility with PhysicsFS 1.0 is important (which it may not be for many people).
        /// </remarks>
        /// <param name="newDir">Directory or archive to add to the path, in platform-dependent notation.</param>
        /// <param name="appendToPath">Nonzero to append to search path, zero to prepend.</param>
        /// <returns>Nonzero if added to path, zero on failure (bogus archive, dir missing, etc). Use PHYSFS_getLastErrorCode() to obtain the specific error.</returns>
        [Obsolete("As of PhysicsFS 2.0, use PHYSFS_mount() instead. This function just wraps it anyhow. This function is equivalent to: PHYSFS_mount(newDir, NULL, appendToPath);", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_addToSearchPath(IntPtr newDir, int appendToPath); // IntPtr => const char*

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_unmount() instead. This function just wraps it anyhow. There's no functional difference except the vocabulary changed from \"adding to the search path\" to \"mounting\" when that functionality was extended, and thus the preferred way to accomplish this function's work is now called \"unmounting.\"", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_removeFromSearchPath(IntPtr oldDir); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getSearchPath(); // IntPtr => char**

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_setSaneConfig(IntPtr organization, IntPtr appName, IntPtr archiveExt, int includeCdRoms, int archivesFirst); // IntPtr => const char*; IntPtr => const char*; IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_mkdir(IntPtr dirName); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_delete(IntPtr filename); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getRealDir(IntPtr filename); // IntPtr (ret) => const char* | IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_enumerateFiles(IntPtr dir); // IntPtr (ret) => char** | IntPtr => const char*

        /// <summary>
        /// Determine if a file exists in the search path.
        /// </summary>
        /// <remarks>
        /// Reports true if there is an entry anywhere in the search path by the name of (fname).
        ///
        /// Note that entries that are symlinks are ignored if PHYSFS_permitSymbolicLinks(1) hasn't been called, so you might end up further down in the search path than expected.
        /// </remarks>
        /// <param name="fname">Filename in platform-independent notation.</param>
        /// <returns>Non-zero if filename exists. zero otherwise.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_exists(IntPtr fname); // IntPtr => const char*

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_stat() instead. This function just wraps it anyhow.", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_isDirectory(IntPtr fname); // IntPtr => const char*

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_stat() instead. This function just wraps it anyhow.", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_isSymbolicLink(IntPtr fname); // IntPtr => const char*

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_stat() instead. This function just wraps it anyhow.", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_getLastModTime(IntPtr filename);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_openWrite(IntPtr filename); // IntPtr (ret) => PHYSFS_File* | IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_openAppend(IntPtr filename); // IntPtr (ret) => PHYSFS_File* | IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_openRead(IntPtr filename); // IntPtr (ret) => PHYSFS_File* | IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_close(IntPtr handle); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_read(IntPtr handle, IntPtr buffer, uint objSize, uint objCount);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_write(IntPtr handle, IntPtr buffer, uint objSize, uint objCount);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_eof(IntPtr handle); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_tell(IntPtr handle); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_seek(IntPtr handle, ulong pos); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_fileLength(IntPtr handle); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_setBuffer(IntPtr handle, ulong bufsize); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_flush(IntPtr handle); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern short PHYSFS_swapSLE16(short val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort PHYSFS_swapULE16(ushort val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_swapSLE32(int val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint PHYSFS_swapULE32(uint val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_swapSLE64(long val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong PHYSFS_swapULE64(ulong val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern short PHYSFS_swapSBE16(short val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort PHYSFS_swapUBE16(ushort val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_swapSBE32(int val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint PHYSFS_swapUBE32(uint val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_swapSBE64(long val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong PHYSFS_swapUBE64(ulong val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readSLE16(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readULE16(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readSLE32(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readULE32(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readSLE64(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readULE64(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readSBE16(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readUBE16(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readSBE32(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readUBE32(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readSBE64(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_readUBE64(IntPtr file, IntPtr val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeSLE16(IntPtr handle, short val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeULE16(IntPtr handle, ushort val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeSBE16(IntPtr handle, short val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeUBE16(IntPtr handle, ushort val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeSLE32(IntPtr handle, int val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeULE32(IntPtr handle, uint val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeSBE32(IntPtr handle, int val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeUBE32(IntPtr handle, uint val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeSLE64(IntPtr handle, long val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeULE64(IntPtr handle, ulong val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeSBE64(IntPtr handle, long val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_writeUBE64(IntPtr handle, ulong val);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_isInit(); //

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_symbolicLinksPermitted();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_setAllocator(IntPtr allocator);

        /// <summary>
        /// Add an archive or directory to the search path.
        /// </summary>
        /// <remarks>
        /// If this is a duplicate, the entry is not added again, even though the function succeeds. You may not add the same archive to two different mountpoints: duplicate checking is done against the archive and not the mountpoint.
        /// 
        /// When you mount an archive, it is added to a virtual file system...all files in all of the archives are interpolated into a single hierachical file tree. Two archives mounted at the same place (or an archive with files overlapping another mountpoint) may have overlapping files: in such a case, the file earliest in the search path is selected, and the other files are inaccessible to the application. This allows archives to be used to override previous revisions; you can use the mounting mechanism to place archives at a specific point in the file tree and prevent overlap; this is useful for downloadable mods that might trample over application data or each other, for example.
        ///
        /// The mountpoint does not need to exist prior to mounting, which is different than those familiar with the Unix concept of "mounting" may expect. As well, more than one archive can be mounted to the same mountpoint, or mountpoints and archive contents can overlap...the interpolation mechanism still functions as usual.
        ///
        /// Specifying a symbolic link to an archive or directory is allowed here, regardless of the state of PHYSFS_permitSymbolicLinks(). That function only deals with symlinks inside the mounted directory or archive.
        /// </remarks>
        /// <param name="newDir">Directory or archive to add to the path, in platform-dependent notation.</param>
        /// <param name="mountPoint">Location in the interpolated tree that this archive will be "mounted", in platform-independent notation. NULL or "" is equivalent to "/".</param>
        /// <param name="appendToPath">Nonzero to append to search path, zero to prepend.</param>
        /// <returns>Nonzero if added to path, zero on failure (bogus archive, dir missing, etc). Use PHYSFS_getLastErrorCode() to obtain the specific error.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_mount(IntPtr newDir, IntPtr mountPoint, int appendToPath); // IntPtr => const char*; IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_getMountPoint(string dir);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_getCdRomDirsCallback(PHYSFS_FP_StringCallback c, IntPtr d);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getSearchPathCallback(PHYSFS_FP_StringCallback c, IntPtr d);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_enumerateFilesCallback(string dir, PHYSFS_FP_EnumFilesCallback c, IntPtr d);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utfFromUcs4(UIntPtr src, string dst, ulong len);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8ToUcs4(string src, UIntPtr dst, ulong len);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8FromUcs2(UIntPtr src, string dst, ulong le);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8ToUcs2(string src, UIntPtr dst, ulong len);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8FromLatin1(string src, string dst, ulong len);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_caseFold(uint from, UIntPtr to);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_utf8stricmp(string str1, string sr2);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_utf16stricmp(UIntPtr str1, UIntPtr str2);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_ucs4stricmp(UIntPtr str1, UIntPtr str2);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_enumerate(string dir, PHYSFS_FP_EnumerateCallback c, IntPtr d);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_unmount(IntPtr oldDir); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getAllocator();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_stat(IntPtr fname, IntPtr stat); // IntPtr => const char*; IntPtr => PHYSFS_Stat*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8FromUtf16(UIntPtr src, string dst, ulong len);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8ToUtf16(string src, UIntPtr dst, ulong len);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_readBytes(IntPtr handle, IntPtr buffer, ulong len); // IntPtr => PHYSFS_File*; IntPtr => void*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_writeBytes(IntPtr handle, IntPtr buffer, ulong len); // IntPtr => PHYSFS_File*; IntPtr => void*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_mountIo(IntPtr io, string newDir, string mountPoint, int appendToPath);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_mountMemory(IntPtr buf, ulong len, IntPtr del, string newDir, string mountPoint, int appendToPath);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_mountHandle(IntPtr file, string newDir, string mountPoint, int appendToPath);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern PHYSFS_ErrorCode PHYSFS_getLastErrorCode(); //

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getErrorByCode(PHYSFS_ErrorCode code); // IntPtr (ret) => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_setErrorCode(PHYSFS_ErrorCode code);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string PHYSFS_getPrefDir(string org, string app);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_registerArchiver(IntPtr archiver);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_deregisterArchiver(string ext);

        #endregion Public Methods
    }
}
