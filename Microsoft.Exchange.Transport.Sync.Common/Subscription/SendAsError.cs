using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	internal enum SendAsError
	{
		Success,
		InvalidSubscriptionGuid,
		SubscriptionDisabledForSendAs
	}
}
