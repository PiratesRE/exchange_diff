using System;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	internal delegate void SubscriptionCacheMessageProcessingCallback(Guid mailboxGuid, SubscriptionCacheMessage cacheMessage, Exception exception);
}
