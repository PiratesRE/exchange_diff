using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.OAuth
{
	internal abstract class OAuthTokenHandler : JwtSecurityTokenHandler
	{
		protected OAuthTokenHandler(JwtSecurityToken token, Uri targetUri)
		{
			this.token = token;
			this.targetUri = targetUri;
			base.RequireSignedTokens = true;
			base.RequireExpirationTime = true;
		}

		public static OAuthTokenHandler DummyHandler
		{
			get
			{
				return OAuthTokenHandler.Dummy.Instance;
			}
		}

		public LocalConfiguration LocalConfiguration
		{
			get
			{
				return this.localConfiguration;
			}
			set
			{
				this.localConfiguration = value;
			}
		}

		public bool IsUnitTestOnlyPath
		{
			get
			{
				return this.isUnitTestOnlyPath;
			}
			set
			{
				this.isUnitTestOnlyPath = value;
			}
		}

		public virtual bool IsSignatureValidated
		{
			get
			{
				return this.signatureValidated;
			}
		}

		public JwtSecurityToken Token
		{
			get
			{
				return this.token;
			}
		}

		public abstract string TokenCategory { get; }

		public virtual IEnumerable<string> ClaimTypesForLogging
		{
			get
			{
				return null;
			}
		}

		public TrustedIssuer MatchedIssuer
		{
			get
			{
				return this.matchedIssuer;
			}
		}

		public static OAuthIdentity GetOAuthIdentity(string rawToken, out string loggableToken)
		{
			TrustedIssuer trustedIssuer;
			return OAuthTokenHandler.GetOAuthIdentity(rawToken, ConfigProvider.Instance.Configuration, false, out loggableToken, out trustedIssuer);
		}

		internal static OAuthIdentity GetOAuthIdentity(string rawToken, LocalConfiguration localConfiguration, bool isUnitTestOnlyPath, out string loggableToken, out TrustedIssuer trustedIssuer)
		{
			OAuthTokenHandler oauthTokenHandler = null;
			loggableToken = null;
			trustedIssuer = null;
			OAuthIdentity oauthIdentity;
			try
			{
				oauthTokenHandler = OAuthTokenHandler.CreateTokenHandler(rawToken, null, out loggableToken);
				oauthTokenHandler.LocalConfiguration = localConfiguration;
				oauthTokenHandler.IsUnitTestOnlyPath = isUnitTestOnlyPath;
				oauthIdentity = oauthTokenHandler.GetOAuthIdentity();
			}
			finally
			{
				if (oauthTokenHandler != null)
				{
					trustedIssuer = oauthTokenHandler.MatchedIssuer;
				}
			}
			return oauthIdentity;
		}

		public virtual AuthenticationAuthority AuthenticationAuthority
		{
			get
			{
				return AuthenticationAuthority.ORGID;
			}
		}

		public OAuthIdentity GetOAuthIdentity()
		{
			this.ThrowIfFalse(this.isUnitTestOnlyPath || OAuthAppPoolLevelPolicy.Instance.IsAllowedProfiles(this.TokenCategory), OAuthErrors.TokenProfileNotApplicable, new object[0]);
			this.ValidateToken();
			this.ThrowIfFalse(this.IsSignatureValidated, OAuthErrors.UnexpectedErrorOccurred, new object[0]);
			return OAuthIdentity.Create(this.organizationId, this.oauthApplication, this.oauthActAsUser);
		}

		public abstract OAuthPreAuthIdentity GetPreAuthIdentity();

		public static string ValidateWacCallbackToken(string rawToken)
		{
			JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(rawToken) as JwtSecurityToken;
			OAuthTokenHandler oauthTokenHandler = OAuthTokenHandler.CreateTokenHandler(jwtSecurityToken, null);
			ExTraceGlobals.OAuthTracer.TraceDebug<OAuthTokenHandler>(0L, "[GetOAuthIdentity] Created the token handler: {0}", oauthTokenHandler);
			oauthTokenHandler.PreValidateToken();
			oauthTokenHandler.ValidateIssuer();
			TokenValidationParameters tokenValidationParameters = oauthTokenHandler.CreateTokenValidationParameters();
			oauthTokenHandler.ValidateToken(jwtSecurityToken, tokenValidationParameters);
			oauthTokenHandler.ThrowIfFalse(oauthTokenHandler.IsSignatureValidated, OAuthErrors.UnexpectedErrorOccurred, new object[0]);
			string value;
			oauthTokenHandler.ThrowIfFalse(OAuthCommon.TryGetClaimValue(jwtSecurityToken, Constants.ClaimTypes.AppContext, out value), OAuthErrors.UnexpectedErrorOccurred, new object[0]);
			Dictionary<string, string> dictionary = value.DeserializeFromJson<Dictionary<string, string>>();
			string result;
			oauthTokenHandler.ThrowIfFalse(dictionary.TryGetValue(Constants.ClaimTypes.Scope, out result), OAuthErrors.UnexpectedErrorOccurred, new object[0]);
			return result;
		}

		public static OAuthTokenHandler CreateTokenHandler(string rawToken, Uri targetUri, out string loggableToken)
		{
			JwtSecurityToken jwtSecurityToken = null;
			OAuthTokenHandler result;
			try
			{
				jwtSecurityToken = (new JwtSecurityTokenHandler().ReadToken(rawToken) as JwtSecurityToken);
				result = OAuthTokenHandler.CreateTokenHandler(jwtSecurityToken, targetUri);
			}
			finally
			{
				loggableToken = OAuthCommon.GetLoggableTokenString(rawToken, jwtSecurityToken);
			}
			return result;
		}

		private static OAuthTokenHandler CreateTokenHandler(JwtSecurityToken token, Uri targetUri)
		{
			string a = null;
			if (OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.Ver, out a))
			{
				if (string.Equals(a, Constants.ClaimValues.ExchangeSelfIssuedVersion1, StringComparison.OrdinalIgnoreCase))
				{
					return new OAuthTokenHandler.V1ProfileExchangeSelfIssuedActAsTokenHandler(token, targetUri);
				}
				if (!string.Equals(a, Constants.ClaimValues.Version1, StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOAuthTokenException(OAuthErrors.UnexpectedErrorOccurred, null, null);
				}
				string text = null;
				string text2 = null;
				string text3 = null;
				if (OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.MsExchCallback, out text3))
				{
					return new OAuthTokenHandler.V1CallbackTokenHandler(token, targetUri);
				}
				if (OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.Scp, out text))
				{
					return new OAuthTokenHandler.V1ProfileAppActAsTokenHandler(token, targetUri);
				}
				if (OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.AppId, out text2))
				{
					return new OAuthTokenHandler.V1ProfileAppTokenHandler(token, targetUri);
				}
				return new OAuthTokenHandler.V1ProfileIdTokenHandler(token, targetUri);
			}
			else
			{
				string actorToken = null;
				if (OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.ActorToken, out actorToken))
				{
					return new OAuthTokenHandler.ActAsTokenHandler(token, targetUri, actorToken);
				}
				string appContextString = null;
				if (OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.AppContext, out appContextString))
				{
					return new OAuthTokenHandler.CallbackTokenHandler(token, targetUri, appContextString);
				}
				return new OAuthTokenHandler.ActorTokenHandler(token, targetUri);
			}
		}

		protected virtual void ValidateToken()
		{
			this.PreValidateToken();
			this.ValidateAudience();
			this.ValidateIssuer();
			TokenValidationParameters tokenValidationParameters = this.CreateTokenValidationParameters();
			base.ValidateToken(this.token, tokenValidationParameters);
			this.ResolveOrganizationId();
			this.ResolveOAuthApplication();
			this.ResolveUserInfo();
			this.PostValidateToken();
		}

		protected virtual void PreValidateToken()
		{
		}

		protected abstract void ValidateAudience();

		protected virtual void ValidateIssuer()
		{
			if (this.token.Header.TryGetValue(Constants.ClaimTypes.X509CertificateThumbprint, out this.x5tHint))
			{
				ExTraceGlobals.OAuthTracer.TraceDebug<string>((long)this.GetHashCode(), "[ValidateIssuer] found x5t in the header, the value is '{0}'", this.x5tHint);
			}
			string issuer = this.token.Issuer;
			this.ThrowIfFalse(!string.IsNullOrEmpty(issuer), OAuthErrors.MissingIssuer, new object[0]);
			this.FindMatchedTrustedIssuer(issuer);
			this.ThrowIfFalse(this.matchedIssuer != null, OAuthErrors.NoConfiguredIssuerMatched, new object[]
			{
				issuer
			});
		}

		protected abstract void FindMatchedTrustedIssuer(string issuer);

		protected virtual TokenValidationParameters CreateTokenValidationParameters()
		{
			TokenValidationParameters tokenValidationParameters = new TokenValidationParameters();
			this.matchedIssuer.SetSigningTokens(this.x5tHint, tokenValidationParameters);
			return tokenValidationParameters;
		}

		protected override void ValidateSignature(JwtSecurityToken jwt, TokenValidationParameters validationParameters)
		{
			try
			{
				base.ValidateSignature(jwt, validationParameters);
			}
			catch (SecurityTokenValidationException ex)
			{
				ExTraceGlobals.OAuthTracer.TraceDebug<string, SecurityTokenValidationException>((long)this.GetHashCode(), "[OAuthTokenHandler:ValidateSignature] the x5t value is {0}, wif library throws: {1}", this.x5tHint, ex);
				this.matchedIssuer.PokeOnlineCertificateProvider();
				this.Throw(OAuthErrors.InvalidSignature, null, ex, this.x5tHint);
			}
			finally
			{
				this.signatureValidated = true;
			}
		}

		protected override void ValidateSigningToken(JwtSecurityToken jwt)
		{
		}

		protected override void ValidateAudience(JwtSecurityToken jwt, TokenValidationParameters validationParameters)
		{
		}

		protected override string ValidateIssuer(JwtSecurityToken jwt, TokenValidationParameters validationParameters)
		{
			return null;
		}

		protected virtual void PostValidateToken()
		{
		}

		protected virtual void ResolveOrganizationId()
		{
			this.organizationId = this.ResolveOrganizationByRealm(this.realmFromAudience);
			this.ThrowIfFalse(this.organizationId != null, OAuthErrors.OrganizationIdNotFoundFromRealm, new object[]
			{
				this.realmFromAudience
			});
		}

		protected abstract void ResolveOAuthApplication();

		protected abstract void ResolveUserInfo();

		protected OrganizationId ResolveOrganizationByRealm(string realm)
		{
			if (AuthCommon.IsMultiTenancyEnabled)
			{
				Guid guid;
				if (Guid.TryParse(realm, out guid))
				{
					try
					{
						return ADSessionSettings.FromExternalDirectoryOrganizationId(guid).GetCurrentOrganizationIdPopulated();
					}
					catch (CannotResolveExternalDirectoryOrganizationIdException innerException)
					{
						this.Throw(OAuthErrors.ExternalOrgIdNotFound, new object[]
						{
							guid
						}, innerException, null);
						goto IL_124;
					}
					catch (CannotResolveTenantNameException innerException2)
					{
						this.Throw(OAuthErrors.ExternalOrgIdNotFound, new object[]
						{
							guid
						}, innerException2, null);
						goto IL_124;
					}
				}
				SmtpDomain smtpDomain;
				if (SmtpDomain.TryParse(realm, out smtpDomain))
				{
					return DomainToOrganizationIdCache.Singleton.Get(smtpDomain);
				}
			}
			else
			{
				SmtpDomain smtpDomain;
				if (SmtpDomain.TryParse(realm, out smtpDomain) && OrganizationId.ForestWideOrgId == DomainToOrganizationIdCache.Singleton.Get(smtpDomain))
				{
					return OrganizationId.ForestWideOrgId;
				}
				if (OAuthCommon.IsRealmMatch(this.localConfiguration.SingleTenancyRealm, realm))
				{
					return OrganizationId.ForestWideOrgId;
				}
				if (this.localConfiguration.AuthServers.Any((AuthServer authServer) => OAuthCommon.IsRealmMatch(authServer.Realm, realm)))
				{
					return OrganizationId.ForestWideOrgId;
				}
			}
			IL_124:
			return null;
		}

		public string GetExtraLoggingInfo()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.AppendFormat("Category:{0}|", this.TokenCategory);
			stringBuilder.AppendFormat("AppId:{0}|", (this.oauthApplication == null) ? string.Empty : this.oauthApplication.Id);
			if (this.ClaimTypesForLogging != null)
			{
				foreach (string text in this.ClaimTypesForLogging)
				{
					object arg;
					if (this.token.Payload.TryGetValue(text, out arg))
					{
						stringBuilder.AppendFormat("{0}:{1}|", text, arg);
					}
				}
			}
			if (this.inboundError != OAuthErrors.NoError)
			{
				stringBuilder.AppendFormat("ErrorCode:{0}", this.inboundError);
			}
			return stringBuilder.ToString();
		}

		public string GetClaimValue(JwtSecurityToken token, string claimType)
		{
			string text = OAuthCommon.TryGetClaimValue(token.Payload, claimType);
			this.ThrowIfFalse(!string.IsNullOrEmpty(text), OAuthErrors.NoClaimFound, new object[]
			{
				claimType
			});
			return text;
		}

		public void Throw(OAuthErrors inboundError, object[] args = null, Exception innerException = null, string periodicKey = null)
		{
			this.ThrowIfFalse(false, inboundError, args, innerException, periodicKey);
		}

		public void ThrowIfFalse(bool condition, OAuthErrors inboundError, params object[] args)
		{
			this.ThrowIfFalse(condition, inboundError, args, null, null);
		}

		public void ThrowIfFalse(bool condition, OAuthErrors inboundError, object[] args, Exception innerException, string logPeriodicKey = null)
		{
			if (!condition)
			{
				this.inboundError = inboundError;
				InvalidOAuthTokenException ex = new InvalidOAuthTokenException(inboundError, args, innerException);
				if (!string.IsNullOrEmpty(logPeriodicKey))
				{
					ex.LogEvent = true;
					ex.LogPeriodicKey = logPeriodicKey;
				}
				throw ex;
			}
		}

		protected readonly JwtSecurityToken token;

		protected readonly Uri targetUri;

		protected LocalConfiguration localConfiguration = ConfigProvider.Instance.Configuration;

		protected bool isUnitTestOnlyPath;

		protected string x5tHint;

		protected string realmFromAudience;

		protected string realmFromIssuer;

		protected TrustedIssuer matchedIssuer;

		protected OrganizationId organizationId;

		protected OAuthApplication oauthApplication;

		protected OAuthActAsUser oauthActAsUser;

		protected bool signatureValidated;

		private OAuthErrors inboundError;

		public abstract class S2STokenHandlerBase : OAuthTokenHandler
		{
			protected S2STokenHandlerBase(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
			}

			protected override void ValidateAudience()
			{
				string audience = this.token.Audience;
				string text = null;
				string text2 = null;
				base.ThrowIfFalse(this.TryParseAudience(audience, out this.appIdFromAudience, out text, out text2), OAuthErrors.InvalidAudience, new object[]
				{
					audience
				});
				base.ThrowIfFalse(OAuthCommon.IsIdMatch(this.appIdFromAudience, this.localConfiguration.ApplicationId), OAuthErrors.InvalidAudience, new object[]
				{
					audience
				});
				if (!this.isUnitTestOnlyPath)
				{
					string authority = this.targetUri.Authority;
					base.ThrowIfFalse(string.Equals(authority, text, StringComparison.OrdinalIgnoreCase), OAuthErrors.UnexpectedHostNameInAudience, new object[]
					{
						authority,
						text
					});
				}
				base.ThrowIfFalse(!string.IsNullOrEmpty(text2), OAuthErrors.EmptyRealmFromAudience, new object[0]);
				this.realmFromAudience = text2;
			}

			protected override void FindMatchedTrustedIssuer(string issuer)
			{
				IssuerMetadata issuerMetadata = null;
				base.ThrowIfFalse(IssuerMetadata.TryParse(issuer, out issuerMetadata), OAuthErrors.InvalidIssuerFormat, new object[]
				{
					issuer
				});
				this.realmFromIssuer = issuerMetadata.Realm;
				this.InternalResolveTrustedIssuer(issuerMetadata);
			}

			protected virtual void InternalResolveTrustedIssuer(IssuerMetadata issuerMetadataFromToken)
			{
				foreach (TrustedIssuer trustedIssuer in this.localConfiguration.TrustedIssuers)
				{
					IssuerMetadata issuerMetadata = trustedIssuer.IssuerMetadata;
					if (issuerMetadata.Kind == IssuerKind.ACS)
					{
						if (issuerMetadata.MatchId(issuerMetadataFromToken))
						{
							if (issuerMetadata.HasEmptyRealm)
							{
								if (OAuthCommon.IsRealmMatch(this.realmFromIssuer, this.realmFromAudience))
								{
									this.matchedIssuer = trustedIssuer;
									return;
								}
							}
							else if (issuerMetadata.MatchRealm(issuerMetadataFromToken))
							{
								this.matchedIssuer = trustedIssuer;
								return;
							}
						}
					}
					else if (issuerMetadata.Kind == IssuerKind.PartnerApp && issuerMetadata.MatchIdAndRealm(issuerMetadataFromToken))
					{
						this.matchedIssuer = trustedIssuer;
						return;
					}
				}
			}

			protected bool TryParseNameId(string nameId, out string appId, out string realm)
			{
				string text;
				realm = (text = null);
				appId = text;
				int num = nameId.IndexOf('@');
				if (num == -1)
				{
					return false;
				}
				realm = nameId.Substring(num + 1);
				appId = nameId.Substring(0, num);
				return true;
			}

			private bool TryParseAudience(string audience, out string appId, out string host, out string realm)
			{
				string text;
				realm = (text = null);
				string text2;
				host = (text2 = text);
				appId = text2;
				int num = audience.LastIndexOf('@');
				if (num == -1)
				{
					return false;
				}
				int num2 = audience.IndexOf('/');
				if (num2 == -1)
				{
					return false;
				}
				if (num < num2)
				{
					return false;
				}
				appId = audience.Substring(0, num2);
				realm = audience.Substring(num + 1);
				host = audience.Substring(num2 + 1, num - num2 - 1);
				return true;
			}

			protected string appIdFromAudience;
		}

		public sealed class ActAsTokenHandler : OAuthTokenHandler.S2STokenHandlerBase
		{
			public ActAsTokenHandler(JwtSecurityToken token, Uri targetUri, string actorToken) : base(token, targetUri)
			{
				base.RequireSignedTokens = false;
				JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(actorToken) as JwtSecurityToken;
				base.ThrowIfFalse(!string.IsNullOrEmpty(jwtSecurityToken.EncodedSignature), OAuthErrors.ActorTokenMustBeSigned, new object[0]);
				base.ThrowIfFalse(string.IsNullOrEmpty(token.EncodedSignature), OAuthErrors.OuterTokenAlsoSigned, new object[0]);
				this.innerTokenHandler = new OAuthTokenHandler.ActorTokenHandler(jwtSecurityToken, targetUri);
			}

			public override bool IsSignatureValidated
			{
				get
				{
					return this.innerTokenHandler.signatureValidated;
				}
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.S2SAppActAsToken;
				}
			}

			protected override void ValidateToken()
			{
				this.innerTokenHandler.localConfiguration = this.localConfiguration;
				this.innerTokenHandler.isUnitTestOnlyPath = this.isUnitTestOnlyPath;
				this.innerTokenHandler.ValidateToken();
				base.ValidateToken();
			}

			protected override void ValidateIssuer()
			{
				IssuerMetadata issuerMetadata = null;
				string issuer = this.token.Issuer;
				base.ThrowIfFalse(IssuerMetadata.TryParse(issuer, out issuerMetadata), OAuthErrors.InvalidOuterTokenIssuerFormat, new object[]
				{
					issuer
				});
				string applicationIdentifier = this.innerTokenHandler.PartnerApplication.ApplicationIdentifier;
				base.ThrowIfFalse(OAuthCommon.IsIdMatch(issuerMetadata.Id, applicationIdentifier), OAuthErrors.InvalidOuterTokenIssuerIdValue, new object[]
				{
					issuerMetadata.Id,
					applicationIdentifier
				});
				this.realmFromIssuer = issuerMetadata.Realm;
			}

			protected override void ValidateAudience()
			{
			}

			protected override TokenValidationParameters CreateTokenValidationParameters()
			{
				return OAuthTokenHandler.ActAsTokenHandler.defaultTokenValidationParameters;
			}

			protected override void ValidateSignature(JwtSecurityToken jwt, TokenValidationParameters validationParameters)
			{
			}

			protected override void ResolveOrganizationId()
			{
				this.organizationId = this.innerTokenHandler.organizationId;
			}

			protected override void ResolveOAuthApplication()
			{
				this.oauthApplication = this.innerTokenHandler.oauthApplication;
			}

			protected override void ResolveUserInfo()
			{
				this.oauthActAsUser = OAuthActAsUser.CreateFromOuterToken(this, this.organizationId, this.token, this.innerTokenHandler.PartnerApplication.AcceptSecurityIdentifierInformation);
			}

			protected override void PostValidateToken()
			{
				if (!OAuthCommon.IsRealmMatch(this.realmFromIssuer, this.innerTokenHandler.realmFromIssuer))
				{
					OrganizationId other = base.ResolveOrganizationByRealm(this.realmFromIssuer);
					base.ThrowIfFalse(this.innerTokenHandler.organizationId.Equals(other), OAuthErrors.InvalidRealmFromOuterTokenIssuer, new object[]
					{
						this.realmFromIssuer
					});
				}
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				string a;
				string lookupValue;
				if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Nii, out a) && string.Equals(a, Constants.NiiClaimValues.BusinessLiveId, StringComparison.OrdinalIgnoreCase))
				{
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Smtp, out lookupValue))
					{
						return new OAuthPreAuthIdentity(OAuthPreAuthType.Smtp, null, lookupValue);
					}
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Upn, out lookupValue))
					{
						return new OAuthPreAuthIdentity(OAuthPreAuthType.WindowsLiveID, null, lookupValue);
					}
				}
				else if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Smtp, out lookupValue))
				{
					return new OAuthPreAuthIdentity(OAuthPreAuthType.Smtp, null, lookupValue);
				}
				return null;
			}

			private static readonly TokenValidationParameters defaultTokenValidationParameters = new TokenValidationParameters();

			private readonly OAuthTokenHandler.ActorTokenHandler innerTokenHandler;
		}

		public sealed class ActorTokenHandler : OAuthTokenHandler.S2STokenHandlerBase
		{
			public ActorTokenHandler(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
			}

			public PartnerApplication PartnerApplication
			{
				get
				{
					return this.partnerApplication;
				}
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.S2SAppOnlyToken;
				}
			}

			protected override void ResolveOAuthApplication()
			{
				string claimValue = base.GetClaimValue(this.token, Constants.ClaimTypes.NameIdentifier);
				string text = null;
				string text2 = null;
				base.ThrowIfFalse(base.TryParseNameId(claimValue, out text, out text2), OAuthErrors.InvalidNameIdFormat, new object[]
				{
					claimValue
				});
				if (this.matchedIssuer.IssuerMetadata.Kind == IssuerKind.PartnerApp)
				{
					this.partnerApplication = this.matchedIssuer.PartnerApplication;
					string id = this.matchedIssuer.IssuerMetadata.Id;
					base.ThrowIfFalse(OAuthCommon.IsIdMatch(text, id), OAuthErrors.UnexpectedAppIdInNameId, new object[]
					{
						id,
						text
					});
					base.ThrowIfFalse(OAuthCommon.IsRealmMatch(text2, this.realmFromIssuer), OAuthErrors.UnexpectedRealmInNameId, new object[]
					{
						this.realmFromIssuer,
						text2
					});
				}
				else if (this.matchedIssuer.IssuerMetadata.Kind == IssuerKind.ACS)
				{
					foreach (PartnerApplication partnerApplication in this.localConfiguration.PartnerApplications)
					{
						if (partnerApplication.UseAuthServer && OAuthCommon.IsIdMatch(partnerApplication.ApplicationIdentifier, text))
						{
							if (!OAuthCommon.IsRealmEmpty(partnerApplication.Realm))
							{
								if (OAuthCommon.IsRealmMatch(text2, partnerApplication.Realm))
								{
									this.partnerApplication = partnerApplication;
									break;
								}
							}
							else if (OAuthCommon.IsRealmMatch(text2, this.realmFromIssuer))
							{
								this.partnerApplication = partnerApplication;
								break;
							}
						}
					}
				}
				if (this.partnerApplication == null && AuthCommon.IsMultiTenancyEnabled)
				{
					PartnerApplication partnerApplication2 = TenantLevelPartnerApplicationCache.Singleton.Get(this.organizationId, text);
					if (partnerApplication2 != null)
					{
						if (partnerApplication2.UseAuthServer && OAuthCommon.IsRealmEmpty(partnerApplication2.Realm) && OAuthCommon.IsRealmMatch(text2, this.realmFromIssuer))
						{
							this.partnerApplication = partnerApplication2;
						}
						else
						{
							ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[OAuthTokenHandler:CustomValidateActorToken] skip the tenant level PA {0}, where UseAuthServer is {1}, realm {2}; the realm from issuer is {3}", new object[]
							{
								partnerApplication2.Name,
								partnerApplication2.UseAuthServer,
								partnerApplication2.Realm,
								this.realmFromIssuer
							});
						}
					}
				}
				base.ThrowIfFalse(this.partnerApplication != null, OAuthErrors.NoMatchingPartnerAppFound, new object[]
				{
					claimValue
				});
				this.oauthApplication = new OAuthApplication(this.partnerApplication);
				this.oauthApplication.IsFromSameOrgExchange = new bool?(OAuthCommon.IsIdMatch(this.appIdFromAudience, text) && OAuthCommon.IsRealmMatch(this.realmFromAudience, text2));
			}

			protected override void ResolveUserInfo()
			{
			}

			protected override void PostValidateToken()
			{
				if (this.matchedIssuer.IssuerMetadata.Kind == IssuerKind.ACS)
				{
					base.ThrowIfFalse(OAuthCommon.IsRealmMatch(this.realmFromAudience, this.realmFromIssuer), OAuthErrors.MismatchedRealmBetweenAudienceAndIssuer, new object[0]);
				}
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				string claimValue = base.GetClaimValue(this.token, Constants.ClaimTypes.NameIdentifier);
				string text = null;
				string text2 = null;
				base.ThrowIfFalse(base.TryParseNameId(claimValue, out text, out text2), OAuthErrors.InvalidNameIdFormat, new object[]
				{
					claimValue
				});
				return new OAuthPreAuthIdentity(OAuthPreAuthType.OrganizationOnly, base.ResolveOrganizationByRealm(text2), text2);
			}

			private PartnerApplication partnerApplication;
		}

		internal sealed class CallbackTokenHandler : OAuthTokenHandler.S2STokenHandlerBase
		{
			public CallbackTokenHandler(JwtSecurityToken token, Uri targetUri, string appContextString) : base(token, targetUri)
			{
				this.appContextString = appContextString;
			}

			public override IEnumerable<string> ClaimTypesForLogging
			{
				get
				{
					return OAuthTokenHandler.CallbackTokenHandler.claimTypesForLogging;
				}
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.CallbackToken;
				}
			}

			protected override void PreValidateToken()
			{
				base.PreValidateToken();
				this.claimsInsideAppContext = this.ExtractClaimsFromAppContextClaim(this.appContextString);
				string a;
				base.ThrowIfFalse(this.claimsInsideAppContext.TryGetValue(Constants.ClaimTypes.MsExchCallback, out a), OAuthErrors.CallbackClaimNotFound, new object[0]);
				base.ThrowIfFalse(string.Equals(a, Constants.ClaimValues.MsExtensionV1, StringComparison.OrdinalIgnoreCase) || string.Equals(a, Constants.ClaimValues.MsOabDownloadV1, StringComparison.OrdinalIgnoreCase), OAuthErrors.InvalidCallbackClaimValue, new object[0]);
				string text = OAuthCommon.TryGetClaimValue(this.token.Payload, Constants.ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(text))
				{
					this.extensionId = base.GetClaimValue(this.token, Constants.ClaimTypes.AppCtxSender);
				}
				else
				{
					string text2 = null;
					string text3 = null;
					base.ThrowIfFalse(base.TryParseNameId(text, out text2, out text3), OAuthErrors.InvalidNameIdFormat, new object[]
					{
						text
					});
					this.extensionId = text2;
				}
				this.claimsInsideAppContext.TryGetValue(Constants.ClaimTypes.Scope, out this.scope);
			}

			protected override void ValidateAudience()
			{
				base.ValidateAudience();
				string text;
				if (!this.isUnitTestOnlyPath && this.claimsInsideAppContext.TryGetValue(Constants.ClaimTypes.MsExchProtocol, out text))
				{
					base.ThrowIfFalse(this.targetUri.LocalPath.StartsWith("/" + text, StringComparison.OrdinalIgnoreCase), OAuthErrors.InvalidCallbackTokenScope, new object[]
					{
						text
					});
				}
			}

			protected override void InternalResolveTrustedIssuer(IssuerMetadata issuerMetadataFromToken)
			{
				base.ThrowIfFalse(OAuthCommon.IsIdMatch(issuerMetadataFromToken.Id, this.localConfiguration.ApplicationId), OAuthErrors.InvalidCallbackTokenIssuer, new object[]
				{
					this.token.Issuer
				});
				this.matchedIssuer = TrustedIssuer.CreateFromExchangeCallback(this.localConfiguration, issuerMetadataFromToken.Realm);
			}

			protected override void ResolveOAuthApplication()
			{
				this.oauthApplication = new OAuthApplication(new OfficeExtensionInfo(this.extensionId, this.scope));
			}

			protected override void ResolveUserInfo()
			{
				this.oauthActAsUser = OAuthActAsUser.CreateFromAppContext(this, this.organizationId, this.claimsInsideAppContext, false);
			}

			protected override void PostValidateToken()
			{
				base.PostValidateToken();
				LocalTokenIssuerMetadata localTokenIssuerMetadata = new LocalTokenIssuerMetadata(this.localConfiguration.ApplicationId, OAuthConfigHelper.GetOrganizationRealm(this.organizationId));
				base.ThrowIfFalse(string.Equals(this.token.Issuer, localTokenIssuerMetadata.GetIssuer(), StringComparison.OrdinalIgnoreCase), OAuthErrors.InvalidCallbackTokenIssuer, new object[]
				{
					this.token.Issuer
				});
			}

			private Dictionary<string, string> ExtractClaimsFromAppContextClaim(string appContextValue)
			{
				Exception ex = null;
				Dictionary<string, string> result = null;
				try
				{
					result = appContextValue.DeserializeFromJson<Dictionary<string, string>>();
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				catch (InvalidOperationException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					ExTraceGlobals.OAuthTracer.TraceWarning<string, Exception>((long)this.GetHashCode(), "[ValidateExchangeCallbackToken] unable to deserialize appctx with {0}, exception {1}", appContextValue, ex);
					base.Throw(OAuthErrors.ExtensionInvalidAppCtxFormat, new object[]
					{
						appContextValue
					}, ex, null);
				}
				return result;
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				this.claimsInsideAppContext = this.ExtractClaimsFromAppContextClaim(this.appContextString);
				string lookupValue;
				if (this.claimsInsideAppContext.TryGetValue(Constants.ClaimTypes.Smtp, out lookupValue))
				{
					return new OAuthPreAuthIdentity(OAuthPreAuthType.Smtp, null, lookupValue);
				}
				return null;
			}

			private readonly string appContextString;

			private Dictionary<string, string> claimsInsideAppContext;

			private string extensionId;

			private string scope;

			private static readonly string[] claimTypesForLogging = new string[]
			{
				Constants.ClaimTypes.AppCtxSender
			};
		}

		private sealed class Dummy : OAuthTokenHandler
		{
			public Dummy() : base(null, null)
			{
			}

			public static OAuthTokenHandler Instance
			{
				get
				{
					return OAuthTokenHandler.Dummy.instance;
				}
			}

			public override string TokenCategory
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			protected override void ValidateAudience()
			{
				throw new NotImplementedException();
			}

			protected override void FindMatchedTrustedIssuer(string issuer)
			{
				throw new NotImplementedException();
			}

			protected override void ResolveOAuthApplication()
			{
				throw new NotImplementedException();
			}

			protected override void ResolveUserInfo()
			{
				throw new NotImplementedException();
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				throw new NotImplementedException();
			}

			private static readonly OAuthTokenHandler.Dummy instance = new OAuthTokenHandler.Dummy();
		}

		public abstract class V1ProfileTokenHandlerBase : OAuthTokenHandler
		{
			public V1ProfileTokenHandlerBase(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
				this.hasTenantId = OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Tid, out this.tenantId);
			}

			protected override void ValidateAudience()
			{
				if (!this.isUnitTestOnlyPath)
				{
					string audience = this.token.Audience;
					Uri targetUri = this.targetUri;
					targetUri.ToString();
					Uri uri;
					base.ThrowIfFalse(Uri.TryCreate(audience, UriKind.Absolute, out uri), OAuthErrors.InvalidAudience, new object[]
					{
						audience
					});
					string leftPart = uri.GetLeftPart(UriPartial.Authority);
					string leftPart2 = targetUri.GetLeftPart(UriPartial.Authority);
					base.ThrowIfFalse(string.Equals(leftPart, leftPart2, StringComparison.OrdinalIgnoreCase), OAuthErrors.UnexpectedHostNameInAudience, new object[]
					{
						leftPart,
						leftPart2
					});
					base.ThrowIfFalse(leftPart2.StartsWith(leftPart, StringComparison.OrdinalIgnoreCase), OAuthErrors.WrongAudience, new object[]
					{
						leftPart,
						leftPart2
					});
				}
			}

			protected override void FindMatchedTrustedIssuer(string issuer)
			{
				base.ThrowIfFalse(Uri.IsWellFormedUriString(issuer, UriKind.Absolute), OAuthErrors.InvalidIssuerFormat, new object[]
				{
					issuer
				});
				foreach (TrustedIssuer trustedIssuer in this.localConfiguration.TrustedIssuers)
				{
					if (trustedIssuer.IssuerMetadata.Kind == IssuerKind.AzureAD || trustedIssuer.IssuerMetadata.Kind == IssuerKind.ADFS)
					{
						string text = trustedIssuer.IssuerMetadata.Id;
						if (this.hasTenantId)
						{
							text = text.Replace(Constants.AzureADCommonEntityIdHint, this.tenantId);
						}
						if (OAuthCommon.IsIdMatch(text, issuer))
						{
							this.matchedIssuer = trustedIssuer;
							return;
						}
					}
				}
			}

			protected override void ResolveOrganizationId()
			{
				if (this.hasTenantId)
				{
					this.realmFromAudience = this.tenantId;
					base.ResolveOrganizationId();
					return;
				}
				if (AuthCommon.IsMultiTenancyEnabled)
				{
					base.ThrowIfFalse(false, OAuthErrors.MissingTenantIdClaim, new object[0]);
					return;
				}
				this.organizationId = OrganizationId.ForestWideOrgId;
			}

			protected override void ResolveOAuthApplication()
			{
				V1ProfileAppInfo v1ProfileAppInfo = new V1ProfileAppInfo(this, this.token);
				this.ValidateV1AppInfo(v1ProfileAppInfo);
				this.oauthApplication = new OAuthApplication(v1ProfileAppInfo);
			}

			protected virtual void ValidateV1AppInfo(V1ProfileAppInfo appInfo)
			{
				if (this.isUnitTestOnlyPath)
				{
					return;
				}
				string scope = appInfo.Scope;
				if (!string.IsNullOrEmpty(scope))
				{
					string[] array = OAuthGrant.ExtractKnownGrants(scope);
					if (array.Length > 0)
					{
						appInfo.Scope = string.Join(" ", OAuthAppPoolLevelPolicy.Instance.GetV1AppScope(array));
					}
				}
				string role = appInfo.Role;
				if (!string.IsNullOrEmpty(role))
				{
					string[] array2 = OAuthGrant.ExtractKnownGrantsFromRole(role);
					if (array2.Length > 0)
					{
						appInfo.Role = string.Join(" ", OAuthAppPoolLevelPolicy.Instance.GetV1AppRole(array2));
					}
				}
			}

			protected readonly bool hasTenantId;

			protected readonly string tenantId;
		}

		internal sealed class V1CallbackTokenHandler : OAuthTokenHandler.V1ProfileTokenHandlerBase
		{
			public V1CallbackTokenHandler(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
			}

			public override IEnumerable<string> ClaimTypesForLogging
			{
				get
				{
					return OAuthTokenHandler.V1CallbackTokenHandler.claimTypesForLogging;
				}
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.V1AppActAsToken;
				}
			}

			protected override void PreValidateToken()
			{
				base.PreValidateToken();
				string a;
				base.ThrowIfFalse(OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.MsExchCallback, out a), OAuthErrors.CallbackClaimNotFound, new object[0]);
				base.ThrowIfFalse(string.Equals(a, Constants.ClaimValues.ExCallbackV1, StringComparison.OrdinalIgnoreCase), OAuthErrors.InvalidCallbackClaimValue, new object[0]);
			}

			protected override void ValidateAudience()
			{
				base.ValidateAudience();
				string text;
				if (!this.isUnitTestOnlyPath && OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.MsExchProtocol, out text))
				{
					base.ThrowIfFalse(this.targetUri.LocalPath.StartsWith("/" + text, StringComparison.OrdinalIgnoreCase), OAuthErrors.InvalidCallbackTokenScope, new object[]
					{
						text
					});
				}
			}

			protected override void FindMatchedTrustedIssuer(string issuer)
			{
				IssuerMetadata issuerMetadata = null;
				base.ThrowIfFalse(IssuerMetadata.TryParse(issuer, out issuerMetadata), OAuthErrors.InvalidIssuerFormat, new object[]
				{
					issuer
				});
				this.realmFromIssuer = issuerMetadata.Realm;
				base.ThrowIfFalse(OAuthCommon.IsIdMatch(issuerMetadata.Id, this.localConfiguration.ApplicationId), OAuthErrors.InvalidCallbackTokenIssuer, new object[]
				{
					this.token.Issuer
				});
				this.matchedIssuer = TrustedIssuer.CreateFromExchangeCallback(this.localConfiguration, issuerMetadata.Realm);
			}

			protected override void ResolveUserInfo()
			{
				string externalDirectoryObjectId;
				if (this.hasTenantId && OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Oid, out externalDirectoryObjectId))
				{
					this.oauthActAsUser = OAuthActAsUser.CreateFromExternalDirectoryObjectId(this.organizationId, externalDirectoryObjectId);
					return;
				}
				string text;
				if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.PrimarySid, out text))
				{
					SecurityIdentifier securityIdentifier = null;
					Exception ex = null;
					try
					{
						securityIdentifier = new SecurityIdentifier(text);
					}
					catch (ArgumentException ex2)
					{
						ex = ex2;
					}
					catch (SystemException ex3)
					{
						ex = ex3;
					}
					base.ThrowIfFalse(ex == null, OAuthErrors.InvalidSidValue, new object[]
					{
						text
					}, ex, null);
					this.oauthActAsUser = OAuthActAsUser.CreateFromPrimarySid(this.organizationId, securityIdentifier);
					return;
				}
				base.Throw(OAuthErrors.NoUserClaimsFound, null, null, null);
			}

			protected override void ValidateV1AppInfo(V1ProfileAppInfo appInfo)
			{
				base.ValidateV1AppInfo(appInfo);
				string scope = appInfo.Scope;
				base.ThrowIfFalse(!string.IsNullOrEmpty(scope), OAuthErrors.InvalidClaimValueFound, new object[]
				{
					Constants.ClaimTypes.Scp,
					scope
				});
			}

			protected override void PostValidateToken()
			{
				base.PostValidateToken();
				LocalTokenIssuerMetadata localTokenIssuerMetadata = new LocalTokenIssuerMetadata(this.localConfiguration.ApplicationId, OAuthConfigHelper.GetOrganizationRealm(this.organizationId));
				base.ThrowIfFalse(string.Equals(this.token.Issuer, localTokenIssuerMetadata.GetIssuer(), StringComparison.OrdinalIgnoreCase), OAuthErrors.InvalidCallbackTokenIssuer, new object[]
				{
					this.token.Issuer
				});
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				string lookupValue;
				if (this.hasTenantId && OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Oid, out lookupValue))
				{
					return new OAuthPreAuthIdentity(OAuthPreAuthType.ExternalDirectoryObjectId, base.ResolveOrganizationByRealm(this.tenantId), lookupValue);
				}
				return null;
			}

			private static readonly string[] claimTypesForLogging = new string[]
			{
				Constants.ClaimTypes.Oid,
				Constants.ClaimTypes.PrimarySid,
				Constants.ClaimTypes.Upn,
				Constants.ClaimTypes.Scp
			};
		}

		public sealed class V1ProfileAppActAsTokenHandler : OAuthTokenHandler.V1ProfileTokenHandlerBase
		{
			public V1ProfileAppActAsTokenHandler(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.V1AppActAsToken;
				}
			}

			public override IEnumerable<string> ClaimTypesForLogging
			{
				get
				{
					return OAuthTokenHandler.V1ProfileAppActAsTokenHandler.claimTypesForLogging;
				}
			}

			protected override void ResolveUserInfo()
			{
				if (!AuthCommon.IsMultiTenancyEnabled)
				{
					string text = null;
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.PrimarySid, out text) || OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.OnPremSid, out text))
					{
						SecurityIdentifier securityIdentifier = null;
						Exception ex = null;
						try
						{
							securityIdentifier = new SecurityIdentifier(text);
						}
						catch (ArgumentException ex2)
						{
							ex = ex2;
						}
						catch (SystemException ex3)
						{
							ex = ex3;
						}
						base.ThrowIfFalse(ex == null, OAuthErrors.InvalidSidValue, new object[]
						{
							text
						}, ex, null);
						this.oauthActAsUser = OAuthActAsUser.CreateFromPrimarySid(this.organizationId, securityIdentifier);
						return;
					}
				}
				else
				{
					string externalDirectoryObjectId = null;
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Oid, out externalDirectoryObjectId))
					{
						this.oauthActAsUser = OAuthActAsUser.CreateFromExternalDirectoryObjectId(this.organizationId, externalDirectoryObjectId);
						return;
					}
				}
				base.Throw(OAuthErrors.NoUserClaimsFound, null, null, null);
			}

			protected override void ValidateV1AppInfo(V1ProfileAppInfo appInfo)
			{
				base.ValidateV1AppInfo(appInfo);
				string scope = appInfo.Scope;
				base.ThrowIfFalse(!string.IsNullOrEmpty(scope), OAuthErrors.InvalidClaimValueFound, new object[]
				{
					Constants.ClaimTypes.Scp,
					scope
				});
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				string lookupValue;
				if (AuthCommon.IsMultiTenancyEnabled && this.hasTenantId && OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Oid, out lookupValue))
				{
					return new OAuthPreAuthIdentity(OAuthPreAuthType.ExternalDirectoryObjectId, base.ResolveOrganizationByRealm(this.tenantId), lookupValue);
				}
				return null;
			}

			private static readonly string[] claimTypesForLogging = new string[]
			{
				Constants.ClaimTypes.Oid,
				Constants.ClaimTypes.PrimarySid,
				"upn",
				"acr",
				"appidacr",
				"amr",
				"scp"
			};
		}

		public sealed class V1ProfileAppTokenHandler : OAuthTokenHandler.V1ProfileTokenHandlerBase
		{
			public V1ProfileAppTokenHandler(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.V1AppOnlyToken;
				}
			}

			public override IEnumerable<string> ClaimTypesForLogging
			{
				get
				{
					return OAuthTokenHandler.V1ProfileAppTokenHandler.claimTypesForLogging;
				}
			}

			protected override void ResolveUserInfo()
			{
			}

			protected override void ValidateV1AppInfo(V1ProfileAppInfo appInfo)
			{
				base.ValidateV1AppInfo(appInfo);
				string role = appInfo.Role;
				base.ThrowIfFalse(!string.IsNullOrEmpty(role), OAuthErrors.InvalidClaimValueFound, new object[]
				{
					Constants.ClaimTypes.Roles,
					role
				});
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				if (this.hasTenantId)
				{
					return new OAuthPreAuthIdentity(OAuthPreAuthType.OrganizationOnly, base.ResolveOrganizationByRealm(this.tenantId), this.tenantId);
				}
				return null;
			}

			private static readonly string[] claimTypesForLogging = new string[]
			{
				Constants.ClaimTypes.Oid
			};
		}

		public abstract class V1ProfileExchangeSelfIssuedTokenHandlerBase : OAuthTokenHandler.V1ProfileTokenHandlerBase
		{
			public V1ProfileExchangeSelfIssuedTokenHandlerBase(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
			}

			public bool IsConsumerMailbox
			{
				get
				{
					if (this.targetOrg == null)
					{
						this.SetTargetOrg();
					}
					return OAuthCommon.IsIdMatch(this.targetOrg, Constants.ConsumerMailboxIdentifier);
				}
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.V1ExchangeSelfIssuedToken;
				}
			}

			protected override void ValidateAudience()
			{
				base.ValidateAudience();
				this.SetTargetOrg();
			}

			protected override void FindMatchedTrustedIssuer(string issuer)
			{
				base.ThrowIfFalse(Uri.IsWellFormedUriString(issuer, UriKind.Absolute), OAuthErrors.InvalidIssuerFormat, new object[]
				{
					issuer
				});
				foreach (TrustedIssuer trustedIssuer in this.localConfiguration.TrustedIssuers)
				{
					if (trustedIssuer.IssuerMetadata.Kind == IssuerKind.PartnerApp)
					{
						string id = trustedIssuer.IssuerMetadata.Id;
						if (OAuthCommon.IsIdMatch(id, issuer))
						{
							this.matchedIssuer = trustedIssuer;
							return;
						}
					}
				}
			}

			protected override void ResolveOrganizationId()
			{
				if (this.IsConsumerMailbox)
				{
					base.ThrowIfFalse(!this.hasTenantId, OAuthErrors.TenantIdClaimShouldNotBeSet, new object[0]);
					this.organizationId = base.ResolveOrganizationByRealm(TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationId);
					return;
				}
				base.ResolveOrganizationByRealm(this.targetOrg);
			}

			protected override void ResolveOAuthApplication()
			{
				V1ProfileAppInfo v1ProfileAppInfo = new V1ProfileAppInfo(this, this.token);
				base.ThrowIfFalse(OAuthCommon.IsIdMatch(this.matchedIssuer.PartnerApplication.ApplicationIdentifier, v1ProfileAppInfo.AppId), OAuthErrors.NoMatchingPartnerAppFound, new object[]
				{
					v1ProfileAppInfo.AppId
				});
				if (this.IsConsumerMailbox)
				{
					base.ValidateV1AppInfo(v1ProfileAppInfo);
					if (!string.IsNullOrEmpty(v1ProfileAppInfo.Scope))
					{
						string[] second = v1ProfileAppInfo.Scope.Split(OAuthTokenHandler.V1ProfileExchangeSelfIssuedTokenHandlerBase.delimiter);
						string[] actAsPermissions = this.matchedIssuer.PartnerApplication.ActAsPermissions;
						IEnumerable<string> values = actAsPermissions.Intersect(second);
						v1ProfileAppInfo.Scope = string.Join(" ", values);
					}
					if (!string.IsNullOrEmpty(v1ProfileAppInfo.Role))
					{
						string[] second2 = v1ProfileAppInfo.Role.Split(OAuthTokenHandler.V1ProfileExchangeSelfIssuedTokenHandlerBase.delimiter);
						string[] appOnlyPermissions = this.matchedIssuer.PartnerApplication.AppOnlyPermissions;
						IEnumerable<string> values2 = appOnlyPermissions.Intersect(second2);
						v1ProfileAppInfo.Role = string.Join(" ", values2);
					}
				}
				this.oauthApplication = new OAuthApplication(v1ProfileAppInfo, this.matchedIssuer.PartnerApplication);
			}

			private void SetTargetOrg()
			{
				Uri uri;
				if (Uri.TryCreate(this.token.Audience, UriKind.Absolute, out uri))
				{
					string text = uri.AbsolutePath.Substring(1);
					this.targetOrg = (text.EndsWith("/") ? text.Substring(0, text.Length - 1) : text);
					return;
				}
				this.targetOrg = "";
			}

			private static char[] delimiter = new char[]
			{
				' '
			};

			private string targetOrg;
		}

		public sealed class V1ProfileExchangeSelfIssuedActAsTokenHandler : OAuthTokenHandler.V1ProfileExchangeSelfIssuedTokenHandlerBase
		{
			public V1ProfileExchangeSelfIssuedActAsTokenHandler(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.V1ExchangeSelfIssuedToken;
				}
			}

			public override IEnumerable<string> ClaimTypesForLogging
			{
				get
				{
					return OAuthTokenHandler.V1ProfileExchangeSelfIssuedActAsTokenHandler.claimTypesForLogging;
				}
			}

			protected override void ResolveOAuthApplication()
			{
				base.ResolveOAuthApplication();
				if (string.IsNullOrEmpty(this.oauthApplication.V1ProfileApp.Scope) && this.oauthApplication.PartnerApplication.LinkedAccount == null)
				{
					base.ThrowIfFalse(false, OAuthErrors.NoAuthorizationValuePresent, new object[]
					{
						Constants.ClaimValues.ExchangeSelfIssuedVersion1,
						this.oauthApplication.V1ProfileApp.AppId
					});
				}
			}

			protected override void ResolveUserInfo()
			{
				string text = null;
				string text2 = null;
				string text3 = null;
				OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Puid, out text);
				OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Smtp, out text2);
				OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Oid, out text3);
				if (base.IsConsumerMailbox)
				{
					base.ThrowIfFalse(!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2), OAuthErrors.NoSmtpOrPuidClaimFound, new object[]
					{
						this.oauthApplication.Id
					});
				}
				if (!string.IsNullOrEmpty(text3))
				{
					this.oauthActAsUser = OAuthActAsUser.CreateFromExternalDirectoryObjectId(this.organizationId, text3);
					return;
				}
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
				{
					this.oauthActAsUser = OAuthActAsUser.CreateFromPuid(this, this.organizationId, text, text2);
					return;
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.oauthActAsUser = OAuthActAsUser.CreateFromPuidOnly(this.organizationId, new NetID(text));
					return;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					this.oauthActAsUser = OAuthActAsUser.CreateFromSmtpOnly(this.organizationId, text2);
					return;
				}
				base.Throw(OAuthErrors.NoUserClaimsFound, null, null, null);
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				if (this.hasTenantId)
				{
					string lookupValue;
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Oid, out lookupValue))
					{
						return new OAuthPreAuthIdentity(OAuthPreAuthType.ExternalDirectoryObjectId, base.ResolveOrganizationByRealm(this.tenantId), lookupValue);
					}
					string lookupValue2;
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Smtp, out lookupValue2))
					{
						return new OAuthPreAuthIdentity(OAuthPreAuthType.Smtp, base.ResolveOrganizationByRealm(this.tenantId), lookupValue2);
					}
				}
				else
				{
					string lookupValue3;
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Puid, out lookupValue3))
					{
						return new OAuthPreAuthIdentity(OAuthPreAuthType.Puid, base.ResolveOrganizationByRealm(TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationId), lookupValue3);
					}
					string lookupValue2;
					if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Smtp, out lookupValue2))
					{
						return new OAuthPreAuthIdentity(OAuthPreAuthType.Smtp, base.ResolveOrganizationByRealm(TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationId), lookupValue2);
					}
				}
				return null;
			}

			private static readonly string[] claimTypesForLogging = new string[]
			{
				Constants.ClaimTypes.Smtp,
				Constants.ClaimTypes.AppId,
				Constants.ClaimTypes.Puid,
				Constants.ClaimTypes.Oid
			};
		}

		public sealed class V1ProfileIdTokenHandler : OAuthTokenHandler.V1ProfileTokenHandlerBase
		{
			public V1ProfileIdTokenHandler(JwtSecurityToken token, Uri targetUri) : base(token, targetUri)
			{
				this.authAuthority = this.GetAuthenticationAuthority();
				this.puid = this.GetPuid(this.authAuthority);
				this.smtpAddress = this.GetSmtpAddress(this.puid, this.authAuthority);
			}

			public override AuthenticationAuthority AuthenticationAuthority
			{
				get
				{
					return this.authAuthority;
				}
			}

			public override string TokenCategory
			{
				get
				{
					return Constants.TokenCategories.V1IdToken;
				}
			}

			public override IEnumerable<string> ClaimTypesForLogging
			{
				get
				{
					return OAuthTokenHandler.V1ProfileIdTokenHandler.claimTypesForLogging;
				}
			}

			public override OAuthPreAuthIdentity GetPreAuthIdentity()
			{
				return new OAuthPreAuthIdentity(OAuthPreAuthType.Puid, null, this.puid);
			}

			protected override void ValidateAudience()
			{
				base.ThrowIfFalse(string.Equals(this.token.Audience, this.localConfiguration.ApplicationId, StringComparison.OrdinalIgnoreCase), OAuthErrors.InvalidAudience, new object[]
				{
					this.token.Audience
				});
			}

			protected override void ResolveOrganizationId()
			{
				if (this.authAuthority == AuthenticationAuthority.MSA)
				{
					this.organizationId = OrganizationId.FromMSAUserNetID(this.puid);
					return;
				}
				base.ResolveOrganizationId();
			}

			protected override void ResolveUserInfo()
			{
				this.oauthActAsUser = OAuthActAsUser.CreateFromPuid(this, this.organizationId, this.puid, this.smtpAddress);
			}

			protected override void ResolveOAuthApplication()
			{
				this.oauthApplication = Constants.IdTokenApplication;
			}

			private AuthenticationAuthority GetAuthenticationAuthority()
			{
				string text;
				if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.IdentityProvider, out text))
				{
					if (!string.IsNullOrWhiteSpace(text) && text.Equals("live.com", StringComparison.OrdinalIgnoreCase))
					{
						return AuthenticationAuthority.MSA;
					}
				}
				else if (OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.AlternateSecurityId, out text) && !string.IsNullOrWhiteSpace(text) && text.StartsWith("1:live.com", StringComparison.OrdinalIgnoreCase))
				{
					return AuthenticationAuthority.MSA;
				}
				return AuthenticationAuthority.ORGID;
			}

			private string GetPuid(AuthenticationAuthority authenticationAuthority)
			{
				string text;
				if (!OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.AlternateSecurityId, out text))
				{
					if (!OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Puid, out text))
					{
						base.Throw(OAuthErrors.NoPuidFound, new object[]
						{
							authenticationAuthority
						}, null, null);
					}
				}
				else
				{
					int num = text.LastIndexOf(':');
					base.ThrowIfFalse(num > 0 || text.Length - num <= 1, OAuthErrors.InvalidClaimValueFound, new object[]
					{
						Constants.ClaimTypes.AlternateSecurityId,
						text
					});
					text = text.Substring(num + 1);
				}
				return text;
			}

			private string GetSmtpAddress(string puid, AuthenticationAuthority authenticationAuthority)
			{
				string result;
				if (!OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.EmailAddress, out result) && !OAuthCommon.TryGetClaimValue(this.token, Constants.ClaimTypes.Upn, out result))
				{
					base.Throw(OAuthErrors.NoEmailAddressFound, new object[]
					{
						authenticationAuthority,
						puid
					}, null, null);
				}
				return result;
			}

			private readonly string puid;

			private readonly AuthenticationAuthority authAuthority;

			private readonly string smtpAddress;

			private static readonly string[] claimTypesForLogging = new string[]
			{
				Constants.ClaimTypes.AlternateSecurityId,
				Constants.ClaimTypes.Audience,
				Constants.ClaimTypes.IdentityProvider,
				Constants.ClaimTypes.Puid
			};
		}
	}
}
