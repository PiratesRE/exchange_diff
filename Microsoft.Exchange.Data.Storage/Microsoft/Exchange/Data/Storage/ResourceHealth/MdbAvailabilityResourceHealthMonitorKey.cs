using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MdbAvailabilityResourceHealthMonitorKey : MdbResourceKey
	{
		public MdbAvailabilityResourceHealthMonitorKey(Guid databaseGuid) : base(ResourceMetricType.MdbAvailability, databaseGuid)
		{
		}

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new MdbAvailabilityResourceHealthMonitor(this);
		}
	}
}
