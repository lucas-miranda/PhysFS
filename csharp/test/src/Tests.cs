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
                            TestWriteDir = "folder2";

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

        /*
        [TestCase("PHYSFS_getDirSeparator")]
        public static bool Test_DirSeparator() {
            string str = PhysFS.PhysFS.PHYSFS_getDirSeparator();

#if PRINT_INFO
            Debug.WriteLine($"  Dir Separator: {str}");
#endif

            return true;
        }
        */

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
            PhysFS.Instance.Mount("folder2", mountPoint: "", appendToPath: true);

            Debug.WriteLine($"  Reading 'just a text.txt' file");

            using (PhysFSFileReader stream = new PhysFSFileReader("just a text.txt")) {
                byte[] buffer = new byte[stream.Length];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string text = Encoding.UTF8.GetString(buffer);

                //Debug.WriteLine($"  File contents:\n{text}");
                Debug.WriteLine($"  Bytes Read: {bytesRead}");
            }

            PhysFS.Deinitialize();

            return true;
        }
        
        [TestCase("PHYSFS_stat")]
        public static bool Test_Stat() {
            PhysFS.Initialize();
            PhysFS.Instance.Mount("folder2", mountPoint: "", appendToPath: true);
            PHYSFS_Stat stat = PhysFS.Stat("just a text.txt");

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
            PhysFS.Instance.Mount("folder2", mountPoint: "", appendToPath: true);

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

        #endregion PHYSFSFileWrite Tests

        #endregion Public Methods
    }
}
