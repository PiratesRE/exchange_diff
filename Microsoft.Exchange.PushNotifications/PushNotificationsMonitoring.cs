using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.PushNotifications
{
	internal static class PushNotificationsMonitoring
	{
		static PushNotificationsMonitoring()
		{
			HashSet<PushNotificationApp> hashSet = new HashSet<PushNotificationApp>();
			foreach (PushNotificationSetupConfig pushNotificationSetupConfig in PushNotificationCannedSet.PushNotificationSetupEnvironmentConfig.Values)
			{
				hashSet.UnionWith(pushNotificationSetupConfig.InstallableBySetup);
			}
			PushNotificationsMonitoring.CannedAppPlatformSet = new Dictionary<string, PushNotificationPlatform>();
			foreach (PushNotificationApp pushNotificationApp in hashSet)
			{
				if (!PushNotificationsMonitoring.CannedAppPlatformSet.ContainsKey(pushNotificationApp.Name))
				{
					PushNotificationsMonitoring.CannedAppPlatformSet.Add(pushNotificationApp.Name, pushNotificationApp.Platform);
				}
			}
		}

		internal static Dictionary<string, PushNotificationPlatform> CannedAppPlatformSet { get; private set; }

		internal static void PublishSuccessNotification(string notificationEvent, string targetResource = "")
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.PushNotificationsProtocol.Name, notificationEvent, targetResource, string.Empty, ResultSeverityLevel.Informational);
			eventNotificationItem.Publish(false);
		}

		internal static void PublishFailureNotification(string notificationEvent, string targetResource = "", string failureReason = "")
		{
			new EventNotificationItem(ExchangeComponent.PushNotificationsProtocol.Name, notificationEvent, targetResource, string.Empty, ResultSeverityLevel.Error)
			{
				StateAttribute2 = failureReason
			}.Publish(false);
		}

		internal const string ApnsChannelConnect = "ApnsChannelConnect";

		internal const string ApnsChannelAuthenticate = "ApnsChannelAuthenticate";

		internal const string ApnsCertPresent = "ApnsCertPresent";

		internal const string ApnsCertPrivateKey = "ApnsCertPrivateKey";

		internal const string ApnsCertValidation = "ApnsCertValidation";

		internal const string ApnsCertLoaded = "ApnsCertLoaded";

		internal const string WnsChannelBackOff = "WnsChannelBackOff";

		internal const string GcmChannelBackOff = "GcmChannelBackOff";

		internal const string WebAppChannelBackOff = "WebAppChannelBackOff";

		internal const string AzureChannelBackOff = "AzureChannelBackOff";

		internal const string AzureHubCreationChannelBackOff = "AzureHubCreationChannelBackOff";

		internal const string AzureDeviceRegistrationChannelBackOff = "AzureDeviceRegistrationChannelBackOff";

		internal const string AzureChallengeRequestChannelBackOff = "AzureChallengeRequestChannelBackOff";

		internal const string SendPublishNotification = "SendPublishNotification";

		internal const string NotificationProcessed = "NotificationProcessed";

		internal const string HubCreationProcessed = "HubCreationProcessed";

		internal const string DeviceRegistrationProcessed = "DeviceRegistrationProcessed";

		internal const string ChallengeRequestProcessed = "ChallengeRequestProcessed";

		internal const string EnterpriseNotificationProcessed = "EnterpriseNotificationProcessed";
	}
}
