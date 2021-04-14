using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureRegistrationChallengeRequest : AzureRequestBase
	{
		public AzureRegistrationChallengeRequest(AzureChallengeRequestNotification notification, AzureSasToken sasToken, string resourceUri) : base("application/json", "POST", sasToken, resourceUri, "")
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			base.Headers["TrackingId"] = notification.Identifier;
			base.RequestBody = notification.SerializedPaylod;
		}

		public const string IssueSecretContentType = "application/json";
	}
}
