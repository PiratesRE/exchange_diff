using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatabaseAvailabilityProviderWrapper : ResourceHealthMonitorWrapper, IDatabaseAvailabilityProvider, IResourceLoadMonitor
	{
		public DatabaseAvailabilityProviderWrapper(MdbAvailabilityResourceHealthMonitor provider) : base(provider)
		{
		}

		public void Update(uint databaseAvailabilityHealth)
		{
			base.CheckExpired();
			MdbAvailabilityResourceHealthMonitor wrappedMonitor = base.GetWrappedMonitor<MdbAvailabilityResourceHealthMonitor>();
			wrappedMonitor.Update(databaseAvailabilityHealth);
		}
	}
}
