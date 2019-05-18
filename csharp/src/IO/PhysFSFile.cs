using System;

namespace PhysFS.IO {
    public class PhysFSFile {
        #region Constructors

        private PhysFSFile(IntPtr handle) {
            Handle = handle;
        }

        public PhysFSFile(string filename, FileMode mode) {
            Filename = filename;
            Mode = mode;

            switch (mode) {
                case FileMode.Read:
                    Handle = Interop.PHYSFS_openRead(filename);
                    break;

                case FileMode.Write:
                    Handle = Interop.PHYSFS_openWrite(filename);
                    break;

                case FileMode.Append:
                    Handle = Interop.PHYSFS_openAppend(filename);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported FileMode '{mode}'");
            }
        }

        #endregion Constructors

        #region Public Properties 

        public FileMode Mode { get; private set; }
        public string Filename { get; private set; }

        #endregion Public Properties 

        #region Internal Properties

        internal IntPtr Handle { get; private set; }

        #endregion Internal Properties

        #region Public Methods

        public static PhysFSFile OpenRead(string filename) {
            IntPtr Handle = Interop.PHYSFS_openRead(filename);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return new PhysFSFile(Handle) {
                Mode = FileMode.Read,
                Filename = filename
            };
        }

        public static PhysFSFile OpenWrite(string filename) {
            IntPtr Handle = Interop.PHYSFS_openWrite(filename);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return new PhysFSFile(Handle) {
                Mode = FileMode.Write,
                Filename = filename
            };
        }

        public static PhysFSFile OpenAppend(string filename) {
            IntPtr Handle = Interop.PHYSFS_openAppend(filename);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return new PhysFSFile(Handle) {
                Mode = FileMode.Append,
                Filename = filename
            };
        }

        public void Close() {
            if (Handle == IntPtr.Zero) {
                return;
            }

            int ret = Interop.PHYSFS_close(Handle);

            if (ret == 0) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            Handle = IntPtr.Zero;
        }

        #endregion Public Methods
    }
}
