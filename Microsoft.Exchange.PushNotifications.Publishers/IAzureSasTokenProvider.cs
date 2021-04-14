using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IAzureSasTokenProvider
	{
		AzureSasToken CreateSasToken(string resourceUri);

		AzureSasToken CreateSasToken(string resourceUri, int expirationInSeconds);
	}
}
