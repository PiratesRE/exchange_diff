using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum ProxyChannelLegacyState
	{
		Init,
		Delaying,
		Publishing,
		Discarding
	}
}
