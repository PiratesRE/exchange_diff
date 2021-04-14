using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MdbResourceHealthMonitorKey : MdbResourceKey
	{
		public MdbResourceHealthMonitorKey(Guid databaseGuid) : base(ResourceMetricType.MdbLatency, databaseGuid)
		{
		}

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new MdbResourceHealthMonitor(this);
		}
	}
}
