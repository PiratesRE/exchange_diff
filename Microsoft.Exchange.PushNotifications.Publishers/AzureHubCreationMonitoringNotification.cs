using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureHubCreationMonitoringNotification : AzureHubCreationNotification
	{
		public AzureHubCreationMonitoringNotification(string appId, string hubName, string partitionName, AzureHubPayload hubPayload) : base(appId, hubName, partitionName, hubPayload)
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
