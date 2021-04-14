using System;

namespace Microsoft.Exchange.Data.Transport
{
	[Serializable]
	public struct RoutingDomain : IEquatable<RoutingDomain>, IComparable<RoutingDomain>
	{
		public RoutingDomain(string domain)
		{
			string text;
			if (!RoutingDomain.TryParse(domain, out text))
			{
				throw new FormatException(string.Format("The format of the specified domain '{0}' isn't valid", domain ?? string.Empty));
			}
			this.domain = text;
		}

		public RoutingDomain(string domain, string type)
		{
			if (string.Equals(type, "smtp", StringComparison.OrdinalIgnoreCase))
			{
				if (!RoutingAddress.IsValidDomain(domain))
				{
					throw new FormatException(string.Format("The format of the specified domain '{0}' isn't valid", domain ?? string.Empty));
				}
			}
			else
			{
				if (string.IsNullOrEmpty(type))
				{
					throw new FormatException("A null or empty routing type isn't valid");
				}
				if (string.IsNullOrEmpty(domain))
				{
					throw new FormatException("A null or empty domain isn't valid");
				}
				if (type.IndexOfAny(RoutingDomain.CharactersNotAllowedInType) != -1)
				{
					throw new FormatException(string.Format("Domain type '{0}' contains at least one invalid character. You can't use the following characters when specifying domain types: '{1}'", type, RoutingDomain.CharactersNotAllowedInType));
				}
			}
			this.domain = type + RoutingDomain.Separator + domain;
		}

		private RoutingDomain(string domain, bool requiresValidation)
		{
			if (!requiresValidation)
			{
				this.domain = domain;
				return;
			}
			string text;
			if (!RoutingDomain.TryParse(domain, out text))
			{
				throw new FormatException(string.Format("The format of the specified domain '{0}' isn't valid", domain ?? string.Empty));
			}
			this.domain = text;
		}

		public string Domain
		{
			get
			{
				if (string.IsNullOrEmpty(this.domain))
				{
					return string.Empty;
				}
				int num = this.domain.IndexOf(RoutingDomain.Separator);
				return this.domain.Substring(num + 1);
			}
		}

		public string Type
		{
			get
			{
				if (string.IsNullOrEmpty(this.domain))
				{
					return string.Empty;
				}
				int length = this.domain.IndexOf(RoutingDomain.Separator);
				return this.domain.Substring(0, length);
			}
		}

		public static RoutingDomain Parse(string domain)
		{
			return new RoutingDomain(domain);
		}

		public static bool TryParse(string domain, out RoutingDomain routingDomain)
		{
			string text;
			if (!string.IsNullOrEmpty(domain) && RoutingDomain.TryParse(domain, out text))
			{
				routingDomain = new RoutingDomain(text, false);
				return true;
			}
			routingDomain = RoutingDomain.Empty;
			return false;
		}

		public static RoutingDomain GetDomainPart(RoutingAddress address)
		{
			string domainPart = address.DomainPart;
			if (domainPart != null)
			{
				return new RoutingDomain(domainPart);
			}
			return RoutingDomain.Empty;
		}

		public static bool operator ==(RoutingDomain value1, RoutingDomain value2)
		{
			return string.Equals(value1.domain, value2.domain, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator !=(RoutingDomain value1, RoutingDomain value2)
		{
			return !(value1 == value2);
		}

		public bool IsSmtp()
		{
			return this.domain.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase);
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(this.domain);
		}

		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(this.domain))
			{
				return this.domain.GetHashCode();
			}
			return 0;
		}

		public int CompareTo(RoutingDomain value)
		{
			return string.Compare(this.ToString(), value.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		public int CompareTo(object domain)
		{
			if (!(domain is RoutingDomain))
			{
				throw new FormatException(string.Format("The domain must be of type RoutingDomain.  Actual type: '{0}'", (domain == null) ? "null" : domain.GetType().ToString()));
			}
			return this.CompareTo((RoutingDomain)domain);
		}

		public bool Equals(RoutingDomain domain)
		{
			return this == domain;
		}

		public override bool Equals(object domain)
		{
			return domain is RoutingDomain && this == (RoutingDomain)domain;
		}

		public override string ToString()
		{
			return this.domain ?? string.Empty;
		}

		private static bool TryParse(string domainRepresentation, out string domain)
		{
			domain = string.Empty;
			if (string.IsNullOrEmpty(domainRepresentation))
			{
				return false;
			}
			int num = domainRepresentation.IndexOf(RoutingDomain.Separator);
			if (num != -1)
			{
				string text = domainRepresentation.Substring(0, num);
				string value = domainRepresentation.Substring(num + 1);
				if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value) || text.IndexOfAny(RoutingDomain.CharactersNotAllowedInType) != -1)
				{
					return false;
				}
				if (text.Equals("smtp", StringComparison.OrdinalIgnoreCase) && !RoutingAddress.IsValidDomain(value))
				{
					return false;
				}
				domain = domainRepresentation;
			}
			else
			{
				if (!RoutingAddress.IsValidDomain(domainRepresentation))
				{
					return false;
				}
				domain = "smtp:" + domainRepresentation;
			}
			return true;
		}

		public const string Smtp = "smtp";

		private const string SmtpWithSeperator = "smtp:";

		public static readonly RoutingDomain Empty = default(RoutingDomain);

		internal static readonly char Separator = ':';

		private static readonly char[] CharactersNotAllowedInType = new char[]
		{
			':'
		};

		private string domain;
	}
}
