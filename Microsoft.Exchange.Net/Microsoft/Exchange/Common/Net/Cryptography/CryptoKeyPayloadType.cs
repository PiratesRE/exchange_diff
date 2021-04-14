using System;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public enum CryptoKeyPayloadType
	{
		Canary,
		SafeRedirectHash,
		SendAddressVerificationMailRequest,
		ImportAccountHelper,
		SvmFeedbackHash,
		SvmFeedbackEncryption,
		CookieCrypto,
		FacebookApi
	}
}
