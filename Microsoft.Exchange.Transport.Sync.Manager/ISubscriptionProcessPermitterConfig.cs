using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISubscriptionProcessPermitterConfig
	{
		bool AggregationSubscriptionsEnabled { get; }

		bool MigrationSubscriptionsEnabled { get; }

		bool PeopleConnectionSubscriptionsEnabled { get; }

		bool PopAggregationEnabled { get; }

		bool DeltaSyncAggregationEnabled { get; }

		bool ImapAggregationEnabled { get; }

		bool FacebookAggregationEnabled { get; }

		bool LinkedInAggregationEnabled { get; }
	}
}
