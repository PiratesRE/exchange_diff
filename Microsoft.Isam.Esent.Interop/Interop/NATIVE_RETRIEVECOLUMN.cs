using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_RETRIEVECOLUMN
	{
		public uint columnid;

		public IntPtr pvData;

		public uint cbData;

		public uint cbActual;

		public uint grbit;

		public uint ibLongValue;

		public uint itagSequence;

		public uint columnidNextTagged;

		public int err;
	}
}
