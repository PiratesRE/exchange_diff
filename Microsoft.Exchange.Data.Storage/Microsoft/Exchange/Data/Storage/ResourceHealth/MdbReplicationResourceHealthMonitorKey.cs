using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MdbReplicationResourceHealthMonitorKey : MdbResourceKey
	{
		public MdbReplicationResourceHealthMonitorKey(Guid databaseGuid) : base(ResourceMetricType.MdbReplication, databaseGuid)
		{
		}

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new MdbReplicationResourceHealthMonitor(this);
		}
	}
}
