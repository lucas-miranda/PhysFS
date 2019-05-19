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

    #region Data Structures

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_Allocator {
        public Interop.PHYSFS_FP_Init Init;         // int   (*Init)(void)
        public Interop.PHYSFS_FP_Deinit Deinit;     // void  (*Deinit)(void)
        public Interop.PHYSFS_FP_Malloc Malloc;     // void* (*Malloc)(PHYSFS_uint64)
        public Interop.PHYSFS_FP_Realloc Realloc;   // void* (*Realloc)(void*, PHYSFS_uint64)
        public Interop.PHYSFS_FP_Free Free;         // void  (*Free)(void*)
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
        public Interop.PHYSFS_FP_openArchive OpenArchive;       // void* (*openArchive) (PHYSFS_Io *io, const char *name, int forWrite, int *claimed)
        public Interop.PHYSFS_FP_enumerate Enumerate;           // PHYSFS_EnumerateCallbackResult (*enumerate) (void *opaque, const char *dirname, PHYSFS_EnumerateCallback cb, const char *origdir, void *callbackdata)
        public Interop.PHYSFS_FP_openRead OpenRead;             // PHYSFS_Io* (*openRead) (void *opaque, const char *fnm)
        public Interop.PHYSFS_FP_openWrite OpenWrite;           // PHYSFS_Io* (*openWrite) (void *opaque, const char *filename)
        public Interop.PHYSFS_FP_openAppend OpenAppend;         // PHYSFS_Io* (*openAppend) (void *opaque, const char *fnm)
        public Interop.PHYSFS_FP_remove Remove;                 // int (*remove) (void *opaque, const char *filename)
        public Interop.PHYSFS_FP_mkdir Mkdir;                   // int (*mkdir) (void *opaque, const char *filename)
        public Interop.PHYSFS_FP_stat Stat;                     // int (*stat) (void *opaque, const char *fn, PHYSFS_Stat *stat)
        public Interop.PHYSFS_FP_closeArchive CloseArchive;     // void (*closeArchive) (void *opaque)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_File {
        public IntPtr Opaque; // void*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PHYSFS_Io {
        public uint Version;
        public IntPtr Opaque;                           // void*
        public Interop.PHYSFS_FP_read Read;             // PHYSFS_sint64 (*read) (struct PHYSFS_Io *io, void *buf, PHYSFS_uin64 len)
        public Interop.PHYSFS_FP_write Write;           // PHYSFS_sint64 (*write) (struct PHYSFS_Io *io, const void *buf, PHYSFS_uin64 len)
        public Interop.PHYSFS_FP_seek Seek;             // int (*seek) (struct PHYSFS_Io *io, PHYSFS_uint64 offset)
        public Interop.PHYSFS_FP_tell Tell;             // PHYSFS_sint64 (*tell) (struct PHYSFS_Io *io)
        public Interop.PHYSFS_FP_length Length;         // PHYSFS_sint64 (*length) (struct PHYSFS_Io *io)
        public Interop.PHYSFS_FP_duplicate Duplicate;   // struct PHYSFS_Io* (*duplicate) (struct PHYSFS_Io *io)
        public Interop.PHYSFS_FP_flush Flush;           // int (*flush) (struct PHYSFS_Io *io)
        public Interop.PHYSFS_FP_destroy Destroy;       // void (*destroy) (struct PHYSFS_Io *io)
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

        #region Function Pointer Types

        // Callbacks

        // PHYSFS_EnumerateCallbackResult (*PHYSFS_EnumerateCallback)(void *data, const char *origdir, const char *fname);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate PHYSFS_EnumerateCallbackResult PHYSFS_FP_EnumerateCallback(IntPtr data, string origDir, string fname);

        // void (*PHYSFS_StringCallback)(void *data, const char *str);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void PHYSFS_FP_StringCallback(IntPtr data, string str);

        [Obsolete("As of PhysicsFS 2.1, Use PHYSFS_EnumerateCallback with PHYSFS_enumerate() instead; it gives you more control over the process.")]
        // void (*PHYSFS_EnumFilesCallback)(void *data, const char *origdir, const char *fname);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void PHYSFS_FP_EnumFilesCallback(IntPtr data, string origdir, string fname);

        // void (*)(void *buffer);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PHYSFS_FP_MountedMemoryUnmount(IntPtr buffer); // unmount callback to PHYSFS_mountMemory()

        // PHYSFS_Allocator
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PHYSFS_FP_Init();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PHYSFS_FP_Deinit();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr PHYSFS_FP_Malloc(ulong size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr PHYSFS_FP_Realloc(IntPtr handle, ulong newSize);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PHYSFS_FP_Free(IntPtr handle);

        // PHYSFS_Archiver
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr PHYSFS_FP_openArchive(IntPtr io, string name, int forWrite, IntPtr claimed);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate PHYSFS_EnumerateCallbackResult PHYSFS_FP_enumerate(IntPtr opaque, string dirname, PHYSFS_FP_EnumerateCallback cb, string origDir, IntPtr callbackdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr PHYSFS_FP_openRead(IntPtr opaque, string fnm);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr PHYSFS_FP_openWrite(IntPtr opaque, string filename);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr PHYSFS_FP_openAppend(IntPtr opaque, string fnm);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PHYSFS_FP_remove(IntPtr opaque, string filename);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PHYSFS_FP_mkdir(IntPtr opaque, string filename);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PHYSFS_FP_stat(IntPtr opaque, string fn, IntPtr stat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PHYSFS_FP_closeArchive(IntPtr opaque);

        // PHYSFS_Io
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long PHYSFS_FP_read(IntPtr io, IntPtr buf, ulong len);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long PHYSFS_FP_write(IntPtr io, IntPtr buf, ulong len);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PHYSFS_FP_seek(IntPtr io, ulong offset);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long PHYSFS_FP_tell(IntPtr io);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long PHYSFS_FP_length(IntPtr io);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr PHYSFS_FP_duplicate(IntPtr io);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PHYSFS_FP_flush(IntPtr io);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PHYSFS_FP_destroy(IntPtr io);

        #endregion Function Pointer Types

        #region Public Methods

        /// <summary>
        /// Get the version of PhysicsFS that is linked against your program.
        /// </summary>
        /// <remarks>
        /// If you are using a shared library (DLL) version of PhysFS, then it is possible that it will be different than the version you compiled against.
        /// This is a real function; the macro PHYSFS_VERSION tells you what version of PhysFS you compiled against
        /// This function may be called safely at any time, even before PHYSFS_init().
        /// </remarks>
        /// <param name="ver">A PHYSFS_Version to output version info.</param>
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

        /// <summary>
        /// Get a list of supported archive types.
        /// Get a list of archive types supported by this implementation of PhysicFS. These are the file formats usable for search path entries. This is for informational purposes only. Note that the extension listed is merely convention: if we list "ZIP", you can open a PkZip-compatible archive with an extension of "XYZ", if you like.
        /// </summary>
        /// <remarks>
        /// The returned value is an array of pointers to PHYSFS_ArchiveInfo structures, with a NULL entry to signify the end of the list.
        /// The return values are pointers to internal memory, and should be considered READ ONLY, and never freed. The returned values are valid until the next call to PHYSFS_deinit(), PHYSFS_registerArchiver(), or PHYSFS_deregisterArchiver().
        /// </remarks>
        /// <returns>READ ONLY Null-terminated array of READ ONLY structures.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_supportedArchiveTypes(); // IntPtr => const PHYSFS_ArchiveInfo**

        /// <summary>
        /// Deallocate resources of lists returned by PhysicsFS.
        /// Certain PhysicsFS functions return lists of information that are dynamically allocated. Use this function to free those resources.
        /// It is safe to pass a NULL here, but doing so will cause a crash in versions before PhysicsFS 2.1.0.
        /// </summary>
        /// <param name="listVar">List of information specified as freeable by this function. Passing NULL is safe; it is a valid no-op.</param>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_freeList(IntPtr listVar); // IntPtr => void*

        /// <summary>
        /// Get human-readable error information.
        /// </summary>
        /// <remarks>
        /// Get the last PhysicsFS error message as a human-readable, null-terminated string. This will return NULL if there's been no error since the last call to this function. The pointer returned by this call points to an internal buffer. Each thread has a unique error state associated with it, but each time a new error message is set, it will overwrite the previous one associated with that thread. It is safe to call this function at anytime, even before PHYSFS_init().
        /// PHYSFS_getLastError() and PHYSFS_getLastErrorCode() both reset the same thread-specific error state. Calling one will wipe out the other's data. If you need both, call PHYSFS_getLastErrorCode(), then pass that value to PHYSFS_getErrorByCode().
        /// As of PhysicsFS 2.1, this function only presents text in the English language, but the strings are static, so you can use them as keys into your own localization dictionary. These strings are meant to be passed on directly to the user.
        /// Generally, applications should only concern themselves with whether a given function failed; however, if your code require more specifics, you should use PHYSFS_getLastErrorCode() instead of this function.
        /// </remarks>
        /// <returns>READ ONLY string of last error message.</returns>
        [Obsolete("Use PHYSFS_getLastErrorCode() and PHYSFS_getErrorByCode() instead.", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getLastError(); // IntPtr => const char*

        /// <summary>
        /// Get platform-dependent dir separator string.
        /// </summary>
        /// <remarks>
        /// This returns "\\" on win32, "/" on Unix, and ":" on MacOS. It may be more than one character, depending on the platform, and your code should take that into account. Note that this is only useful for setting up the search/write paths, since access into those dirs always use '/' (platform-independent notation) to separate directories. This is also handy for getting platform-independent access when using stdio calls.
        /// </remarks>
        /// <returns>READ ONLY null-terminated string of platform's dir separator.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_getDirSeparator(); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_permitSymbolicLink(int allow); //

        /// <summary>
        /// Get an array of paths to available CD-ROM drives.
        /// </summary>
        /// <remarks>
        /// The dirs returned are platform-dependent ("D:\" on Win32, "/cdrom" or whatnot on Unix). Dirs are only returned if there is a disc ready and accessible in the drive. So if you've got two drives (D: and E:), and only E: has a disc in it, then that's all you get. If the user inserts a disc in D: and you call this function again, you get both drives. If, on a Unix box, the user unmounts a disc and remounts it elsewhere, the next call to this function will reflect that change.
        /// This function refers to "CD-ROM" media, but it really means "inserted disc media," such as DVD-ROM, HD-DVD, CDRW, and Blu-Ray discs. It looks for filesystems, and as such won't report an audio CD, unless there's a mounted filesystem track on it.
        /// The returned value is an array of strings, with a NULL entry to signify the end of the list:
        /// </remarks>
        /// <example>
        /// char **cds = PHYSFS_getCdRomDirs();
        /// char **i;
        ///
        /// for (i = cds; *i != NULL; i++)
        ///     printf("cdrom dir [%s] is available.\n", *i);
        ///
        /// PHYSFS_freeList(cds);
        /// </example>
        /// <returns>Null-terminated array of null-terminated strings.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getCdRomDirs(); // IntPtr => char**

        /// <summary>
        /// Get the path where the application resides.
        /// </summary>
        /// <remarks>
        /// Helper function.
        /// Get the "base dir". This is the directory where the application was run from, which is probably the installation directory, and may or may not be the process's current working directory.
        /// You should probably use the base dir in your search path.
        /// </remarks>
        /// <returns>READ ONLY string of base dir in platform-dependent notation.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getBaseDir(); // IntPtr => const char*

        /// <summary>
        /// Get the path where user's home directory resides.
        /// </summary>
        /// <remarks>
        /// Helper function.
        /// Get the "user dir". This is meant to be a suggestion of where a specific user of the system can store files. On Unix, this is her home directory. On systems with no concept of multiple home directories (MacOS, win95), this will default to something like "C:\mybasedir\users\username" where "username" will either be the login name, or "default" if the platform doesn't support multiple users, either. </remarks>
        /// <returns>READ ONLY string of user dir in platform-dependent notation.</returns>
        [Obsolete("As of PhysicsFS 2.1, you probably want PHYSFS_getPrefDir().")]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getUserDir(); // IntPtr => const char*

        /// <summary>
        /// Get path where PhysicsFS will allow file writing.
        /// Get the current write dir. The default write dir is NULL.
        /// </summary>
        /// <returns>READ ONLY string of write dir in platform-dependent notation, OR NULL IF NO WRITE PATH IS CURRENTLY SET.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getWriteDir(); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_setWriteDir(string newDir);

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
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_addToSearchPath(string newDir, int appendToPath);

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_unmount() instead. This function just wraps it anyhow. There's no functional difference except the vocabulary changed from \"adding to the search path\" to \"mounting\" when that functionality was extended, and thus the preferred way to accomplish this function's work is now called \"unmounting.\"", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_removeFromSearchPath(string oldDir);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getSearchPath(); // IntPtr => char**

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_setSaneConfig(string organization, string appName, string archiveExt, int includeCdRoms, int archivesFirst);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_mkdir(string dirName);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_delete(string filename);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_getRealDir(string filename); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_enumerateFiles(string dir); // IntPtr => char**

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
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_exists(string fname);

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_stat() instead. This function just wraps it anyhow.", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_isDirectory(string fname);

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_stat() instead. This function just wraps it anyhow.", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_isSymbolicLink(string fname);

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_stat() instead. This function just wraps it anyhow.", error: false)]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern long PHYSFS_getLastModTime(string filename);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_openWrite(string filename); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_openAppend(string filename); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_openRead(string filename); // IntPtr => PHYSFS_File*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_close(IntPtr handle); // IntPtr => PHYSFS_File*

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_readBytes() instead. This function just wraps it anyhow. This function never clarified what would happen if you managed to read a partial object, so working at the byte level makes this cleaner for everyone, especially now that PHYSFS_Io interfaces can be supplied by the application.")]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_read(IntPtr handle, IntPtr buffer, uint objSize, uint objCount); // IntPtr => PHYSFS_File*; IntPtr => const void*;

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_writeBytes() instead. This function just wraps it anyhow. This function never clarified what would happen if you managed to write a partial object, so working at the byte level makes this cleaner for everyone, especially now that PHYSFS_Io interfaces can be supplied by the application.")]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_write(IntPtr handle, IntPtr buffer, uint objSize, uint objCount); // IntPtr => PHYSFS_File*; IntPtr => void*;

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
        public static extern int PHYSFS_symbolicLinksPermitted(); //

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_setAllocator(IntPtr allocator); // 

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
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_mount(string newDir, string mountPoint, int appendToPath);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_getMountPoint(string dir); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_getCdRomDirsCallback(PHYSFS_FP_StringCallback c, IntPtr d); // IntPtr => void*;

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_getSearchPathCallback(PHYSFS_FP_StringCallback c, IntPtr d); // IntPtr => void*;

        [Obsolete("As of PhysicsFS 2.1, use PHYSFS_enumerate() instead. This function has no way to report errors (or to have the callback signal an error or request a stop), so if data will be lost, your callback has no way to direct the process, and your calling app has no way to know.")]
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void PHYSFS_enumerateFilesCallback(string dir, PHYSFS_FP_EnumFilesCallback c, IntPtr d); // IntPtr => void*;

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

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_enumerate(string dir, PHYSFS_FP_EnumerateCallback c, IntPtr d); // IntPtr => void*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_unmount(string oldDir); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getAllocator();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_stat(string fname, IntPtr stat); // IntPtr => PHYSFS_Stat*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8FromUtf16(IntPtr src, IntPtr dst, ulong len); // IntPtr => ushort*; IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_utf8ToUtf16(IntPtr src, IntPtr dst, ulong len); // IntPtr => const char*; IntPtr => ushort*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_readBytes(IntPtr handle, IntPtr buffer, ulong len); // IntPtr => PHYSFS_File*; IntPtr => void*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long PHYSFS_writeBytes(IntPtr handle, IntPtr buffer, ulong len); // IntPtr => PHYSFS_File*; IntPtr => void*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_mountIo(IntPtr io, string newDir, string mountPoint, int appendToPath); // IntPtr => PHYSFS_Io*;

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_mountMemory(IntPtr buf, ulong len, PHYSFS_FP_MountedMemoryUnmount del, string newDir, string mountPoint, int appendToPath); // IntPtr => const void*;

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_mountHandle(IntPtr file, string newDir, string mountPoint, int appendToPath); // IntPtr => PHSYFS_File*;

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern PHYSFS_ErrorCode PHYSFS_getLastErrorCode(); //

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PHYSFS_getErrorByCode(PHYSFS_ErrorCode code); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PHYSFS_setErrorCode(PHYSFS_ErrorCode code); //

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PHYSFS_getPrefDir(string org, string app); // IntPtr => const char*

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PHYSFS_registerArchiver(IntPtr archiver);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int PHYSFS_deregisterArchiver(string ext);

        #endregion Public Methods
    }
}
