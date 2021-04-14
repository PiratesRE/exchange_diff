using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Probes
{
	internal static class ObserverHeartbeatResultExtension
	{
		internal static bool Succeeded(this ObserverHeartbeatResult result)
		{
			if (result <= ObserverHeartbeatResult.MachineNotResponsive)
			{
				if (result != ObserverHeartbeatResult.Success && result != ObserverHeartbeatResult.MachineNotResponsive)
				{
					return false;
				}
			}
			else if (result != ObserverHeartbeatResult.MonitoringOffline && result != ObserverHeartbeatResult.NoLongerObserver)
			{
				return false;
			}
			return true;
		}
	}
}
