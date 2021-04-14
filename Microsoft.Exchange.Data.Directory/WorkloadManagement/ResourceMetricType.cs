using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	public enum ResourceMetricType
	{
		None,
		ActiveDirectoryReplicationLatency,
		MdbLatency,
		Processor,
		MdbReplication,
		CiAgeOfLastNotification,
		MdbAvailability = 7,
		DiskLatency,
		Remote = 1000
	}
}
