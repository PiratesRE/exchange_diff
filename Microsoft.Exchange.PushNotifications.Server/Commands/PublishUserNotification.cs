using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class PublishUserNotification : PublishNotificationsBase<RemoteUserNotification>
	{
		public PublishUserNotification(RemoteUserNotification notification, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(notification, publisherManager, asyncCallback, asyncState)
		{
		}

		protected override void Publish()
		{
			string externalDirectoryOrgId = (base.RequestInstance != null && base.RequestInstance.Payload != null) ? base.RequestInstance.Payload.TenantId : null;
			OrganizationId organizationId = OrganizationIdConverter.Default.GetOrganizationId(externalDirectoryOrgId);
			PushNotificationPublishingContext context = new PushNotificationPublishingContext(base.NotificationSource, organizationId, false, null);
			base.PublisherManager.Publish(base.RequestInstance, context);
		}
	}
}
