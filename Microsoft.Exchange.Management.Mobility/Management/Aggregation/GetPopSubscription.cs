using System;
using System.Management.Automation;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Get", "PopSubscription", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class GetPopSubscription : GetSubscriptionBase<PopSubscriptionProxy>
	{
		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				return AggregationSubscriptionType.Pop;
			}
		}
	}
}
