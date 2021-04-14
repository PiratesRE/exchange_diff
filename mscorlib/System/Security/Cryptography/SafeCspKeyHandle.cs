using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	[SecurityCritical]
	internal sealed class SafeCspKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeCspKeyHandle() : base(true)
		{
		}

		[DllImport("advapi32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptDestroyKey(IntPtr hKey);

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return SafeCspKeyHandle.CryptDestroyKey(this.handle);
		}
	}
}
