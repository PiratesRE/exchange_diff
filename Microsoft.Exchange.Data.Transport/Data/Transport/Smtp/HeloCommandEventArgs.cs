using System;
using System.Net;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class HeloCommandEventArgs : ReceiveCommandEventArgs
	{
		internal HeloCommandEventArgs()
		{
		}

		internal HeloCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
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

		internal static bool IsValidIpv6WindowsAddress(string domain)
		{
			IPAddress ipaddress;
			return (domain != null && domain.StartsWith("[IPv6:", StringComparison.InvariantCultureIgnoreCase) && domain.EndsWith("]") && IPAddress.TryParse(domain.Substring(6, domain.Length - 7), out ipaddress)) || (domain != null && IPAddress.TryParse(domain, out ipaddress));
		}

		private string domain;
	}
}
