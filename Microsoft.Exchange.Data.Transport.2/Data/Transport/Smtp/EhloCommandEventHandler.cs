using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public delegate void EhloCommandEventHandler(ReceiveCommandEventSource source, EhloCommandEventArgs e);
}
