using System;
using Microsoft.Exchange.Data.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WebAppNotificationFactory : PushNotificationFactory<WebAppNotification>
	{
		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId)
		{
			return new MailboxNotificationRecipient(appId, recipientId.ToString(), DateTime.UtcNow, null);
		}

		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId)
		{
			throw new NotImplementedException("RecipientId is required as int for creating WebApp MonitoringRecipient");
		}

		protected override bool TryCreateUnseenEmailNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out WebAppNotification notification)
		{
			notification = new WebAppNotification(recipient.AppId, context.OrgId, "PublishO365Notification", new O365Notification(recipient.DeviceId, payload.UnseenEmailCount.ToString()).ToJson());
			return true;
		}

		protected override bool TryCreateMonitoringNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out WebAppNotification notification)
		{
			notification = new WebAppMonitoringNotification(recipient.AppId, recipient.DeviceId);
			return true;
		}

		public static readonly WebAppNotificationFactory Default = new WebAppNotificationFactory();
	}
}
