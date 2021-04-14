using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IAutoProvisionOverrideProvider
	{
		bool TryGetOverrides(string domain, AggregationSubscriptionType type, out string[] overrideHosts, out bool trustForSendAs);
	}
}
