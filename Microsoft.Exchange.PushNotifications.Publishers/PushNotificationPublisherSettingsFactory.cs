using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationPublisherSettingsFactory
	{
		public PushNotificationPublisherSettings Create(IPushNotificationRawSettings rawSettings)
		{
			ArgumentValidator.ThrowIfNull("rawSettings", rawSettings);
			switch (rawSettings.Platform)
			{
			case PushNotificationPlatform.APNS:
				return this.CreateApnsPublisherSettings(rawSettings);
			case PushNotificationPlatform.PendingGet:
				return this.CreatePendingGetPublisherSettings(rawSettings);
			case PushNotificationPlatform.WNS:
				return this.CreateWnsPublisherSettings(rawSettings);
			case PushNotificationPlatform.Proxy:
				return this.CreateProxyPublisherSettings(rawSettings);
			case PushNotificationPlatform.GCM:
				return this.CreateGcmPublisherSettings(rawSettings);
			case PushNotificationPlatform.WebApp:
				return this.CreateWebAppPublisherSettings(rawSettings);
			case PushNotificationPlatform.Azure:
				return this.CreateAzurePublisherSettings(rawSettings);
			case PushNotificationPlatform.AzureHubCreation:
				return this.CreateAzureHubCreationPublisherSettings(rawSettings);
			case PushNotificationPlatform.AzureChallengeRequest:
				return this.CreateAzureChallengeRequestPublisherSettings(rawSettings);
			case PushNotificationPlatform.AzureDeviceRegistration:
				return this.CreateAzureDeviceRegistrationPublisherSettings(rawSettings);
			default:
				throw new NotSupportedException("Unsupported PushNotificationPlatform: " + rawSettings.Platform.ToString());
			}
		}

		private ApnsPublisherSettings CreateApnsPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			ApnsEndPoint host = new ApnsEndPoint(rawSettings.Url ?? "gateway.push.apple.com", rawSettings.Port ?? 2195, rawSettings.SecondaryUrl ?? "feedback.push.apple.com", rawSettings.SecondaryPort ?? 2196);
			ApnsChannelSettings channelSettings = new ApnsChannelSettings(rawSettings.Name, rawSettings.AuthenticationKey, rawSettings.AuthenticationKeyFallback ?? null, host, rawSettings.ConnectStepTimeout ?? 500, rawSettings.ConnectTotalTimeout ?? 300000, rawSettings.ConnectRetryMax ?? 3, rawSettings.ConnectRetryDelay ?? 1500, rawSettings.AuthenticateRetryMax ?? 2, rawSettings.ReadTimeout ?? 5000, rawSettings.WriteTimeout ?? 5000, rawSettings.BackOffTimeInSeconds ?? 600, rawSettings.IgnoreCertificateErrors ?? false);
			return new ApnsPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}

		private WnsPublisherSettings CreateWnsPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			WnsChannelSettings channelSettings = new WnsChannelSettings(rawSettings.Name, rawSettings.AuthenticationId, rawSettings.AuthenticationKey, rawSettings.IsAuthenticationKeyEncrypted ?? true, rawSettings.SecondaryUrl ?? "https://login.live.com/accesstoken.srf", rawSettings.ConnectTotalTimeout ?? 60000, rawSettings.ConnectStepTimeout ?? 500, rawSettings.ConnectRetryDelay ?? 1500, rawSettings.AuthenticateRetryMax ?? 2, rawSettings.BackOffTimeInSeconds ?? 600);
			return new WnsPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}

		private GcmPublisherSettings CreateGcmPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			GcmChannelSettings channelSettings = new GcmChannelSettings(rawSettings.Name, rawSettings.AuthenticationId, rawSettings.AuthenticationKey, rawSettings.IsAuthenticationKeyEncrypted ?? true, rawSettings.Url ?? "https://android.googleapis.com/gcm/send", rawSettings.ConnectTotalTimeout ?? 60000, rawSettings.ConnectStepTimeout ?? 500, rawSettings.BackOffTimeInSeconds ?? 600);
			return new GcmPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}

		private WebAppPublisherSettings CreateWebAppPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			WebAppChannelSettings channelSettings = new WebAppChannelSettings(rawSettings.Name, rawSettings.ConnectTotalTimeout ?? 60000, rawSettings.ConnectStepTimeout ?? 500, rawSettings.BackOffTimeInSeconds ?? 300);
			return new WebAppPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}

		private AzurePublisherSettings CreateAzurePublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			if (rawSettings.IsDefaultPartitionName != null && rawSettings.IsDefaultPartitionName.Value)
			{
				PushNotificationsCrimsonEvents.DefaultPartitionUsedForNamespace.LogPeriodic<string, string>(rawSettings.Name, TimeSpan.FromHours(12.0), rawSettings.Name, Environment.MachineName);
			}
			AzureChannelSettings channelSettings = new AzureChannelSettings(rawSettings.Name, rawSettings.UriTemplate ?? "https://{0}-{1}.servicebus.windows.net/exo/{2}/{3}", rawSettings.AuthenticationId, rawSettings.AuthenticationKey, rawSettings.IsAuthenticationKeyEncrypted ?? true, rawSettings.RegistrationTemplate, rawSettings.RegistrationEnabled ?? false, rawSettings.PartitionName, rawSettings.ConnectTotalTimeout ?? 60000, rawSettings.ConnectStepTimeout ?? 500, rawSettings.MaximumCacheSize ?? 10000, rawSettings.BackOffTimeInSeconds ?? 600);
			return new AzurePublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, rawSettings.MultifactorRegistrationEnabled ?? false, channelSettings);
		}

		private AzureHubCreationPublisherSettings CreateAzureHubCreationPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			AzureHubCreationChannelSettings channelSettings = new AzureHubCreationChannelSettings(rawSettings.Name, rawSettings.Url ?? "https://{0}-{1}-sb.accesscontrol.windows.net/", rawSettings.SecondaryUrl ?? "http://{0}-{1}.servicebus.windows.net/exo/", rawSettings.UriTemplate ?? "https://{0}-{1}.servicebus.windows.net/exo/{2}{3}", rawSettings.AuthenticationId, rawSettings.AuthenticationKey, rawSettings.IsAuthenticationKeyEncrypted ?? true, rawSettings.ConnectTotalTimeout ?? 60000, rawSettings.ConnectStepTimeout ?? 500, rawSettings.ConnectRetryDelay ?? 1500, rawSettings.MaximumCacheSize ?? 10000, rawSettings.BackOffTimeInSeconds ?? 600);
			return new AzureHubCreationPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}

		private AzureChallengeRequestPublisherSettings CreateAzureChallengeRequestPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			AzureChallengeRequestChannelSettings channelSettings = new AzureChallengeRequestChannelSettings(rawSettings.Name, rawSettings.ConnectTotalTimeout ?? 60000, rawSettings.ConnectStepTimeout ?? 500, rawSettings.BackOffTimeInSeconds ?? 600);
			return new AzureChallengeRequestPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}

		private AzureDeviceRegistrationPublisherSettings CreateAzureDeviceRegistrationPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			AzureDeviceRegistrationChannelSettings channelSettings = new AzureDeviceRegistrationChannelSettings(rawSettings.Name, rawSettings.ConnectTotalTimeout ?? 60000, rawSettings.ConnectStepTimeout ?? 500, rawSettings.MaximumCacheSize ?? 10000, rawSettings.BackOffTimeInSeconds ?? 600);
			return new AzureDeviceRegistrationPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}

		private PendingGetPublisherSettings CreatePendingGetPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			return new PendingGetPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? true, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15);
		}

		private ProxyPublisherSettings CreateProxyPublisherSettings(IPushNotificationRawSettings rawSettings)
		{
			PushNotificationApp pushNotificationApp = rawSettings as PushNotificationApp;
			ProxyChannelSettings channelSettings = new ProxyChannelSettings(rawSettings.Name, rawSettings.Url, rawSettings.AuthenticationKey, (pushNotificationApp != null) ? pushNotificationApp.LastUpdateTimeUtc : null, rawSettings.ConnectRetryMax ?? 3, rawSettings.ConnectRetryDelay ?? 1500, rawSettings.ConnectStepTimeout ?? 500, rawSettings.BackOffTimeInSeconds ?? 600);
			return new ProxyPublisherSettings(rawSettings.Name, rawSettings.Enabled ?? false, rawSettings.AuthenticationId, rawSettings.ExchangeMinimumVersion ?? null, rawSettings.ExchangeMaximumVersion ?? null, rawSettings.QueueSize ?? 10000, rawSettings.NumberOfChannels ?? 1, rawSettings.AddTimeout ?? 15, channelSettings);
		}
	}
}
