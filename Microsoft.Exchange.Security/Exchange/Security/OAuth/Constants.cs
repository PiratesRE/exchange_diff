using System;

namespace Microsoft.Exchange.Security.OAuth
{
	internal static class Constants
	{
		public static readonly OAuthApplication IdTokenApplication = new OAuthApplication(new V1ProfileAppInfo("ID_TOKEN_APP_2038A917-6EE8-4C7D-A755-E7AB95B87AE5", null, null));

		public static readonly string BearerAuthenticationType = "Bearer";

		public static readonly string BearerPreAuthenticationType = "BearerPreAuth";

		public static readonly string WWWAuthenticateHeader = "WWW-Authenticate";

		public static readonly string ConsumerMailboxIdentifier = "outlook.com";

		public static readonly string MSExchangeSelfIssuingTokenRealm = "00000002-0000-0ff1-ce00-200000000000";

		public static readonly string RequestCompletedHttpContextKeyName = "RequestCompleted";

		public static readonly string AzureADCommonEntityIdHint = "{tenantid}";

		public static class ChallengeTokens
		{
			public static readonly string Realm = "realm";

			public static readonly string ClientId = "client_id";

			public static readonly string TrustedIssuers = "trusted_issuers";

			public static readonly string AuthorizationUri = "authorization_uri";

			public static readonly string Error = "error";
		}

		public static class ClaimTypes
		{
			public static readonly string X509CertificateThumbprint = "x5t";

			public static readonly string NameIdentifier = "nameid";

			public static readonly string Nii = "nii";

			public static readonly string Smtp = "smtp";

			public static readonly string Sip = "sip";

			public static readonly string Upn = "upn";

			public static readonly string ActorToken = "actortoken";

			public static readonly string MsExchImmutableId = "msexchuid";

			public static readonly string MsExchCallback = "msexchcallback";

			public static readonly string MsExchProtocol = "msexchprot";

			public static readonly string MsExchangeDelegatedAuth = "msexchdauth";

			public static readonly string AppContext = "appctx";

			public static readonly string TrustedForDelegation = "trustedfordelegation";

			public static readonly string AppId = "appid";

			public static readonly string Scp = "scp";

			public static readonly string Roles = "roles";

			public static readonly string Tid = "tid";

			public static readonly string Ver = "ver";

			public static readonly string Oid = "oid";

			public static readonly string PrimarySid = "primarysid";

			public static readonly string OnPremSid = "onprem_sid";

			public static readonly string AppCtxSender = "appctxsender";

			public static readonly string Scope = "scope";

			public static readonly string IsBrowserHostedApp = "isbrowserhostedapp";

			public static readonly string AuthMetaDocumentUrl = "amurl";

			public static readonly string Version = "version";

			public static readonly string AlternateSecurityId = "altsecid";

			public static readonly string Audience = "aud";

			public static readonly string Issuer = "iss";

			public static readonly string Expiry = "exp";

			public static readonly string IdentityProvider = "idp";

			public static readonly string Puid = "puid";

			public static readonly string EmailAddress = "email";
		}

		public static class ClaimValues
		{
			public static readonly string Version1 = "1.0";

			public static readonly string ExchangeSelfIssuedVersion1 = "MSExchange.SelfIssued.V1";

			public static readonly string UserImpersonation = "user_impersonation";

			public static readonly string FullAccess = "full_access";

			public static readonly string MsExtensionV1 = "MsExtension.V1";

			public static readonly string MsOabDownloadV1 = "MsOabDownload.V1";

			public static readonly string ExIdTokV1 = "ExIdTok.V1";

			public static readonly string ExCallbackV1 = "ExCallback.V1";

			public static readonly string ProtocolOwa = "owa";

			public static readonly string ProtocolEws = "ews";

			public static readonly string ProtocolOab = "oab";
		}

		public static class NiiClaimValues
		{
			public static readonly string ActiveDirectory = "urn:office:idp:activedirectory";

			public static readonly string BusinessLiveId = "urn:federation:MicrosoftOnline";

			public static readonly string LegacyBusinessLiveId = "urn:office:idp:orgid";
		}

		public static class TokenCategories
		{
			public static readonly string S2SAppActAsToken = "S2SAppActAs";

			public static readonly string S2SAppOnlyToken = "S2SAppOnly";

			public static readonly string CallbackToken = "Callback";

			public static readonly string V1AppActAsToken = "V1AppActAs";

			public static readonly string V1AppOnlyToken = "V1AppOnly";

			public static readonly string V1IdToken = "V1IdToken";

			public static readonly string V1ExchangeSelfIssuedToken = "V1ExchangeSelfIssued";

			public static readonly string V1CallbackToken = "V1Callback";

			public static readonly string[] All = new string[]
			{
				Constants.TokenCategories.S2SAppActAsToken,
				Constants.TokenCategories.S2SAppOnlyToken,
				Constants.TokenCategories.CallbackToken,
				Constants.TokenCategories.V1AppActAsToken,
				Constants.TokenCategories.V1AppOnlyToken,
				Constants.TokenCategories.V1IdToken,
				Constants.TokenCategories.V1ExchangeSelfIssuedToken,
				Constants.TokenCategories.V1CallbackToken
			};
		}
	}
}
