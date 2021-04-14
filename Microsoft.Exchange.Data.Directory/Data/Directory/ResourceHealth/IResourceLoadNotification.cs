using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal interface IResourceLoadNotification
	{
		Guid SubscribeToHealthNotifications(WorkloadClassification classification, HealthRecoveryNotification delegateToFire);

		bool UnsubscribeFromHealthNotifications(Guid registrationKey);
	}
}
