using System;
using System.Management.Automation;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Get", "HotmailSubscription", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class GetHotmailSubscription : GetSubscriptionBase<HotmailSubscriptionProxy>
	{
		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				return AggregationSubscriptionType.DeltaSyncMail;
			}
		}
	}
}
