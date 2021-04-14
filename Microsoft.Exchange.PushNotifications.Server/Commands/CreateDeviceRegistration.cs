using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class CreateDeviceRegistration : PublishNotificationsBase<AzureDeviceRegistrationInfo>
	{
		public CreateDeviceRegistration(AzureDeviceRegistrationInfo registrationInfo, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(registrationInfo, publisherManager, asyncCallback, asyncState)
		{
		}

		public CreateDeviceRegistration(AzureDeviceRegistrationInfo registrationInfo, PushNotificationPublisherManager publisherManager, string hubName, AsyncCallback asyncCallback, object asyncState) : base(registrationInfo, publisherManager, asyncCallback, asyncState)
		{
			this.HubName = hubName;
		}

		protected string HubName { get; set; }

		protected override void Publish()
		{
			base.PublisherManager.Publish(base.RequestInstance, new PushNotificationPublishingContext(base.NotificationSource, OrganizationId.ForestWideOrgId, false, this.HubName));
		}
	}
}
