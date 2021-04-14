using System;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	public enum MonitorAlertState
	{
		Unknown,
		Healthy,
		Degraded,
		Unhealthy,
		Repairing,
		Disabled
	}
}
