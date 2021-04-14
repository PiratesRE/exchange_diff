using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class PublishNotifications : PublishNotificationsBase<MailboxNotificationBatch>
	{
		public PublishNotifications(MailboxNotificationBatch notifications, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(notifications, publisherManager, asyncCallback, asyncState)
		{
		}

		public PublishNotifications(MailboxNotificationBatch notifications, PushNotificationPublisherManager publisherManager, string hubName, AsyncCallback asyncCallback, object asyncState) : base(notifications, publisherManager, asyncCallback, asyncState)
		{
			this.HubName = hubName;
		}

		protected string HubName { get; set; }

		protected override void Publish()
		{
			foreach (MailboxNotification mailboxNotification in base.RequestInstance.Notifications)
			{
				string externalDirectoryOrgId = (mailboxNotification != null && mailboxNotification.Payload != null) ? mailboxNotification.Payload.TenantId : null;
				OrganizationId organizationId = OrganizationIdConverter.Default.GetOrganizationId(externalDirectoryOrgId);
				PushNotificationPublishingContext context = new PushNotificationPublishingContext(base.NotificationSource, organizationId, false, this.HubName);
				base.PublisherManager.Publish(mailboxNotification, context);
			}
		}
	}
}
