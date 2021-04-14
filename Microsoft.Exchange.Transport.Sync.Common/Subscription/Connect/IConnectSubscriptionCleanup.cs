using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConnectSubscriptionCleanup
	{
		void Cleanup(MailboxSession mailbox, IConnectSubscription subscription, bool sendRPCNotification = true);
	}
}
