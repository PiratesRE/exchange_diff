using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal interface IDatabaseAvailabilityProvider : IResourceLoadMonitor
	{
		void Update(uint databaseAvailabilityHealth);
	}
}
