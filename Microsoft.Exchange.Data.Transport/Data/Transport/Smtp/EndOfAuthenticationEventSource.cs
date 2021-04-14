using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class EndOfAuthenticationEventSource : ReceiveEventSource
	{
		internal EndOfAuthenticationEventSource(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public abstract void RejectAuthentication(SmtpResponse response);
	}
}
