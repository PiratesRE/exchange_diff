using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubscriptionManager
	{
		void UpdateSubscriptionToMailbox(MailboxSession mailboxSession, ISyncWorkerData subscription);

		void DeleteSubscription(MailboxSession mailboxSession, ISyncWorkerData subscription, bool sendRpcNotification);
	}
}
