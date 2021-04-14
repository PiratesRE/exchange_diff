using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public delegate void EndOfHeadersEventHandler(ReceiveMessageEventSource source, EndOfHeadersEventArgs e);
}
