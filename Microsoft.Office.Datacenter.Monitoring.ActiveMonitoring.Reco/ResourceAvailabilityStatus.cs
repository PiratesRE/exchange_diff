using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public enum ResourceAvailabilityStatus
	{
		Ready,
		Arbitrating,
		Maintenance,
		Unknown,
		Offline
	}
}
