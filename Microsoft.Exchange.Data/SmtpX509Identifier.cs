using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class SmtpX509Identifier
	{
		public SmtpX509Identifier(string subject, SmtpDomainWithSubdomains subjectCN, string issuer)
		{
			ArgumentValidator.ThrowIfNull("subject", subject);
			ArgumentValidator.ThrowIfNull("subjectCN", subjectCN);
			this.CertificateSubject = subject;
			this.SubjectCommonName = subjectCN;
			this.CertificateIssuer = issuer;
		}

		public SmtpX509Identifier(string smtpX509Identifier)
		{
			string certificateSubject;
			SmtpDomainWithSubdomains subjectCommonName;
			string certificateIssuer;
			bool flag;
			if (!SmtpX509Identifier.InternalTryParse(smtpX509Identifier, out certificateSubject, out subjectCommonName, out certificateIssuer, out flag))
			{
				string s = string.IsNullOrEmpty(smtpX509Identifier) ? string.Empty : smtpX509Identifier;
				string message = flag ? DataStrings.InvalidDomainInSmtpX509Identifier(s) : DataStrings.InvalidSmtpX509Identifier(s);
				throw new StrongTypeFormatException(message, "SmtpX509Identifier");
			}
			this.CertificateIssuer = certificateIssuer;
			this.CertificateSubject = certificateSubject;
			this.SubjectCommonName = subjectCommonName;
		}

		protected SmtpX509Identifier()
		{
		}

		public string CertificateSubject { get; protected set; }

		public string CertificateIssuer { get; protected set; }

		public SmtpDomainWithSubdomains SubjectCommonName { get; protected set; }

		public static SmtpX509Identifier Parse(string s)
		{
			return new SmtpX509Identifier(s);
		}

		public static bool TryParse(string s, out SmtpX509Identifier result)
		{
			result = null;
			string subject;
			SmtpDomainWithSubdomains subjectCN;
			string issuer;
			bool flag;
			if (!SmtpX509Identifier.InternalTryParse(s, out subject, out subjectCN, out issuer, out flag))
			{
				return false;
			}
			result = new SmtpX509Identifier(subject, subjectCN, issuer);
			return true;
		}

		public bool Equals(SmtpX509Identifier rhs)
		{
			return rhs != null && string.Equals(this.CertificateSubject, rhs.CertificateSubject, StringComparison.OrdinalIgnoreCase) && string.Equals(this.CertificateIssuer, rhs.CertificateIssuer, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object rhs)
		{
			return this.Equals(rhs as SmtpX509Identifier);
		}

		public override string ToString()
		{
			string arg = string.Empty;
			if (!string.IsNullOrEmpty(this.CertificateIssuer))
			{
				arg = "<I>" + this.CertificateIssuer;
			}
			return string.Format("{0}{1}{2}", arg, "<S>", this.CertificateSubject);
		}

		public override int GetHashCode()
		{
			string text = this.CertificateIssuer ?? string.Empty;
			return this.CertificateSubject.GetHashCode() ^ text.GetHashCode();
		}

		private static bool InternalTryParse(string s, out string subject, out SmtpDomainWithSubdomains commonName, out string issuer, out bool invalidDomainError)
		{
			Match match = Regex.Match(s, "^(<I>([^<]+))?(<S>(.+))", RegexOptions.IgnoreCase);
			return SmtpX509Identifier.TryParseFromRegexMatch(match, out subject, out commonName, out issuer, out invalidDomainError);
		}

		protected static bool TryParseFromRegexMatch(Match match, out string subject, out SmtpDomainWithSubdomains commonName, out string issuer, out bool invalidDomainError)
		{
			subject = null;
			issuer = null;
			commonName = null;
			invalidDomainError = false;
			if (match.Success)
			{
				Match match2 = Regex.Match(match.Groups[4].Value, "CN=(.*?)($|,)", RegexOptions.IgnoreCase);
				if (!match2.Success)
				{
					return false;
				}
				if (!SmtpDomainWithSubdomains.TryParse(match2.Groups[1].Value, out commonName))
				{
					invalidDomainError = true;
					return false;
				}
				try
				{
					subject = new X500DistinguishedName(match.Groups[4].Value, X500DistinguishedNameFlags.None).Format(false);
					issuer = new X500DistinguishedName(match.Groups[2].Value, X500DistinguishedNameFlags.None).Format(false);
					return true;
				}
				catch (CryptographicException)
				{
					return false;
				}
				return false;
			}
			return false;
		}

		public const string IssuerPrefix = "<I>";

		public const string SubjectPrefix = "<S>";

		private const string FormatRegularExpression = "^(<I>([^<]+))?(<S>(.+))";

		private const string CommonNameRegularExpression = "CN=(.*?)($|,)";
	}
}
