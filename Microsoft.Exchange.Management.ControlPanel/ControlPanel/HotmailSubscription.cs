using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class HotmailSubscription : PimSubscription
	{
		public HotmailSubscription(HotmailSubscriptionProxy subscription) : base(subscription)
		{
		}
	}
}
