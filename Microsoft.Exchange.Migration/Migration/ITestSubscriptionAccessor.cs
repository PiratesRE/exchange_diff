using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ITestSubscriptionAccessor
	{
		string GetDebuggingContext();

		TestSubscriptionSnapshot CreateSubscription(TestSubscriptionAspect aspect);

		TestSubscriptionSnapshot TestCreateSubscription(TestSubscriptionAspect aspect);

		SnapshotStatus RetrieveSubscriptionStatus(Guid identity);

		TestSubscriptionSnapshot RetrieveSubscriptionSnapshot(Guid identity);

		void UpdateSubscriptionStatus(Guid identity, SnapshotStatus status);

		void UpdateSubscription(Guid identity, TestSubscriptionAspect aspect);
	}
}
