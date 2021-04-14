using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class PublishOutlookNotifications : PublishNotificationsBase<OutlookNotificationBatch>
	{
		public PublishOutlookNotifications(OutlookNotificationBatch notifications, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(notifications, publisherManager, asyncCallback, asyncState)
		{
		}

		protected override void Publish()
		{
			foreach (OutlookNotification outlookNotification in base.RequestInstance.Notifications)
			{
				string externalDirectoryOrgId = (outlookNotification != null && outlookNotification.Payload != null) ? outlookNotification.Payload.TenantId : null;
				OrganizationId organizationId = OrganizationIdConverter.Default.GetOrganizationId(externalDirectoryOrgId);
				PushNotificationPublishingContext context = new PushNotificationPublishingContext(base.NotificationSource, organizationId, false, null);
				base.PublisherManager.Publish(outlookNotification, context);
			}
		}
	}
}
