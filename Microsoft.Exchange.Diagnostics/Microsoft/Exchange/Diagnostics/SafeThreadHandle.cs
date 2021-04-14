using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class SafeThreadHandle : DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeThreadHandle()
		{
		}

		internal SafeThreadHandle(IntPtr handle) : base(handle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeThreadHandle>(this);
		}

		protected override bool ReleaseHandle()
		{
			return DiagnosticsNativeMethods.CloseHandle(this.handle);
		}
	}
}
