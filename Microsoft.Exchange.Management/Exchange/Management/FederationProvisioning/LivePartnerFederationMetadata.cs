using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	public sealed class LivePartnerFederationMetadata : PartnerFederationMetadata
	{
		private static void NullWriteWarning(LocalizedString message)
		{
		}

		private static void NullWriteVerbose(LocalizedString message)
		{
		}

		public static LivePartnerFederationMetadata LoadFrom(Uri partnerFederationMetadataEpr, WriteVerboseDelegate writeVerbose = null)
		{
			if (partnerFederationMetadataEpr == null)
			{
				throw new ArgumentNullException("partnerFederationMetadataEpr");
			}
			if (writeVerbose == null)
			{
				writeVerbose = new WriteVerboseDelegate(LivePartnerFederationMetadata.NullWriteVerbose);
			}
			LivePartnerFederationMetadata livePartnerFederationMetadata = new LivePartnerFederationMetadata(writeVerbose);
			XPathDocument federationMetadataXPathDocument = livePartnerFederationMetadata.GetFederationMetadataXPathDocument(partnerFederationMetadataEpr);
			livePartnerFederationMetadata.TokenIssuerMetadataEpr = partnerFederationMetadataEpr;
			livePartnerFederationMetadata.Parse(federationMetadataXPathDocument);
			livePartnerFederationMetadata.PolicyReferenceUri = "EX_MBI_FED_SSL";
			return livePartnerFederationMetadata;
		}

		public static void InitializeDataObjectFromMetadata(FederationTrust federationTrust, PartnerFederationMetadata partnerFederationMetadata, WriteWarningDelegate writeWarning)
		{
			if (writeWarning == null)
			{
				writeWarning = new WriteWarningDelegate(LivePartnerFederationMetadata.NullWriteWarning);
			}
			federationTrust.PolicyReferenceUri = partnerFederationMetadata.PolicyReferenceUri;
			federationTrust.TokenIssuerMetadataEpr = partnerFederationMetadata.TokenIssuerMetadataEpr;
			federationTrust.TokenIssuerUri = partnerFederationMetadata.TokenIssuerUri;
			federationTrust.TokenIssuerEpr = partnerFederationMetadata.TokenIssuerEpr;
			federationTrust.WebRequestorRedirectEpr = partnerFederationMetadata.WebRequestorRedirectEpr;
			federationTrust.TokenIssuerCertReference = partnerFederationMetadata.TokenIssuerCertReference;
			federationTrust.TokenIssuerPrevCertReference = partnerFederationMetadata.TokenIssuerPrevCertReference;
			if (partnerFederationMetadata.TokenIssuerCertificate != null && partnerFederationMetadata.TokenIssuerPrevCertificate != null && partnerFederationMetadata.TokenIssuerPrevCertificate.NotAfter > partnerFederationMetadata.TokenIssuerCertificate.NotAfter)
			{
				X509Certificate2 tokenIssuerCertificate = partnerFederationMetadata.TokenIssuerCertificate;
				partnerFederationMetadata.TokenIssuerCertificate = partnerFederationMetadata.TokenIssuerPrevCertificate;
				partnerFederationMetadata.TokenIssuerPrevCertificate = tokenIssuerCertificate;
			}
			if (partnerFederationMetadata.TokenIssuerCertificate != null)
			{
				if (partnerFederationMetadata.TokenIssuerCertificate.NotAfter > DateTime.UtcNow)
				{
					if (federationTrust.TokenIssuerCertificate == null || !federationTrust.TokenIssuerCertificate.Thumbprint.Equals(partnerFederationMetadata.TokenIssuerCertificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
					{
						federationTrust.TokenIssuerCertificate = partnerFederationMetadata.TokenIssuerCertificate;
					}
				}
				else
				{
					writeWarning(Strings.WarningIssuerCertificateExpired(partnerFederationMetadata.TokenIssuerCertificate.Thumbprint));
					if (federationTrust.TokenIssuerCertificate != null)
					{
						federationTrust.TokenIssuerCertificate = null;
					}
				}
			}
			if (partnerFederationMetadata.TokenIssuerPrevCertificate != null)
			{
				if (partnerFederationMetadata.TokenIssuerPrevCertificate.NotAfter > DateTime.UtcNow)
				{
					if (federationTrust.TokenIssuerPrevCertificate == null || !federationTrust.TokenIssuerPrevCertificate.Thumbprint.Equals(partnerFederationMetadata.TokenIssuerPrevCertificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
					{
						federationTrust.TokenIssuerPrevCertificate = partnerFederationMetadata.TokenIssuerPrevCertificate;
					}
				}
				else
				{
					writeWarning(Strings.WarningIssuerCertificateExpired(partnerFederationMetadata.TokenIssuerPrevCertificate.Thumbprint));
					if (federationTrust.TokenIssuerPrevCertificate != null)
					{
						federationTrust.TokenIssuerPrevCertificate = null;
					}
				}
			}
			if (federationTrust.TokenIssuerCertificate == null && federationTrust.TokenIssuerPrevCertificate != null)
			{
				federationTrust.TokenIssuerCertificate = federationTrust.TokenIssuerPrevCertificate;
				federationTrust.TokenIssuerPrevCertificate = null;
			}
			if (federationTrust.TokenIssuerCertificate == null && federationTrust.TokenIssuerPrevCertificate == null)
			{
				throw new FederationMetadataException(Strings.NoValidIssuerCertificate);
			}
		}

		private LivePartnerFederationMetadata(WriteVerboseDelegate writeVerbose) : base(writeVerbose)
		{
			base.TokenIssuerCertReference = "stscer";
			base.TokenIssuerPrevCertReference = "stsbcer";
		}

		protected override void Parse(XPathDocument pathDocument)
		{
			base.TokenIssuerUri = null;
			XPathNavigator xpathNavigator = pathDocument.CreateNavigator();
			if (xpathNavigator == null)
			{
				throw new FederationMetadataException(Strings.ErrorCorruptFederationMetadata);
			}
			xpathNavigator.MoveToChild(XPathNodeType.Element);
			base.WriteVerbose(Strings.ParsingFederationMetadata(xpathNavigator.OuterXml));
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xpathNavigator.NameTable);
			xmlNamespaceManager.AddNamespace("fed", "http://schemas.xmlsoap.org/ws/2006/03/federation");
			xmlNamespaceManager.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			xmlNamespaceManager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
			xmlNamespaceManager.AddNamespace("wsa", "http://www.w3.org/2005/08/addressing");
			string xpath = string.Format(CultureInfo.InvariantCulture, "//{0}:Federation", new object[]
			{
				"fed"
			});
			string xpath2 = string.Format(CultureInfo.InvariantCulture, "//{0}:IssuerNamesOffered//{0}:IssuerName", new object[]
			{
				"fed"
			});
			string xpath3 = string.Format(CultureInfo.InvariantCulture, "//{0}:TokenSigningKeyInfo[@{1}:Id=\"{3}\"]//{2}:X509Certificate", new object[]
			{
				"fed",
				"wsu",
				"ds",
				base.TokenIssuerCertReference
			});
			string xpath4 = string.Format(CultureInfo.InvariantCulture, "//{0}:TokenSigningKeyInfo[@{1}:Id=\"{3}\"]//{2}:X509Certificate", new object[]
			{
				"fed",
				"wsu",
				"ds",
				base.TokenIssuerPrevCertReference
			});
			string xpath5 = string.Format(CultureInfo.InvariantCulture, "//{0}:TargetServiceEndpoint//{1}:Address", new object[]
			{
				"fed",
				"wsa"
			});
			string xpath6 = string.Format(CultureInfo.InvariantCulture, "//{0}:WebRequestorRedirectEndpoint//{1}:Address", new object[]
			{
				"fed",
				"wsa"
			});
			XPathExpression xpathExpression = xpathNavigator.Compile(xpath);
			xpathExpression.SetContext(xmlNamespaceManager);
			XPathNodeIterator xpathNodeIterator = xpathNavigator.Select(xpathExpression);
			while (xpathNodeIterator.MoveNext())
			{
				XPathNavigator xpathNavigator2 = xpathNodeIterator.Current;
				XPathNavigator xpathNavigator3 = xpathNavigator2.SelectSingleNode(xpath2, xmlNamespaceManager);
				if (xpathNavigator3 != null)
				{
					string attribute = xpathNavigator3.GetAttribute("uri", string.Empty);
					if (!string.IsNullOrEmpty(attribute))
					{
						base.TokenIssuerUri = new Uri(attribute.Trim());
						base.WriteVerbose(Strings.ParsingTokenIssuerUri(base.TokenIssuerUri.ToString()));
						xpathNavigator3 = xpathNodeIterator.Current.SelectSingleNode(xpath3, xmlNamespaceManager);
						if (xpathNavigator3 == null || string.IsNullOrEmpty(xpathNavigator3.Value))
						{
							throw new FederationMetadataException(Strings.ErrorInvalidFederationMetadata(Strings.CannotLocateCurrentCertificate));
						}
						base.TokenIssuerCertificate = this.ParseCertFromString(xpathNavigator3.Value);
						base.WriteVerbose(Strings.ParsingTokenIssuerCertificate(base.TokenIssuerCertificate.Thumbprint));
						xpathNavigator3 = xpathNodeIterator.Current.SelectSingleNode(xpath4, xmlNamespaceManager);
						if (xpathNavigator3 != null && !string.IsNullOrEmpty(xpathNavigator3.Value))
						{
							base.TokenIssuerPrevCertificate = this.ParseCertFromString(xpathNavigator3.Value);
							base.WriteVerbose(Strings.ParsingTokenIssuerPreviousCertificate(base.TokenIssuerPrevCertificate.Thumbprint));
						}
						xpathNavigator3 = xpathNodeIterator.Current.SelectSingleNode(xpath5, xmlNamespaceManager);
						if (xpathNavigator3 == null || string.IsNullOrEmpty(xpathNavigator3.Value))
						{
							throw new FederationMetadataException(Strings.ErrorInvalidFederationMetadata(Strings.CannotLocateTargetServiceEndpoint));
						}
						base.TokenIssuerEpr = new Uri(xpathNavigator3.Value.Trim(), UriKind.Absolute);
						base.WriteVerbose(Strings.ParsingTokenIssuerEndPoint(base.TokenIssuerEpr.ToString()));
						xpathNavigator3 = xpathNodeIterator.Current.SelectSingleNode(xpath6, xmlNamespaceManager);
						if (xpathNavigator3 == null || string.IsNullOrEmpty(xpathNavigator3.Value))
						{
							throw new FederationMetadataException(Strings.ErrorInvalidFederationMetadata(Strings.CannotLocateWebRequestorEndpoint));
						}
						base.WebRequestorRedirectEpr = new Uri(xpathNavigator3.Value.Trim(), UriKind.Absolute);
						base.WriteVerbose(Strings.ParsingWebRequestorRedirectEndPoint(base.WebRequestorRedirectEpr.ToString()));
						break;
					}
				}
			}
			if (null == base.TokenIssuerUri)
			{
				throw new FederationMetadataException(Strings.ErrorInvalidFederationMetadata(Strings.CannotLocateFederationInfo));
			}
		}

		private X509Certificate2 ParseCertFromString(string certAsString)
		{
			X509Certificate2 result;
			try
			{
				certAsString = certAsString.Trim();
				byte[] rawData = Convert.FromBase64String(certAsString);
				X509Certificate2 x509Certificate = new X509Certificate2(rawData);
				result = x509Certificate;
			}
			catch (CryptographicException ex)
			{
				throw new FederationMetadataException(Strings.ErrorInvalidFederationMetadata(ex.Message));
			}
			return result;
		}

		private const string CurrentKeyReference = "stscer";

		private const string PreviousKeyReference = "stsbcer";
	}
}
