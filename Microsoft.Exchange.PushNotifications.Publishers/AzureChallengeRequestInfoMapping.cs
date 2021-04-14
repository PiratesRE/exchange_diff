using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureChallengeRequestInfoMapping : IPushNotificationMapping<AzureChallengeRequestNotification>
	{
		public AzureChallengeRequestInfoMapping(PushNotificationPublisherConfiguration configuration)
		{
			this.InputType = typeof(AzureChallengeRequestInfo);
			this.publisherConfiguration = configuration.AzureSendPublisherSettings;
		}

		public Type InputType { get; private set; }

		public bool TryMap(Notification notification, PushNotificationPublishingContext context, out AzureChallengeRequestNotification pushNotification)
		{
			AzureChallengeRequestInfo azureChallengeRequestInfo = notification as AzureChallengeRequestInfo;
			if (azureChallengeRequestInfo == null)
			{
				throw new InvalidOperationException("Notification type not supported: " + notification.ToFullString());
			}
			if (this.publisherConfiguration.ContainsKey(azureChallengeRequestInfo.TargetAppId))
			{
				AzureChannelSettings channelSettings = this.publisherConfiguration[azureChallengeRequestInfo.TargetAppId].ChannelSettings;
				IAzureSasTokenProvider azureSasTokenProvider = channelSettings.AzureSasTokenProvider;
				AzureUriTemplate uriTemplate = channelSettings.UriTemplate;
				if (azureChallengeRequestInfo.IsMonitoring)
				{
					pushNotification = new AzureChallengeRequestMonitoringNotification(azureChallengeRequestInfo.AppId, azureChallengeRequestInfo.TargetAppId, azureSasTokenProvider, uriTemplate, azureChallengeRequestInfo.DeviceId, new AzureChallengeRequestPayload(azureChallengeRequestInfo.Platform.Value, azureChallengeRequestInfo.DeviceChallenge), azureChallengeRequestInfo.HubName);
				}
				else
				{
					string hubName = azureChallengeRequestInfo.HubName;
					if (string.IsNullOrEmpty(hubName) && !string.IsNullOrEmpty(context.HubName))
					{
						hubName = context.HubName;
					}
					pushNotification = new AzureChallengeRequestNotification(azureChallengeRequestInfo.AppId, azureChallengeRequestInfo.TargetAppId, azureSasTokenProvider, uriTemplate, azureChallengeRequestInfo.DeviceId, new AzureChallengeRequestPayload(azureChallengeRequestInfo.Platform.Value, azureChallengeRequestInfo.DeviceChallenge), hubName);
				}
				return true;
			}
			pushNotification = null;
			return false;
		}

		private readonly Dictionary<string, AzurePublisherSettings> publisherConfiguration;
	}
}
