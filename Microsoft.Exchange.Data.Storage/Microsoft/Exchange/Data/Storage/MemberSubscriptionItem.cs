using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MemberSubscriptionItem : IMemberSubscriptionItem
	{
		public MemberSubscriptionItem(string subscriptionId, ExDateTime lastUpdateTimeUTC)
		{
			ArgumentValidator.ThrowIfNull("subscriptionId", subscriptionId);
			this.SubscriptionId = subscriptionId;
			this.LastUpdateTimeUTC = lastUpdateTimeUTC;
		}

		public string SubscriptionId { get; private set; }

		public ExDateTime LastUpdateTimeUTC { get; set; }
	}
}
