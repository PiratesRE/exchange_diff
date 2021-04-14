using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	public static class CookieSignHelper
	{
		public static string GetSignedHashFromCookieItem(string compactToken, string keyItem, string puid, string expireTime, bool generatedByOfflineOrgId, out string error)
		{
			if (!CookieSignHelper.InitializeCertIfNecessary(out error))
			{
				error += "Certificate is being loaded.";
				return null;
			}
			X509Certificate2 x509Certificate = CookieSignHelper.certs[0];
			if (x509Certificate == null)
			{
				error += "No available certificate for signing the cookie.";
				return null;
			}
			RSACryptoServiceProvider rsacryptoServiceProvider = x509Certificate.PrivateKey as RSACryptoServiceProvider;
			if (rsacryptoServiceProvider != null)
			{
				error = null;
				try
				{
					byte[] bytes = Encoding.Default.GetBytes(string.Concat(new object[]
					{
						compactToken,
						keyItem,
						puid,
						expireTime,
						generatedByOfflineOrgId
					}));
					Stopwatch stopwatch = Stopwatch.StartNew();
					byte[] inArray = rsacryptoServiceProvider.SignData(bytes, new SHA1CryptoServiceProvider());
					stopwatch.Stop();
					error = "GetSignedHashFromCookieItem" + stopwatch.Elapsed.Ticks.ToString();
					return Convert.ToBase64String(inArray);
				}
				catch (CryptographicException ex)
				{
					error = ex.ToString();
					return null;
				}
			}
			error = string.Format("{0} does not have a private Key.", x509Certificate.Thumbprint);
			return null;
		}

		public static bool VerifySignedHash(string compactToken, string keyItem, string puid, string expireTime, bool generatedByOfflineOrgId, string signedHash, out string error)
		{
			if (!CookieSignHelper.InitializeCertIfNecessary(out error))
			{
				error += "Certificate is being loaded.";
				return false;
			}
			bool result;
			try
			{
				byte[] bytes = Encoding.Default.GetBytes(string.Concat(new object[]
				{
					compactToken,
					keyItem,
					puid,
					expireTime,
					generatedByOfflineOrgId
				}));
				byte[] signature = Convert.FromBase64String(signedHash);
				bool flag = true;
				foreach (X509Certificate2 x509Certificate in CookieSignHelper.certs)
				{
					if (x509Certificate != null && x509Certificate.PublicKey != null)
					{
						RSACryptoServiceProvider rsacryptoServiceProvider = x509Certificate.PublicKey.Key as RSACryptoServiceProvider;
						if (rsacryptoServiceProvider != null)
						{
							flag = false;
							Stopwatch stopwatch = Stopwatch.StartNew();
							if (rsacryptoServiceProvider.VerifyData(bytes, new SHA1CryptoServiceProvider(), signature))
							{
								stopwatch.Stop();
								error = "VerifySignedHash" + stopwatch.Elapsed.Ticks.ToString();
								return true;
							}
							stopwatch.Stop();
						}
					}
				}
				if (flag)
				{
					error += "No available certificate for verifying the signed hash.";
				}
				result = false;
			}
			catch (FormatException ex)
			{
				error = ex.ToString();
				result = false;
			}
			return result;
		}

		private static bool InitializeCertIfNecessary(out string errorMsg)
		{
			errorMsg = null;
			if (CookieSignHelper.certs == null)
			{
				try
				{
					if (Monitor.TryEnter(CookieSignHelper.certLock) && CookieSignHelper.certs == null)
					{
						CookieSignHelper.certs = new X509Certificate2[2];
						try
						{
							CookieSignHelper.certs[0] = OAuthConfigHelper.GetCurrentSigningKey();
							CookieSignHelper.certs[1] = OAuthConfigHelper.GetPreviousSigningKey();
							return true;
						}
						catch (InvalidAuthConfigurationException ex)
						{
							errorMsg = ex.ToString();
						}
					}
					return false;
				}
				finally
				{
					if (Monitor.IsEntered(CookieSignHelper.certLock))
					{
						Monitor.Exit(CookieSignHelper.certLock);
					}
				}
				return true;
			}
			return true;
		}

		private static IList<X509Certificate2> certs;

		private static object certLock = new object();
	}
}
