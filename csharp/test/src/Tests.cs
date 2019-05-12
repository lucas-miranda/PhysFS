#define PRINT_INFO

using System.Collections;
using System.Diagnostics;
using System.Text;

using PhysFS.Stream;

namespace PhysFS.Test {
    public static class Tests {
        #region Public Members

        public const string TestArchiveFilePath = "folder.zip",
                            TestFilepath = "test.txt",
                            TestFolder = "folder",
                            TestWriteDir = "folder";

        #endregion Public Members

        #region Public Methods

        [TestCase("PHYSFS_getLinkedVersion")]
        public static bool Test_GetLinkedVersion() {
            PHYSFS_Version version = PhysFS.LinkedVersion;

#if PRINT_INFO
            Debug.WriteLine($"  Linked PhysFS version: {version.Major}.{version.Minor}.{version.Patch}");
#endif

            return true;
        }

        [TestCase("PHYSFS_mount")]
        public static bool Test_Mount() {
            PhysFS.Initialize();

            bool ret = PhysFS.Instance.Mount(TestArchiveFilePath, mountPoint: "", appendToPath: true);

            PhysFS.Deinitialize();

            return ret;
        }

        [TestCase("PHYSFS_exists")]
        public static bool Test_Exists() {
            PhysFS.Initialize();
            PhysFS.Instance.Mount(TestArchiveFilePath, mountPoint: "", appendToPath: true);

            bool ret = PhysFS.Instance.Exists(TestFilepath);

            PhysFS.Deinitialize();

            return ret;
        }

        [TestCase("PHYSFS_supportedArchiveTypes")]
        public static bool Test_SupportedArchiveTypes() {
            PhysFS.Initialize();

            PHYSFS_ArchiveInfo[] supportedArchiveTypes = PhysFS.Instance.SupportedArchiveTypes;

            #if PRINT_INFO
            foreach (PHYSFS_ArchiveInfo archiveInfo in supportedArchiveTypes) {
                Debug.WriteLine($"  ext: {archiveInfo.Extension} |  desc: {archiveInfo.Description} |  author: {archiveInfo.Author} |  url: {archiveInfo.Url} |  sym links: {archiveInfo.SupportsSymlinks == 1}");
            }
#endif

            PhysFS.Deinitialize();

            return true;
        }

        [TestCase("PHYSFS_getDirSeparator")]
        public static bool Test_DirSeparator() {
            string str = PhysFS.DirSeparator;

#if PRINT_INFO
            Debug.WriteLine($"  Dir Separator: {str}");
#endif

            return true;
        }

        [TestCase("PHYSFS_getRomsDirs, PHYSFS_getBaseDir, PHYSFS_getUserDir")]
        public static bool Test_GetDirs() {
            PhysFS.Initialize();

            string[] cdromDirs = PhysFS.Instance.CdRomDirectories;

#if PRINT_INFO
            Debug.WriteLine($"  CdRom available dirs: {string.Join(", ", cdromDirs)}");
#endif

            string baseDir = PhysFS.Instance.BaseDirectory;

#if PRINT_INFO
            Debug.WriteLine($"  Base Dir: {baseDir}");
#endif

            string userDir = PhysFS.Instance.UserDirectory;

#if PRINT_INFO
            Debug.WriteLine($"  User Dir: {userDir}");
#endif

            PhysFS.Deinitialize();

            return true;
        }

        [TestCase("PHYSFS_setWriteDir, PHYSFS_getWriteDir")]
        public static bool Test_WriteDir() {
            PhysFS.Initialize();

            Debug.WriteLine($"  Setting Write Dir to '{TestWriteDir}'");
            PhysFS.Instance.WriteDirectory = TestWriteDir;

            string writeDir = PhysFS.Instance.WriteDirectory;
            Debug.WriteLine($"  Getting Write Dir: {writeDir}");

            PhysFS.Deinitialize();

            return writeDir == TestWriteDir;
        }

        #region PHYSFSFileWrite Tests

        [TestCase("Stream.PhysFSFileWriter")]
        public static bool Test_PhysFSFileWriter() {
            PhysFS.Initialize();

            PhysFS.Instance.WriteDirectory = TestWriteDir;
            Debug.WriteLine($"  Write Dir: {PhysFS.Instance.WriteDirectory}");

            using (PhysFSFileWriter stream = new PhysFSFileWriter("test-write-stream.txt", append: false)) {
                string text = "Just a PhysFSFileWrite test...";
                byte[] textBytes = Encoding.UTF8.GetBytes(text);

                Debug.WriteLine($"  Before writing, file length: {stream.Length}");

                Debug.WriteLine("  Writing to file");
                stream.Write(textBytes, 0, textBytes.Length);

                Debug.WriteLine($"  After writing, file length: {stream.Length}");
            }

            PhysFS.Deinitialize();

            return true;
        }

        [TestCase("Stream.PhysFSFIleReader")]
        public static bool Test_PhysFSFileReader() {
            PhysFS.Initialize();
            PhysFS.Instance.Mount(TestFolder, mountPoint: "", appendToPath: true);

            Debug.WriteLine($"  Reading 'just a text.txt' file");

            using (PhysFSFileReader stream = new PhysFSFileReader("test.txt")) {
                byte[] buffer = new byte[stream.Length];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string text = Encoding.UTF8.GetString(buffer);

                //Debug.WriteLine($"  File contents:\n{text}");
                Debug.WriteLine($"  Bytes Read: {bytesRead}");
            }

            PhysFS.Deinitialize();

            return true;
        }

