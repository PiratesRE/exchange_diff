using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Notification
{
	internal enum SubscriptionNotificationResult
	{
		Success,
		ServerVersionMismatch,
		ServerStopped,
		ProcessingFailed,
		PropertyBagError,
		UnknownMethodError
	}
}
