using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_ENUMCOLUMNVALUE
	{
		public uint itagSequence;

		public int err;

		public uint cbData;

		public IntPtr pvData;
	}
}
