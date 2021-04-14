using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RmsOnlinePrivateKeyDecryptor : IPrivateKeyDecryptor
	{
		public byte[] Decrypt(string encryptedData)
		{
			RmsUtil.ThrowIfParameterNull(encryptedData, "encryptedData");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(encryptedData, "encryptedData");
			byte[] result;
			try
			{
				result = this.DecryptTenantsPrivateKey(encryptedData);
			}
			catch (CryptographicException ex)
			{
				string ski;
				Exception ex2;
				if (RmsUtil.TryExtractDecryptionCertificateSKIFromEncryptedXml(encryptedData, out ski, out ex2))
				{
					throw new PrivateKeyDecryptionFailedException(ex.Message + " " + Strings.RequiredDecryptionCertificate(ski), ex);
				}
				throw new PrivateKeyDecryptionFailedException(ex2.Message, ex2);
			}
			return result;
		}

		protected virtual XmlDocument DecryptKeyBlobXml(string encryptedXmlString)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(encryptedXmlString);
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.DecryptDocument();
			this.ThrowIfKeyBlobXmlFailsSchemaValidation(xmlDocument);
			return xmlDocument;
		}

		private byte[] DecryptTenantsPrivateKey(string encryptedData)
		{
			XmlDocument xmlDocument = this.DecryptKeyBlobXml(encryptedData);
			XmlElement xmlElement = xmlDocument.GetElementsByTagName("KeyBlob")[0] as XmlElement;
			byte[] result;
			try
			{
				result = Convert.FromBase64String(xmlElement.InnerText);
			}
			catch (FormatException innerException)
			{
				throw new PrivateKeyDecryptionFailedException("Decrypted private key XML KeyBlob element contains bad Base64 characters", innerException);
			}
			return result;
		}

		private void ThrowIfKeyBlobXmlFailsSchemaValidation(XmlDocument xmlDocument)
		{
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KeyBlobSchema.xsd"))
			{
				if (manifestResourceStream == null)
				{
					throw new PrivateKeyDecryptionFailedException("Unable to load XML schema KeyBlobSchema.xsd", null);
				}
				xmlDocument.Schemas.Add(null, XmlReader.Create(manifestResourceStream));
				xmlDocument.Validate(delegate(object _, ValidationEventArgs eventArgs)
				{
					throw new PrivateKeyDecryptionFailedException("Decrypted private key XML failed schema validation", eventArgs.Exception);
				});
			}
		}
	}
}