        #endregion PHYSFSFileWrite Tests
        
        [TestCase("PHYSFS_stat")]
        public static bool Test_Stat() {
            PhysFS.Initialize();
            PhysFS.Instance.Mount(TestFolder, mountPoint: "", appendToPath: true);
            PHYSFS_Stat stat = PhysFS.Stat("test.txt");

            System.DateTimeOffset createTime = System.DateTimeOffset.FromUnixTimeSeconds(stat.CreateTime),
                                  modTime = System.DateTimeOffset.FromUnixTimeSeconds(stat.ModTime),
                                  accessTime = System.DateTimeOffset.FromUnixTimeSeconds(stat.AccessTime);

            Debug.WriteLine($"  File Size: {stat.FileSize} bytes, ModTime: {modTime}, Create Time: {createTime}, Access Time: {accessTime}, File Type: {stat.FileType}, Is Readonly: {System.Convert.ToBoolean(stat.IsReadonly)}");

            PhysFS.Deinitialize();
            return true;
        }

        [TestCase("PhysFS.Enumerate")]
        public static bool Test_Enumerate() {
            PhysFS.Initialize();
            PhysFS.Instance.Mount(TestFolder, mountPoint: "", appendToPath: true);

            Debug.WriteLine("  Enumerating all files (explicit callback result returning):");

            PhysFS.Instance.Enumerate(
                "/",
                FileCallbackExplicit
            );

            Debug.WriteLine("  Enumerating all files (using enumerator):");

            PhysFS.Instance.Enumerate(
                "/",
                FileCallbackEnumerator
            );

            PhysFS.Deinitialize();

            return true;

            PHYSFS_EnumerateCallbackResult FileCallbackExplicit(string dir, string filename) {
                /*
                To stop enumerating:

                if (filename.Contains("test")) {
                    Debug.WriteLine("    > stopping...");
                    return PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_STOP;
                }


                To signaling an error to PhysFS:

                if (filename.Contains("test")) {
                    Debug.WriteLine("    > some error has happened!");
                    return PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_ERROR;
                }
                */

                Debug.WriteLine($"    dir: {dir}, filename: {filename}");
                return PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_OK;
            }

            IEnumerator FileCallbackEnumerator(string dir, string filename) {
                /*
                To stop enumerating:

                if (filename.Contains("test")) {
                    Debug.WriteLine("    > stopping...");
                    yield false;
                }


                To signaling an error to PhysFS:

                if (filename.Contains("test")) {
                    Debug.WriteLine("    > some error has happened!");
                    yield break;
                }
                */

                Debug.WriteLine($"    dir: {dir}, filename: {filename}");
                yield return true;
            }
        }

        [TestCase("PhysFS.MountMemory")]
        public static bool Test_MountMemory() {
            PhysFS.Initialize();

            byte[] folderBytes = System.IO.File.ReadAllBytes("folder.zip");
            Debug.WriteLine($"  Reading 'folder.zip', size: {folderBytes.Length} bytes");

            Debug.WriteLine("  Mounting file on memory");
            PhysFS.Instance.MountMemory(folderBytes, "memory-file.zip", "/memory-mount-test/", appendToPath: true);

            ShowAllFilesAt("/");
            ShowSearchPaths();

            PhysFS.Deinitialize();
            return true;
        }

        [TestCase("PhysFS.MountIOStream")]
        public static bool Test_MountIOStream() {
            PhysFS.Initialize();

            TestStream stream = new TestStream("folder.zip");
            PhysFS.Instance.MountIOStream(stream, "file-from-iostream.zip", mountPoint: "/", appendToPath: true);

            ShowAllFilesAt("/");
            ShowSearchPaths();

            PhysFS.Deinitialize();
            return true;
        }

        #endregion Public Methods

        #region Private Methods

        private static void ShowAllFilesAt(string baseDir, int tab = 0) {
            string tabString = new string(' ', tab * 2);
            Debug.WriteLine($"  {tabString}Files at '{baseDir}':\n");

            PhysFS.Instance.Enumerate(
                baseDir,
                (string dir, string filename) => {
                    Debug.WriteLine($"    {tabString}dir: {dir}, filename: {filename}");

                    string fullPath;
                    if (dir.EndsWith("/")) {
                        fullPath = string.Concat(dir, filename);
                    } else {
                        fullPath = string.Concat(dir, "/", filename);
                    }

                    if (PhysFS.Stat(fullPath).FileType == PHYSFS_FileType.PHYSFS_FILETYPE_DIRECTORY) {
                        ShowAllFilesAt(fullPath, tab + 1);
                    }

                    return PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_OK;
                }
            );
        }

        private static void ShowSearchPaths() {
            Debug.WriteLine("  Enumerating search paths:");

            PhysFS.Instance.EnumerateSearchPath(
                (string path) => {
                    Debug.WriteLine($"    path: {path}");
                }
            );
        }

        #endregion Private Methods
    }
}
