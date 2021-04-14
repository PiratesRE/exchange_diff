using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using Microsoft.IdentityModel.S2S.Tokens;

namespace Microsoft.Exchange.Management.ControlPanel.Psws
{
	internal sealed class TokenIssuer
	{
		internal TokenIssuer()
		{
			this.settings = TokenIssuerSettings.CreateFromConfiguration();
		}

		internal TokenIssuer(TokenIssuerSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings cannot be null");
			}
			this.settings = settings;
		}

		internal string GetToken(string tenantId, IDictionary<string, string> claims = null)
		{
			if (string.IsNullOrWhiteSpace(tenantId))
			{
				throw new ArgumentNullException("tenantId cannot be null or empty");
			}
			if (claims == null)
			{
				claims = new Dictionary<string, string>();
			}
			return this.IssueUserAccessToken(tenantId, this.IssueTenantAccessToken(tenantId), claims);
		}

		private string IssueTenantAccessToken(string tenantId)
		{
			string text = string.Format("{0}@{1}", this.settings.PartnerId, tenantId);
			string arg = string.Format("{0}/{1}", this.settings.AcsId, this.settings.AcsUrl.Authority);
			string text2 = string.Format("{0}@{1}", arg, tenantId);
			JsonWebSecurityToken jsonWebSecurityToken = new JsonWebSecurityToken(text, text2, DateTime.UtcNow, DateTime.UtcNow.AddDays(1.0), Enumerable.Empty<JsonWebTokenClaim>(), CertificateStore.GetSigningCredentials(this.settings.CertificateSubject));
			string text3 = string.Format("{0}/{1}@{2}", this.settings.ServiceId, this.settings.ServiceHostName, tenantId);
			OAuth2AccessTokenRequest oauth2AccessTokenRequest = OAuth2MessageFactory.CreateAccessTokenRequestWithAssertion(jsonWebSecurityToken, text3);
			oauth2AccessTokenRequest.Scope = text3;
			OAuth2S2SClient oauth2S2SClient = new OAuth2S2SClient();
			OAuth2AccessTokenResponse oauth2AccessTokenResponse = (OAuth2AccessTokenResponse)oauth2S2SClient.Issue(this.settings.AcsUrl.AbsoluteUri, oauth2AccessTokenRequest);
			return oauth2AccessTokenResponse.AccessToken;
		}

		private string IssueUserAccessToken(string tenantId, string actorToken, IDictionary<string, string> claims)
		{
			string text = string.Format("{0}@{1}", this.settings.PartnerId, tenantId);
			string text2 = string.Format("{0}/{1}@{2}", this.settings.ServiceId, this.settings.ServiceHostName, tenantId);
			List<JsonWebTokenClaim> first = new List<JsonWebTokenClaim>
			{
				new JsonWebTokenClaim("actortoken", actorToken),
				new JsonWebTokenClaim(TokenIssuer.ClaimTypeNii, TokenIssuer.ClaimValueNii)
			};
			IEnumerable<JsonWebTokenClaim> source = first.Concat(from claim in claims
			select new JsonWebTokenClaim(claim.Key, claim.Value));
			JsonWebSecurityToken jsonWebSecurityToken = new JsonWebSecurityToken(text, text2, DateTime.UtcNow, DateTime.UtcNow.AddDays(1.0), source.ToArray<JsonWebTokenClaim>());
			return new JsonWebSecurityTokenHandler().WriteTokenAsString(jsonWebSecurityToken);
		}

		internal static readonly string ClaimTypeNameId = "nameid";

		internal static readonly string ClaimTypeUpn = "upn";

		internal static readonly string ClaimTypeNii = "nii";

		internal static readonly string ClaimValueNii = "urn:federation:microsoftonline";

		internal static readonly string AuthorizationHeader = "Authorization";

		internal static readonly string ItemTagUpn = "RPSMemberName";

		internal static readonly string ItemTagPuid = "RPSPUID";

		internal static readonly string ItemTagTenantId = "TenantId";

		private TokenIssuerSettings settings;
	}
}
