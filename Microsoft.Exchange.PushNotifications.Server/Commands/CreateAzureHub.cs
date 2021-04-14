using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class CreateAzureHub : PublishNotificationsBase<AzureHubDefinition>
	{
		public CreateAzureHub(AzureHubDefinition hubDefinition, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(hubDefinition, publisherManager, asyncCallback, asyncState)
		{
		}

		protected override void Publish()
		{
			base.PublisherManager.Publish(base.RequestInstance, new PushNotificationPublishingContext(base.NotificationSource, OrganizationId.ForestWideOrgId, false, null));
		}
	}
}
