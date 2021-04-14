using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IMonitoringMailboxNotificationRecipientFactory
	{
		MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId);

		MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId);
	}
}
