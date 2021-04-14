using System;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	internal struct NATIVE_INDEX_COLUMN
	{
		public uint columnid;

		public uint relop;

		public IntPtr pvData;

		public uint cbData;

		public uint grbit;
	}
}
