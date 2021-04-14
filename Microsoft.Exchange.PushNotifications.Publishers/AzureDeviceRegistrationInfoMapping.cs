using System;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureDeviceRegistrationInfoMapping : IPushNotificationMapping<AzureDeviceRegistrationNotification>
	{
		public AzureDeviceRegistrationInfoMapping(PushNotificationPublisherConfiguration configuration)
		{
			this.InputType = typeof(AzureDeviceRegistrationInfo);
			this.publisherConfiguration = configuration.AzureSendPublisherSettings;
		}

		public Type InputType { get; private set; }

		public bool TryMap(Notification notification, PushNotificationPublishingContext context, out AzureDeviceRegistrationNotification pushNotification)
		{
			AzureDeviceRegistrationInfo azureDeviceRegistrationInfo = notification as AzureDeviceRegistrationInfo;
			if (azureDeviceRegistrationInfo == null)
			{
				throw new InvalidOperationException("Notification type not supported: " + notification.ToFullString());
			}
			if (!this.publisherConfiguration.ContainsKey(azureDeviceRegistrationInfo.TargetAppId))
			{
				pushNotification = null;
				return false;
			}
			AzurePublisherSettings azurePublisherSettings = this.publisherConfiguration[azureDeviceRegistrationInfo.TargetAppId];
			AzureChannelSettings channelSettings = azurePublisherSettings.ChannelSettings;
			if (channelSettings.IsRegistrationEnabled && !azurePublisherSettings.IsMultifactorRegistrationEnabled)
			{
				pushNotification = null;
				if (PushNotificationsCrimsonEvents.AzureDeviceRegistrationMappingDroppingRequest.IsEnabled(PushNotificationsCrimsonEvent.Provider))
				{
					PushNotificationsCrimsonEvents.AzureDeviceRegistrationMappingDroppingRequest.Log<string, string>(azureDeviceRegistrationInfo.TargetAppId, azureDeviceRegistrationInfo.RecipientId);
				}
				return false;
			}
			if (azureDeviceRegistrationInfo.IsMonitoring)
			{
				pushNotification = new AzureDeviceRegistrationMonitoringNotification(azureDeviceRegistrationInfo.AppId, azureDeviceRegistrationInfo.TargetAppId, channelSettings.AzureSasTokenProvider, channelSettings.UriTemplate, new AzureDeviceRegistrationPayload(azureDeviceRegistrationInfo.RecipientId, channelSettings.RegistrationTemplate, azureDeviceRegistrationInfo.Tag), azureDeviceRegistrationInfo.HubName, azureDeviceRegistrationInfo.ServerChallenge);
			}
			else
			{
				string hubName = azureDeviceRegistrationInfo.HubName;
				if (string.IsNullOrEmpty(hubName) && !string.IsNullOrEmpty(context.HubName))
				{
					hubName = context.HubName;
				}
				pushNotification = new AzureDeviceRegistrationNotification(azureDeviceRegistrationInfo.AppId, azureDeviceRegistrationInfo.TargetAppId, channelSettings.AzureSasTokenProvider, channelSettings.UriTemplate, new AzureDeviceRegistrationPayload(azureDeviceRegistrationInfo.RecipientId, channelSettings.RegistrationTemplate, azureDeviceRegistrationInfo.Tag), hubName, azureDeviceRegistrationInfo.ServerChallenge);
			}
			return true;
		}

		private readonly Dictionary<string, AzurePublisherSettings> publisherConfiguration;
	}
}
