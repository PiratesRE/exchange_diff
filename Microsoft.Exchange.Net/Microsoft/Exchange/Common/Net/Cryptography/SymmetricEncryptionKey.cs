using System;
using System.Security;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public class SymmetricEncryptionKey : CryptographicKey
	{
		public SymmetricEncryptionKey(CryptoKeyPayloadType payload, byte version, CryptoAlgorithm algorithm, DateTime activeDate, DateTime expireDate, SecureString encryptedKey = null, string initializationVector = null) : base(payload, version, algorithm, activeDate, expireDate, encryptedKey)
		{
			this.InitializationVector = initializationVector;
		}

		public string InitializationVector { get; set; }
	}
}
