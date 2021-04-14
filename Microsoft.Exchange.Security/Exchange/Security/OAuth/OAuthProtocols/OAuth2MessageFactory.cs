using System;
using System.IdentityModel.Tokens;
using System.IO;

namespace Microsoft.Exchange.Security.OAuth.OAuthProtocols
{
	internal static class OAuth2MessageFactory
	{
		public static OAuth2Message CreateFromEncodedResponse(StreamReader reader)
		{
			string text = reader.ReadToEnd();
			if (text.StartsWith("{\"error"))
			{
				return OAuth2ErrorResponse.CreateFromEncodedResponse(text);
			}
			return OAuth2AccessTokenResponse.Read(text);
		}

		public static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAssertion(JwtSecurityToken token, string resource)
		{
			JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
			string assertion = jwtSecurityTokenHandler.WriteToken(token);
			return new OAuth2AccessTokenRequest
			{
				GrantType = "http://oauth.net/grant_type/jwt/1.0/bearer",
				Assertion = assertion,
				Resource = resource
			};
		}
	}
}
