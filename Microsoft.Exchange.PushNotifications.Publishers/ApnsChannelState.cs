using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum ApnsChannelState
	{
		Init,
		Connecting,
		DelayingConnect,
		Authenticating,
		Reading,
		Sending,
		Waiting
	}
}
