using System;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public interface ICryptoProvider
	{
		CryptographicKey GetKeyByPayload(CryptoKeyPayloadType payload);

		CryptographicKey GetKeyByVersion(CryptoKeyPayloadType payload, byte version);
	}
}
