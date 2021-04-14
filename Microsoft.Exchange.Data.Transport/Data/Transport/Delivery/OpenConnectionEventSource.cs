using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	public abstract class OpenConnectionEventSource
	{
		public abstract void FailQueue(SmtpResponse smtpResponse);

		public abstract void DeferQueue(SmtpResponse smtpResponse);

		public abstract void DeferQueue(SmtpResponse smtpResponse, TimeSpan interval);

		public abstract void RegisterConnection(string remoteHost, SmtpResponse smtpResponse);
	}
}
