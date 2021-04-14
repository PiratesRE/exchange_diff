using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_INDEXCREATE
	{
		public uint cbStruct;

		public IntPtr szIndexName;

		public IntPtr szKey;

		public uint cbKey;

		public uint grbit;

		public uint ulDensity;

		public unsafe NATIVE_UNICODEINDEX* pidxUnicode;

		public IntPtr cbVarSegMac;

		public IntPtr rgconditionalcolumn;

		public uint cConditionalColumn;

		public int err;
	}
}
