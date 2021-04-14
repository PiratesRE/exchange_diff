using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal delegate void QueueDataAvailableEventHandler<T>(QueueDataAvailableEventSource<T> src, QueueDataAvailableEventArgs<T> e);
}
