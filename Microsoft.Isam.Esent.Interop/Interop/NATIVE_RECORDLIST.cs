using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_RECORDLIST
	{
		public uint cbStruct;

		public IntPtr tableid;

		public uint cRecords;

		public uint columnidBookmark;
	}
}
