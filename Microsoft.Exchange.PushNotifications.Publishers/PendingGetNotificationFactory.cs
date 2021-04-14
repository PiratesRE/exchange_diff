using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PendingGetNotificationFactory : PushNotificationFactory<PendingGetNotification>
	{
		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId)
		{
			throw new NotImplementedException("Monitoring PendingGet publishers is not supported");
		}

		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId)
		{
			throw new NotImplementedException("Monitoring PendingGet publishers is not supported");
		}

		protected override bool TryCreateUnseenEmailNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out PendingGetNotification notification)
		{
			notification = new PendingGetNotification(recipient.AppId, context.OrgId, recipient.DeviceId, new PendingGetPayload(payload.UnseenEmailCount, false));
			return true;
		}

		protected override bool TryCreateMonitoringNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out PendingGetNotification notification)
		{
			notification = null;
			return false;
		}

		public static readonly PendingGetNotificationFactory Default = new PendingGetNotificationFactory();
	}
}
