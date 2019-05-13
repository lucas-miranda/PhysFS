using System;

namespace PhysFS.IO {
    public class PhysFSFile {
        #region Constructors

        private PhysFSFile(IntPtr handle) {
            Handle = handle;
        }

        #endregion Constructors

        #region Internal Properties

        internal IntPtr Handle { get; private set; }

        #endregion Internal Properties

        #region Public Methods

        public static PhysFSFile OpenRead(string filename) {
            IntPtr Handle = Interop.PHYSFS_openRead(filename);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return new PhysFSFile(Handle);
        }

        public static PhysFSFile OpenWrite(string filename) {
            IntPtr Handle = Interop.PHYSFS_openWrite(filename);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return new PhysFSFile(Handle);
        }

        public static PhysFSFile OpenAppend(string filename) {
            IntPtr Handle = Interop.PHYSFS_openAppend(filename);

            if (Handle == IntPtr.Zero) {
                throw new PhysFSException(Interop.PHYSFS_getLastErrorCode());
            }

            return new PhysFSFile(Handle);
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
