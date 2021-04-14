using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SamlSecurityTokenManager : ClientCredentialsSecurityTokenManager
	{
		public SamlSecurityTokenManager(SamlClientCredentials samlClientCredentials) : base(samlClientCredentials)
		{
			this.samlClientCredentials = samlClientCredentials;
		}

		public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
		{
			if (string.Equals(tokenRequirement.TokenType, SecurityTokenTypes.Saml, StringComparison.OrdinalIgnoreCase) || string.Equals(tokenRequirement.TokenType, "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1", StringComparison.OrdinalIgnoreCase))
			{
				if (this.cachedSecurityTokenProvider == null)
				{
					this.cachedSecurityTokenProvider = new SamlSecurityTokenProvider(this.samlClientCredentials);
				}
				return this.cachedSecurityTokenProvider;
			}
			return base.CreateSecurityTokenProvider(tokenRequirement);
		}

		private SamlClientCredentials samlClientCredentials;

		private SamlSecurityTokenProvider cachedSecurityTokenProvider;
	}
}
