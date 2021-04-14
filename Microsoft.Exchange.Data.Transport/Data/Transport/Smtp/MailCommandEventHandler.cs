using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public delegate void MailCommandEventHandler(ReceiveCommandEventSource source, MailCommandEventArgs e);
}
