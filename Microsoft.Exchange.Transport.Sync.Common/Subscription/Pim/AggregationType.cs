using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	public enum AggregationType
	{
		Aggregation,
		Mirrored = 16,
		Migration = 32,
		PeopleConnection = 64,
		All = 255
	}
}
