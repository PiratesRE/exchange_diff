using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class CertificateRequestInfo
	{
		public bool IsExportable
		{
			get
			{
				return (this.options & CertificateCreationOption.Exportable) == CertificateCreationOption.Exportable;
			}
			set
			{
				if (value)
				{
					this.options |= CertificateCreationOption.Exportable;
					return;
				}
				this.options &= ~CertificateCreationOption.Exportable;
			}
		}

		public X500DistinguishedName Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		public IEnumerable<string> AlternativeDomainNames
		{
			get
			{
				return this.subjectAlternativeNames;
			}
			set
			{
				this.subjectAlternativeNames = value;
			}
		}

		public int KeySize
		{
			get
			{
				if (this.SourceProvider != CertificateCreationOption.DSSProvider)
				{
					return this.keySize;
				}
				return 1024;
			}
			set
			{
				this.keySize = value;
			}
		}

		public string FriendlyName
		{
			get
			{
				return this.friendlyName ?? "Microsoft Exchange";
			}
			set
			{
				this.friendlyName = value;
			}
		}

		public CertificateCreationOption SourceProvider
		{
			get
			{
				if ((this.options & CertificateCreationOption.DSSProvider) != CertificateCreationOption.DSSProvider)
				{
					return CertificateCreationOption.RSAProvider;
				}
				return CertificateCreationOption.DSSProvider;
			}
			set
			{
				this.options &= ~(CertificateCreationOption.RSAProvider | CertificateCreationOption.DSSProvider);
				this.options |= ((value == CertificateCreationOption.DSSProvider) ? CertificateCreationOption.DSSProvider : CertificateCreationOption.RSAProvider);
			}
		}

		public void ValidateDomainNamesAndSetSubject()
		{
			if (this.subjectAlternativeNames != null)
			{
				foreach (string text in this.subjectAlternativeNames)
				{
					if (!Dns.IsValidName(text) && (!Dns.IsValidWildcardName(text) || text.Length <= 2))
					{
						throw new ArgumentException(string.Format("Invalid FQDN '{0}' in list of alternate domain names", text), "AlternativeDomainNames");
					}
					if (this.subject == null && text.Length < 64 && text[0] != '*')
					{
						this.subject = new X500DistinguishedName("cn=" + text);
					}
				}
			}
			if (this.subject == null)
			{
				throw new ArgumentException("No valid subject was found");
			}
		}

		public X509ExtensionCollection GetExtensions()
		{
			X509ExtensionCollection x509ExtensionCollection = new X509ExtensionCollection();
			if (this.SourceProvider == CertificateCreationOption.RSAProvider)
			{
				x509ExtensionCollection.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, true));
			}
			else
			{
				x509ExtensionCollection.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
			}
			if (this.subjectAlternativeNames != null)
			{
				x509ExtensionCollection.Add(new X509SubjectAltNameExtension(this.subjectAlternativeNames, false));
			}
			x509ExtensionCollection.Add(new X509BasicConstraintsExtension(false, false, 0, true));
			return x509ExtensionCollection;
		}

		private const int DssKeySize = 1024;

		private IEnumerable<string> subjectAlternativeNames;

		private CertificateCreationOption options;

		private int keySize = 2048;

		private string friendlyName;

		private X500DistinguishedName subject;
	}
}
