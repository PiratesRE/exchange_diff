using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsNotificationFactory : PushNotificationFactory<ApnsNotification>
	{
		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId)
		{
			return new MailboxNotificationRecipient(appId, string.Format("{0:d64}", recipientId), DateTime.UtcNow, null);
		}

		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId)
		{
			throw new NotImplementedException("RecipientId is required as int for creating APNS MonitoringRecipient");
		}

		protected override bool TryCreateUnseenEmailNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out ApnsNotification notification)
		{
			return this.TryCreateApnsNotification(payload, recipient, context.OrgId, null, null, out notification);
		}

		protected override bool TryCreateMonitoringNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out ApnsNotification notification)
		{
			notification = new ApnsMonitoringNotification(recipient.AppId, recipient.DeviceId);
			return true;
		}

		private bool TryCreateApnsNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, OrganizationId tenantId, ApnsAlert apnsAlert, string storeObjectId, out ApnsNotification notification)
		{
			notification = new ApnsNotification(recipient.AppId, tenantId, recipient.DeviceId, new ApnsPayload(new ApnsPayloadBasicData(payload.UnseenEmailCount, null, apnsAlert, (payload.BackgroundSyncType == BackgroundSyncType.None) ? 0 : 1), storeObjectId, base.GetBackgroundSyncTypeString(payload.BackgroundSyncType)), 0, recipient.LastSubscriptionUpdate);
			return true;
		}

		public static readonly ApnsNotificationFactory Default = new ApnsNotificationFactory();
	}
}
