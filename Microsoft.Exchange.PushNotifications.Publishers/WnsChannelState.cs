using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum WnsChannelState
	{
		Init,
		Authenticating,
		Delaying,
		Sending,
		Discarding
	}
}
