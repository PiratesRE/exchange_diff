using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SendAsSubscriptionsAndPeopleConnectResult
	{
		public List<PimAggregationSubscription> PimSendAsAggregationSubscriptionList { get; private set; }

		public bool PeopleConnectionsExist { get; private set; }

		public SendAsSubscriptionsAndPeopleConnectResult(List<PimAggregationSubscription> pimSendAsAggregationSubscriptionList, bool peopleConnectionExist)
		{
			this.PimSendAsAggregationSubscriptionList = pimSendAsAggregationSubscriptionList;
			this.PeopleConnectionsExist = peopleConnectionExist;
		}
	}
}
