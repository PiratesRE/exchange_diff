using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureDeviceRegistrationPublisherFactory : PushNotificationPublisherFactory
	{
		public AzureDeviceRegistrationPublisherFactory(List<IPushNotificationMapping<AzureDeviceRegistrationNotification>> mappings = null, AzureHubEventHandler hubEventHandler = null)
		{
			this.Mappings = mappings;
			this.AzureHubHandler = hubEventHandler;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.AzureDeviceRegistration;
			}
		}

		public virtual AzureHubEventHandler AzureHubHandler { get; private set; }

		private List<IPushNotificationMapping<AzureDeviceRegistrationNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			AzureDeviceRegistrationPublisherSettings azureDeviceRegistrationPublisherSettings = settings as AzureDeviceRegistrationPublisherSettings;
			if (azureDeviceRegistrationPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an AzureDeviceRegistrationPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			AzureDeviceRegistrationPublisher azureDeviceRegistrationPublisher = new AzureDeviceRegistrationPublisher(azureDeviceRegistrationPublisherSettings, this.Mappings);
			if (this.AzureHubHandler != null)
			{
				azureDeviceRegistrationPublisher.MissingHubDetected += this.AzureHubHandler.OnMissingHub;
			}
			return azureDeviceRegistrationPublisher;
		}
	}
}
