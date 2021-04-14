using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IPendingGetConnection
	{
		bool FireUnseenEmailNotification(int unseenCount, int notificationId);

		void SubscribeToUnseenEmailNotification(PendingGetContext pendingGetContext, long timeoutInMilliseconds, int latestUnseenEmailNotificationId);
	}
}
