using System;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public delegate void ResolvedMessageEventHandler(ResolvedMessageEventSource source, QueuedMessageEventArgs e);
}
