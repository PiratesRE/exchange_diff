using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Security.OAuth
{
	public enum OAuthOutboundErrorCodes
	{
		[Description("No Error")]
		NoError,
		[Description("Unable to load OAuth configuration.")]
		OAuthConfigurationUnavailable,
		[Description("Missing signing certificate.")]
		MissingSigningCertificate,
		[Description("The signing certificate should have a private key with at least 2048 bits.")]
		CertificatePrivateKeySizeTooSmall,
		[Description("Unable to get token from Auth Server. Error code: '{0}'. Description: '{1}'.")]
		UnableToGetTokenFromACS,
		[Description("The client ID inside the challenge returned from '{0}' was empty.")]
		EmptyClientId,
		[Description("The challenge value returned from '{0}' is not valid.")]
		InvalidChallenge,
		[Description("Multiple Auth Servers have an empty realm configured.")]
		InvalidConfigurationMultipleAuthServerWithEmptyRealm,
		[Description("Multiple Auth Servers have same realm '{0}'.")]
		InvalidConfigurationMultipleAuthServerWithSameRealm,
		[Description("The UserPrincipalName property was missing on the mailbox object and couldn't be added to the user context in the token.")]
		UPNValueNotProvided,
		[Description("The trusted issuers contained the following entries '{0}'. None of them are configured locally.")]
		NoMatchedTokenIssuer,
		[Description("The matched Auth Server '{0}' has an empty realm.")]
		MissingRealmInAuthServer,
		[Description("Unable to get at least one valid claim-value for the user to build the user context.")]
		EmptyClaimsForUser,
		[Description("The specified url may not support OAuth.")]
		InvalidOAuthEndpoint,
		[Description("The claim nameidClaim is empty.")]
		EmptyNameIdClaim
	}
}
