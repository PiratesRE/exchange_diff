using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PrivilegeControl : DisposeTrackableBase
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmServiceMonitorTracer;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PrivilegeControl>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (disposing && this.privControl != null)
				{
					this.privControl.Dispose();
					this.privControl = null;
				}
			}
		}

		public bool TryEnable(string privilege, out Exception ex)
		{
			bool result = false;
			ex = null;
			try
			{
				this.privControl = new Privilege("SeDebugPrivilege");
				this.privControl.Enable();
				result = true;
			}
			catch (PrivilegeNotHeldException ex2)
			{
				ex = ex2;
				PrivilegeControl.Tracer.TraceError<string, PrivilegeNotHeldException>(0L, "TryEnable failed to set priv({0}): {1}", privilege, ex2);
			}
			return result;
		}

		private Privilege privControl;
	}
}
