using System;
using System.Management.Automation;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Get", "ConnectSubscription", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class GetConnectSubscription : GetSubscriptionBase<ConnectSubscriptionProxy>
	{
		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				return AggregationSubscriptionType.AllThatSupportPolicyInducedDeletion;
			}
		}

		protected override AggregationType AggregationTypeValue
		{
			get
			{
				return AggregationType.PeopleConnection;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || ConnectSubscriptionTaskKnownExceptions.IsKnown(exception);
		}
	}
}
