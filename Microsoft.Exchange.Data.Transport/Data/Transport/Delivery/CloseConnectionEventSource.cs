using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	public abstract class CloseConnectionEventSource
	{
		public abstract void UnregisterConnection(SmtpResponse smtpResponse);
	}
}
