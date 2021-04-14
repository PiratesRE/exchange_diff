using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class OAuthTokenBuilder
	{
		public OAuthTokenBuilder(OrganizationId organizationId, LocalConfiguration localConfiguration = null, string caller = null) : this(OAuthConfigHelper.GetOrganizationRealm(organizationId), OAuthConfigHelper.GetTenantId(organizationId), localConfiguration, caller)
		{
			this.testOnlyPath = false;
		}

		public OAuthTokenBuilder(string realm, string tenantId, LocalConfiguration localConfiguration, string caller = null)
		{
			this.testOnlyPath = true;
			if (localConfiguration == null)
			{
				localConfiguration = ConfigProvider.Instance.Configuration;
			}
			if (localConfiguration == null)
			{
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.OAuthConfigurationUnavailable, null, null);
			}
			this.tenantId = tenantId;
			this.localConfiguration = localConfiguration;
			this.localTokenIssuer = new LocalTokenIssuer(this.localConfiguration, realm);
			this.localTokenIssuer.Caller = caller;
			this.caller = caller;
			this.acsAuthServers = (from authServer in localConfiguration.AuthServers
			where authServer.Type == AuthServerType.MicrosoftACS
			select authServer).ToArray<AuthServer>();
		}

		public IOutboundTracer Tracer
		{
			get
			{
				return this.tracer;
			}
			set
			{
				this.tracer = value;
			}
		}

		public bool IncludeNameIdOnly
		{
			get
			{
				return this.includeNameIdOnly;
			}
			set
			{
				this.includeNameIdOnly = value;
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

		public TokenResult GetAppToken(string applicationId, string destinationHost, string realmFromChallenge, string trustedIssuersFromChallenge, string userDomain)
		{
			ExTraceGlobals.OAuthTracer.TraceFunction((long)this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] Entering with applicationId: {0}, destinationHost: {1}, realmFromChallenge: {2}, trustedIssuersFromChallenge: {3}, userDomain: {4}", new object[]
			{
				applicationId,
				destinationHost,
				realmFromChallenge,
				trustedIssuersFromChallenge,
				userDomain
			});
			return this.GetAppToken(applicationId, destinationHost, realmFromChallenge, IssuerMetadata.Parse(trustedIssuersFromChallenge), userDomain);
		}

		public TokenResult GetAppToken(string applicationId, string destinationHost, string realmFromChallenge, IssuerMetadata[] trustedIssuersFromChallenge, string userDomain)
		{
			TokenResult tokenResult = null;
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] start building the apptoken", new object[0]);
			if (string.IsNullOrEmpty(applicationId))
			{
				this.Tracer.LogError(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] unable to continue building token, due to empty client id", new object[0]);
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.EmptyClientId, destinationHost, null);
			}
			if (trustedIssuersFromChallenge == null && string.IsNullOrEmpty(realmFromChallenge))
			{
				this.Tracer.LogError(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] unable to continue building token, given that both trusted_issuer and realm from the challenge are empty", new object[0]);
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.InvalidChallenge, destinationHost, null);
			}
			if (trustedIssuersFromChallenge != null)
			{
				bool flag = Array.IndexOf<IssuerMetadata>(trustedIssuersFromChallenge, this.localTokenIssuer.IssuerMetadata) != -1;
				if (flag)
				{
					string text = string.Format("{0}/{1}@{2}", applicationId, destinationHost, realmFromChallenge ?? userDomain);
					this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] trusted_issuer contains the local token issuer, start building a self-signed token for '{0}' as audience", new object[]
					{
						text
					});
					tokenResult = this.localTokenIssuer.GetAppToken(text);
				}
				else
				{
					AuthServer authServer = null;
					this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] checking enabled auth servers", new object[0]);
					foreach (AuthServer authServer2 in this.acsAuthServers)
					{
						IssuerMetadata issuerMetadata = IssuerMetadata.Create(authServer2);
						foreach (IssuerMetadata other in trustedIssuersFromChallenge)
						{
							if (issuerMetadata.MatchId(other))
							{
								authServer = authServer2;
								break;
							}
						}
						if (authServer != null)
						{
							this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] trusted_issuer includes the auth server '{0}': {1}, ", new object[]
							{
								authServer2.Name,
								issuerMetadata
							});
							break;
						}
						this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] trusted_issuer does NOT include the auth server '{0}': {1}, ", new object[]
						{
							authServer2.Name,
							issuerMetadata
						});
					}
					if (authServer != null)
					{
						if (!OAuthCommon.IsRealmEmpty(authServer.Realm))
						{
							this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] updating the tenant id with the auth server realm; current tenant id value is '{0}', new value is '{1}'", new object[]
							{
								this.tenantId,
								authServer.Realm
							});
							this.tenantId = authServer.Realm;
						}
						if (string.IsNullOrEmpty(this.tenantId))
						{
							this.Tracer.LogError(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] unable to continue building token given the tenant id is null. This usually indicates a bad configuration. The selected auth server is '{0}' with {1}", new object[]
							{
								authServer.Name,
								IssuerMetadata.Create(authServer)
							});
							throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.MissingRealmInAuthServer, authServer.Name, null);
						}
						string format = "{0}/{1}@{2}";
						object applicationId2 = applicationId;
						string arg;
						if ((arg = realmFromChallenge) == null)
						{
							arg = (this.tenantId ?? userDomain);
						}
						string text2 = string.Format(format, applicationId2, destinationHost, arg);
						this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] trying to get the apptoken from the auth server '{0}' for resource '{1}'", new object[]
						{
							authServer.Name,
							text2
						});
						tokenResult = ACSTokenCache.Instance.GetActorToken(new ACSTokenBuildRequest(this.testOnlyPath ? this.localTokenIssuer.SigningCert : null, this.localTokenIssuer.IssuerMetadata.Id, authServer, this.tenantId, text2, this.Caller), this.Tracer, this.ClientRequestId);
					}
				}
			}
			if (tokenResult != null)
			{
				this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] finish building apptoken; the token is {0}", new object[]
				{
					tokenResult
				});
				return tokenResult;
			}
			if (realmFromChallenge == null)
			{
				string text3 = string.Join(",", from issuer in trustedIssuersFromChallenge
				select issuer.ToTrustedIssuerString());
				this.Tracer.LogError(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] unable to continue building token; no locally configured issuer was in the trusted_issuer list, realm from challenge was also empty. trust_issuers was {0}", new object[]
				{
					text3
				});
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.NoMatchedTokenIssuer, text3, null);
			}
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] start building the token based on the realm from the challenge", new object[0]);
			PartnerApplication partnerApplication = this.localConfiguration.PartnerApplications.FirstOrDefault((PartnerApplication pa) => OAuthCommon.IsIdMatch(pa.ApplicationIdentifier, applicationId) && OAuthCommon.IsRealmMatch(pa.Realm, realmFromChallenge) && !pa.UseAuthServer);
			if (partnerApplication != null)
			{
				string text4 = string.Format("{0}/{1}@{2}", applicationId, destinationHost, realmFromChallenge);
				this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] found the configured partner application '{0}@{1}'; start building a self-signed token for '{2}' as audience", new object[]
				{
					applicationId,
					realmFromChallenge,
					text4
				});
				tokenResult = this.localTokenIssuer.GetAppToken(text4);
			}
			else
			{
				AuthServer[] array2 = (from issuer in this.acsAuthServers
				where OAuthCommon.IsRealmMatch(issuer.Realm, realmFromChallenge)
				select issuer).ToArray<AuthServer>();
				if (array2.Length > 1)
				{
					this.Tracer.LogError(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] unable to continue building token since {0} auth servers with the realm '{1}' were found.", new object[]
					{
						array2.Length,
						realmFromChallenge
					});
					throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.InvalidConfigurationMultipleAuthServerWithSameRealm, realmFromChallenge, null);
				}
				AuthServer authServer3;
				if (array2.Length == 0)
				{
					this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] no auth server with the realm '{0} was found', checking any auth server with empty realm.", new object[]
					{
						realmFromChallenge
					});
					array2 = (from issuer in this.acsAuthServers
					where OAuthCommon.IsRealmEmpty(issuer.Realm)
					select issuer).ToArray<AuthServer>();
					if (array2.Length != 1)
					{
						this.Tracer.LogError(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] unable to continue building token, since not 1 but {0} auth server(s) with empty realm were found", new object[]
						{
							array2.Length
						});
						throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.InvalidConfigurationMultipleAuthServerWithEmptyRealm, null, null);
					}
					authServer3 = array2[0];
					this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] the auth server '{0}' has empty realm, it has metadata {1}", new object[]
					{
						authServer3.Name,
						IssuerMetadata.Create(authServer3)
					});
				}
				else
				{
					this.tenantId = realmFromChallenge;
					authServer3 = array2[0];
					this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] exactly 1 auth server with the realm '{0}' was found, '{1}' has metadata {2}", new object[]
					{
						realmFromChallenge,
						authServer3.Name,
						IssuerMetadata.Create(authServer3)
					});
				}
				string format2 = "{0}/{1}@{2}";
				object applicationId3 = applicationId;
				string arg2;
				if ((arg2 = realmFromChallenge) == null)
				{
					arg2 = (this.tenantId ?? userDomain);
				}
				string text5 = string.Format(format2, applicationId3, destinationHost, arg2);
				this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] trying to get the apptoken from the auth server '{0}' for the resource '{1}'", new object[]
				{
					authServer3.Name,
					text5
				});
				tokenResult = ACSTokenCache.Instance.GetActorToken(new ACSTokenBuildRequest(this.testOnlyPath ? this.localTokenIssuer.SigningCert : null, this.localTokenIssuer.IssuerMetadata.Id, authServer3, this.tenantId, text5, this.Caller), this.Tracer, this.ClientRequestId);
			}
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder:GetAppToken] finish building apptoken, it is {0}", new object[]
			{
				tokenResult
			});
			return tokenResult;
		}

		public TokenResult GetAppWithUserToken(string applicationId, string destinationHost, string realmFromChallenge, string trustedIssuersFromChallenge, string userDomain, ADUser adUser)
		{
			return this.GetAppWithUserToken(applicationId, destinationHost, realmFromChallenge, IssuerMetadata.Parse(trustedIssuersFromChallenge), userDomain, ClaimProvider.Create(adUser));
		}

		public TokenResult GetAppWithUserToken(string applicationId, string destinationHost, string realmFromChallenge, string trustedIssuersFromChallenge, string userDomain, MiniRecipient miniRecipient)
		{
			return this.GetAppWithUserToken(applicationId, destinationHost, realmFromChallenge, IssuerMetadata.Parse(trustedIssuersFromChallenge), userDomain, ClaimProvider.Create(miniRecipient));
		}

		public TokenResult GetAppWithUserToken(string applicationId, string destinationHost, string realmFromChallenge, IssuerMetadata[] trustedIssuersFromChallenge, string userDomain, ClaimProvider claimProvider)
		{
			TokenResult appToken = this.GetAppToken(applicationId, destinationHost, realmFromChallenge, trustedIssuersFromChallenge, userDomain);
			claimProvider.IncludeNameIdOnly = this.IncludeNameIdOnly;
			bool flag = this.AllowClaimProviderToIncludeNameId(applicationId, realmFromChallenge);
			if (flag)
			{
				claimProvider.IsAllowedToIncludeNameId = flag;
			}
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder.GetAppWithUserToken] nameid {0} allowed to be included in the claim set", new object[]
			{
				flag ? "is" : "is not"
			});
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder.GetAppWithUserToken] only nameid to be included in the claim: {0}", new object[]
			{
				this.IncludeNameIdOnly ? "yes" : "no"
			});
			string text = string.Format("{0}/{1}@{2}", applicationId, destinationHost, realmFromChallenge ?? (this.tenantId ?? userDomain));
			this.Tracer.LogInformation(this.GetHashCode(), "[OAuthTokenBuilder.GetAppWithUserToken] building token with user context for the audience '{0}'", new object[]
			{
				text
			});
			Dictionary<string, string> claims = claimProvider.GetClaims();
			return this.localTokenIssuer.GetAppWithUserToken(appToken, this.tenantId, text, claims);
		}

		private bool AllowClaimProviderToIncludeNameId(string applicationId, string realmFromChallenge)
		{
			return !AuthCommon.IsWindowsLiveIDEnabled || ((string.Equals(applicationId, WellknownPartnerApplicationIdentifiers.SharePoint, StringComparison.OrdinalIgnoreCase) || string.Equals(applicationId, WellknownPartnerApplicationIdentifiers.Exchange, StringComparison.OrdinalIgnoreCase) || string.Equals(applicationId, WellknownPartnerApplicationIdentifiers.Lync, StringComparison.OrdinalIgnoreCase) || string.Equals(applicationId, WellknownPartnerApplicationIdentifiers.AAD, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrEmpty(realmFromChallenge) || string.Equals(realmFromChallenge, this.tenantId, StringComparison.OrdinalIgnoreCase)));
		}

		private readonly LocalTokenIssuer localTokenIssuer;

		private readonly AuthServer[] acsAuthServers;

		private readonly LocalConfiguration localConfiguration;

		private readonly bool testOnlyPath;

		private string tenantId;

		private IOutboundTracer tracer = DefaultOutboundTracer.Instance;

		private Guid? clientRequestId = null;

		private bool includeNameIdOnly;

		private string caller;
	}
}
