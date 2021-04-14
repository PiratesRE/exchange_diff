using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureNotificationFactory : PushNotificationFactory<AzureNotification>
	{
		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId)
		{
			return new MailboxNotificationRecipient(appId, recipientId, DateTime.UtcNow, null);
		}

		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId)
		{
			throw new NotImplementedException("RecipientId is required as string for creating Azure MonitoringRecipient");
		}

		protected override bool TryCreateUnseenEmailNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out AzureNotification notification)
		{
			notification = this.CreateAzureNotification(payload, recipient, context, null, null);
			return true;
		}

		protected override bool TryCreateMonitoringNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out AzureNotification notification)
		{
			notification = new AzureMonitoringNotification(recipient.AppId, payload.TenantId, recipient.DeviceId);
			return true;
		}

		private AzureNotification CreateAzureNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, string message, string storeObjectId)
		{
			string recipient2 = (!string.IsNullOrWhiteSpace(recipient.InstallationId)) ? recipient.InstallationId : recipient.DeviceId;
			if (OrganizationId.ForestWideOrgId.Equals(context.OrgId) && !string.IsNullOrEmpty(context.HubName))
			{
				return new AzureNotification(recipient.AppId, recipient2, context.HubName, new AzurePayload(payload.UnseenEmailCount, message, storeObjectId, base.GetBackgroundSyncTypeString(payload.BackgroundSyncType)), context.RequireDeviceRegistration);
			}
			return new AzureNotification(recipient.AppId, recipient2, context.OrgId, new AzurePayload(payload.UnseenEmailCount, message, storeObjectId, base.GetBackgroundSyncTypeString(payload.BackgroundSyncType)), context.RequireDeviceRegistration);
		}

		public static readonly AzureNotificationFactory Default = new AzureNotificationFactory();
	}
}
