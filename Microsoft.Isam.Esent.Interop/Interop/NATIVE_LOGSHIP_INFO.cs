using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_LOGSHIP_INFO
	{
		public uint ulType;

		public uint cchName;

		public IntPtr wszName;
	}
}
