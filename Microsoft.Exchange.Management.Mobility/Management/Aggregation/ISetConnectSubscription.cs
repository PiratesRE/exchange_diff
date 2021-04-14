using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISetConnectSubscription
	{
		void StampChangesOn(ConnectSubscriptionProxy subscription);

		void NotifyApps(MailboxSession mailbox);
	}
}
