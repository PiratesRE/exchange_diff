using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SubscribeToGroupNotification : SubscribeToNotificationBase
	{
		public SubscribeToGroupNotification(NotificationSubscribeJsonRequest request, CallContext callContext, SubscriptionData[] subscriptionData) : base(request, callContext, subscriptionData)
		{
		}

		protected override void InternalCreateASubscription(IMailboxContext userContext, SubscriptionData subscription, bool remoteSubscription)
		{
			NotificationType notificationType = subscription.Parameters.NotificationType;
			switch (notificationType)
			{
			case NotificationType.RowNotification:
				if (subscription.Parameters.FolderId != null)
				{
					throw new ArgumentException("Subscription parameter FolderId cannot be specified on Group subscriptions");
				}
				this.CreateSubscriptionForWellKnownFolder(userContext, subscription, DefaultFolderType.Inbox);
				return;
			case NotificationType.CalendarItemNotification:
				this.CreateSubscriptionForWellKnownFolder(userContext, subscription, DefaultFolderType.Calendar);
				return;
			default:
			{
				if (notificationType == NotificationType.SearchNotification)
				{
					bool flag;
					RemoteNotificationManager.Instance.Subscribe(userContext.Key.ToString(), userContext.ExchangePrincipal.MailboxInfo.MailboxGuid.ToString(), "SearchNotification", subscription.Parameters.ChannelId, RemoteRequestProcessor.GetRequesterUserId(base.CallContext.HttpContext.Request.Headers), NotificationType.SearchNotification, out flag);
					return;
				}
				if (notificationType != NotificationType.InstantSearchNotification)
				{
					base.InternalCreateASubscription(userContext, subscription, true);
					return;
				}
				UserContext userContext2 = userContext as UserContext;
				if (userContext2 == null)
				{
					throw new OwaInvalidOperationException("UserContext isn't a full user context. Cannot subscribe to InstantSearch notifications.");
				}
				InstantSearchRemoteNotificationHandler instantSearchRemoteNotificationHandler = userContext2.InstantSearchNotificationHandler as InstantSearchRemoteNotificationHandler;
				if (instantSearchRemoteNotificationHandler == null)
				{
					throw new OwaInvalidOperationException("Wrong notification handler for an InstantSearch remote subscription scenario.");
				}
				instantSearchRemoteNotificationHandler.RegisterNotifier(subscription.SubscriptionId);
				return;
			}
			}
		}

		private void CreateSubscriptionForWellKnownFolder(IMailboxContext userContext, SubscriptionData subscription, DefaultFolderType folderType)
		{
			try
			{
				userContext.LockAndReconnectMailboxSession(3000);
				StoreObjectId defaultFolderId = userContext.MailboxSession.GetDefaultFolderId(folderType);
				subscription.Parameters.FolderId = StoreId.StoreIdToEwsId(userContext.MailboxSession.MailboxGuid, defaultFolderId);
			}
			finally
			{
				if (userContext.MailboxSessionLockedByCurrentThread())
				{
					userContext.UnlockAndDisconnectMailboxSession();
				}
			}
			base.InternalCreateASubscription(userContext, subscription, true);
		}
	}
}
