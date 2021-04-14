using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum AzureChannelState
	{
		Init,
		ReadRegistration,
		NewRegistration,
		Sending,
		Discarding
	}
}
