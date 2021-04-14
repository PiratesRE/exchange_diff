using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionMessageHelper
	{
		public SubscriptionMessageHelper() : this(CommonLoggingHelper.SyncLogSession)
		{
		}

		protected SubscriptionMessageHelper(SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.syncLogSession = syncLogSession;
		}

		public virtual void SaveSubscription(MailboxSession mailboxSession, ISyncWorkerData subscription)
		{
			StoreObjectId subscriptionMessageId = subscription.SubscriptionMessageId;
			if (subscriptionMessageId == null)
			{
				using (Folder folder = Folder.Bind(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)))
				{
					using (MessageItem messageItem = MessageItem.CreateAssociated(mailboxSession, folder.Id))
					{
						this.SaveSubscriptionToMessage(subscription, messageItem);
						messageItem.Load(new PropertyDefinition[]
						{
							ItemSchema.Id
						});
						subscription.SubscriptionMessageId = messageItem.Id.ObjectId;
					}
					return;
				}
			}
			AggregationSubscriptionType subscriptionType = subscription.SubscriptionType;
			PropertyDefinition[] propertyDefinitions = SubscriptionManager.GetPropertyDefinitions(subscriptionType);
			using (MessageItem messageItem2 = MessageItem.Bind(mailboxSession, subscriptionMessageId, propertyDefinitions))
			{
				messageItem2.OpenAsReadWrite();
				this.SaveSubscriptionToMessage(subscription, messageItem2);
			}
		}

		public virtual void SaveSubscriptionToMessage(ISyncWorkerData subscription, MessageItem message)
		{
			this.syncLogSession.RetailAssert(subscription.Status != AggregationStatus.InvalidVersion, "Invalid Version messages cannot be saved back. They are to be marked in memory only.", new object[0]);
			subscription.SetToMessageObject(message);
			message.Save(SaveMode.NoConflictResolution);
		}

		public virtual void DeleteSubscription(MailboxSession mailboxSession, StoreId messageId)
		{
			mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				messageId
			});
		}

		private readonly SyncLogSession syncLogSession;
	}
}
