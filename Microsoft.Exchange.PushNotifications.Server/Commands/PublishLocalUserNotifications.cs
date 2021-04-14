using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class PublishLocalUserNotifications : PublishNotificationsBase<LocalUserNotificationBatch>
	{
		public PublishLocalUserNotifications(LocalUserNotificationBatch notifications, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(notifications, publisherManager, asyncCallback, asyncState)
		{
		}

		protected override void Publish()
		{
			foreach (LocalUserNotification localUserNotification in base.RequestInstance.Notifications)
			{
				string externalDirectoryOrgId = (localUserNotification != null && localUserNotification.Payload != null) ? localUserNotification.Payload.TenantId : null;
				OrganizationId organizationId = OrganizationIdConverter.Default.GetOrganizationId(externalDirectoryOrgId);
				PushNotificationPublishingContext context = new PushNotificationPublishingContext(base.NotificationSource, organizationId, false, null);
				base.PublisherManager.Publish(localUserNotification, context);
			}
		}
	}
}
