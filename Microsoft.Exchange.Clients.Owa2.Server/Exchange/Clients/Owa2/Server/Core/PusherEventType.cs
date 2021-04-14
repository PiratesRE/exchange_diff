using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum PusherEventType
	{
		Distribute,
		Push,
		PushFailed,
		ConcurrentLimit,
		PusherThreadStart,
		PusherThreadCleanup,
		PusherThreadEnd
	}
}
