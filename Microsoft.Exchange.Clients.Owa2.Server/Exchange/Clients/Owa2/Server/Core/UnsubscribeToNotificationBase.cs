using System;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class UnsubscribeToNotificationBase : ServiceCommand<bool>
	{
		protected UnsubscribeToNotificationBase(CallContext callContext, SubscriptionData[] subscriptionData) : base(callContext)
		{
			if (subscriptionData == null)
			{
				throw new ArgumentNullException("Subscription data cannot be null");
			}
			this.subscriptionData = subscriptionData;
			OwsLogRegistry.Register(base.GetType().Name, typeof(SubscribeToNotificationMetadata), new Type[0]);
		}

		protected override bool InternalExecute()
		{
			IMailboxContext mailboxContext = UserContextManager.GetMailboxContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, false);
			if (mailboxContext.NotificationManager == null)
			{
				throw new OwaInvalidOperationException("UserContext.NotificationManager is null");
			}
			for (int i = 0; i < this.subscriptionData.Length; i++)
			{
				SubscriptionData subscriptionData = this.subscriptionData[i];
				bool flag = RemoteRequestProcessor.IsRemoteRequest(base.CallContext.HttpContext.Request.Headers);
				if (flag && string.IsNullOrWhiteSpace(subscriptionData.Parameters.ChannelId))
				{
					throw new OwaInvalidRequestException("ChannelId is null or empty. ChannelId is required for remote notification unsubscribe requests.");
				}
				this.InternalUnsubscribeNotification(mailboxContext, subscriptionData);
				if (flag)
				{
					RemoteNotificationManager.Instance.UnSubscribe(mailboxContext.Key.ToString(), subscriptionData.SubscriptionId, subscriptionData.Parameters.ChannelId, RemoteRequestProcessor.GetRequesterUserId(base.CallContext.HttpContext.Request.Headers));
				}
			}
			return true;
		}

		protected virtual void InternalUnsubscribeNotification(IMailboxContext userContext, SubscriptionData subscription)
		{
			if (subscription.Parameters == null)
			{
				throw new ArgumentNullException("Subscription data parameters cannot be null");
			}
			NotificationType notificationType = subscription.Parameters.NotificationType;
			switch (notificationType)
			{
			case NotificationType.RowNotification:
			case NotificationType.CalendarItemNotification:
				userContext.NotificationManager.UnsubscribeForRowNotifications(subscription.SubscriptionId, subscription.Parameters);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<NotificationType>((long)this.GetHashCode(), "[UnsubscribeToNotificationBase::InternalExecute] Unsubscribe for row notifications ({0})", subscription.Parameters.NotificationType);
				return;
			default:
				if (notificationType == NotificationType.SearchNotification)
				{
					RemoteNotificationManager.Instance.UnSubscribe(userContext.Key.ToString(), "SearchNotification", subscription.Parameters.ChannelId, RemoteRequestProcessor.GetRequesterUserId(base.CallContext.HttpContext.Request.Headers));
					return;
				}
				if (notificationType != NotificationType.InstantSearchNotification)
				{
					throw new OwaInvalidOperationException("Invalid Notification type specified when calling unsubscribe");
				}
				return;
			}
		}

		private readonly SubscriptionData[] subscriptionData;
	}
}
