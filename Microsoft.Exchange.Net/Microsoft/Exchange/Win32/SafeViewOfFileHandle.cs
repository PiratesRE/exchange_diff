using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeViewOfFileHandle : SafeHandleZeroIsInvalid
	{
		internal SafeViewOfFileHandle()
		{
		}

		protected override bool ReleaseHandle()
		{
			return SafeViewOfFileHandle.UnmapViewOfFile(this.handle);
		}

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
	}
}
