using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_OBJECTLIST
	{
		public uint cbStruct;

		public IntPtr tableid;

		public uint cRecord;

		public uint columnidcontainername;

		public uint columnidobjectname;

		public uint columnidobjtyp;

		[Obsolete("Unused member")]
		public uint columniddtCreate;

		[Obsolete("Unused member")]
		public uint columniddtUpdate;

		public uint columnidgrbit;

		public uint columnidflags;

		public uint columnidcRecord;

		public uint columnidcPage;
	}
}
