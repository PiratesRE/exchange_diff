using System;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public static class CryptoKeyStore
	{
		public static void Initialize(ICryptoProvider provider)
		{
			CryptoKeyStore.provider = provider;
		}

		public static CryptographicKey GetKeyByPayload(CryptoKeyPayloadType payload)
		{
			return CryptoKeyStore.provider.GetKeyByPayload(payload);
		}

		public static CryptographicKey GetKeyByVersion(CryptoKeyPayloadType payload, byte version)
		{
			return CryptoKeyStore.provider.GetKeyByVersion(payload, version);
		}

		public static bool IsValidKey(CryptographicKey key)
		{
			return false;
		}

		private static ICryptoProvider provider = DefaultCryptoProvider.Provider;
	}
}
