using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_SPACEHINTS
	{
		public uint cbStruct;

		public uint ulInitialDensity;

		public uint cbInitial;

		public uint grbit;

		public uint ulMaintDensity;

		public uint ulGrowth;

		public uint cbMinExtent;

		public uint cbMaxExtent;
	}
}
