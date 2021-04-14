using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[Flags]
	internal enum SubscriptionEvents
	{
		None = 0,
		WorkItemCompleted = 1,
		WorkItemFailedLoadSubscription = 2,
		All = 255
	}
}
