using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Transport.Sync.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MigrationStatusOverride
	{
		public MigrationStatusOverride(DetailedAggregationStatus detailedStatus, MigrationSubscriptionStatus migrationDetailedStatus)
		{
			this.DetailedAggregationStatus = detailedStatus;
			this.MigrationSubscriptionStatus = migrationDetailedStatus;
		}

		public DetailedAggregationStatus DetailedAggregationStatus { get; private set; }

		public MigrationSubscriptionStatus MigrationSubscriptionStatus { get; private set; }
	}
}
