using System;

namespace Microsoft.Exchange.Security.OAuth.OAuthProtocols
{
	internal class OAuth2ErrorResponse : OAuth2Message
	{
		private OAuth2ErrorResponse()
		{
		}

		public OAuth2ErrorResponse(string error)
		{
			this.Error = error;
		}

		public static OAuth2ErrorResponse CreateFromEncodedResponse(string responseString)
		{
			OAuth2ErrorResponse oauth2ErrorResponse = new OAuth2ErrorResponse();
			oauth2ErrorResponse.DecodeFromJson(responseString);
			if (string.IsNullOrEmpty(oauth2ErrorResponse.Error))
			{
				throw new ArgumentException("Error property is null or empty. This message is not a valid OAuth2 error response.", "responseString");
			}
			return oauth2ErrorResponse;
		}

		public override string ToString()
		{
			return base.EncodeToJson();
		}

		public string Error
		{
			get
			{
				return base.Message["error"];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Error property cannot be null or empty.", "value");
				}
				base.Message["error"] = value;
			}
		}

		public string ErrorDescription
		{
			get
			{
				return base.Message["error_description"];
			}
			set
			{
				base.Message["error_description"] = value;
			}
		}

		public string ErrorUri
		{
			get
			{
				return base.Message["error_uri"];
			}
			set
			{
				base.Message["error_uri"] = value;
			}
		}
	}
}
