using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SymmetricEncryptedXml
	{
		public static XmlElement Encrypt(XmlElement xmlElement, SymmetricSecurityKey symmetricSecurityKey)
		{
			EncryptedXml encryptedXml = new EncryptedXml();
			encryptedXml.AddKeyNameMapping("key", symmetricSecurityKey.GetSymmetricAlgorithm("http://www.w3.org/2001/04/xmlenc#tripledes-cbc"));
			EncryptedData encryptedData = encryptedXml.Encrypt(xmlElement, "key");
			return encryptedData.GetXml();
		}

		public static XmlElement Decrypt(XmlElement xmlElement, SymmetricSecurityKey symmetricSecurityKey)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlNode newChild = xmlDocument.ImportNode(xmlElement, true);
			xmlDocument.AppendChild(newChild);
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping("key", symmetricSecurityKey.GetSymmetricAlgorithm("http://www.w3.org/2001/04/xmlenc#tripledes-cbc"));
			encryptedXml.DecryptDocument();
			return xmlDocument.DocumentElement;
		}

		private const string KeyName = "key";
	}
}
