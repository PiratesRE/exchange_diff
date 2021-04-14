using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class SmtpDomain
	{
		public SmtpDomain(string domain) : this(domain, true)
		{
		}

		protected SmtpDomain(string domain, bool check)
		{
			if (check && !SmtpAddress.IsValidDomain(domain))
			{
				throw new FormatException(DataStrings.InvalidSmtpDomainName(domain));
			}
			this.domain = domain;
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public static SmtpDomain Parse(string domain)
		{
			return new SmtpDomain(domain);
		}

		public static bool TryParse(string domain, out SmtpDomain obj)
		{
			obj = null;
			if (!string.IsNullOrEmpty(domain) && SmtpAddress.IsValidDomain(domain))
			{
				obj = new SmtpDomain(domain, false);
				return true;
			}
			return false;
		}

		public static SmtpDomain GetDomainPart(RoutingAddress address)
		{
			string domainPart = address.DomainPart;
			if (domainPart != null)
			{
				return new SmtpDomain(domainPart, false);
			}
			return null;
		}

		public bool Equals(SmtpDomain rhs)
		{
			return rhs != null && this.domain.Equals(rhs.domain, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object comparand)
		{
			SmtpDomain smtpDomain = comparand as SmtpDomain;
			return smtpDomain != null && this.Equals(smtpDomain);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == -1)
			{
				this.hashCode = ((this.domain == null) ? 0 : this.domain.ToLowerInvariant().GetHashCode());
			}
			return this.hashCode;
		}

		public override string ToString()
		{
			return this.domain;
		}

		public const int MaxLength = 255;

		private string domain;

		private int hashCode = -1;
	}
}
