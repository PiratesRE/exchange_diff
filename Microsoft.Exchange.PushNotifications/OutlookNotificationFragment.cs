using System;

namespace Microsoft.Exchange.PushNotifications
{
	internal class OutlookNotificationFragment : MulticastNotificationFragment<OutlookNotificationPayload, OutlookNotificationRecipient>
	{
		public OutlookNotificationFragment(string notificationId, OutlookNotificationPayload payload, OutlookNotificationRecipient recipient) : base(notificationId, payload, recipient)
		{
		}
	}
}
