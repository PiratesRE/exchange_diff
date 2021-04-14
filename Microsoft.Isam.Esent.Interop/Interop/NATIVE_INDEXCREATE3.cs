using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_INDEXCREATE3
	{
		public uint cbStruct;

		public IntPtr szIndexName;

		public IntPtr szKey;

		public uint cbKey;

		public uint grbit;

		public uint ulDensity;

		public unsafe NATIVE_UNICODEINDEX2* pidxUnicode;

		public IntPtr cbVarSegMac;

		public IntPtr rgconditionalcolumn;

		public uint cConditionalColumn;

		public int err;

		public uint cbKeyMost;

		public IntPtr pSpaceHints;
	}
}
