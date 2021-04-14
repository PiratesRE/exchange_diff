using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class SmtpDomainWithSubdomains : IEquatable<SmtpDomainWithSubdomains>
	{
		public SmtpDomainWithSubdomains(SmtpDomain domain, bool includeSubdomains)
		{
			if (domain == null && !includeSubdomains)
			{
				throw new StrongTypeFormatException(DataStrings.InvalidSmtpDomain, "Domain");
			}
			this.domain = domain;
			this.includeSubdomains = includeSubdomains;
		}

		public SmtpDomainWithSubdomains(string s) : this(s, false)
		{
		}

		public SmtpDomainWithSubdomains(string s, bool includeSubdomains)
		{
			if (SmtpDomainWithSubdomains.InternalTryParse(s, out this.domain, out this.includeSubdomains))
			{
				this.includeSubdomains = (this.includeSubdomains || includeSubdomains);
				return;
			}
			throw new StrongTypeFormatException(DataStrings.InvalidSmtpDomainName(s), "Domain");
		}

		public string Address
		{
			get
			{
				return this.ToString();
			}
		}

		public bool IncludeSubDomains
		{
			get
			{
				return this.includeSubdomains;
			}
		}

		public string Domain
		{
			get
			{
				if (!this.IsStar)
				{
					return this.domain.Domain;
				}
				return "*";
			}
		}

		public SmtpDomain SmtpDomain
		{
			get
			{
				return this.domain;
			}
		}

		public bool IsStar
		{
			get
			{
				return this.domain == null;
			}
		}

		public static SmtpDomainWithSubdomains Parse(string s)
		{
			SmtpDomainWithSubdomains result;
			if (SmtpDomainWithSubdomains.TryParse(s, out result))
			{
				return result;
			}
			throw new StrongTypeFormatException(DataStrings.InvalidSmtpDomainName(s), "Domain");
		}

		public static bool TryParse(string s, out SmtpDomainWithSubdomains result)
		{
			SmtpDomain smtpDomain;
			bool flag;
			if (SmtpDomainWithSubdomains.InternalTryParse(s, out smtpDomain, out flag))
			{
				result = new SmtpDomainWithSubdomains(smtpDomain, flag);
				return true;
			}
			result = null;
			return false;
		}

		public override string ToString()
		{
			if (this.domain == null)
			{
				return "*";
			}
			if (this.includeSubdomains)
			{
				return "*." + this.domain.Domain;
			}
			return this.domain.Domain;
		}

		public int Match(string toMatch)
		{
			if (this.domain == null)
			{
				return 0;
			}
			if (this.includeSubdomains)
			{
				if (toMatch.Length >= this.Domain.Length && toMatch.EndsWith(this.Domain, StringComparison.OrdinalIgnoreCase))
				{
					string text = toMatch.Substring(0, toMatch.Length - this.Domain.Length);
					if (text.Length == 0 || text.EndsWith("."))
					{
						return this.Domain.Length;
					}
				}
			}
			else if (toMatch.Length == this.Domain.Length && toMatch.Equals(this.Domain, StringComparison.OrdinalIgnoreCase))
			{
				return this.Domain.Length;
			}
			return -1;
		}

		private static bool InternalTryParse(string s, out SmtpDomain domain, out bool includeSubdomains)
		{
			domain = null;
			includeSubdomains = false;
			if (string.IsNullOrEmpty(s) || s.Trim().Length == 0)
			{
				return false;
			}
			if (s.Length == 1 && string.Equals(s, "*", StringComparison.OrdinalIgnoreCase))
			{
				domain = null;
				includeSubdomains = true;
				return true;
			}
			includeSubdomains = s.StartsWith("*.", StringComparison.OrdinalIgnoreCase);
			if (includeSubdomains)
			{
				s = s.Substring(2);
			}
			return SmtpDomain.TryParse(s, out domain);
		}

		public bool Equals(SmtpDomainWithSubdomains rhs)
		{
			if (this.includeSubdomains != rhs.includeSubdomains)
			{
				return false;
			}
			if (this.domain == null)
			{
				return rhs.domain == null;
			}
			return rhs.domain != null && this.domain.Equals(rhs.domain);
		}

		public override bool Equals(object comparand)
		{
			SmtpDomainWithSubdomains smtpDomainWithSubdomains = comparand as SmtpDomainWithSubdomains;
			return smtpDomainWithSubdomains != null && this.Equals(smtpDomainWithSubdomains);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == -1)
			{
				int num = (this.domain == null) ? 0 : this.domain.GetHashCode();
				num ^= (this.includeSubdomains ? 1 : 0);
				this.hashCode = num;
			}
			return this.hashCode;
		}

		private const string Star = "*";

		private const string Dot = ".";

		private const string StarDot = "*.";

		public const int MaxLength = 257;

		private readonly SmtpDomain domain;

		private readonly bool includeSubdomains;

		private int hashCode = -1;

		public static readonly SmtpDomainWithSubdomains StarDomain = new SmtpDomainWithSubdomains("*");
	}
}
