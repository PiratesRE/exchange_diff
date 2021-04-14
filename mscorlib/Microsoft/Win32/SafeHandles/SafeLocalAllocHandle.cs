using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeLocalAllocHandle : SafeBuffer
	{
		private SafeLocalAllocHandle() : base(true)
		{
		}

		internal SafeLocalAllocHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		internal static SafeLocalAllocHandle InvalidHandle
		{
			get
			{
				return new SafeLocalAllocHandle(IntPtr.Zero);
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.LocalFree(this.handle) == IntPtr.Zero;
		}
	}
}
