using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	internal sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeTokenHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		private SafeTokenHandle() : base(true)
		{
		}

		internal static SafeTokenHandle InvalidHandle
		{
			get
			{
				return new SafeTokenHandle(IntPtr.Zero);
			}
		}

		protected override bool ReleaseHandle()
		{
			return NativeMethods.CloseHandle(this.handle);
		}
	}
}
