using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum AzureHubCreationChannelState
	{
		Init,
		Authenticating,
		Delaying,
		Sending,
		Discarding
	}
}
