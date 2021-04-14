using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class TokenDecryption
	{
		public TokenDecryption(IEnumerable<X509Certificate2> trustedTokenIssuerCertificates, IEnumerable<X509Certificate2> tokenDecryptionCertificates)
		{
			this.trustedTokenIssuerCertificates = trustedTokenIssuerCertificates;
			this.tokenDecryptionCertificates = tokenDecryptionCertificates;
		}

		public SamlSecurityToken DecryptToken(XmlElement encryptedToken)
		{
			byte[] array = this.DecryptTokenInternal(encryptedToken);
			if (array == null)
			{
				TokenDecryption.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to decrypt encrypted XML token: {0}", encryptedToken.OuterXml);
				throw new TokenDecryptionException();
			}
			TokenDecryption.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Decrypted token content : {0}", Encoding.ASCII.GetString(array));
			return this.GetSecurityToken(array);
		}

		private SamlSecurityToken GetSecurityToken(byte[] decryptedTokenBytes)
		{
			List<SecurityToken> list = new List<SecurityToken>();
			TokenDecryption.AddToSecurityTokenList(list, this.tokenDecryptionCertificates);
			TokenDecryption.AddToSecurityTokenList(list, this.trustedTokenIssuerCertificates);
			SecurityTokenResolver tokenResolver = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(list.AsReadOnly(), true);
			SamlSecurityToken result;
			using (MemoryStream memoryStream = new MemoryStream(decryptedTokenBytes))
			{
				using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(memoryStream))
				{
					xmlTextReader.MoveToContent();
					try
					{
						result = (SamlSecurityToken)WSSecurityTokenSerializer.DefaultInstance.ReadToken(xmlTextReader, tokenResolver);
					}
					catch (SecurityTokenException arg)
					{
						TokenDecryption.Tracer.TraceError<SecurityTokenException>((long)this.GetHashCode(), "Unable to read token due exception {0}", arg);
						throw new TokenDecryptionException();
					}
				}
			}
			return result;
		}

		private static void AddToSecurityTokenList(List<SecurityToken> securityTokenList, IEnumerable<X509Certificate2> certificates)
		{
			foreach (X509Certificate2 certificate in certificates)
			{
				securityTokenList.Add(new X509SecurityToken(certificate));
			}
		}

		private byte[] DecryptTokenInternal(XmlElement encryptedToken)
		{
			string subjectKeyIdentifier = this.GetSubjectKeyIdentifier(encryptedToken);
			if (subjectKeyIdentifier == null)
			{
				TokenDecryption.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to find key identifier in encrypted XML token: {0}", encryptedToken.OuterXml);
				throw new TokenDecryptionException();
			}
			X509Certificate2 x509Certificate = this.FindBySubjectKeyIdentifier(subjectKeyIdentifier);
			if (x509Certificate == null)
			{
				TokenDecryption.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to find certificate based on Subject Key Identifier: {0}", subjectKeyIdentifier);
				throw new TokenDecryptionException();
			}
			EncryptedKey encryptedKey = null;
			EncryptedData encryptedData = new EncryptedData();
			encryptedData.LoadXml(encryptedToken);
			foreach (object obj in encryptedData.KeyInfo)
			{
				KeyInfoClause keyInfoClause = (KeyInfoClause)obj;
				KeyInfoEncryptedKey keyInfoEncryptedKey = keyInfoClause as KeyInfoEncryptedKey;
				if (keyInfoEncryptedKey != null)
				{
					keyInfoEncryptedKey.EncryptedKey.KeyInfo.AddClause(new KeyInfoName(subjectKeyIdentifier));
					encryptedKey = keyInfoEncryptedKey.EncryptedKey;
					break;
				}
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlNode newChild = xmlDocument.ImportNode(encryptedData.GetXml(), true);
			xmlDocument.AppendChild(newChild);
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping(subjectKeyIdentifier, x509Certificate.PrivateKey);
			encryptedXml.DecryptEncryptedKey(encryptedKey);
			encryptedXml.GetDecryptionIV(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
			SymmetricAlgorithm decryptionKey = encryptedXml.GetDecryptionKey(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
			return encryptedXml.DecryptData(encryptedData, decryptionKey);
		}

		private XmlElement GetRequiredChildElement(XmlElementDefinition xmlElementDefinition, XmlElement xmlElement)
		{
			XmlElement singleElementByName = xmlElementDefinition.GetSingleElementByName(xmlElement);
			if (singleElementByName == null)
			{
				throw new TokenDecryptionException();
			}
			return singleElementByName;
		}

		private string GetSubjectKeyIdentifier(XmlElement encryptedToken)
		{
			XmlElement requiredChildElement = this.GetRequiredChildElement(XmlDigitalSignature.KeyInfo, encryptedToken);
			XmlElement requiredChildElement2 = this.GetRequiredChildElement(XmlEncryption.EncryptedKey, requiredChildElement);
			XmlElement requiredChildElement3 = this.GetRequiredChildElement(XmlDigitalSignature.KeyInfo, requiredChildElement2);
			XmlElement requiredChildElement4 = this.GetRequiredChildElement(WSSecurityExtensions.SecurityTokenReference, requiredChildElement3);
			XmlElement requiredChildElement5 = this.GetRequiredChildElement(WSSecurityExtensions.KeyIdentifier, requiredChildElement4);
			return TokenDecryption.GetHex(Convert.FromBase64String(requiredChildElement5.InnerText));
		}

		private static string GetHex(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append(bytes[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		private X509Certificate2 FindBySubjectKeyIdentifier(string subjectKeyIdentifier)
		{
			foreach (X509Certificate2 x509Certificate in this.tokenDecryptionCertificates)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(subjectKeyIdentifier, TokenDecryption.GetSubjectKeyIdentifier(x509Certificate)))
				{
					return x509Certificate;
				}
			}
			return null;
		}

		private static string GetSubjectKeyIdentifier(X509Certificate2 certificate)
		{
			foreach (X509Extension x509Extension in certificate.Extensions)
			{
				X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension = x509Extension as X509SubjectKeyIdentifierExtension;
				if (x509SubjectKeyIdentifierExtension != null)
				{
					return x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier;
				}
			}
			return null;
		}

		private IEnumerable<X509Certificate2> trustedTokenIssuerCertificates;

		private IEnumerable<X509Certificate2> tokenDecryptionCertificates;

		private static readonly Trace Tracer = ExTraceGlobals.WSTrustTracer;
	}
}
