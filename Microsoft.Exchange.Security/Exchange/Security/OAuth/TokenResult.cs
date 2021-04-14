using System;
using System.IdentityModel.Tokens;
using System.Text;
using Microsoft.Exchange.Security.OAuth.OAuthProtocols;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class TokenResult
	{
		public TokenResult(string tokenString, DateTime expireDate)
		{
			this.tokenString = tokenString;
			this.expirationDate = expireDate;
		}

		public TokenResult(OAuth2AccessTokenResponse response) : this(response.AccessToken, DateTime.UtcNow.AddSeconds((double)int.Parse(response.ExpiresIn)))
		{
		}

		public TokenResult(JwtSecurityToken tokenObject, DateTime expireDate)
		{
			this.token = tokenObject;
			this.expirationDate = expireDate;
		}

		public JwtSecurityToken Token
		{
			get
			{
				return this.token;
			}
		}

		public string TokenString
		{
			get
			{
				if (this.tokenString == null && this.token != null)
				{
					this.tokenString = new JwtSecurityTokenHandler().WriteToken(this.token);
				}
				return this.tokenString;
			}
		}

		public string Base64String
		{
			get
			{
				if (this.base64String == null)
				{
					string text = this.ToString();
					if (text != null)
					{
						this.base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
					}
				}
				return this.base64String;
			}
		}

		public DateTime ExpirationDate
		{
			get
			{
				return this.expirationDate;
			}
		}

		public TimeSpan RemainingTokenLifeTime
		{
			get
			{
				return this.expirationDate - DateTime.UtcNow;
			}
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = OAuthCommon.GetReadableTokenString(this.TokenString);
			}
			return this.toString;
		}

		private readonly DateTime expirationDate;

		private JwtSecurityToken token;

		private string tokenString;

		private string toString;

		private string base64String;
	}
}
