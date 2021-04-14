using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_INDEXLIST
	{
		public uint cbStruct;

		public IntPtr tableid;

		public uint cRecord;

		public uint columnidindexname;

		public uint columnidgrbitIndex;

		public uint columnidcKey;

		public uint columnidcEntry;

		public uint columnidcPage;

		public uint columnidcColumn;

		public uint columnidiColumn;

		public uint columnidcolumnid;

		public uint columnidcoltyp;

		[Obsolete("Deprecated")]
		public uint columnidCountry;

		public uint columnidLangid;

		public uint columnidCp;

		[Obsolete("Deprecated")]
		public uint columnidCollate;

		public uint columnidgrbitColumn;

		public uint columnidcolumnname;

		public uint columnidLCMapFlags;
	}
}
