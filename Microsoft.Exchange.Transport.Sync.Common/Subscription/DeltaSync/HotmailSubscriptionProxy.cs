using System;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync
{
	[Serializable]
	public sealed class HotmailSubscriptionProxy : WindowsLiveSubscriptionProxy
	{
		public HotmailSubscriptionProxy() : this(new DeltaSyncAggregationSubscription())
		{
		}

		internal HotmailSubscriptionProxy(DeltaSyncAggregationSubscription subscription) : base(subscription)
		{
		}
	}
}
