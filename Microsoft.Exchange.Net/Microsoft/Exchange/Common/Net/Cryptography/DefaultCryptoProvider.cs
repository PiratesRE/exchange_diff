using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public class DefaultCryptoProvider : ICryptoProvider
	{
		private DefaultCryptoProvider()
		{
			DefaultCryptoProvider.keys = new Dictionary<CryptoKeyPayloadType, List<CryptographicKey>>();
			DefaultCryptoProvider.LoadKeys();
		}

		public static DefaultCryptoProvider Provider
		{
			get
			{
				if (DefaultCryptoProvider.provider == null)
				{
					lock (DefaultCryptoProvider.locker)
					{
						if (DefaultCryptoProvider.provider == null)
						{
							DefaultCryptoProvider.provider = new DefaultCryptoProvider();
						}
					}
				}
				return DefaultCryptoProvider.provider;
			}
		}

		public CryptographicKey GetKeyByPayload(CryptoKeyPayloadType payload)
		{
			if (!DefaultCryptoProvider.DoesCryptoKeyExist(payload))
			{
				throw new CryptographicException("Payload: {0} doesn't exist in key store", payload.ToString());
			}
			return DefaultCryptoProvider.keys[payload][0];
		}

		public CryptographicKey GetKeyByVersion(CryptoKeyPayloadType payload, byte version)
		{
			if (!DefaultCryptoProvider.DoesCryptoKeyExist(payload))
			{
				throw new CryptographicException("Payload: {0} doesn't exist in key store", payload.ToString());
			}
			foreach (CryptographicKey cryptographicKey in DefaultCryptoProvider.keys[payload])
			{
				if (cryptographicKey.Version.Equals(version))
				{
					return cryptographicKey;
				}
			}
			throw new CryptographicException(string.Concat(new object[]
			{
				"Couldn't find key with payload: ",
				payload,
				" specific version: ",
				version
			}));
		}

		private static bool DoesCryptoKeyExist(CryptoKeyPayloadType payload)
		{
			return DefaultCryptoProvider.keys.ContainsKey(payload) && DefaultCryptoProvider.keys[payload] != null && DefaultCryptoProvider.keys[payload].Count > 0;
		}

		private static void LoadKeys()
		{
			string unsecureString = "AAQAAAB0ZXN0BQAAAGhlbGxveHh4eHh4eHh4eHh4eHh4eHh4eAYQERITFBW8lb12KNSqJLUhqv/aK+1RPdzJog==";
			string unsecureString2 = "AAQAAAB0ZRF0BQAAAGhlbGxveBh0Q3h4eHioeHh4eHh4eHh4eAYQERITFBW8lb12KNSqJLUhqv/aK+1RPdzJog==";
			string unsecureString3 = "AAQAAAB0ZXN0BQAAAGhlbGxveHh4eHh4eHh4eHh4eHh4eHh4eAYQERITFBW8lb12KNSqJLUhqv/aK+1RPdzJog==";
			string unsecureString4 = "AAQgAAB0ZXN0BQQAAGjVbGxvdXh4eHh4EHgSeHh4eHh4eFh4eAYQuxITFBW8lb12KNSqJLUhqh/aK+1RPdzJog==";
			string unsecureString5 = "e8I+E4D+basbn5JtSCfdM1IQD6RmxFsITp9He3b9dXQ=";
			string unsecureString6 = "JoaeQ5OpBIFUQ/pqsuuLDkhxeoTS0vH8cPlnL2yXB60=";
			string unsecureString7 = "wu3C0tVdHO5crnt8tD3n4sOoiFxUJu7/aUNNRI0CrSY=";
			string initializationVector = "6/VimsoHVgpKbCtKfr6KgQ==";
			CryptoAlgorithm cryptoAlgorithm = new CryptoAlgorithm(1, "HMACSHA256");
			CryptoAlgorithm algorithm = new CryptoAlgorithm(2, "AES");
			CryptoAlgorithm algorithm2 = new CryptoAlgorithm(3, "Rijndael");
			CryptoAlgorithm cryptoAlgorithm2 = new CryptoAlgorithm(4, "SHA256");
			CryptoAlgorithm cryptoAlgorithm3 = new CryptoAlgorithm(5, "AesManaged");
			CryptoAlgorithm.PreferredHashAlgorithm = cryptoAlgorithm2.Name;
			CryptoAlgorithm.PreferredKeyedHashAlgorithm = cryptoAlgorithm.Name;
			CryptoAlgorithm.PreferredSymmetricAlgorithm = cryptoAlgorithm3.Name;
			CryptographicKey item = new HMACKey(CryptoKeyPayloadType.SafeRedirectHash, 1, cryptoAlgorithm, DateTime.MinValue, DateTime.MaxValue, TextUtil.ConvertToSecureString(unsecureString));
			CryptographicKey item2 = new HMACKey(CryptoKeyPayloadType.Canary, 1, cryptoAlgorithm, DateTime.MinValue, DateTime.MaxValue, TextUtil.ConvertToSecureString(unsecureString2));
			CryptographicKey item3 = new HMACKey(CryptoKeyPayloadType.SendAddressVerificationMailRequest, 1, cryptoAlgorithm, DateTime.MinValue, DateTime.MaxValue, TextUtil.ConvertToSecureString(unsecureString3));
			CryptographicKey item4 = new HMACKey(CryptoKeyPayloadType.ImportAccountHelper, 1, cryptoAlgorithm, DateTime.MinValue, DateTime.MaxValue, TextUtil.ConvertToSecureString(unsecureString4));
			CryptographicKey item5 = new HMACKey(CryptoKeyPayloadType.SvmFeedbackHash, 1, cryptoAlgorithm, DateTime.MinValue, DateTime.MaxValue, TextUtil.ConvertToSecureString(unsecureString5));
			CryptographicKey item6 = new SymmetricEncryptionKey(CryptoKeyPayloadType.SvmFeedbackEncryption, 1, algorithm, DateTime.MinValue, DateTime.MaxValue, TextUtil.ConvertToSecureString(unsecureString6), null);
			CryptographicKey item7 = new SymmetricEncryptionKey(CryptoKeyPayloadType.CookieCrypto, 1, algorithm2, DateTime.MinValue, DateTime.MaxValue, TextUtil.ConvertToSecureString(unsecureString7), initializationVector);
			DefaultCryptoProvider.keys.Add(CryptoKeyPayloadType.SafeRedirectHash, new List<CryptographicKey>
			{
				item
			});
			DefaultCryptoProvider.keys.Add(CryptoKeyPayloadType.Canary, new List<CryptographicKey>
			{
				item2
			});
			DefaultCryptoProvider.keys.Add(CryptoKeyPayloadType.SendAddressVerificationMailRequest, new List<CryptographicKey>
			{
				item3
			});
			DefaultCryptoProvider.keys.Add(CryptoKeyPayloadType.ImportAccountHelper, new List<CryptographicKey>
			{
				item4
			});
			DefaultCryptoProvider.keys.Add(CryptoKeyPayloadType.SvmFeedbackHash, new List<CryptographicKey>
			{
				item5
			});
			DefaultCryptoProvider.keys.Add(CryptoKeyPayloadType.SvmFeedbackEncryption, new List<CryptographicKey>
			{
				item6
			});
			DefaultCryptoProvider.keys.Add(CryptoKeyPayloadType.CookieCrypto, new List<CryptographicKey>
			{
				item7
			});
		}

		private static readonly object locker = new object();

		private static Dictionary<CryptoKeyPayloadType, List<CryptographicKey>> keys;

		private static volatile DefaultCryptoProvider provider;
	}
}
