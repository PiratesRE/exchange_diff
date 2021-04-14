using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class MonitoredScope : DisposeTracker
	{
		public MonitoredScope(string groupName, string funcName, params IScopedPerformanceMonitor[] monitors) : this(new ScopeInfo(groupName, funcName), monitors)
		{
		}

		protected MonitoredScope(ScopeInfo scopeInfo, params IScopedPerformanceMonitor[] monitors)
		{
			this.scopeInfo = scopeInfo;
			this.monitors = monitors;
			this.monitorStarted = new bool[monitors.Length];
			for (int i = 0; i < monitors.Length; i++)
			{
				this.monitorStarted[i] = monitors[i].Start(scopeInfo);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				for (int i = 0; i < this.monitors.Length; i++)
				{
					if (this.monitorStarted[i])
					{
						this.monitors[i].End(this.scopeInfo);
					}
				}
			}
		}

		private readonly ScopeInfo scopeInfo;

		private readonly IScopedPerformanceMonitor[] monitors;

		private readonly bool[] monitorStarted;
	}
}
