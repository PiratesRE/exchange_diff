using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class StartTlsCommandEventArgs : ReceiveCommandEventArgs
	{
		internal StartTlsCommandEventArgs()
		{
		}

		internal StartTlsCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public bool RequestClientTlsCertificate
		{
			get
			{
				return base.SmtpSession.RequestClientTlsCertificate;
			}
			set
			{
				base.SmtpSession.RequestClientTlsCertificate = value;
			}
		}
	}
}
