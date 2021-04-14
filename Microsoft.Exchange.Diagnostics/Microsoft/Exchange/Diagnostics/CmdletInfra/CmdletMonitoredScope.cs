using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class CmdletMonitoredScope : MonitoredScope
	{
		public CmdletMonitoredScope(Guid cmdletUniqueId, string groupName, string funcName, params IScopedPerformanceMonitor[] monitors) : base(new CmdletScopeInfo(cmdletUniqueId, groupName, funcName), monitors)
		{
		}
	}
}
