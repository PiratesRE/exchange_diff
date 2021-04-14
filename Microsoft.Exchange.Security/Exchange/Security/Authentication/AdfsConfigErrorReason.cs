using System;

namespace Microsoft.Exchange.Security.Authentication
{
	public enum AdfsConfigErrorReason
	{
		DuplicateClaims,
		UpnClaimMissing,
		GroupSidsClaimMissing,
		InvalidUpn,
		NoCertificates,
		CertificatesMismatch
	}
}
