using System.Text;
using System.Diagnostics;

using PhysFS.Core;
using PhysFS.IO.Stream;

namespace PhysFS.Test {
    public struct TestData {
        public byte[] Data;
        public int Length;
    }

    public class TestArchiver : IPhysFSArchiver<TestData> {
        public uint Version {
            get {
                return 0U;
            }
        }

        public string Extension {
            get {
                return "xyz";
            }
        }

        public string Description {
            get {
                return "An XYZ File.";
            }
        }

        public string Author {
            get {
                return "Random Person";
            }
        }

        public string Url {
            get {
                return "www.github.com/lucas-miranda";
            }
        }

        public bool SupportSymLinks {
            get {
                return false;
            }
        }

        public TestData OpenArchive(IPhysFSStream stream, string name, bool forWrite, out bool claimed) {
            Debug.WriteLine($"trying archive with name {name}");
            if (!name.EndsWith("xyz")) {
                Debug.WriteLine($"can't open");
                claimed = false;
                return new TestData();
            }

            Debug.WriteLine($"open archive, with name {name}, for write? {forWrite}");

            long length = stream.Length();
            stream.Seek(0UL);
            byte[] buffer = new byte[length];
            stream.Read(buffer, (ulong) length);

            TestData testData = new TestData {
                Data = buffer,
                Length = buffer.Length
            };

            claimed = true;
            return testData;
        }

        public PHYSFS_EnumerateCallbackResult Enumerate(ref TestData data, string dirname, EnumerateCallback callback, string origDir) {
            Debug.WriteLine("enumerating");
            return PHYSFS_EnumerateCallbackResult.PHYSFS_ENUM_STOP;
        }

        public IPhysFSStream OpenRead(ref TestData data, string filename) {
            Debug.WriteLine("open read");

            string str = $"this is a reading test from filename '{filename}'";
            byte[] textBytes = Encoding.UTF8.GetBytes(str);

            PhysFSStandardStream stream = new PhysFSStandardStream(textBytes);
            return stream;
        }

        public IPhysFSStream OpenWrite(ref TestData data, string filename) {
            Debug.WriteLine("open write");
            PhysFSStandardStream stream = new PhysFSStandardStream(data.Data);
            return stream;
        }

        public IPhysFSStream OpenAppend(ref TestData data, string filename) {
            Debug.WriteLine("open append");
            PhysFSStandardStream stream = new PhysFSStandardStream(data.Data);
            return stream;
        }

        public bool Remove(ref TestData data, string filename) {
            Debug.WriteLine("Remove");
            return false;
        }

        public bool Mkdir(ref TestData data, string filename) {
            Debug.WriteLine("Mkdir");
            return false;
        }

        public bool Stat(ref TestData data, string filename, out PHYSFS_Stat stat) {
            Debug.WriteLine("Stat");
            stat = new PHYSFS_Stat();
            return true;
        }

        public void CloseArchive(ref TestData data) {
            Debug.WriteLine("Close Archive");
        }
    }
}
