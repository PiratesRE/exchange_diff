using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubscriptionStatus
	{
		AggregationStatus Status { get; }

		DetailedAggregationStatus SubStatus { get; }

		MigrationSubscriptionStatus MigrationSubscriptionStatus { get; }

		bool IsInitialSyncComplete { get; }

		DateTime? LastSyncTime { get; }

		DateTime? LastSuccessfulSyncTime { get; }

		long? ItemsSynced { get; }

		long? ItemsSkipped { get; }

		DateTime? LastSyncNowRequestTime { get; }
	}
}
