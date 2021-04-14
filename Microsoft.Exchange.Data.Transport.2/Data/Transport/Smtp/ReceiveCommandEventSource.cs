using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class ReceiveCommandEventSource : ReceiveEventSource
	{
		internal ReceiveCommandEventSource(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public abstract void RejectCommand(SmtpResponse response);

		public abstract void RejectCommand(SmtpResponse response, string trackingContext);
	}
}
