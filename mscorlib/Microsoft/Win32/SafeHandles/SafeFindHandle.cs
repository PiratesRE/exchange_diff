using System;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityCritical]
		internal SafeFindHandle() : base(true)
		{
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.FindClose(this.handle);
		}
	}
}
