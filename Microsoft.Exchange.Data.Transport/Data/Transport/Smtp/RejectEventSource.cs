using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class RejectEventSource : ReceiveEventSource
	{
		internal RejectEventSource(SmtpSession smtpSession) : base(smtpSession)
		{
		}
	}
}
