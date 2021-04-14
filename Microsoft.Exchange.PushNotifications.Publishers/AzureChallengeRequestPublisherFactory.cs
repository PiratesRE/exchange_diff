using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureChallengeRequestPublisherFactory : PushNotificationPublisherFactory
	{
		public AzureChallengeRequestPublisherFactory(List<IPushNotificationMapping<AzureChallengeRequestNotification>> mappings = null, AzureHubEventHandler hubEventHandler = null)
		{
			this.Mappings = mappings;
			this.AzureHubHandler = hubEventHandler;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.AzureChallengeRequest;
			}
		}

		public virtual AzureHubEventHandler AzureHubHandler { get; private set; }

		private List<IPushNotificationMapping<AzureChallengeRequestNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			AzureChallengeRequestPublisherSettings azureChallengeRequestPublisherSettings = settings as AzureChallengeRequestPublisherSettings;
			if (azureChallengeRequestPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an AzureSecretRequestPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			AzureChallengeRequestPublisher azureChallengeRequestPublisher = new AzureChallengeRequestPublisher(azureChallengeRequestPublisherSettings, this.Mappings);
			if (this.AzureHubHandler != null)
			{
				azureChallengeRequestPublisher.MissingHubDetected += this.AzureHubHandler.OnMissingHub;
			}
			return azureChallengeRequestPublisher;
		}
	}
}
