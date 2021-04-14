using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_COLUMNCREATE
	{
		public uint cbStruct;

		public IntPtr szColumnName;

		public uint coltyp;

		public uint cbMax;

		public uint grbit;

		public IntPtr pvDefault;

		public uint cbDefault;

		public uint cp;

		public uint columnid;

		public int err;
	}
}
