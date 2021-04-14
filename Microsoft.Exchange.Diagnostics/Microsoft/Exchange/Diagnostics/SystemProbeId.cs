using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class SystemProbeId
	{
		internal static X509Certificate2 EncryptionCertificate
		{
			get
			{
				return SystemProbeId.testEncryptionCertificate;
			}
			set
			{
				SystemProbeId.testEncryptionCertificate = value;
			}
		}

		public static string EncryptProbeGuid(Guid guid, DateTime timestamp)
		{
			if (guid == Guid.Empty)
			{
				throw new ArgumentException("Guid to encrypt must not be Guid.Empty");
			}
			X509Certificate2 publicCertificate = SystemProbeId.GetPublicCertificate();
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			byte[] bytes = unicodeEncoding.GetBytes(string.Format("{0}|{1}", guid.ToString(), timestamp.Ticks.ToString()));
			RSACryptoServiceProvider rsacryptoServiceProvider = (RSACryptoServiceProvider)publicCertificate.PublicKey.Key;
			return Convert.ToBase64String(rsacryptoServiceProvider.Encrypt(bytes, false));
		}

		public static void DecryptProbeGuid(string cyphertext, out Guid guid, out DateTime timestamp)
		{
			X509Certificate2 privateCertificate = SystemProbeId.GetPrivateCertificate();
			RSACryptoServiceProvider rsacryptoServiceProvider = (RSACryptoServiceProvider)privateCertificate.PrivateKey;
			byte[] rgb;
			try
			{
				rgb = Convert.FromBase64String(cyphertext);
			}
			catch (ArgumentNullException innerException)
			{
				throw new SystemProbeException(SystemProbeStrings.NullEncryptedData, innerException);
			}
			catch (FormatException innerException2)
			{
				throw new SystemProbeException(SystemProbeStrings.EncryptedDataNotValidBase64String, innerException2);
			}
			string[] array = null;
			try
			{
				byte[] array2 = rsacryptoServiceProvider.Decrypt(rgb, false);
				if (array2 != null)
				{
					UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
					string @string = unicodeEncoding.GetString(array2);
					if (!string.IsNullOrEmpty(@string))
					{
						array = @string.Split("|".ToCharArray());
					}
				}
			}
			catch (CryptographicException innerException3)
			{
				throw new SystemProbeException(SystemProbeStrings.EncryptedDataCannotBeDecrypted, innerException3);
			}
			if (array == null || array.Length != 2)
			{
				throw new SystemProbeException(SystemProbeStrings.InvalidGuidInDecryptedText);
			}
			guid = Guid.Empty;
			timestamp = DateTime.MinValue;
			if (!Guid.TryParse(array[0], out guid))
			{
				throw new SystemProbeException(SystemProbeStrings.InvalidGuidInDecryptedText);
			}
			long ticks = 0L;
			if (!long.TryParse(array[1], out ticks))
			{
				throw new SystemProbeException(SystemProbeStrings.InvalidTimeInDecryptedText);
			}
			timestamp = new DateTime(ticks);
		}

		internal static X509Certificate2 GetPublicCertificate()
		{
			return SystemProbeId.GetCertificate(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
		}

		internal static X509Certificate2 GetPrivateCertificate()
		{
			return SystemProbeId.GetCertificate(StoreName.My, StoreLocation.LocalMachine);
		}

		private static X509Certificate2 GetCertificate(StoreName storeName, StoreLocation storeLocation)
		{
			X509Certificate2 x509Certificate = SystemProbeId.testEncryptionCertificate;
			if (x509Certificate == null)
			{
				X509Store x509Store = new X509Store(storeName, storeLocation);
				x509Store.Open(OpenFlags.ReadOnly);
				foreach (X509Certificate2 x509Certificate2 in x509Store.Certificates)
				{
					if (x509Certificate2.Subject == "CN=FfoSystemProbe")
					{
						x509Certificate = x509Certificate2;
						break;
					}
				}
				x509Store.Close();
			}
			if (x509Certificate == null)
			{
				throw new SystemProbeException(SystemProbeStrings.CertificateNotFound);
			}
			if (DateTime.UtcNow < x509Certificate.NotBefore || DateTime.UtcNow > x509Certificate.NotAfter)
			{
				throw new SystemProbeException(SystemProbeStrings.CertificateTimeNotValid(x509Certificate.GetEffectiveDateString(), x509Certificate.GetExpirationDateString()));
			}
			if (!SystemProbeId.IsSelfSigned(x509Certificate) && !x509Certificate.Verify())
			{
				throw new SystemProbeException(SystemProbeStrings.CertificateNotSigned);
			}
			return x509Certificate;
		}

		private static bool IsSelfSigned(X509Certificate cert)
		{
			return cert.Issuer == cert.Subject;
		}

		private static X509Certificate2 testEncryptionCertificate;
	}
}
