using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface INewConnectSubscription
	{
		IConfigurable PrepareSubscription(MailboxSession mailbox, ConnectSubscriptionProxy subscription);

		void InitializeFolderAndNotifyApps(MailboxSession mailbox, ConnectSubscriptionProxy subscription);

		string SubscriptionName { get; }

		string SubscriptionDisplayName { get; }
	}
}
