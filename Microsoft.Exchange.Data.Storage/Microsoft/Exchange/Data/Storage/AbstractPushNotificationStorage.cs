using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractPushNotificationStorage : AbstractItem, IPushNotificationStorage, IDisposable
	{
		public virtual string TenantId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual List<PushNotificationServerSubscription> GetActiveNotificationSubscriptions(IMailboxSession mailboxSession, uint expirationInHours)
		{
			throw new NotImplementedException();
		}

		public virtual List<StoreObjectId> GetExpiredNotificationSubscriptions(uint expirationInHours)
		{
			throw new NotImplementedException();
		}

		public virtual List<PushNotificationServerSubscription> GetNotificationSubscriptions(IMailboxSession mailboxSession)
		{
			throw new NotImplementedException();
		}

		public virtual IPushNotificationSubscriptionItem CreateOrUpdateSubscriptionItem(IMailboxSession mailboxSession, string subscriptionId, PushNotificationServerSubscription subscription)
		{
			throw new NotImplementedException();
		}

		public virtual void DeleteAllSubscriptions()
		{
			throw new NotImplementedException();
		}

		public virtual void DeleteExpiredSubscriptions(uint expirationInHours)
		{
			throw new NotImplementedException();
		}

		public virtual void DeleteSubscription(StoreObjectId itemId)
		{
			throw new NotImplementedException();
		}

		public virtual void DeleteSubscription(string subscriptionId)
		{
			throw new NotImplementedException();
		}
	}
}
