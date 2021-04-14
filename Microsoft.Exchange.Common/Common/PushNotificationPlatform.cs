using System;

namespace Microsoft.Exchange.Common
{
	public enum PushNotificationPlatform
	{
		None,
		APNS,
		PendingGet,
		WNS,
		Proxy,
		GCM,
		WebApp,
		Azure,
		AzureHubCreation,
		AzureChallengeRequest,
		AzureDeviceRegistration
	}
}
