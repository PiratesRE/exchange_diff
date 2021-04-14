using System;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SubscribeToGroupUnseenNotification : SubscribeToNotificationBase
	{
		public SubscribeToGroupUnseenNotification(NotificationSubscribeJsonRequest request, CallContext callContext, SubscriptionData[] subscriptionData) : base(request, callContext, subscriptionData)
		{
		}

		public static bool RequestShouldUseSharedContext(string methodName)
		{
			return methodName == "SubscribeToGroupUnseenNotification";
		}

		protected override void InternalCreateASubscription(IMailboxContext userContext, SubscriptionData subscription, bool remoteSubscription)
		{
			if (subscription.Parameters.FolderId != null)
			{
				throw new ArgumentException("Subscription parameter FolderId cannot be specified on Group subscriptions");
			}
			if (subscription.Parameters.NotificationType != NotificationType.UnseenItemNotification)
			{
				throw new OwaInvalidOperationException("Invalid Notification type specified when calling SubscribeToGroupUnseenNotification");
			}
			if (!(userContext is SharedContext))
			{
				throw new OwaInvalidOperationException("Unseen Notifications should be using a SharedContext");
			}
			this.metricType = SubscribeToNotificationMetadata.UnseenItemNotificationLatency;
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			UserMailboxLocator mailboxLocator = UserMailboxLocator.Instantiate(adrecipientSession, base.CallContext.AccessingADUser);
			string subscriptionId = userContext.NotificationManager.SubscribeToUnseenItemNotification(subscription.SubscriptionId, mailboxLocator, adrecipientSession);
			subscription.SubscriptionId = subscriptionId;
		}
	}
}
