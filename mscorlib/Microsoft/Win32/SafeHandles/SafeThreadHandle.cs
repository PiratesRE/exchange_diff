using System;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeThreadHandle() : base(true)
		{
		}

		internal SafeThreadHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.CloseHandle(this.handle);
		}
	}
}
