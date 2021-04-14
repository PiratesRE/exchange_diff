using System;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeProcessHandle() : base(true)
		{
		}

		internal SafeProcessHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		internal static SafeProcessHandle InvalidHandle
		{
			get
			{
				return new SafeProcessHandle(IntPtr.Zero);
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.CloseHandle(this.handle);
		}
	}
}
