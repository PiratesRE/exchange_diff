using System;

namespace Microsoft.Exchange.PushNotifications
{
	internal class MailboxNotificationFragment : MulticastNotificationFragment<MailboxNotificationPayload, MailboxNotificationRecipient>
	{
		public MailboxNotificationFragment(string notificationId, MailboxNotificationPayload payload, MailboxNotificationRecipient recipient) : base(notificationId, payload, recipient)
		{
		}
	}
}
