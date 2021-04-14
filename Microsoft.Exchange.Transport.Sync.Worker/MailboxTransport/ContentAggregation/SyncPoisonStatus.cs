using System;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	internal enum SyncPoisonStatus
	{
		CleanSubscription,
		SuspectedSubscription,
		PoisonousItems,
		PoisonousSubscription
	}
}
