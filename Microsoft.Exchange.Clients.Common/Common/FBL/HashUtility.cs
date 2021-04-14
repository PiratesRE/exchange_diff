using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Common.Net.Cryptography;

namespace Microsoft.Exchange.Clients.Common.FBL
{
	internal class HashUtility
	{
		public static byte[] ComputeHash(string[] hashComponents, CryptoKeyPayloadType payloadKey, byte version)
		{
			HMAC hmacforCryptoKey = HashUtility.GetHMACForCryptoKey(payloadKey, version);
			byte[] bytes = Encoding.UTF8.GetBytes(string.Join(string.Empty, hashComponents));
			return hmacforCryptoKey.ComputeHash(bytes);
		}

		public static byte[] ComputeHash(string[] hashComponents, CryptoKeyPayloadType payloadKey)
		{
			return HashUtility.ComputeHash(hashComponents, payloadKey, 1);
		}

		private static HMAC GetHMACForCryptoKey(CryptoKeyPayloadType payloadKey, byte version)
		{
			HMAC hmac = null;
			CryptographicKey keyByPayload = CryptoKeyStore.GetKeyByPayload(payloadKey);
			if (version == 0)
			{
				hmac = HMAC.Create("HMACSHA1");
				hmac.Key = keyByPayload.Key;
			}
			else if (version == 1)
			{
				hmac = HMAC.Create(keyByPayload.Algorithm.Name);
				hmac.Key = keyByPayload.Key;
			}
			return hmac;
		}

		public const byte CurrentVersion = 1;

		private const byte Sha1Version = 0;

		private const byte Sha256Version = 1;
	}
}
