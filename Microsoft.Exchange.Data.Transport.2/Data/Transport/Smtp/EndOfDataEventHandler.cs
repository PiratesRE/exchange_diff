using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public delegate void EndOfDataEventHandler(ReceiveMessageEventSource source, EndOfDataEventArgs e);
}
