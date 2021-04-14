using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum WnsResultErrorType
	{
		Unknown,
		Timeout,
		Throttle,
		AuthTokenExpired,
		ServerUnavailable
	}
}
