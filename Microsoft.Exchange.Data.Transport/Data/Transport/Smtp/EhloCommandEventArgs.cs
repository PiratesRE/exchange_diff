using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class EhloCommandEventArgs : ReceiveCommandEventArgs
	{
		internal EhloCommandEventArgs()
		{
		}

		internal EhloCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!RoutingAddress.IsValidDomain(value) && !RoutingAddress.IsDomainIPLiteral(value) && !HeloCommandEventArgs.IsValidIpv6WindowsAddress(value))
				{
					throw new ArgumentException(string.Format("Invalid SMTP domain {0}.  SMTP domains should be of the form 'contoso.com'", value));
				}
				this.domain = value;
			}
		}

		public bool DisableStartTls
		{
			get
			{
				return base.SmtpSession.DisableStartTls;
			}
			set
			{
				base.SmtpSession.DisableStartTls = value;
			}
		}

		private string domain;
	}
}
