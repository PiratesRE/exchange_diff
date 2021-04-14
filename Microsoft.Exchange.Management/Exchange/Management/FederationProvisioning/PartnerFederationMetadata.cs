using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	public abstract class PartnerFederationMetadata
	{
		public PartnerFederationMetadata(WriteVerboseDelegate writeVerbose)
		{
			this.writeVerbose = writeVerbose;
		}

		public X509Certificate2 TokenIssuerCertificate
		{
			get
			{
				return this.tokenIssuerCertificate;
			}
			set
			{
				this.tokenIssuerCertificate = value;
			}
		}

		public X509Certificate2 TokenIssuerPrevCertificate
		{
			get
			{
				return this.tokenIssuerPrevCertificate;
			}
			set
			{
				this.tokenIssuerPrevCertificate = value;
			}
		}

		public string PolicyReferenceUri
		{
			get
			{
				return this.policyReferenceUri;
			}
			set
			{
				this.policyReferenceUri = value;
			}
		}

		public Uri TokenIssuerMetadataEpr
		{
			get
			{
				return this.tokenIssuerMetadataEpr;
			}
			set
			{
				this.tokenIssuerMetadataEpr = value;
			}
		}

		public Uri TokenIssuerUri
		{
			get
			{
				return this.tokenIssuerUri;
			}
			set
			{
				this.tokenIssuerUri = value;
			}
		}

		public Uri TokenIssuerEpr
		{
			get
			{
				return this.tokenIssuerEpr;
			}
			set
			{
				this.tokenIssuerEpr = value;
			}
		}

		public Uri WebRequestorRedirectEpr
		{
			get
			{
				return this.webRequestorRedirectEpr;
			}
			set
			{
				this.webRequestorRedirectEpr = value;
			}
		}

		public string TokenIssuerCertReference
		{
			get
			{
				return this.tokenIssuerCertReference;
			}
			set
			{
				this.tokenIssuerCertReference = value;
			}
		}

		public string TokenIssuerPrevCertReference
		{
			get
			{
				return this.tokenIssuerPrevCertReference;
			}
			set
			{
				this.tokenIssuerPrevCertReference = value;
			}
		}

		protected WriteVerboseDelegate WriteVerbose
		{
			get
			{
				return this.writeVerbose;
			}
		}

		protected virtual void Parse(XPathDocument xmlFederationMetadata)
		{
		}

		protected XPathDocument GetFederationMetadataXPathDocument(Uri partnerFederationMetadataEpr)
		{
			if (null == partnerFederationMetadataEpr)
			{
				throw new ArgumentNullException("PartnerFederationMetadataEpr");
			}
			this.WriteVerbose(Strings.RequestingFederationMetadataFromEndPoint(partnerFederationMetadataEpr.ToString()));
			Exception ex = null;
			string s = null;
			DateTime t = DateTime.UtcNow.Add(TimeSpan.FromMinutes(1.0));
			do
			{
				if (ex != null)
				{
					this.WriteVerbose(Strings.FailedToRetrieveFederationMetadata(ex.ToString()));
					Thread.Sleep(TimeSpan.FromSeconds(5.0));
					ex = null;
				}
				using (PartnerFederationMetadata.TimeOutWebClient timeOutWebClient = new PartnerFederationMetadata.TimeOutWebClient(59000))
				{
					timeOutWebClient.Credentials = CredentialCache.DefaultCredentials;
					WebProxy webProxy = LiveConfiguration.GetWebProxy(this.WriteVerbose);
					timeOutWebClient.Proxy = (webProxy ?? new WebProxy());
					timeOutWebClient.Headers.Add(HttpRequestHeader.UserAgent, "MicrosoftExchangeFedTrustManagement");
					try
					{
						s = timeOutWebClient.DownloadString(partnerFederationMetadataEpr);
					}
					catch (WebException ex2)
					{
						ex = ex2;
					}
					catch (IOException ex3)
					{
						ex = ex3;
					}
					catch (ProtocolViolationException ex4)
					{
						ex = ex4;
					}
				}
			}
			while (ex != null && DateTime.UtcNow < t);
			if (ex != null)
			{
				throw new FederationMetadataException(Strings.ErrorAccessingFederationMetadata(ex.Message));
			}
			XPathDocument result = null;
			try
			{
				StringReader textReader = new StringReader(s);
				result = SafeXmlFactory.CreateXPathDocument(textReader);
			}
			catch (XmlException ex5)
			{
				throw new FederationMetadataException(Strings.ErrorInvalidFederationMetadata(ex5.Message));
			}
			catch (XPathException ex6)
			{
				throw new FederationMetadataException(Strings.ErrorInvalidFederationMetadata(ex6.Message));
			}
			return result;
		}

		public const string FederationSchemaNamespace = "http://schemas.xmlsoap.org/ws/2006/03/federation";

		public const string WSSecurityExtSchemaNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

		public const string DigitalSigSchemaNamespace = "http://www.w3.org/2000/09/xmldsig#";

		public const string WebServiceUtilitySchemaNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

		public const string WebServiceAddressingSchemaNamespace = "http://www.w3.org/2005/08/addressing";

		public const string FederationSchemaNamespacePrefix = "fed";

		public const string WSSecurityExtSchemaNamespacePrefix = "wsse";

		public const string DigitalSigSchemaNamespacePrefix = "ds";

		public const string WebServiceUtilitySchemaNamespacePrefix = "wsu";

		public const string WebServiceAddressingSchemaNamespacePrefix = "wsa";

		public const string FedTrustMetadataClientUserAgent = "MicrosoftExchangeFedTrustManagement";

		private const int DefaultFederatedMetadataRequestTimeout = 59000;

		private string tokenIssuerPrevCertReference;

		private string tokenIssuerCertReference;

		private Uri tokenIssuerMetadataEpr;

		private string policyReferenceUri;

		private X509Certificate2 tokenIssuerPrevCertificate;

		private X509Certificate2 tokenIssuerCertificate;

		private Uri tokenIssuerUri;

		private Uri tokenIssuerEpr;

		private Uri webRequestorRedirectEpr;

		private WriteVerboseDelegate writeVerbose;

		private class TimeOutWebClient : WebClient
		{
			public TimeOutWebClient(int timeout)
			{
				this.Timeout = timeout;
			}

			public int Timeout { get; private set; }

			protected override WebRequest GetWebRequest(Uri uri)
			{
				WebRequest webRequest = base.GetWebRequest(uri);
				if (this.Timeout != 0)
				{
					webRequest.Timeout = this.Timeout;
				}
				return webRequest;
			}
		}
	}
}
