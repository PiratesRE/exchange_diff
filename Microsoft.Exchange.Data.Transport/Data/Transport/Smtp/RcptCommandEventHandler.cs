using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public delegate void RcptCommandEventHandler(ReceiveCommandEventSource source, RcptCommandEventArgs e);
}
