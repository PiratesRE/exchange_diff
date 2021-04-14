using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_INSTANCE_INFO
	{
		public IntPtr hInstanceId;

		public IntPtr szInstanceName;

		public IntPtr cDatabases;

		public unsafe IntPtr* szDatabaseFileName;

		public unsafe IntPtr* szDatabaseDisplayName;

		[Obsolete("SLV files are not supported")]
		public unsafe IntPtr* szDatabaseSLVFileName;
	}
}
