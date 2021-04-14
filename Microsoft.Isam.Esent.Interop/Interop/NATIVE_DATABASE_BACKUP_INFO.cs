using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_DATABASE_BACKUP_INFO
	{
		public void FreeNativeDatabaseBackupInfo()
		{
			Marshal.FreeHGlobal(this.wszDatabaseDisplayName);
			this.wszDatabaseDisplayName = IntPtr.Zero;
			Marshal.FreeHGlobal(this.wszDatabaseStreams);
			this.wszDatabaseStreams = IntPtr.Zero;
		}

		public IntPtr wszDatabaseDisplayName;

		public uint cwDatabaseStreams;

		public IntPtr wszDatabaseStreams;

		public Guid guidDatabase;

		public uint ulIconIndexDatabase;

		public uint fDatabaseFlags;
	}
}
