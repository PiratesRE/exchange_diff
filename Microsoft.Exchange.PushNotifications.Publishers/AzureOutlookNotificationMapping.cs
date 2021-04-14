using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureOutlookNotificationMapping : IPushNotificationMapping<AzureNotification>
	{
		public AzureOutlookNotificationMapping()
		{
			this.InputType = typeof(OutlookNotificationFragment);
		}

		public Type InputType { get; private set; }

		private OrganizationIdConverter OrgIdConverter { get; set; }

		public bool TryMap(Notification notification, PushNotificationPublishingContext context, out AzureNotification pushNotification)
		{
			OutlookNotificationFragment outlookNotificationFragment = notification as OutlookNotificationFragment;
			if (outlookNotificationFragment == null)
			{
				throw new InvalidOperationException("Notification type not supported: " + notification.ToFullString());
			}
			pushNotification = new AzureNotification(outlookNotificationFragment.Recipient.AppId, outlookNotificationFragment.Recipient.DeviceId, context.OrgId, new AzurePayload(null, null, outlookNotificationFragment.Payload.Data, null), false);
			return true;
		}
	}
}
