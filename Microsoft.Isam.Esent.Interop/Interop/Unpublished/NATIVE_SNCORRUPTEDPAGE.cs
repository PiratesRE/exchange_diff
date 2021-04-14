using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_SNCORRUPTEDPAGE
	{
		public uint cbStruct;

		public IntPtr wszDatabase;

		public uint dbid;

		public NATIVE_DBINFOMISC7 dbinfomisc;

		public uint pageNumber;
	}
}
