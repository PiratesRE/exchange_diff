using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class LocalTokenIssuer
	{
		public LocalTokenIssuer(OrganizationId organizationId) : this(ConfigProvider.Instance.Configuration, OAuthConfigHelper.GetOrganizationRealm(organizationId))
		{
		}

		public LocalTokenIssuer(LocalConfiguration localConfiguration, string realm)
		{
			if (localConfiguration == null || localConfiguration.SigningKey == null)
			{
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.MissingSigningCertificate, null, null);
			}
			if (localConfiguration.SigningKey.PrivateKey.KeySize < 2048)
			{
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.CertificatePrivateKeySizeTooSmall, null, null);
			}
			this.signingCert = localConfiguration.SigningKey;
			this.issuerMetadata = new LocalTokenIssuerMetadata(localConfiguration.ApplicationId, realm);
		}

		internal LocalTokenIssuer(X509Certificate2 signingCert, LocalTokenIssuerMetadata issuerMetadata)
		{
			this.signingCert = signingCert;
			this.issuerMetadata = issuerMetadata;
		}

		public string Caller
		{
			get
			{
				return this.caller;
			}
			set
			{
				this.caller = value;
			}
		}

		public TokenResult GetAppToken(string audience)
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow.Add(LocalTokenIssuer.ActorTokenLifetime);
			X509SigningCredentials signingCredentials = this.GetSigningCredentials();
			JwtSecurityToken tokenObject = new JwtSecurityToken(this.issuerMetadata.GetIssuer(), audience, new Claim[]
			{
				new Claim(Constants.ClaimTypes.NameIdentifier, this.issuerMetadata.GetNameId()),
				new Claim(Constants.ClaimTypes.TrustedForDelegation, "true")
			}, new Lifetime(utcNow, dateTime), signingCredentials);
			TokenResult tokenResult = new TokenResult(tokenObject, dateTime);
			OutboundProtocolLog.BeginAppend("GetSelfIssuedAppToken", "ok", 0L, this.Caller, null, null, (audience != null && audience.IndexOf('@') >= 0) ? audience.Substring(audience.IndexOf('@') + 1) : null, audience, null, null, null, tokenResult.RemainingTokenLifeTime, tokenResult);
			return tokenResult;
		}

		public TokenResult GetTokenForACS(string tenantId, string acsAudience)
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow.Add(LocalTokenIssuer.ACSRequestTokenLifetime);
			X509SigningCredentials signingCredentials = this.GetSigningCredentials();
			JwtSecurityToken tokenObject = new JwtSecurityToken(this.issuerMetadata.GetIssuer(tenantId), acsAudience, new Claim[0], new Lifetime(utcNow, dateTime), signingCredentials);
			return new TokenResult(tokenObject, dateTime);
		}

		public TokenResult GetAppWithUserToken(TokenResult actor, string realmFromAuthServer, string audience, IDictionary<string, string> userClaims)
		{
			List<Claim> list = new List<Claim>(from c in userClaims
			select new Claim(c.Key, c.Value));
			list.Add(new Claim(Constants.ClaimTypes.ActorToken, actor.TokenString));
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow.Add(LocalTokenIssuer.OuterTokenLifetime);
			JwtSecurityToken tokenObject = new JwtSecurityToken(string.IsNullOrEmpty(realmFromAuthServer) ? this.issuerMetadata.GetIssuer() : this.issuerMetadata.GetIssuer(realmFromAuthServer), audience, list, new Lifetime(utcNow, dateTime), null);
			return new TokenResult(tokenObject, (dateTime < actor.ExpirationDate) ? dateTime : actor.ExpirationDate);
		}

		public TokenResult GetCallerIdentityToken(string extensionId, IDictionary<string, string> claims)
		{
			OAuthCommon.VerifyNonNullArgument("extensionId", extensionId);
			OAuthCommon.VerifyNonNullArgument("claims", claims);
			ExTraceGlobals.OAuthTracer.TraceFunction<string, int>((long)this.GetHashCode(), "[LocalTokenIssuer:GetCallerIdentityToken] Entering with extensionId: {0}, with {1} claims", extensionId, claims.Count);
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow.Add(LocalTokenIssuer.ExtensionIdentityTokenLifetime);
			X509SigningCredentials signingCredentials = this.GetSigningCredentials();
			Claim claim = this.ConvertToAppContextClaim(claims);
			JwtSecurityToken tokenObject = new JwtSecurityToken(this.issuerMetadata.GetIssuer(), extensionId, new Claim[]
			{
				new Claim(Constants.ClaimTypes.AppCtxSender, this.issuerMetadata.GetNameId()),
				new Claim(Constants.ClaimTypes.IsBrowserHostedApp, bool.TrueString),
				claim
			}, new Lifetime(utcNow, dateTime), signingCredentials);
			return new TokenResult(tokenObject, dateTime);
		}

		public TokenResult GetExtensionCallbackToken(string appId, string authIdForExtension, Uri requestUri, ADUser adUser, string scope = null)
		{
			OAuthCommon.VerifyNonNullArgument("appId", appId);
			OAuthCommon.VerifyNonNullArgument("authIdForExtension", authIdForExtension);
			OAuthCommon.VerifyNonNullArgument("requestUri", requestUri);
			OAuthCommon.VerifyNonNullArgument("adUser", adUser);
			Dictionary<string, string> dictionary = new Dictionary<string, string>(3);
			dictionary.Add(Constants.ClaimTypes.Smtp, adUser.PrimarySmtpAddress.ToString());
			if (!string.IsNullOrWhiteSpace(scope))
			{
				dictionary.Add(Constants.ClaimTypes.Scope, scope);
			}
			dictionary.Add(Constants.ClaimTypes.MsExchCallback, Constants.ClaimValues.MsExtensionV1);
			dictionary.Add(Constants.ClaimTypes.MsExchProtocol, Constants.ClaimValues.ProtocolEws);
			List<Claim> list = new List<Claim>(5);
			list.Add(new Claim(Constants.ClaimTypes.NameIdentifier, string.Format("{0}@{1}", appId, this.issuerMetadata.Realm)));
			ExTraceGlobals.OAuthTracer.TraceFunction<string, int>((long)this.GetHashCode(), "[LocalTokenIssuer:GetExtensionCallbackToken] Entering with authIdForExtension: {0}, with {1} claims", authIdForExtension, dictionary.Count);
			TimeSpan lifetime = string.IsNullOrWhiteSpace(scope) ? LocalTokenIssuer.ExtensionCallbackTokenLifetime : LocalTokenIssuer.ScopedTokenLifetime;
			string appCtxSenderString = string.IsNullOrWhiteSpace(scope) ? authIdForExtension : string.Format("{0}@{1}", authIdForExtension, this.issuerMetadata.Realm);
			return this.InternalGetCallbackToken(requestUri, list, dictionary, lifetime, appCtxSenderString);
		}

		public TokenResult GetWacCallbackToken(Uri requestUri, string primarySmtpAddress, string attachmentId)
		{
			OAuthCommon.VerifyNonNullArgument("requestUri", requestUri);
			OAuthCommon.VerifyNonNullArgument("primarySmtpAddress", primarySmtpAddress);
			OAuthCommon.VerifyNonNullArgument("attachmentId", attachmentId);
			ExTraceGlobals.OAuthTracer.TraceFunction((long)this.GetHashCode(), "[LocalTokenIssuer:GetWacCallbackToken] Entering.");
			Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
			dictionary.Add(Constants.ClaimTypes.Smtp, primarySmtpAddress);
			dictionary.Add(Constants.ClaimTypes.Scope, attachmentId);
			dictionary.Add(Constants.ClaimTypes.MsExchCallback, Constants.ClaimValues.MsExtensionV1);
			dictionary.Add(Constants.ClaimTypes.MsExchProtocol, Constants.ClaimValues.ProtocolOwa);
			string appCtxSenderString = string.Format("OwaWac@{0}", this.issuerMetadata.Realm);
			return this.InternalGetCallbackToken(requestUri, null, dictionary, LocalTokenIssuer.ActorTokenLifetime, appCtxSenderString);
		}

		public TokenResult GetOabDownloadToken(Uri requestUri, string upn)
		{
			OAuthCommon.VerifyNonNullArgument("requestUri", requestUri);
			OAuthCommon.VerifyNonNullArgument("upn", upn);
			Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
			dictionary.Add(Constants.ClaimTypes.MsExchCallback, Constants.ClaimValues.MsOabDownloadV1);
			dictionary.Add(Constants.ClaimTypes.MsExchProtocol, Constants.ClaimValues.ProtocolOab);
			dictionary.Add(Constants.ClaimTypes.Upn, upn);
			dictionary.Add(Constants.ClaimTypes.Nii, Constants.NiiClaimValues.ActiveDirectory);
			string appCtxSenderString = requestUri.ToString();
			return this.InternalGetCallbackToken(requestUri, null, dictionary, LocalTokenIssuer.OabDownloadTokenLifetime, appCtxSenderString);
		}

		public TokenResult GetSelfIssuedV1CallbackToken(Uri requestUri, JwtSecurityToken originalToken)
		{
			OAuthCommon.VerifyNonNullArgument("requestUri", requestUri);
			OAuthCommon.VerifyNonNullArgument("originalToken", originalToken);
			X509SigningCredentials signingCredentials = this.GetSigningCredentials();
			List<Claim> list = new List<Claim>();
			string protocolFromRequestUri = this.GetProtocolFromRequestUri(requestUri);
			list.Add(new Claim(Constants.ClaimTypes.Ver, Constants.ClaimValues.Version1));
			list.Add(new Claim(Constants.ClaimTypes.MsExchCallback, Constants.ClaimValues.ExCallbackV1));
			list.Add(new Claim(Constants.ClaimTypes.MsExchProtocol, protocolFromRequestUri));
			string value;
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.Oid, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.Oid, value));
			}
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.Tid, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.Tid, value));
			}
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.PrimarySid, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.PrimarySid, value));
			}
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.Upn, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.Upn, value));
			}
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.Puid, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.Puid, value));
			}
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.EmailAddress, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.EmailAddress, value));
			}
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.Scp, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.Scp, value));
			}
			if (OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.AppId, out value))
			{
				list.Add(new Claim(Constants.ClaimTypes.AppId, value));
			}
			string text = null;
			OAuthCommon.TryGetClaimValue(originalToken, Constants.ClaimTypes.Audience, out text);
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow.Add(LocalTokenIssuer.V1CallbackTokenLifetime);
			JwtSecurityToken tokenObject = new JwtSecurityToken(this.issuerMetadata.GetIssuer(), text, list, new Lifetime(utcNow, dateTime), signingCredentials);
			return new TokenResult(tokenObject, dateTime);
		}

		private string GetProtocolFromRequestUri(Uri requestUri)
		{
			string result = string.Empty;
			if (requestUri.Segments.Length > 1)
			{
				result = requestUri.Segments[1].TrimEnd(new char[]
				{
					'/'
				});
			}
			return result;
		}

		private TokenResult InternalGetCallbackToken(Uri requestUri, List<Claim> jwtClaims, Dictionary<string, string> claims, TimeSpan lifetime, string appCtxSenderString)
		{
			X509SigningCredentials signingCredentials = this.GetSigningCredentials();
			Claim item = this.ConvertToAppContextClaim(claims);
			string text = string.Format("{0}/{1}@{2}", this.issuerMetadata.Id, requestUri.Authority, this.issuerMetadata.Realm);
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow.Add(lifetime);
			if (jwtClaims == null)
			{
				jwtClaims = new List<Claim>();
			}
			jwtClaims.Add(new Claim(Constants.ClaimTypes.AppCtxSender, appCtxSenderString));
			jwtClaims.Add(item);
			JwtSecurityToken tokenObject = new JwtSecurityToken(this.issuerMetadata.GetIssuer(), text, jwtClaims, new Lifetime(utcNow, dateTime), signingCredentials);
			return new TokenResult(tokenObject, dateTime);
		}

		private Claim ConvertToAppContextClaim(IDictionary<string, string> claims)
		{
			return new Claim(Constants.ClaimTypes.AppContext, claims.SerializeToJson());
		}

		private X509SigningCredentials GetSigningCredentials()
		{
			return new X509SigningCredentials(this.signingCert, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", "http://www.w3.org/2001/04/xmlenc#sha256");
		}

		public LocalTokenIssuerMetadata IssuerMetadata
		{
			get
			{
				return this.issuerMetadata;
			}
		}

		public X509Certificate2 SigningCert
		{
			get
			{
				return this.signingCert;
			}
		}

		private static readonly TimeSpan OuterTokenLifetime = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan ActorTokenLifetime = TimeSpan.FromHours(24.0);

		private static readonly TimeSpan ACSRequestTokenLifetime = TimeSpan.FromMinutes(10.0);

		private static readonly TimeSpan ExtensionIdentityTokenLifetime = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan ExtensionCallbackTokenLifetime = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan OabDownloadTokenLifetime = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan ScopedTokenLifetime = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan V1CallbackTokenLifetime = TimeSpan.FromHours(23.0);

		private readonly LocalTokenIssuerMetadata issuerMetadata;

		private readonly X509Certificate2 signingCert;

		private string caller;
	}
}
