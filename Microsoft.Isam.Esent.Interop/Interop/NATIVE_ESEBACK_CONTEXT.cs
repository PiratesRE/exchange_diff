using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_ESEBACK_CONTEXT
	{
		public uint cbSize;

		public IntPtr wszServerName;

		public IntPtr pvApplicationData;
	}
}
