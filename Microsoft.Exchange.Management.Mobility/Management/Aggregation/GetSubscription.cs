using System;
using System.Management.Automation;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Get", "Subscription", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class GetSubscription : GetSubscriptionBase<PimSubscriptionProxy>
	{
		[Parameter(Mandatory = false)]
		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return (AggregationSubscriptionType)base.Fields["SubscriptionType"];
			}
			set
			{
				base.Fields["SubscriptionType"] = value;
			}
		}

		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				if (!base.Fields.IsModified("SubscriptionType"))
				{
					return AggregationSubscriptionType.All;
				}
				return this.SubscriptionType;
			}
		}
	}
}
