using System;
using System.IdentityModel.Tokens;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Exchange.Security.OAuth.OAuthProtocols;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class OAuthTokenRequest
	{
		public string AcsUrl { get; set; }

		public string Audience { get; set; }

		public string Issuer { get; set; }

		public string Resource { get; set; }

		public JwtSecurityToken JwtInputToken { get; set; }

		public NetHelpersWebResponse AcsNetResponse { get; set; }

		public string AcsTokenResultString { get; set; }

		internal OAuth2AccessTokenRequest AcsTokenRequest { get; set; }

		internal OAuth2AccessTokenResponse AcsTokenResponse { get; set; }
	}
}
