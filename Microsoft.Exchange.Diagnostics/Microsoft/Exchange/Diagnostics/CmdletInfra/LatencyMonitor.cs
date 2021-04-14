using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class LatencyMonitor : IScopedPerformanceMonitor
	{
		public LatencyMonitor(LatencyTracker latencyTracker)
		{
			this.latencyTracker = latencyTracker;
		}

		public bool Start(ScopeInfo scopeInfo)
		{
			return this.latencyTracker != null && this.latencyTracker.StartInternalTracking(scopeInfo.GroupName, scopeInfo.FuncName, false);
		}

		public void End(ScopeInfo scopeInfo)
		{
			if (this.latencyTracker == null)
			{
				return;
			}
			this.latencyTracker.EndInternalTracking(scopeInfo.GroupName, scopeInfo.FuncName);
		}

		private readonly LatencyTracker latencyTracker;
	}
}
