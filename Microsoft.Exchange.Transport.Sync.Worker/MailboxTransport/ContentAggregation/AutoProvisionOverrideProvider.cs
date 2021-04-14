using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AutoProvisionOverrideProvider : IAutoProvisionOverrideProvider
	{
		public bool TryGetOverrides(string domain, AggregationSubscriptionType type, out string[] overrideHosts, out bool trustForSendAs)
		{
			return AutoProvisionOverride.TryGetOverrides(domain, type, out overrideHosts, out trustForSendAs);
		}
	}
}
