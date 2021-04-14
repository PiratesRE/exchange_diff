using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class RsetCommandEventArgs : ReceiveCommandEventArgs
	{
		internal RsetCommandEventArgs()
		{
		}

		internal RsetCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}
	}
}
