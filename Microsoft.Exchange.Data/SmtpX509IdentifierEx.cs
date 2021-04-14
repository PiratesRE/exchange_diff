using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class SmtpX509IdentifierEx : SmtpX509Identifier
	{
		public SmtpX509IdentifierEx(string subject, SmtpDomainWithSubdomains subjectCN, string issuer, SmtpDomainWithSubdomains[] domains) : base(subject, subjectCN, issuer)
		{
			this.CertificateDomains = domains;
		}

		public SmtpX509IdentifierEx(string smtpX509Identifier)
		{
			string certificateSubject;
			SmtpDomainWithSubdomains subjectCommonName;
			string certificateIssuer;
			SmtpDomainWithSubdomains[] certificateDomains;
			bool flag;
			if (!SmtpX509IdentifierEx.InternalTryParse(smtpX509Identifier, out certificateSubject, out subjectCommonName, out certificateIssuer, out certificateDomains, out flag))
			{
				string s = string.IsNullOrEmpty(smtpX509Identifier) ? string.Empty : smtpX509Identifier;
				string message;
				if (flag)
				{
					message = DataStrings.InvalidDomainInSmtpX509Identifier(s);
				}
				else
				{
					message = DataStrings.InvalidSmtpX509Identifier(s);
				}
				throw new StrongTypeFormatException(message, "SmtpX509IdentifierEx");
			}
			base.CertificateIssuer = certificateIssuer;
			base.CertificateSubject = certificateSubject;
			base.SubjectCommonName = subjectCommonName;
			this.CertificateDomains = certificateDomains;
		}

		public SmtpDomainWithSubdomains[] CertificateDomains { get; private set; }

		public new static SmtpX509IdentifierEx Parse(string s)
		{
			return new SmtpX509IdentifierEx(s);
		}

		public static bool TryParse(string s, out SmtpX509IdentifierEx result)
		{
			result = null;
			string subject;
			SmtpDomainWithSubdomains subjectCN;
			string issuer;
			SmtpDomainWithSubdomains[] domains;
			bool flag;
			if (!SmtpX509IdentifierEx.InternalTryParse(s, out subject, out subjectCN, out issuer, out domains, out flag))
			{
				return false;
			}
			result = new SmtpX509IdentifierEx(subject, subjectCN, issuer, domains);
			return true;
		}

		public bool Matches(TlsCertificate certificate)
		{
			if (certificate == null)
			{
				return false;
			}
			if (certificate.TlsCertificateName is SmtpDomainWithSubdomains)
			{
				return this.Matches(certificate.TlsCertificateName as SmtpDomainWithSubdomains);
			}
			return certificate.TlsCertificateName is SmtpX509Identifier && this.Matches(certificate.TlsCertificateName as SmtpX509Identifier);
		}

		public bool Matches(SmtpDomainWithSubdomains fqdn)
		{
			if (fqdn == null)
			{
				return false;
			}
			string fqdnString = fqdn.ToString();
			return base.SubjectCommonName.Match(fqdnString) >= 0 || fqdn.Match(base.SubjectCommonName.ToString()) >= 0 || (this.CertificateDomains != null && this.CertificateDomains.Any((SmtpDomainWithSubdomains domain) => domain.Match(fqdnString) >= 0 || fqdn.Match(domain.ToString()) >= 0));
		}

		public bool Matches(SmtpX509Identifier certificate)
		{
			return certificate != null && string.Equals(base.CertificateSubject, certificate.CertificateSubject, StringComparison.OrdinalIgnoreCase) && (string.IsNullOrEmpty(base.CertificateIssuer) || string.IsNullOrEmpty(certificate.CertificateIssuer) || string.Equals(base.CertificateIssuer, certificate.CertificateIssuer, StringComparison.OrdinalIgnoreCase));
		}

		public bool Equals(SmtpX509IdentifierEx rhs)
		{
			if (!base.Equals(rhs))
			{
				return false;
			}
			if (this.CertificateDomains != null)
			{
				if (rhs.CertificateDomains == null)
				{
					return false;
				}
				if (this.CertificateDomains.Length != rhs.CertificateDomains.Length)
				{
					return false;
				}
				foreach (SmtpDomainWithSubdomains value in this.CertificateDomains)
				{
					if (!rhs.CertificateDomains.Contains(value))
					{
						return false;
					}
				}
			}
			else if (rhs.CertificateDomains != null)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object rhs)
		{
			return this.Equals(rhs as SmtpX509IdentifierEx);
		}

		public override string ToString()
		{
			string arg = string.Empty;
			if (this.CertificateDomains != null && this.CertificateDomains.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (SmtpDomainWithSubdomains value in this.CertificateDomains)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(";");
				}
				arg = "<D>" + stringBuilder.ToString(0, stringBuilder.Length - 1);
			}
			return string.Format("{0}{1}", base.ToString(), arg);
		}

		public override int GetHashCode()
		{
			SmtpDomainWithSubdomains[] array = this.CertificateDomains ?? new SmtpDomainWithSubdomains[0];
			return base.GetHashCode() ^ array.GetHashCode();
		}

		private static bool InternalTryParse(string s, out string subject, out SmtpDomainWithSubdomains commonName, out string issuer, out SmtpDomainWithSubdomains[] domains, out bool invalidDomainError)
		{
			domains = null;
			Match match = Regex.Match(s, "^(<I>([^<]+))?(<S>([^<]+))(<D>(.+))?", RegexOptions.IgnoreCase);
			if (!SmtpX509Identifier.TryParseFromRegexMatch(match, out subject, out commonName, out issuer, out invalidDomainError))
			{
				return false;
			}
			if (match.Success && !string.IsNullOrEmpty(match.Groups[6].Value))
			{
				string[] array = match.Groups[6].Value.Split(new string[]
				{
					";"
				}, StringSplitOptions.RemoveEmptyEntries);
				domains = new SmtpDomainWithSubdomains[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (!SmtpDomainWithSubdomains.TryParse(array[i], out domains[i]))
					{
						invalidDomainError = true;
						return false;
					}
				}
			}
			return true;
		}

		private const string DomainsPrefix = "<D>";

		private const string DomainsSeparator = ";";

		private const string FormatRegularExpression = "^(<I>([^<]+))?(<S>([^<]+))(<D>(.+))?";
	}
}
