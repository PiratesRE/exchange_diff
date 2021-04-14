using System;
using System.Collections.Specialized;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Security.OAuth.OAuthProtocols
{
	internal class OAuth2AccessTokenRequest : OAuth2Message
	{
		private static StringCollection GetTokenResponseParameters()
		{
			return new StringCollection
			{
				"access_token",
				"expires_in"
			};
		}

		public static OAuth2AccessTokenRequest Read(StreamReader reader)
		{
			string requestString = null;
			try
			{
				requestString = reader.ReadToEnd();
			}
			catch (DecoderFallbackException innerException)
			{
				throw new InvalidDataException("Request encoding is not ASCII", innerException);
			}
			return OAuth2AccessTokenRequest.Read(requestString);
		}

		public static OAuth2AccessTokenRequest Read(string requestString)
		{
			OAuth2AccessTokenRequest oauth2AccessTokenRequest = new OAuth2AccessTokenRequest();
			try
			{
				oauth2AccessTokenRequest.Decode(requestString);
			}
			catch (InvalidRequestException)
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(requestString);
				if (string.IsNullOrEmpty(nameValueCollection["client_id"]) && string.IsNullOrEmpty(nameValueCollection["assertion"]))
				{
					throw new InvalidDataException("The request body must contain a client_id or assertion parameter.");
				}
				throw;
			}
			foreach (string value in oauth2AccessTokenRequest.Keys)
			{
				if (OAuth2AccessTokenRequest.TokenResponseParameters.Contains(value))
				{
					throw new InvalidDataException();
				}
			}
			return oauth2AccessTokenRequest;
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

		public string Resource
		{
			get
			{
				return base.Message["resource"];
			}
			set
			{
				base.Message["resource"] = value;
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

		public string AppContext
		{
			get
			{
				return base["AppContext"];
			}
			set
			{
				base["AppContext"] = value;
			}
		}

		public string Assertion
		{
			get
			{
				return base["assertion"];
			}
			set
			{
				base["assertion"] = value;
			}
		}

		public string GrantType
		{
			get
			{
				return base["grant_type"];
			}
			set
			{
				base["grant_type"] = value;
			}
		}

		public string ClientId
		{
			get
			{
				return base["client_id"];
			}
			set
			{
				base["client_id"] = value;
			}
		}

		public string ClientSecret
		{
			get
			{
				return base["client_secret"];
			}
			set
			{
				base["client_secret"] = value;
			}
		}

		public string Code
		{
			get
			{
				return base["code"];
			}
			set
			{
				base["code"] = value;
			}
		}

		public string Realm
		{
			get
			{
				return base["realm"];
			}
			set
			{
				base["realm"] = value;
			}
		}

		public string Username
		{
			get
			{
				return base["username"];
			}
			set
			{
				base["username"] = value;
			}
		}

		public string Password
		{
			get
			{
				return base["password"];
			}
			set
			{
				base["password"] = value;
			}
		}

		public string RedirectUri
		{
			get
			{
				return base["redirect_uri"];
			}
			set
			{
				base["redirect_uri"] = value;
			}
		}

		public void SetCustomProperty(string propertyName, string propertyValue)
		{
			base[propertyName] = propertyValue;
		}

		public virtual void Write(StreamWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(base.Encode());
		}

		public static StringCollection TokenResponseParameters = OAuth2AccessTokenRequest.GetTokenResponseParameters();
	}
}
