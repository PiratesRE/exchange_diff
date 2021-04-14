using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Sharing;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal static class SharingKeyHandler
	{
		public static SmtpAddress Decrypt(XmlElement encryptedSharingKey, SymmetricSecurityKey symmetricSecurityKey)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			try
			{
				xmlDocument.AppendChild(xmlDocument.ImportNode(encryptedSharingKey, true));
			}
			catch (XmlException)
			{
				SharingKeyHandler.Tracer.TraceError<string>(0L, "Unable to import XML element of sharing key: {0}", encryptedSharingKey.OuterXml);
				return SmtpAddress.Empty;
			}
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping("key", symmetricSecurityKey.GetSymmetricAlgorithm("http://www.w3.org/2001/04/xmlenc#tripledes-cbc"));
			try
			{
				encryptedXml.DecryptDocument();
			}
			catch (CryptographicException)
			{
				SharingKeyHandler.Tracer.TraceError<string>(0L, "Unable to decrypt XML element sharing key: {0}", encryptedSharingKey.OuterXml);
				return SmtpAddress.Empty;
			}
			return new SmtpAddress(xmlDocument.DocumentElement.InnerText);
		}

		public static XmlElement Encrypt(SmtpAddress externalId, SymmetricSecurityKey symmetricSecurityKey)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("SharingKey");
			xmlElement.InnerText = externalId.ToString();
			EncryptedXml encryptedXml = new EncryptedXml();
			encryptedXml.AddKeyNameMapping("key", symmetricSecurityKey.GetSymmetricAlgorithm("http://www.w3.org/2001/04/xmlenc#tripledes-cbc"));
			EncryptedData encryptedData = encryptedXml.Encrypt(xmlElement, "key");
			return encryptedData.GetXml();
		}

		private const string KeyName = "key";

		private static readonly Trace Tracer = ExTraceGlobals.SharingKeyHandlerTracer;
	}
}
