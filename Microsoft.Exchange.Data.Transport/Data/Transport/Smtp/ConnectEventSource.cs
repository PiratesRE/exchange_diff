using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class ConnectEventSource : ReceiveEventSource
	{
		internal ConnectEventSource(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public abstract void RejectConnection(SmtpResponse response);
	}
}
