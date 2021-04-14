using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class DiskLatencyResourceKey : MdbResourceKey
	{
		public DiskLatencyResourceKey(Guid databaseGuid) : base(ResourceMetricType.DiskLatency, databaseGuid)
		{
			DatabaseInformation databaseInformation = DatabaseInformationCache.Singleton.Get(databaseGuid);
			this.DatabaseVolumeName = ((databaseInformation != null) ? databaseInformation.DatabaseVolumeName : string.Empty);
		}

		public string DatabaseVolumeName { get; private set; }

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new DiskLatencyResourceMonitor(this);
		}
	}
}
