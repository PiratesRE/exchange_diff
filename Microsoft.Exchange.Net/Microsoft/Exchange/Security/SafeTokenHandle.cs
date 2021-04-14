using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security
{
	internal sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeTokenHandle() : base(true)
		{
		}

		internal SafeTokenHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return SspiNativeMethods.CloseHandle(this.handle);
		}
	}
}
