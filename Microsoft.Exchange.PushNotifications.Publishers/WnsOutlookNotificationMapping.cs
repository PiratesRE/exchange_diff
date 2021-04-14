using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsOutlookNotificationMapping : IPushNotificationMapping<WnsNotification>
	{
		public WnsOutlookNotificationMapping()
		{
			this.InputType = typeof(OutlookNotificationFragment);
		}

		public Type InputType { get; private set; }

		private OrganizationIdConverter OrgIdConverter { get; set; }

		public bool TryMap(Notification notification, PushNotificationPublishingContext context, out WnsNotification pushNotification)
		{
			OutlookNotificationFragment outlookNotificationFragment = notification as OutlookNotificationFragment;
			if (outlookNotificationFragment == null)
			{
				throw new InvalidOperationException("Notification type not supported: " + notification.ToFullString());
			}
			pushNotification = new WnsRawNotification(outlookNotificationFragment.Recipient.AppId, context.OrgId, outlookNotificationFragment.Recipient.DeviceId, outlookNotificationFragment.Payload.Data);
			return true;
		}
	}
}
