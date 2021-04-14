using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubscriptionAccessor
	{
		SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem);

		SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem);

		SnapshotStatus RetrieveSubscriptionStatus(ISubscriptionId subscriptionId);

		SubscriptionSnapshot RetrieveSubscriptionSnapshot(ISubscriptionId subscriptionId);

		bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription);

		bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false);

		bool SuspendSubscription(ISubscriptionId subscriptionId);

		bool RemoveSubscription(ISubscriptionId subscriptionId);
	}
}
