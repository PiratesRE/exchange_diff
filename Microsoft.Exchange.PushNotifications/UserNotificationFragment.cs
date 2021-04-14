using System;

namespace Microsoft.Exchange.PushNotifications
{
	internal class UserNotificationFragment<T> : MulticastNotificationFragment<T, UserNotificationRecipient> where T : UserNotificationPayload
	{
		public UserNotificationFragment(string notificationId, T payload, UserNotificationRecipient recipient) : base(notificationId, payload, recipient)
		{
		}
	}
}
