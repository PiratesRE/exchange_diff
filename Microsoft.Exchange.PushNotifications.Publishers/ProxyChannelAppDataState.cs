using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum ProxyChannelAppDataState
	{
		Init,
		AppDataRequesting,
		AppDataUpdating,
		Discarding,
		Updated
	}
}
