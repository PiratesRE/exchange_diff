using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubEventHandler
	{
		public AzureHubEventHandler()
		{
			this.PublishingContext = new PushNotificationPublishingContext(base.GetType().Name, OrganizationId.ForestWideOrgId, false, null);
		}

		public PushNotificationPublisherManager PublisherManager { get; set; }

		private PushNotificationPublishingContext PublishingContext { get; set; }

		public virtual void OnMissingHub(object sender, MissingHubEventArgs missingHubArgs)
		{
			if (this.PublisherManager != null)
			{
				this.PublisherManager.Publish(new AzureHubDefinition(missingHubArgs.HubName, missingHubArgs.TargetAppId), this.PublishingContext);
			}
		}
	}
}
