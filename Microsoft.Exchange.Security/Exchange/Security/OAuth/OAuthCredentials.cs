using System;
using System.Diagnostics;
using System.Net;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class OAuthCredentials : ICredentials
	{
		static OAuthCredentials()
		{
			ExTraceGlobals.OAuthTracer.TraceFunction(0L, "[OAuthCredentials.cctor] Entering");
			AuthenticationManager.Register(new OAuthCredentials.OAuthAuthenticationModule());
		}

		private OAuthCredentials(OrganizationId organizationId, string userDomain)
		{
			OAuthCommon.VerifyNonNullArgument("organizationId", organizationId);
			OAuthCommon.VerifyNonNullArgument("userDomain", userDomain);
			this.organizationId = organizationId;
			this.userDomain = userDomain;
		}

		private OAuthCredentials(OrganizationId organizationId, ADUser actAsUser, string userDomain = null)
		{
			OAuthCommon.VerifyNonNullArgument("organizationId", organizationId);
			OAuthCommon.VerifyNonNullArgument("actAsUser", actAsUser);
			ExTraceGlobals.OAuthTracer.TraceDebug<ADUser, string>(0L, "[OAuthCredentials:ctor] actAsUser is {0}, userDomain is {1}", actAsUser, userDomain);
			this.organizationId = organizationId;
			this.userDomain = (userDomain ?? ((SmtpAddress)actAsUser[ADRecipientSchema.PrimarySmtpAddress]).Domain);
			this.adUser = actAsUser;
		}

		private OAuthCredentials(OrganizationId organizationId, MiniRecipient actAsUser, string userDomain = null)
		{
			OAuthCommon.VerifyNonNullArgument("organizationId", organizationId);
			OAuthCommon.VerifyNonNullArgument("actAsUser", actAsUser);
			ExTraceGlobals.OAuthTracer.TraceDebug<MiniRecipient, string>(0L, "[OAuthCredentials:ctor] actAsUser is {0}, userDomain is {1}", actAsUser, userDomain);
			this.organizationId = organizationId;
			SmtpAddress primarySmtpAddress = actAsUser.PrimarySmtpAddress;
			this.userDomain = (userDomain ?? primarySmtpAddress.Domain);
			this.miniRecipient = actAsUser;
		}

		public static OAuthCredentials GetOAuthCredentialsForAppToken(OrganizationId organizationId, string userDomain)
		{
			return new OAuthCredentials(organizationId, userDomain);
		}

		public static OAuthCredentials GetOAuthCredentialsForAppActAsToken(OrganizationId organizationId, ADUser actAsUser, string userDomain = null)
		{
			return new OAuthCredentials(organizationId, actAsUser, userDomain);
		}

		public static OAuthCredentials GetOAuthCredentialsForAppActAsToken(OrganizationId organizationId, MiniRecipient actAsUser, string userDomain = null)
		{
			return new OAuthCredentials(organizationId, actAsUser, userDomain);
		}

		public LocalConfiguration LocalConfiguration
		{
			get
			{
				return this.localConfiguration;
			}
			set
			{
				OAuthCommon.VerifyNonNullArgument("localConfiguration", value);
				this.localConfiguration = value;
			}
		}

		public IOutboundTracer Tracer
		{
			get
			{
				return this.tracer;
			}
			set
			{
				OAuthCommon.VerifyNonNullArgument("tracer", value);
				this.tracer = value;
			}
		}

		public Guid? ClientRequestId
		{
			get
			{
				return this.clientRequestId;
			}
			set
			{
				this.clientRequestId = value;
			}
		}

		public bool IncludeNameIdOnly
		{
			get
			{
				return this.includeNameIdOnly != null && this.includeNameIdOnly.Value;
			}
			set
			{
				if (this.includeNameIdOnly != null)
				{
					throw new InvalidOperationException("can not modify the value once set");
				}
				this.includeNameIdOnly = new bool?(value);
			}
		}

		public string UserDomain
		{
			get
			{
				return this.userDomain;
			}
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

		internal static void ClearCache()
		{
			OAuthCredentials.challengeCache.Clear();
		}

		internal Authorization Authenticate(string challengeString, WebRequest webRequest, bool preAuthenticate)
		{
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] entering", new object[0]);
			if (webRequest == null)
			{
				throw new ArgumentNullException("request");
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			Uri requestUri = webRequest.RequestUri;
			string text = requestUri.ToString();
			this.caller = webRequest.Headers[HttpRequestHeader.UserAgent];
			HttpAuthenticationChallenge httpAuthenticationChallenge = null;
			HttpAuthenticationChallenge httpAuthenticationChallenge2 = null;
			if (preAuthenticate)
			{
				this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] trying to build the token for '{0}' as pre-auth is specified.", new object[]
				{
					requestUri
				});
				if (OAuthCredentials.challengeCache.TryGetValue(webRequest.RequestUri, out httpAuthenticationChallenge2))
				{
					this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] the challenge for '{0}' found in the cache: {1}.", new object[]
					{
						text,
						httpAuthenticationChallenge2
					});
					httpAuthenticationChallenge = httpAuthenticationChallenge2;
				}
				else
				{
					this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] the challenge for '{0}' not found in the cache.", new object[]
					{
						text
					});
				}
			}
			else
			{
				this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] challenge from '{0}' received: {1} ", new object[]
				{
					text,
					challengeString
				});
				HttpAuthenticationResponseHeader httpAuthenticationResponseHeader = HttpAuthenticationResponseHeader.Parse(challengeString);
				httpAuthenticationChallenge = httpAuthenticationResponseHeader.FindFirstChallenge(Constants.BearerAuthenticationType);
				if (httpAuthenticationChallenge != null)
				{
					OAuthCredentials.challengeCache.InsertAbsolute(requestUri, httpAuthenticationChallenge, OAuthCredentials.ChallengeObjectAbsoluteExpiration, null);
				}
			}
			if (httpAuthenticationChallenge != null)
			{
				string authority = webRequest.RequestUri.Authority;
				lock (this.lockObj)
				{
					if (authority == this.lastRequestUriAuthority && this.localConfiguration == this.lastLocalConfiguration && httpAuthenticationChallenge.Equals(this.lastChallengeObject) && this.lastTokenResult != null)
					{
						TimeSpan remainingTokenLifeTime = this.lastTokenResult.RemainingTokenLifeTime;
						this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] the last token remaining life time is : {0}", new object[]
						{
							remainingTokenLifeTime
						});
						bool flag2 = remainingTokenLifeTime > OAuthCredentials.RemainingLifetimeLimitToReuseLastToken;
						if (flag2)
						{
							this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] re-use the last token: {0}", new object[]
							{
								this.lastTokenResult
							});
							this.Tracer.LogToken(this.GetHashCode(), this.lastTokenResult.ToString());
							OutboundProtocolLog.BeginAppend("SendCached", "ok", stopwatch.ElapsedMilliseconds, this.caller, this.clientRequestId, text, this.userDomain, null, null, null, null, remainingTokenLifeTime, this.lastTokenResult);
							return new Authorization(OAuthCommon.WriteAuthorizationHeader(this.lastTokenResult.TokenString), true);
						}
					}
				}
				TokenResult tokenResult = null;
				try
				{
					tokenResult = this.GetToken(webRequest, httpAuthenticationChallenge);
				}
				catch (OAuthTokenRequestFailedException ex)
				{
					string message = ex.Message;
					this.Tracer.LogError(this.GetHashCode(), "{0}", new object[]
					{
						message
					});
					OutboundProtocolLog.BeginAppend("SendNew", "fail", stopwatch.ElapsedMilliseconds, this.caller, this.clientRequestId, text, this.userDomain, null, ex.GetKeyForErrorCode(), message, null, TimeSpan.Zero, tokenResult);
					throw;
				}
				OutboundProtocolLog.BeginAppend("SendNew", "ok", stopwatch.ElapsedMilliseconds, this.caller, this.clientRequestId, text, this.userDomain, null, null, null, null, tokenResult.RemainingTokenLifeTime, tokenResult);
				lock (this.lockObj)
				{
					this.lastRequestUriAuthority = authority;
					this.lastChallengeObject = httpAuthenticationChallenge;
					this.lastLocalConfiguration = this.localConfiguration;
					this.lastTokenResult = tokenResult;
				}
				this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] send request to '{0}' with the bearer token: '{1}'", new object[]
				{
					text,
					tokenResult
				});
				this.Tracer.LogToken(this.GetHashCode(), tokenResult.ToString());
				return new Authorization(OAuthCommon.WriteAuthorizationHeader(tokenResult.TokenString), true);
			}
			string text2 = webRequest.Headers[HttpRequestHeader.Authorization];
			if (!string.IsNullOrEmpty(text2) && text2.TrimStart(new char[0]).StartsWith(Constants.BearerAuthenticationType, StringComparison.OrdinalIgnoreCase))
			{
				this.Tracer.LogError(this.GetHashCode(), "[OAuthCredentials:Authenticate] the authorization header was '{0}', but no challenge returned from '{1}'. That url may not support OAuth", new object[]
				{
					text2,
					text
				});
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.InvalidOAuthEndpoint, requestUri.AbsoluteUri, null);
			}
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:Authenticate] send requst to '{0}' with empty bearer token", new object[]
			{
				text
			});
			OutboundProtocolLog.BeginAppend("SendEmpty", "ok", stopwatch.ElapsedMilliseconds, this.caller, this.clientRequestId, text, this.userDomain, null, null, null, null, TimeSpan.Zero, null);
			return new Authorization(OAuthCommon.WriteAuthorizationHeader(string.Empty), false);
		}

		private TokenResult GetToken(WebRequest webRequest, HttpAuthenticationChallenge challengeObject)
		{
			string authority = webRequest.RequestUri.Authority;
			string clientId = challengeObject.ClientId;
			string realm = challengeObject.Realm;
			string trustedIssuers = challengeObject.TrustedIssuers;
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:GetToken] client-id: '{0}', realm: '{1}', trusted_issuer: '{2}'", new object[]
			{
				clientId,
				realm,
				trustedIssuers
			});
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthCredentials:GetToken] start building a token for the user domain '{0}'", new object[]
			{
				this.userDomain
			});
			OAuthTokenBuilder oauthTokenBuilder = new OAuthTokenBuilder(this.organizationId, this.localConfiguration, this.caller);
			oauthTokenBuilder.Tracer = this.Tracer;
			oauthTokenBuilder.IncludeNameIdOnly = this.IncludeNameIdOnly;
			TokenResult result;
			if (this.adUser != null)
			{
				result = oauthTokenBuilder.GetAppWithUserToken(clientId, authority, realm, trustedIssuers, this.userDomain, this.adUser);
			}
			else if (this.miniRecipient != null)
			{
				result = oauthTokenBuilder.GetAppWithUserToken(clientId, authority, realm, trustedIssuers, this.userDomain, this.miniRecipient);
			}
			else
			{
				result = oauthTokenBuilder.GetAppToken(clientId, authority, realm, trustedIssuers, this.userDomain);
			}
			return result;
		}

		public NetworkCredential GetCredential(Uri uri, string authType)
		{
			return null;
		}

		private static readonly TimeSpan ChallengeObjectAbsoluteExpiration = TimeSpan.FromMinutes(60.0);

		private static readonly TimeoutCache<Uri, HttpAuthenticationChallenge> challengeCache = new TimeoutCache<Uri, HttpAuthenticationChallenge>(2, 500, false);

		private static readonly TimeSpan RemainingLifetimeLimitToReuseLastToken = TimeSpan.FromHours(7.0);

		private readonly OrganizationId organizationId;

		private readonly ADUser adUser;

		private readonly MiniRecipient miniRecipient;

		private readonly string userDomain;

		private IOutboundTracer tracer = DefaultOutboundTracer.Instance;

		private LocalConfiguration localConfiguration;

		private Guid? clientRequestId = null;

		private bool? includeNameIdOnly = null;

		private string caller;

		private object lockObj = new object();

		private LocalConfiguration lastLocalConfiguration;

		private string lastRequestUriAuthority;

		private HttpAuthenticationChallenge lastChallengeObject;

		private TokenResult lastTokenResult;

		public class OAuthAuthenticationModule : IAuthenticationModule
		{
			public string AuthenticationType
			{
				get
				{
					return Constants.BearerAuthenticationType;
				}
			}

			public bool CanPreAuthenticate
			{
				get
				{
					return true;
				}
			}

			public Authorization Authenticate(string challenge, WebRequest request, ICredentials credentials)
			{
				ExTraceGlobals.OAuthTracer.TraceFunction((long)this.GetHashCode(), "[OAuthAuthenticationModule:Authenticate] Entering");
				OAuthCredentials oauthCredentials = credentials as OAuthCredentials;
				if (oauthCredentials == null)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<Type>((long)this.GetHashCode(), "[OAuthAuthenticationModule:Authenticate] Leaving since the credentials is of type {0}", credentials.GetType());
					return null;
				}
				return oauthCredentials.Authenticate(challenge, request, false);
			}

			public Authorization PreAuthenticate(WebRequest request, ICredentials credentials)
			{
				ExTraceGlobals.OAuthTracer.TraceFunction((long)this.GetHashCode(), "[OAuthAuthenticationModule:PreAuthenticate] Entering");
				OAuthCredentials oauthCredentials = credentials as OAuthCredentials;
				if (oauthCredentials == null)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<Type>((long)this.GetHashCode(), "[OAuthAuthenticationModule:PreAuthenticate] Leaving since the credentials is of type {0}", credentials.GetType());
					return null;
				}
				return oauthCredentials.Authenticate(null, request, true);
			}
		}
	}
}
