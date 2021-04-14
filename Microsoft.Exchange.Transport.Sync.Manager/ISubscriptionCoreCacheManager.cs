using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubscriptionCoreCacheManager
	{
		void DeleteCacheMessage(MailboxSession systemMailboxSession, StoreObjectId cacheMessageId);

		void SaveCacheMessage(SubscriptionCacheMessage cacheMessage);

		SubscriptionCacheMessage BindCacheMessage(MailboxSession systemMailboxSession, StoreObjectId cacheFolderId, Guid mailboxGuid, bool loadSubscriptions);

		SubscriptionCacheMessage BindCacheMessage(MailboxSession systemMailboxSession, StoreObjectId cacheMessageId, bool loadSubscriptions);

		SubscriptionCacheMessage CreateCacheMessage(MailboxSession systemMailboxSession, StoreObjectId cacheFolderId, Guid mailboxGuid, ExDateTime subscriptionListTimestamp);
	}
}
