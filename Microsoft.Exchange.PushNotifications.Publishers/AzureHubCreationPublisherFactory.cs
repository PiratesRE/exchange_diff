using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubCreationPublisherFactory : PushNotificationPublisherFactory
	{
		public AzureHubCreationPublisherFactory(List<IPushNotificationMapping<AzureHubCreationNotification>> mappings = null)
		{
			this.Mappings = mappings;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.AzureHubCreation;
			}
		}

		private List<IPushNotificationMapping<AzureHubCreationNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			AzureHubCreationPublisherSettings azureHubCreationPublisherSettings = settings as AzureHubCreationPublisherSettings;
			if (azureHubCreationPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an AzureHubCreationPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new AzureHubCreationPublisher(azureHubCreationPublisherSettings, this.Mappings);
		}
	}
}
