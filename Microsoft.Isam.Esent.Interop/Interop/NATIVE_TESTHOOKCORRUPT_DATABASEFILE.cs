using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_TESTHOOKCORRUPT_DATABASEFILE
	{
		public uint cbStruct;

		public uint grbit;

		public IntPtr szDatabaseFilePath;

		public long pgnoTarget;

		public long iSubTarget;
	}
}
