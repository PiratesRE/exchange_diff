using System;

namespace Microsoft.Exchange.Security.OAuth.OAuthProtocols
{
	internal class OAuth2AccessTokenResponse : OAuth2Message
	{
		public static OAuth2AccessTokenResponse Read(string responseString)
		{
			OAuth2AccessTokenResponse oauth2AccessTokenResponse = new OAuth2AccessTokenResponse();
			oauth2AccessTokenResponse.DecodeFromJson(responseString);
			return oauth2AccessTokenResponse;
		}

		public override string ToString()
		{
			return base.EncodeToJson();
		}

		public string AccessToken
		{
			get
			{
				return base.Message["access_token"];
			}
			set
			{
				base.Message["access_token"] = value;
			}
		}

		public virtual string ExpiresIn
		{
			get
			{
				return base.Message["expires_in"];
			}
			set
			{
				base.Message["expires_in"] = value;
			}
		}

		public string RefreshToken
		{
			get
			{
				return base.Message["refresh_token"];
			}
			set
			{
				base.Message["refresh_token"] = value;
			}
		}

		public string Scope
		{
			get
			{
				return base.Message["scope"];
			}
			set
			{
				base.Message["scope"] = value;
			}
		}

		public string TokenType
		{
			get
			{
				return base.Message["token_type"];
			}
			set
			{
				base.Message["token_type"] = value;
			}
		}
	}
}
