using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureNewRegistrationRequest : AzureRequestBase
	{
		public AzureNewRegistrationRequest(AzureNotification notification, AzureSasToken sasToken, string resourceUri, string registrationTemplate) : base("application/atom+xml;type=entry;charset=utf-8", "POST", sasToken, resourceUri, "")
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			base.RequestBody = AzureNewRegistrationRequest.CreateRequestBody(notification.RecipientId, notification.DeviceId, registrationTemplate);
		}

		public static string CreateRequestBody(string tags, string deviceToken, string registrationTemplate)
		{
			return string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\">{0}</content></entry>", string.Format(registrationTemplate, tags, deviceToken));
		}

		public const string NewRegistrationContentType = "application/atom+xml;type=entry;charset=utf-8";

		private const string RequestBodyTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\">{0}</content></entry>";
	}
}
