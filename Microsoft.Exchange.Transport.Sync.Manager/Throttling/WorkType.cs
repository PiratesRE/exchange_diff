using System;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	internal enum WorkType
	{
		AggregationSubscriptionSaved,
		AggregationIncremental,
		AggregationInitial,
		MigrationInitial,
		MigrationIncremental,
		MigrationFinalization,
		OwaLogonTriggeredSyncNow,
		OwaActivityTriggeredSyncNow,
		OwaRefreshButtonTriggeredSyncNow,
		PeopleConnectionInitial,
		PeopleConnectionTriggered,
		PeopleConnectionIncremental,
		PolicyInducedDelete
	}
}
