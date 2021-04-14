using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Management.RightsManagement
{
	internal class TrustedPublishingDomainParser
	{
		public static TrustedDocDomain Parse(SecureString password, byte[] rawData)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			if (rawData == null || rawData.Length == 0)
			{
				throw new ArgumentNullException("rawData");
			}
			EncryptedTrustedDocDomain encryptedTrustedDocDomain = null;
			using (MemoryStream memoryStream = new MemoryStream(rawData))
			{
				try
				{
					SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(ArrayList), new Type[]
					{
						typeof(EncryptedTrustedDocDomain[])
					});
					ArrayList arrayList = safeXmlSerializer.Deserialize(memoryStream) as ArrayList;
					if (arrayList == null || arrayList.Count != 1)
					{
						throw new TrustedPublishingDomainParser.ParseFailedException("EncryptedTPD_NoOrMoreThanOneElementsIn");
					}
					encryptedTrustedDocDomain = (arrayList[0] as EncryptedTrustedDocDomain);
					if (encryptedTrustedDocDomain == null)
					{
						throw new TrustedPublishingDomainParser.ParseFailedException("EncryptedTPD_NotOfType_EncryptedTrustedDocDomain");
					}
				}
				catch (InvalidOperationException innerException)
				{
					throw new TrustedPublishingDomainParser.ParseFailedException("EncryptedTPD_XmlDeserializationFailed", innerException);
				}
				catch (XmlException innerException2)
				{
					throw new TrustedPublishingDomainParser.ParseFailedException("EncryptedTPD_XmlDeserializationFailed", innerException2);
				}
			}
			if (string.IsNullOrEmpty(encryptedTrustedDocDomain.m_strTrustedDocDomainInfo))
			{
				throw new TrustedPublishingDomainParser.ParseFailedException("EncryptedTPD_NoTrustedDocDomainInfo");
			}
			return TrustedPublishingDomainParser.DecryptTrustedDocDomainData(encryptedTrustedDocDomain, password);
		}

		private static TrustedDocDomain DecryptTrustedDocDomainData(EncryptedTrustedDocDomain encryptedData, SecureString password)
		{
			IPrivateKeyDecryptor privateKeyDecryptor = new OnPremisePrivateKeyDecryptor(password);
			byte[] buffer = null;
			try
			{
				buffer = privateKeyDecryptor.Decrypt(encryptedData.m_strTrustedDocDomainInfo);
			}
			catch (PrivateKeyDecryptionFailedException innerException)
			{
				throw new TrustedPublishingDomainParser.ParseFailedException("OnPremisePrivateKeyDecryptor.Decrypt() failed", innerException);
			}
			TrustedDocDomain result;
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				try
				{
					SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(ArrayList), new Type[]
					{
						typeof(TrustedDocDomain[])
					});
					ArrayList arrayList = safeXmlSerializer.Deserialize(memoryStream) as ArrayList;
					if (arrayList == null || arrayList.Count != 1)
					{
						throw new TrustedPublishingDomainParser.ParseFailedException("DecryptedTPD_NoORMoreThanOneElements");
					}
					TrustedDocDomain trustedDocDomain = arrayList[0] as TrustedDocDomain;
					if (trustedDocDomain == null)
					{
						throw new TrustedPublishingDomainParser.ParseFailedException("DecryptedTPD_NotOfType_TrustedDocDomain");
					}
					trustedDocDomain.m_ttdki.strEncryptedPrivateKey = encryptedData.m_strKeyData;
					result = trustedDocDomain;
				}
				catch (InvalidOperationException innerException2)
				{
					throw new TrustedPublishingDomainParser.ParseFailedException("DecryptedTPD_XmlDeserializationFailed", innerException2);
				}
				catch (XmlException innerException3)
				{
					throw new TrustedPublishingDomainParser.ParseFailedException("DecryptedTPD_XmlDeserializationFailed", innerException3);
				}
			}
			return result;
		}

		[Serializable]
		public class ParseFailedException : Exception
		{
			public ParseFailedException(string message) : base(message)
			{
			}

			public ParseFailedException(string message, Exception innerException) : base(message, innerException)
			{
			}

			protected ParseFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}
	}
}
