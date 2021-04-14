using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class CmdletLatencyMonitor : IScopedPerformanceMonitor
	{
		private CmdletLatencyMonitor()
		{
		}

		public bool Start(ScopeInfo scopeInfo)
		{
			return CmdletLatencyTracker.StartInternalTracking(CmdletLatencyMonitor.GetCmdletUniqueId(scopeInfo), scopeInfo.GroupName, scopeInfo.FuncName, false);
		}

		public void End(ScopeInfo scopeInfo)
		{
			CmdletLatencyTracker.EndInternalTracking(CmdletLatencyMonitor.GetCmdletUniqueId(scopeInfo), scopeInfo.GroupName, scopeInfo.FuncName);
		}

		private static Guid GetCmdletUniqueId(ScopeInfo scopeInfo)
		{
			Guid result = Guid.Empty;
			if (scopeInfo is CmdletScopeInfo)
			{
				result = ((CmdletScopeInfo)scopeInfo).CmdletUniqueId;
			}
			return result;
		}

		internal static readonly CmdletLatencyMonitor Instance = new CmdletLatencyMonitor();
	}
}
