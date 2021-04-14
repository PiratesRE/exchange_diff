using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Security.OAuth
{
	internal enum OAuthErrors
	{
		[Description("No Error")]
		NoError,
		[Description("The token has an invalid signature.")]
		InvalidSignature = 1001,
		[Description("The user context must be unsigned.")]
		OuterTokenAlsoSigned = 2001,
		[Description("The inner actor token must be signed.")]
		ActorTokenMustBeSigned,
		[Description("The token is missing the issuer.")]
		MissingIssuer,
		[Description("The callback token is missing one or more expected claim types.")]
		CallbackClaimNotFound,
		[Description("The token is missing the claim type '{0}'.")]
		NoClaimFound,
		[Description("The issuer claim value is invalid '{0}'.")]
		InvalidIssuerFormat,
		[Description("The outer token issuer claim value is invalid '{0}'")]
		InvalidOuterTokenIssuerFormat,
		[Description("The issuer of the token is unknown. Issuer was '{0}'.")]
		NoConfiguredIssuerMatched,
		[Description("The nameid claim value is invalid '{0}'.")]
		InvalidNameIdFormat,
		[Description("The audience claim value is invalid '{0}'.")]
		InvalidAudience,
		[Description("The audience contains a realm that is different than the issuer's realm.")]
		MismatchedRealmBetweenAudienceAndIssuer,
		[Description("The callback token contains one or more invalid claim values.")]
		InvalidCallbackClaimValue,
		[Description("The appctx claim type in the token is invalid '{0}'.")]
		ExtensionInvalidAppCtxFormat,
		[Description("The callback token contains an invalid issuer '{0}'.")]
		InvalidCallbackTokenIssuer,
		[Description("The user-context issuer '{1}' does not match the application-context nameid '{0}'")]
		InvalidOuterTokenIssuerIdValue,
		[Description("The outer token issuer realm '{0}' is invalid.")]
		InvalidRealmFromOuterTokenIssuer,
		[Description("The realm in the nameid claim value is invalid.  Expected '{0}'. Actual '{1}'.")]
		UnexpectedRealmInNameId,
		[Description("No applicable user context claims found.")]
		NoUserClaimsFound,
		[Description("Invalid SID value '{0}' in primary SID claim type")]
		InvalidSidValue,
		[Description("This token profile is not applicable for the current protocol.")]
		TokenProfileNotApplicable,
		[Description("The token has invalid value '{0}' for the claim type '{1}'.")]
		InvalidClaimValueFound,
		[Description("Unable to read or process token, additional details: '{0}'.")]
		UnableToReadToken,
		[Description("Token for app '{0}' does not have smtp or puid claim.")]
		NoSmtpOrPuidClaimFound,
		[Description("The token with version '{0}' should have valid scope claim or linked account associated with partner application '{1}'.")]
		NoAuthorizationValuePresent,
		[Description("The token has expired.")]
		TokenExpired = 3001,
		[Description("The audience in the token does not specify a realm.")]
		EmptyRealmFromAudience = 4001,
		[Description("The hostname component of the audience claim value is invalid. Expected '{0}'. Actual '{1}'.")]
		UnexpectedHostNameInAudience,
		[Description("The audience of the token '{0}' doesn't match the endpoint '{1}' that received the request.")]
		WrongAudience,
		[Description("The tenant for context-id '{0}' does not exist.")]
		ExternalOrgIdNotFound = 5001,
		[Description("The tenant for realm '{0}' does not exist.")]
		OrganizationIdNotFoundFromRealm,
		[Description("The tid claim is missing.")]
		MissingTenantIdClaim,
		[Description("The tid claim should not be set for Consumer mailbox tokens.")]
		TenantIdClaimShouldNotBeSet,
		[Description("The user specified by the user-context in the token does not exist.")]
		NoUserFoundWithGivenClaims = 6001,
		[Description("The user specified by the user-context in the token is ambiguous.")]
		MoreThan1UserFoundWithGivenClaims,
		[Description("The MasterAccountSid doesn't match the SID claim.")]
		NameIdNotMatchMasterAccountSid,
		[Description("The user's mailbox version is not supported for access using OAuth.")]
		UserOAuthNotSupported,
		[Description("PUID in the nameid claim was not from BusinessLiveID")]
		NameIdNotMatchLiveIDInstanceType,
		[Description("test only: ExceptionDuringProxyDownLevelCheckNullSid")]
		TestOnlyExceptionDuringProxyDownLevelCheckNullSid,
		[Description("test only: ExceptionDuringRehydration")]
		TestOnlyExceptionDuringRehydration,
		[Description("test only: ExceptionDuringOAuthCATGeneration")]
		TestOnlyExceptionDuringOAuthCATGeneration,
		[Description("The Application Identifier '{0}' is unknown.")]
		NoMatchingPartnerAppFound = 7001,
		[Description("The application identifier in the nameid claim value is invalid.  Expected '{0}'. Actual '{1}'.")]
		UnexpectedAppIdInNameId,
		[Description("The Active Directory operation failed.")]
		ADOperationFailed = 8001,
		[Description("An unexpected error occurred.")]
		UnexpectedErrorOccurred,
		[Description("The callback token's protocol claim value '{0}' doesn't match the current requested protocol.")]
		InvalidCallbackTokenScope = 9001,
		[Description("The token contains no scope information, or scope can not be understood.")]
		NoGrantPresented = 9004,
		[Description("The call you are trying to access is not supported with OAuth token.")]
		NotSupportedWithV1AppToken,
		[Description("The token contains not enough scope to make this call.")]
		NotEnoughGrantPresented,
		[Description("The call should access the mailbox specified in the oauth token.")]
		AllowAccessOwnMailboxOnly,
		[Description("The PUID value was not found for [{0}] identity.")]
		NoPuidFound,
		[Description("The email address was not found for [{0}] identity, PUID={1}.")]
		NoEmailAddressFound,
		[Description("The certificate referenced by token with key {0} could not be located on server {1}.")]
		NoCertificateFound = 10001,
		[Description("Office Shared error codes")]
		OfficeSharedErrorCodes = 4000000,
		[Description("Flighting is not enabled for domain {0}.")]
		FlightingNotEnabled = 4001001
	}
}
