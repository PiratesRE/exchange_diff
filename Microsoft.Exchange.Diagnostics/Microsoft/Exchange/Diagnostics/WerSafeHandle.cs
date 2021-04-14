using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class WerSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public WerSafeHandle() : this(IntPtr.Zero)
		{
		}

		public WerSafeHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			DiagnosticsNativeMethods.WerReportCloseHandle(this.handle);
			return true;
		}
	}
}
