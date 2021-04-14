using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class NewMailNotificationHandler : MapiNotificationHandlerBase
	{
		public NewMailNotificationHandler(string subscriptionId, IMailboxContext userContext) : base(subscriptionId, userContext, false)
		{
			this.newMailNotifier = new NewMailNotifier(subscriptionId, userContext);
			this.newMailNotifier.RegisterWithPendingRequestNotifier();
		}

		internal StoreObjectId InboxFolderId
		{
			get
			{
				if (this.inboxFolderId == null)
				{
					try
					{
						base.UserContext.LockAndReconnectMailboxSession(3000);
						this.inboxFolderId = base.UserContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
					}
					catch (OwaLockTimeoutException ex)
					{
						ExTraceGlobals.CoreCallTracer.TraceError<string>((long)this.GetHashCode(), "User context lock timed out in trying to get InboxFolderId. Exception: {0}", ex.Message);
					}
					finally
					{
						if (base.UserContext.MailboxSessionLockedByCurrentThread())
						{
							base.UserContext.UnlockAndDisconnectMailboxSession();
						}
					}
				}
				return this.inboxFolderId;
			}
		}

		internal override void HandleNotificationInternal(Notification notif, MapiNotificationsLogEvent logEvent, object context)
		{
			NewMailNotification newMailNotification = notif as NewMailNotification;
			if (newMailNotification == null)
			{
				return;
			}
			if (newMailNotification.NewMailItemId == null || newMailNotification.ParentFolderId == null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "notification has a null notifying item id");
				return;
			}
			StoreObjectId parentFolderId = newMailNotification.ParentFolderId;
			if (parentFolderId == null || newMailNotification.NewMailItemId == null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "notification has a null notifying item id");
				return;
			}
			if (!parentFolderId.Equals(this.InboxFolderId))
			{
				return;
			}
			NewMailNotificationPayload newMailNotificationPayload = this.BindToItemAndCreatePayload(newMailNotification);
			if (newMailNotificationPayload != null)
			{
				this.newMailNotifier.Payload = newMailNotificationPayload;
				this.newMailNotifier.PickupData();
			}
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
			lock (base.SyncRoot)
			{
				base.InitSubscription();
			}
			this.newMailNotifier.PickupData();
		}

		protected override void InitSubscriptionInternal()
		{
			if (!base.UserContext.MailboxSessionLockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method NewMailNotificationHandler.InitSubscriptionInternal");
			}
			base.Subscription = Subscription.CreateMailboxSubscription(base.UserContext.MailboxSession, new NotificationHandler(base.HandleNotification), NotificationType.NewMail);
		}

		protected NewMailNotificationPayload BindToItemAndCreatePayload(NewMailNotification notification)
		{
			NewMailNotificationPayload result;
			try
			{
				base.UserContext.LockAndReconnectMailboxSession(3000);
				MessageItem newMessage = null;
				NewMailNotificationPayload payload = new NewMailNotificationPayload();
				payload.Source = MailboxLocation.FromMailboxContext(base.UserContext);
				payload.SubscriptionId = base.SubscriptionId;
				try
				{
					newMessage = Item.BindAsMessage(base.UserContext.MailboxSession, notification.NewMailItemId, this.GetAdditionalPropsToLoad());
					ExchangeVersion.ExecuteWithSpecifiedVersion(ExchangeVersion.Exchange2012, delegate
					{
						payload.ItemId = IdConverter.ConvertStoreItemIdToItemId(newMessage.Id, this.UserContext.MailboxSession).Id;
						payload.ConversationId = IdConverter.ConversationIdToEwsId(this.UserContext.MailboxSession.MailboxGuid, newMessage.GetConversation(new PropertyDefinition[0]).ConversationId);
					});
					if (newMessage != null)
					{
						if (newMessage.From != null && newMessage.From.DisplayName != null)
						{
							payload.Sender = newMessage.From.DisplayName;
						}
						if (newMessage.Subject != null)
						{
							payload.Subject = newMessage.Subject;
						}
						string previewText = newMessage.Body.PreviewText;
						if (previewText != null)
						{
							payload.PreviewText = previewText;
						}
						this.OnPayloadCreated(newMessage, payload);
						result = payload;
					}
					else
					{
						result = null;
					}
				}
				catch (ObjectNotFoundException)
				{
					result = null;
				}
				finally
				{
					if (newMessage != null)
					{
						newMessage.Dispose();
					}
				}
			}
			finally
			{
				if (base.UserContext.MailboxSessionLockedByCurrentThread())
				{
					base.UserContext.UnlockAndDisconnectMailboxSession();
				}
			}
			return result;
		}

		protected virtual PropertyDefinition[] GetAdditionalPropsToLoad()
		{
			return null;
		}

		protected virtual void OnPayloadCreated(MessageItem newMessage, NewMailNotificationPayload payload)
		{
		}

		private PropertyDefinition[] querySubscriptionProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.DisplayName,
			FolderSchema.ItemCount,
			FolderSchema.UnreadCount
		};

		private NewMailNotifier newMailNotifier;

		private StoreObjectId inboxFolderId;
	}
}
