using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureChallengeRequestMonitoringNotification : AzureChallengeRequestNotification
	{
		public AzureChallengeRequestMonitoringNotification(string appId, string targetAppId, IAzureSasTokenProvider sasTokenProvider, AzureUriTemplate uriTemplate, string deviceId, AzureChallengeRequestPayload payload, string hubName) : base(appId, targetAppId, sasTokenProvider, uriTemplate, deviceId, payload, hubName)
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
