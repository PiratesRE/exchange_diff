using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	public enum AggregationStatus
	{
		Succeeded,
		InProgress,
		Delayed,
		Disabled,
		Poisonous,
		InvalidVersion
	}
}
