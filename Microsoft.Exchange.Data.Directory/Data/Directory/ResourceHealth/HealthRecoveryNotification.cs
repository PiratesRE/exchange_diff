using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal delegate void HealthRecoveryNotification(ResourceKey key, WorkloadClassification classification, Guid notificationCookie);
}
