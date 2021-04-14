using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_COLUMNLIST
	{
		public uint cbStruct;

		public IntPtr tableid;

		public uint cRecord;

		public uint columnidPresentationOrder;

		public uint columnidcolumnname;

		public uint columnidcolumnid;

		public uint columnidcoltyp;

		public uint columnidCountry;

		public uint columnidLangid;

		public uint columnidCp;

		public uint columnidCollate;

		public uint columnidcbMax;

		public uint columnidgrbit;

		public uint columnidDefault;

		public uint columnidBaseTableName;

		public uint columnidBaseColumnName;

		public uint columnidDefinitionName;
	}
}
