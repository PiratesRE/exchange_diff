using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UnsubscribeToNotification : UnsubscribeToNotificationBase
	{
		public UnsubscribeToNotification(CallContext callContext, SubscriptionData[] subscriptionData) : base(callContext, subscriptionData)
		{
		}

		protected override void InternalUnsubscribeNotification(IMailboxContext userContext, SubscriptionData subscription)
		{
			if (subscription.Parameters == null)
			{
				throw new ArgumentNullException("Subscription data parameters cannot be null");
			}
			NotificationType notificationType = subscription.Parameters.NotificationType;
			if (notificationType == NotificationType.UnseenItemNotification)
			{
				userContext.NotificationManager.UnsubscribeToUnseenCountNotification(subscription.SubscriptionId, subscription.Parameters);
				return;
			}
			base.InternalUnsubscribeNotification(userContext, subscription);
		}
	}
}
