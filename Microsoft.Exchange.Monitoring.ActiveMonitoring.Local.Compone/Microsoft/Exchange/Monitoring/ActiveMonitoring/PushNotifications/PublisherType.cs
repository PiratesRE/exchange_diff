using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	public enum PublisherType
	{
		APNS,
		WNS,
		GCM,
		WebApp,
		Azure,
		AzureHubCreation,
		AzureDeviceRegistration,
		AzureChallengeRequest
	}
}
