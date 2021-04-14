using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatabaseReplicationProviderWrapper : ResourceHealthMonitorWrapper, IDatabaseReplicationProvider, IResourceLoadMonitor
	{
		public DatabaseReplicationProviderWrapper(MdbReplicationResourceHealthMonitor provider) : base(provider)
		{
		}

		public void Update(uint databaseReplicationHealth)
		{
			base.CheckExpired();
			MdbReplicationResourceHealthMonitor wrappedMonitor = base.GetWrappedMonitor<MdbReplicationResourceHealthMonitor>();
			wrappedMonitor.Update(databaseReplicationHealth);
		}
	}
}
