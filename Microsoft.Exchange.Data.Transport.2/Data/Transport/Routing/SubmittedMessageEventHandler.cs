using System;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public delegate void SubmittedMessageEventHandler(SubmittedMessageEventSource source, QueuedMessageEventArgs e);
}
