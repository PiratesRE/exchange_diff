using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal enum TokenValidationResult
	{
		Valid,
		InvalidUnknownExternalIdentity,
		InvalidUnknownEncryption,
		InvalidTokenFailedValidation,
		InvalidTokenFormat,
		InvalidTrustBroker,
		InvalidTarget,
		InvalidOffer,
		InvalidUnknownEmailAddress,
		InvalidExpired
	}
}
