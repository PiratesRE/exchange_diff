using System;
using System.Security;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public abstract class CryptographicKey
	{
		public CryptographicKey(CryptoKeyPayloadType payload, byte version, CryptoAlgorithm algorithm, DateTime activeDate, DateTime expireDate, SecureString encryptedKey = null)
		{
			this.Payload = payload;
			this.Version = version;
			this.Algorithm = algorithm;
			this.ActiveDate = activeDate;
			this.ExpireDate = expireDate;
			this.EncryptedKey = encryptedKey;
		}

		public byte Version { get; private set; }

		public SecureString EncryptedKey { get; set; }

		public CryptoKeyPayloadType Payload { get; private set; }

		public CryptoAlgorithm Algorithm { get; private set; }

		public DateTime ActiveDate { get; private set; }

		public DateTime ExpireDate { get; private set; }

		public byte[] Key
		{
			get
			{
				return Convert.FromBase64String(TextUtil.ConvertToUnsecureString(this.EncryptedKey));
			}
		}
	}
}
