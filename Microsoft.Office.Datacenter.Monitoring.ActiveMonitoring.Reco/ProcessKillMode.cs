using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public enum ProcessKillMode
	{
		SelfOnly,
		SelfAndChildren,
		ChildrenAndSelf
	}
}
