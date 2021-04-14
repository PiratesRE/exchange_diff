using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_COLUMNDEF
	{
		public uint cbStruct;

		public uint columnid;

		public uint coltyp;

		[Obsolete("Reserved")]
		public ushort wCountry;

		[Obsolete("Use cp")]
		public ushort langid;

		public ushort cp;

		[Obsolete("Reserved")]
		public ushort wCollate;

		public uint cbMax;

		public uint grbit;
	}
}
