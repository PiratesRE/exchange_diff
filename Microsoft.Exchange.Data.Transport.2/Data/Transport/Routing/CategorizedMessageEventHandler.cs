using System;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public delegate void CategorizedMessageEventHandler(CategorizedMessageEventSource source, QueuedMessageEventArgs e);
}
