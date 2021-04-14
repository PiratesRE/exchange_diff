using System;
using System.Management.Automation;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Get", "ImapSubscription", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class GetImapSubscription : GetSubscriptionBase<IMAPSubscriptionProxy>
	{
		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				return AggregationSubscriptionType.IMAP;
			}
		}
	}
}
