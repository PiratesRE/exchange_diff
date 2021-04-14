using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureReadRegistrationRequest : AzureRequestBase
	{
		public AzureReadRegistrationRequest(AzureSasToken sasToken, string resourceUri) : base("application/atom+xml;type=entry;charset=utf-8", "GET", sasToken, resourceUri, "&$top=1")
		{
		}

		public const string ReadRegistrationContentType = "application/atom+xml;type=entry;charset=utf-8";

		public const string ReadRegistrationAdditionalParameters = "&$top=1";
	}
}
