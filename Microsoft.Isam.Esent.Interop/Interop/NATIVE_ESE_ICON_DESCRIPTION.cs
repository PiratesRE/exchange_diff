using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_ESE_ICON_DESCRIPTION
	{
		public void FreeNativeIconDescription()
		{
			Marshal.FreeHGlobal(this.pvData);
			this.pvData = IntPtr.Zero;
		}

		public uint ulSize;

		public IntPtr pvData;
	}
}
