using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureNewRegistrationIdRequest : AzureRequestBase
	{
		public AzureNewRegistrationIdRequest(AzureDeviceRegistrationNotification notification, AzureSasToken sasToken, string resourceUri) : base("application/atom+xml;type=entry;charset=utf-8", "POST", sasToken, resourceUri, "")
		{
			base.RequestBody = string.Empty;
		}

		public const string NewRegistrationIdContentType = "application/atom+xml;type=entry;charset=utf-8";
	}
}
