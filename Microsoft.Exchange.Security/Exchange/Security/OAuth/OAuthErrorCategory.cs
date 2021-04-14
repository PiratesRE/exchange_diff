using System;

namespace Microsoft.Exchange.Security.OAuth
{
	internal enum OAuthErrorCategory
	{
		Probe,
		InvalidSignature = 2000000,
		InvalidToken,
		TokenExpired,
		InvalidResource,
		InvalidTenant,
		InvalidUser,
		InvalidClient,
		InternalError,
		InvalidGrant,
		InvalidCertificate,
		OAuthNotAvailable = 4000000
	}
}
