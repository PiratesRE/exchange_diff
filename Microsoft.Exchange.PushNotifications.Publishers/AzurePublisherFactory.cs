using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzurePublisherFactory : PushNotificationPublisherFactory
	{
		public AzurePublisherFactory(List<IPushNotificationMapping<AzureNotification>> mappings = null)
		{
			this.Mappings = mappings;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.Azure;
			}
		}

		private List<IPushNotificationMapping<AzureNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			AzurePublisherSettings azurePublisherSettings = settings as AzurePublisherSettings;
			if (azurePublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an AzurePublisherFactory instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new AzurePublisher(azurePublisherSettings, this.Mappings);
		}
	}
}
