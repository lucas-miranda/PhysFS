#define PRINT_INFO

using System.Diagnostics;
using System.Text;

namespace PhysFS_CS_Test {
    public static class Tests {
        #region Public Members

        public const string TestArchiveFilePath = "folder.zip",
                            TestFilepath = "folder/test.txt",
                            TestWriteDir = "folder2";

        #endregion Public Members

        #region Public Methods

        [TestCase("PHYSFS_getLinkedVersion")]
        public static bool Test_GetLinkedVersion() {
            PhysFS.PHYSFS_Version version = PhysFS.PhysFS.LinkedVersion;

#if PRINT_INFO
            Debug.WriteLine($"  Linked PhysFS version: {version.major}.{version.minor}.{version.patch}");
#endif

            return true;
        }

        /*
        [TestCase("PHYSFS_mount")]
        public static bool Test_Mount() {
            //Begin();

            bool ret = PhysFS.PhysFS.PHYSFS_mount(TestArchiveFilePath, "", 1) != 0;

            if (!ret) {
                throw new PHYSFSErrorException(PhysFS.PhysFS.PHYSFS_getLastErrorCode());
            }

            //End();

            return ret;
        }
        */

        /*
        [TestCase("PHYSFS_exists")]
        public static bool Test_Exists() {
            BeginWithMount();

            bool ret = PhysFS.PhysFS.PHYSFS_exists(TestFilepath) != 0;

            if (!ret) {
                throw new PHYSFSErrorException(PhysFS.PhysFS.PHYSFS_getLastErrorCode());
            }

            End();

            return ret;
        }
        */

        [TestCase("PHYSFS_supportedArchiveTypes")]
        public static bool Test_SupportedArchiveTypes() {
            //Begin();

            PhysFS.PHYSFS_ArchiveInfo[] supportedArchiveTypes = PhysFS.PhysFS.Instance.SupportedArchiveTypes;

            #if PRINT_INFO
            foreach (PhysFS.PHYSFS_ArchiveInfo archiveInfo in supportedArchiveTypes) {
                Debug.WriteLine($"  ext: {archiveInfo.extension} |  desc: {archiveInfo.description} |  author: {archiveInfo.author} |  url: {archiveInfo.url} |  sym links: {archiveInfo.supportsSymlinks == 1}");
            }
#endif

            //End();

            return true;
        }

        /*
        [TestCase("PHYSFS_getLastError")]
        public static bool Test_GetLastError() {
            //Begin();

            PhysFS.PhysFS.PHYSFS_mount("inexistent folder", "", 1);

            string str = PhysFS.PhysFS.PHYSFS_getLastError();

            bool ret = str.Length > 0;

#if PRINT_INFO
            Debug.WriteLine($"  Testing error with inexistent folder, Last Error Message: {str}");
#endif

            //End();

            return ret;
        }
        */

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
            //Begin();

            string[] cdromDirs = PhysFS.PhysFS.Instance.CdRomDirectories;

#if PRINT_INFO
            Debug.WriteLine($"  CdRom available dirs: {string.Join(", ", cdromDirs)}");
#endif

            string baseDir = PhysFS.PhysFS.Instance.BaseDirectory;

#if PRINT_INFO
            Debug.WriteLine($"  Base Dir: {baseDir}");
#endif

            string userDir = PhysFS.PhysFS.Instance.UserDirectory;

#if PRINT_INFO
            Debug.WriteLine($"  User Dir: {userDir}");
#endif

            //End();

            return true;
        }

        /*
        [TestCase("PHYSFS_setWriteDir, PHYSFS_getWriteDir")]
        public static bool Test_WriteDir() {
            Begin();
            Debug.WriteLine($"  Setting Write Dir to '{TestWriteDir}'");
            int writeRet = PhysFS.LowLevel.PhysFS.PHYSFS_setWriteDir(TestWriteDir);

            if (writeRet == 0) {
                throw new PHYSFSErrorException(PhysFS.PhysFS.PHYSFS_getLastErrorCode());
            }

            string writeDir = PhysFS.PhysFS.PHYSFS_getWriteDir();
            Debug.WriteLine($"  Getting Write Dir: {writeDir}");

            End();

            return writeDir == TestWriteDir;
        }
        */

        #region PHYSFSFileWrite Tests

        [TestCase("Stream.PhysFSFileWrite")]
        public static bool Test_PhysFSFileWriter() {
            PhysFS.PhysFS.Instance.WriteDirectory = "folder.zip";

            Debug.WriteLine($"  Write Dir: {PhysFS.PhysFS.Instance.WriteDirectory}");

            using (PhysFS.Stream.PhysFSFileWriter stream = new PhysFS.Stream.PhysFSFileWriter("test-write-stream.txt")) {
                string message = "Just a PhysFSFileWrite test...";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                Debug.WriteLine($"  File length: {stream.Length}");

                Debug.WriteLine("  Writing to file");
                stream.Write(messageBytes, 0, messageBytes.Length);

                Debug.WriteLine($"  File length: {stream.Length}");
            }

            return true;
        }

        #endregion PHYSFSFileWrite Tests

        #endregion Public Methods

        #region Private Methods

        /*
        private static void Begin() {
            PhysFS.PhysFS.PHYSFS_init(null);

            if (PhysFS.PhysFS.PHYSFS_isInit() == 0) {
                throw new PHYSFSErrorException(PhysFS.PhysFS.PHYSFS_getLastErrorCode());
            }
        }

        private static void BeginWithMount(string dir = TestArchiveFilePath, string mountPoint = "", int appendToPath = 1) {
            Begin();

            if (PhysFS.PhysFS.PHYSFS_mount(dir, mountPoint, appendToPath) == 0) {
                throw new PHYSFSErrorException(PhysFS.PhysFS.PHYSFS_getLastErrorCode());
            }
        }

        private static void End() {
            if (PhysFS.PhysFS.PHYSFS_deinit() == 0) {
                throw new PHYSFSErrorException(PhysFS.PhysFS.PHYSFS_getLastErrorCode());
            }
        }
        */

        #endregion Private Methods
    }
}
