using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmNotificationFactory : PushNotificationFactory<GcmNotification>
	{
		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId)
		{
			return new MailboxNotificationRecipient(appId, recipientId.ToString(), DateTime.UtcNow, null);
		}

		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId)
		{
			throw new NotImplementedException("RecipientId is required as int for creating GCM MonitoringRecipient");
		}

		protected override bool TryCreateUnseenEmailNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out GcmNotification notification)
		{
			notification = this.CreateGcmNotification(payload, recipient, context.OrgId, null, null);
			return true;
		}

		protected override bool TryCreateMonitoringNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out GcmNotification notification)
		{
			notification = new GcmMonitoringNotification(recipient.AppId, recipient.DeviceId);
			return true;
		}

		private GcmNotification CreateGcmNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, OrganizationId tenantId, string message, string extraData)
		{
			return new GcmNotification(recipient.AppId, tenantId, recipient.DeviceId, new GcmPayload(payload.UnseenEmailCount, message, extraData, payload.BackgroundSyncType), "c", null, null);
		}

		public static readonly GcmNotificationFactory Default = new GcmNotificationFactory();
	}
}
