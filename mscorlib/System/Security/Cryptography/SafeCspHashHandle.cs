using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	[SecurityCritical]
	internal sealed class SafeCspHashHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeCspHashHandle() : base(true)
		{
		}

		[DllImport("advapi32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptDestroyHash(IntPtr hKey);

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return SafeCspHashHandle.CryptDestroyHash(this.handle);
		}
	}
}
