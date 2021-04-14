using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Net.WSSecurity
{
	[XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", ElementName = "Security", IsNullable = false)]
	public class WSSecurityHeader : SoapHeader
	{
		internal static WSSecurityHeader Create(XmlElement securityHeader)
		{
			return new WSSecurityHeader
			{
				MustUnderstand = true,
				EncryptedToken = securityHeader["EncryptedData", "http://www.w3.org/2001/04/xmlenc#"]
			};
		}

		internal static WSSecurityHeader Create(RequestedToken token)
		{
			return new WSSecurityHeader
			{
				MustUnderstand = true,
				EncryptedToken = token.SecurityToken
			};
		}

		internal static WSSecurityHeader Create(X509Certificate2 certificate)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.PreserveWhitespace = true;
			DateTime utcNow = DateTime.UtcNow;
			Timestamp timestamp = new Timestamp("_0", utcNow, utcNow + WSSecurityHeader.TimestampDuration);
			XmlElement xml = timestamp.GetXml(xmlDocument);
			XmlElement signature = WSSecurityHeader.CreateSignature(xmlDocument, new XmlElement[]
			{
				xml
			}, certificate);
			return new WSSecurityHeader
			{
				MustUnderstand = true,
				Timestamp = xml,
				Signature = signature
			};
		}

		private static XmlElement CreateSignature(XmlDocument xmlDocument, XmlElement[] elementsToSign, X509Certificate2 certificate)
		{
			SecuritySignedXml securitySignedXml = new SecuritySignedXml(xmlDocument, elementsToSign);
			XmlDsigExcC14NTransform transform = new XmlDsigExcC14NTransform();
			foreach (XmlElement xmlElement in elementsToSign)
			{
				string attributeValue = WSSecurityUtility.Id.GetAttributeValue(xmlElement);
				Reference reference = new Reference("#" + attributeValue);
				reference.AddTransform(transform);
				securitySignedXml.Signature.SignedInfo.AddReference(reference);
			}
			securitySignedXml.SigningKey = certificate.PrivateKey;
			securitySignedXml.Signature.KeyInfo = new KeyInfo();
			securitySignedXml.Signature.KeyInfo.AddClause(new KeyInfoX509Data(certificate));
			securitySignedXml.ComputeSignature();
			return securitySignedXml.GetXml();
		}

		private const string EncryptedDataElementName = "EncryptedData";

		private const string SignatureElementName = "Signature";

		private const string TimestampElementName = "Timestamp";

		private static readonly TimeSpan TimestampDuration = TimeSpan.FromMinutes(5.0);

		[XmlAnyElement(Name = "Timestamp", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
		public XmlElement Timestamp;

		[XmlAnyElement(Name = "EncryptedData", Namespace = "http://www.w3.org/2001/04/xmlenc#")]
		public XmlElement EncryptedToken;

		[XmlAnyElement(Name = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public XmlElement Signature;
	}
}
