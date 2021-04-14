using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_RSTMAP
	{
		public void FreeHGlobal()
		{
			LibraryHelpers.MarshalFreeHGlobal(this.szDatabaseName);
			LibraryHelpers.MarshalFreeHGlobal(this.szNewDatabaseName);
		}

		public IntPtr szDatabaseName;

		public IntPtr szNewDatabaseName;
	}
}
