using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMemberSubscriptionItem
	{
		string SubscriptionId { get; }

		ExDateTime LastUpdateTimeUTC { get; }
	}
}
