using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubCreationRequest : AzureRequestBase
	{
		public AzureHubCreationRequest(AzureHubCreationNotification notification, AcsAccessToken azureKey, string resourceUri) : base("application/xml;type=entry;charset=utf-8", "PUT", azureKey, resourceUri, "")
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			base.RequestBody = notification.SerializedPaylod;
		}

		public const string CreateHubContentType = "application/xml;type=entry;charset=utf-8";
	}
}
