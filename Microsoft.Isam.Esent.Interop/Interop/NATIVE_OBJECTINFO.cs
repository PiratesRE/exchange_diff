using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_OBJECTINFO
	{
		public uint cbStruct;

		public uint objtyp;

		[Obsolete("Unused member")]
		public double ignored1;

		[Obsolete("Unused member")]
		public double ignored2;

		public uint grbit;

		public uint flags;

		public uint cRecord;

		public uint cPage;
	}
}
