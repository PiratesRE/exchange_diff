using System;

namespace Microsoft.Exchange.PushNotifications
{
	internal class RemoteUserNotificationFragment : UserNotificationFragment<RemoteUserNotificationPayload>
	{
		public RemoteUserNotificationFragment(string notificationId, RemoteUserNotificationPayload payload, UserNotificationRecipient recipient) : base(notificationId, payload, recipient)
		{
		}
	}
}
