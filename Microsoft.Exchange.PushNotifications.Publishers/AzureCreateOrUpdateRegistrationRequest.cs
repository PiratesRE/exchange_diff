using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureCreateOrUpdateRegistrationRequest : AzureRequestBase
	{
		public AzureCreateOrUpdateRegistrationRequest(AzureDeviceRegistrationNotification notification, AzureSasToken sasToken, string resourceUri) : base("application/atom+xml;type=entry;charset=utf-8", "PUT", sasToken, resourceUri, "")
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			base.RequestBody = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\">{0}</content></entry>", notification.SerializedPaylod);
			if (!string.IsNullOrWhiteSpace(notification.ServerChallenge))
			{
				base.Headers["ServiceBusNotification-RegistrationSecret"] = notification.ServerChallenge;
			}
		}

		public const string NewRegistrationContentType = "application/atom+xml;type=entry;charset=utf-8";

		public const string MultiFactorChallengeHeaderName = "ServiceBusNotification-RegistrationSecret";

		private const string RequestBodyTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\">{0}</content></entry>";
	}
}
