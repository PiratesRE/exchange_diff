using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ServerThrottlingResource
	{
		internal static void InitializeServerThrottlingObjects(bool initializeRHM)
		{
			if (initializeRHM)
			{
				ThrottlingPerfCounterWrapper.Initialize(BudgetType.ResourceTracking, null, true);
				ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.MRS);
			}
		}
	}
}
