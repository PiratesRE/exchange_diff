using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal class ToolhelpSnapshotSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public ToolhelpSnapshotSafeHandle() : base(true)
		{
			base.SetHandle(IntPtr.Zero);
		}

		protected override bool ReleaseHandle()
		{
			return DiagnosticsNativeMethods.CloseHandle(this.handle);
		}
	}
}
