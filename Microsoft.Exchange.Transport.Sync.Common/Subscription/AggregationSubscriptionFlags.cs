using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[Flags]
	internal enum AggregationSubscriptionFlags
	{
		IsMigration = 1,
		IsInitialSyncDone = 16
	}
}
