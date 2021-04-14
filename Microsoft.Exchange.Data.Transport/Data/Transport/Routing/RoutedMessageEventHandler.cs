using System;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public delegate void RoutedMessageEventHandler(RoutedMessageEventSource source, QueuedMessageEventArgs e);
}
