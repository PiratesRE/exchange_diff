using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_SETCOLUMN
	{
		public uint columnid;

		public IntPtr pvData;

		public uint cbData;

		public uint grbit;

		public uint ibLongValue;

		public uint itagSequence;

		public uint err;
	}
}
