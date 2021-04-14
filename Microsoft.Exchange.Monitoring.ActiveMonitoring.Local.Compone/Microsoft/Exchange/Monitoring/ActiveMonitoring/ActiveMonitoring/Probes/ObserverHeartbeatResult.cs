using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Probes
{
	[Flags]
	internal enum ObserverHeartbeatResult
	{
		None = 0,
		Success = 1,
		OldResponderResult = 2,
		NoResponderResult = 4,
		ServiceNotResponsive = 8,
		MachineNotResponsive = 16,
		MonitoringOffline = 32,
		NoLongerObserver = 64
	}
}
