using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class NoopCommandEventArgs : ReceiveCommandEventArgs
	{
		internal NoopCommandEventArgs()
		{
		}

		internal NoopCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}
	}
}
