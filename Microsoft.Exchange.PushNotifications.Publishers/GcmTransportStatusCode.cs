using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum GcmTransportStatusCode
	{
		Unknown,
		Success,
		Unauthorized,
		ServerUnavailable,
		InternalServerError,
		Timeout
	}
}
