using System;
using System.Security;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public class HMACKey : CryptographicKey
	{
		public HMACKey(CryptoKeyPayloadType payload, byte version, CryptoAlgorithm algorithm, DateTime activeDate, DateTime expireDate, SecureString encryptedKey = null) : base(payload, version, algorithm, activeDate, expireDate, encryptedKey)
		{
		}
	}
}
