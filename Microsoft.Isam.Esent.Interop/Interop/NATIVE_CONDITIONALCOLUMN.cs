using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_CONDITIONALCOLUMN
	{
		public uint cbStruct;

		public IntPtr szColumnName;

		public uint grbit;
	}
}
