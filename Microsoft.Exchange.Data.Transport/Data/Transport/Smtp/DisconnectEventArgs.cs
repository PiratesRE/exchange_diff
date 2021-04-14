using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class DisconnectEventArgs : ReceiveEventArgs
	{
		public DisconnectReason DisconnectReason
		{
			get
			{
				return base.SmtpSession.DisconnectReason;
			}
		}

		internal DisconnectEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}
	}
}
