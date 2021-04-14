using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureDeviceRegistrationMonitoringNotification : AzureDeviceRegistrationNotification
	{
		public AzureDeviceRegistrationMonitoringNotification(string appId, string targetAppId, IAzureSasTokenProvider sasTokenProvider, AzureUriTemplate uriTemplate, AzureDeviceRegistrationPayload payload, string hubName, string serverChallenge = null) : base(appId, targetAppId, sasTokenProvider, uriTemplate, payload, hubName, serverChallenge)
		{
		}

		public override bool IsMonitoring
		{
			get
			{
				return true;
			}
		}
	}
}
