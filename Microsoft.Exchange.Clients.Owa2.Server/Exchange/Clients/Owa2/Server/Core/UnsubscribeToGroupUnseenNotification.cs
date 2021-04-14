using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UnsubscribeToGroupUnseenNotification : UnsubscribeToNotificationBase
	{
		public UnsubscribeToGroupUnseenNotification(CallContext callContext, SubscriptionData[] subscriptionData) : base(callContext, subscriptionData)
		{
		}

		public static bool RequestShouldUseSharedContext(string methodName)
		{
			return methodName == "UnsubscribeToGroupUnseenNotification";
		}

		protected override void InternalUnsubscribeNotification(IMailboxContext userContext, SubscriptionData subscription)
		{
			if (subscription.Parameters == null)
			{
				throw new ArgumentNullException("Subscription data parameters cannot be null");
			}
			if (subscription.Parameters.NotificationType != NotificationType.UnseenItemNotification)
			{
				throw new OwaInvalidOperationException("Invalid Notification type specified when calling UnsubscribeToGroupUnseenNotification");
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[UnsubscribeToGroupUnseenNotification::InternalUnsubscribeNotification] Unsubscribe for unseen notifications for subscription {0}", subscription.SubscriptionId);
		}
	}
}
