using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class ReceiveCommandEventArgs : ReceiveEventArgs
	{
		internal ReceiveCommandEventArgs()
		{
		}

		internal ReceiveCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}
	}
}
