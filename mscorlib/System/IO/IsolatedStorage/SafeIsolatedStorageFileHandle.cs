using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace System.IO.IsolatedStorage
{
	[SecurityCritical]
	internal sealed class SafeIsolatedStorageFileHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void Close(IntPtr file);

		private SafeIsolatedStorageFileHandle() : base(true)
		{
			base.SetHandle(IntPtr.Zero);
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			SafeIsolatedStorageFileHandle.Close(this.handle);
			return true;
		}
	}
}
