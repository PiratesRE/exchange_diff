using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class TlsCertificate
	{
		public TlsCertificate(string certificateName)
		{
			SmtpDomainWithSubdomains smtpDomainWithSubdomains = null;
			SmtpX509Identifier smtpX509Identifier = null;
			if (!TlsCertificate.InternalTryParse(certificateName, out smtpDomainWithSubdomains, out smtpX509Identifier))
			{
				string s = string.IsNullOrEmpty(certificateName) ? string.Empty : certificateName;
				throw new StrongTypeFormatException(DataStrings.InvalidTlsCertificateName(s), "TlsCertificateName");
			}
			this.tlsCertificateName = (smtpDomainWithSubdomains ?? smtpX509Identifier);
		}

		public TlsCertificate(SmtpDomainWithSubdomains fqdn, SmtpX509Identifier x509Identifier)
		{
			if ((fqdn == null && x509Identifier == null) || (fqdn != null && x509Identifier != null))
			{
				throw new ArgumentException("FQDN and X509Identifier both cannot be null or both have values");
			}
			this.tlsCertificateName = (fqdn ?? x509Identifier);
		}

		public static TlsCertificate Parse(string certificateName)
		{
			return new TlsCertificate(certificateName);
		}

		public static bool TryParse(string certificateName, out TlsCertificate tlsCertificate)
		{
			tlsCertificate = null;
			SmtpDomainWithSubdomains fqdn = null;
			SmtpX509Identifier x509Identifier = null;
			if (!TlsCertificate.InternalTryParse(certificateName, out fqdn, out x509Identifier))
			{
				return false;
			}
			tlsCertificate = new TlsCertificate(fqdn, x509Identifier);
			return true;
		}

		public override string ToString()
		{
			return this.tlsCertificateName.ToString();
		}

		public override int GetHashCode()
		{
			return this.tlsCertificateName.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TlsCertificate);
		}

		public bool Equals(TlsCertificate rhs)
		{
			return rhs != null && this.tlsCertificateName.Equals(rhs.tlsCertificateName);
		}

		public object TlsCertificateName
		{
			get
			{
				return this.tlsCertificateName;
			}
			private set
			{
				this.tlsCertificateName = value;
			}
		}

		private static bool InternalTryParse(string certificateName, out SmtpDomainWithSubdomains fqdn, out SmtpX509Identifier x509Identifier)
		{
			fqdn = null;
			x509Identifier = null;
			return SmtpDomainWithSubdomains.TryParse(certificateName, out fqdn) || SmtpX509Identifier.TryParse(certificateName, out x509Identifier);
		}

		private object tlsCertificateName;
	}
}
