using System;

namespace Microsoft.Exchange.PushNotifications
{
	internal class LocalUserNotificationFragment : UserNotificationFragment<LocalUserNotificationPayload>
	{
		public LocalUserNotificationFragment(string notificationId, LocalUserNotificationPayload payload, UserNotificationRecipient recipient) : base(notificationId, payload, recipient)
		{
		}
	}
}
