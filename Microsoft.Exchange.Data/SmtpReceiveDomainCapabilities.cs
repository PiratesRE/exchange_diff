using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class SmtpReceiveDomainCapabilities
	{
		public SmtpReceiveDomainCapabilities(SmtpDomainWithSubdomains domain, SmtpReceiveCapabilities capabilities, SmtpX509Identifier x509Identifier)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("Domain or the Common Name in the X509 Identifier");
			}
			this.domain = domain;
			this.capabilities = capabilities;
			this.smtpX509Identifier = x509Identifier;
		}

		public SmtpReceiveDomainCapabilities(string s)
		{
			if (!SmtpReceiveDomainCapabilities.InternalTryParse(s, false, out this.domain, out this.capabilities, out this.smtpX509Identifier))
			{
				throw new StrongTypeFormatException(DataStrings.InvalidSmtpReceiveDomainCapabilities(s), "DomainCapabilities");
			}
		}

		public SmtpDomainWithSubdomains Domain
		{
			get
			{
				return this.domain;
			}
		}

		public SmtpReceiveCapabilities Capabilities
		{
			get
			{
				return this.capabilities;
			}
		}

		public SmtpX509Identifier SmtpX509Identifier
		{
			get
			{
				return this.smtpX509Identifier;
			}
		}

		public static SmtpReceiveDomainCapabilities Parse(string s)
		{
			return new SmtpReceiveDomainCapabilities(s);
		}

		public static bool TryParse(string s, out SmtpReceiveDomainCapabilities result)
		{
			result = null;
			SmtpDomainWithSubdomains smtpDomainWithSubdomains;
			SmtpReceiveCapabilities smtpReceiveCapabilities;
			SmtpX509Identifier x509Identifier;
			if (!SmtpReceiveDomainCapabilities.InternalTryParse(s, false, out smtpDomainWithSubdomains, out smtpReceiveCapabilities, out x509Identifier))
			{
				return false;
			}
			result = new SmtpReceiveDomainCapabilities(smtpDomainWithSubdomains, smtpReceiveCapabilities, x509Identifier);
			return true;
		}

		public static SmtpReceiveDomainCapabilities FromADString(string s)
		{
			SmtpDomainWithSubdomains smtpDomainWithSubdomains;
			SmtpReceiveCapabilities smtpReceiveCapabilities;
			SmtpX509Identifier x509Identifier;
			if (!SmtpReceiveDomainCapabilities.InternalTryParseFromAD(s, true, out smtpDomainWithSubdomains, out smtpReceiveCapabilities, out x509Identifier))
			{
				throw new StrongTypeFormatException(DataStrings.InvalidSmtpReceiveDomainCapabilities(s), "DomainCapabilities");
			}
			return new SmtpReceiveDomainCapabilities(smtpDomainWithSubdomains, smtpReceiveCapabilities, x509Identifier);
		}

		public override string ToString()
		{
			string arg = this.capabilities.ToString().Replace(" ", null);
			string arg2 = (this.SmtpX509Identifier != null) ? this.SmtpX509Identifier.ToString().Replace(":", "::") : this.Domain.ToString();
			return arg2 + ':' + arg;
		}

		public string ToADString()
		{
			string text = (this.SmtpX509Identifier != null) ? (':' + this.SmtpX509Identifier.ToString().Replace(":", "::")) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}{2}", new object[]
			{
				this.domain,
				(int)this.capabilities,
				text
			});
		}

		public bool Equals(SmtpReceiveDomainCapabilities rhs)
		{
			return rhs != null && this.Capabilities == rhs.Capabilities && this.Domain.Equals(rhs.Domain) && ((this.SmtpX509Identifier == null && rhs.SmtpX509Identifier == null) || (this.SmtpX509Identifier != null && rhs.SmtpX509Identifier != null && this.SmtpX509Identifier.Equals(rhs.SmtpX509Identifier)));
		}

		public override bool Equals(object rhs)
		{
			return this.Equals(rhs as SmtpReceiveDomainCapabilities);
		}

		public override int GetHashCode()
		{
			int num = this.domain.GetHashCode() ^ this.capabilities.GetHashCode();
			if (this.SmtpX509Identifier != null)
			{
				num ^= this.SmtpX509Identifier.GetHashCode();
			}
			return num;
		}

		private static bool InternalTryParse(string s, bool extendedFormat, out SmtpDomainWithSubdomains domain, out SmtpReceiveCapabilities capabilities, out SmtpX509Identifier x509Identifier)
		{
			domain = null;
			x509Identifier = null;
			capabilities = SmtpReceiveCapabilities.None;
			if (string.IsNullOrWhiteSpace(s))
			{
				return false;
			}
			int num;
			string stringPart = SmtpReceiveDomainCapabilities.GetStringPart(s, true, true, 0, out num);
			if (num == s.Length - 1 || num == s.Length)
			{
				return false;
			}
			SmtpX509Identifier smtpX509Identifier = null;
			if (SmtpX509Identifier.TryParse(stringPart, out smtpX509Identifier))
			{
				domain = smtpX509Identifier.SubjectCommonName;
				x509Identifier = smtpX509Identifier;
			}
			if (domain == null && !SmtpDomainWithSubdomains.TryParse(stringPart.Trim(), out domain))
			{
				return false;
			}
			int num2;
			string stringPart2 = SmtpReceiveDomainCapabilities.GetStringPart(s, extendedFormat, false, num + 1, out num2);
			if (!SmtpReceiveDomainCapabilities.TryGetCapabilities(stringPart2, out capabilities))
			{
				domain = null;
				x509Identifier = null;
				return false;
			}
			return true;
		}

		private static bool InternalTryParseFromAD(string s, bool extendedFormat, out SmtpDomainWithSubdomains domain, out SmtpReceiveCapabilities capabilities, out SmtpX509Identifier x509Identifier)
		{
			domain = null;
			x509Identifier = null;
			capabilities = SmtpReceiveCapabilities.None;
			if (string.IsNullOrWhiteSpace(s))
			{
				return false;
			}
			int num = s.IndexOf(':');
			if (num <= 0 || num == s.Length - 1)
			{
				return false;
			}
			string text = s.Substring(0, num);
			if (!SmtpDomainWithSubdomains.TryParse(text.Trim(), out domain))
			{
				return false;
			}
			int num2;
			string stringPart = SmtpReceiveDomainCapabilities.GetStringPart(s, extendedFormat, false, num + 1, out num2);
			if (!SmtpReceiveDomainCapabilities.TryGetCapabilities(stringPart, out capabilities))
			{
				domain = null;
				return false;
			}
			if (num2 > num && num2 < s.Length)
			{
				int num3;
				string stringPart2 = SmtpReceiveDomainCapabilities.GetStringPart(s, extendedFormat, true, num2 + 1, out num3);
				SmtpX509Identifier smtpX509Identifier = null;
				if (SmtpX509Identifier.TryParse(stringPart2, out smtpX509Identifier))
				{
					domain = smtpX509Identifier.SubjectCommonName;
					x509Identifier = smtpX509Identifier;
				}
				else
				{
					x509Identifier = null;
				}
			}
			return true;
		}

		private static bool TryGetCapabilities(string capabilitiesPart, out SmtpReceiveCapabilities capabilities)
		{
			return EnumValidator.TryParse<SmtpReceiveCapabilities>(capabilitiesPart, EnumParseOptions.AllowNumericConstants | EnumParseOptions.IgnoreCase, out capabilities);
		}

		private static string GetStringPart(string s, bool extendedFormat, bool domainSeparatorEncoded, int startIndex, out int endIndex)
		{
			endIndex = s.Length;
			int length = endIndex - startIndex;
			if (extendedFormat)
			{
				int startIndex2 = startIndex;
				int num;
				while ((num = s.IndexOf(':', startIndex2)) > 0)
				{
					if (!domainSeparatorEncoded || num + 1 >= endIndex || s[num + 1] != ':')
					{
						endIndex = num;
						length = endIndex - startIndex;
						break;
					}
					if (num + 2 >= endIndex)
					{
						break;
					}
					startIndex2 = num + 2;
				}
			}
			return s.Substring(startIndex, length).Replace("::", ":");
		}

		private const char DomainSeparator = ':';

		private const string DomainSeparatorString = ":";

		private const string EncodedDomainSeparatorString = "::";

		private SmtpDomainWithSubdomains domain;

		private SmtpReceiveCapabilities capabilities;

		private SmtpX509Identifier smtpX509Identifier;
	}
}
