#define PRINT_INFO

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
            Debug.WriteLine($"  Linked PhysFS version: {version.major}.{version.minor}.{version.patch}");
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
                Debug.WriteLine($"  ext: {archiveInfo.extension} |  desc: {archiveInfo.description} |  author: {archiveInfo.author} |  url: {archiveInfo.url} |  sym links: {archiveInfo.supportsSymlinks == 1}");
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

        [TestCase("Stream.PhysFSFileWrite")]
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

        #endregion PHYSFSFileWrite Tests

        #endregion Public Methods
    }
}
