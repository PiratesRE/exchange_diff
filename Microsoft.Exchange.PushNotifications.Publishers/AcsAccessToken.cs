using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PushNotifications.Extensions;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AcsAccessToken
	{
		public AcsAccessToken(string accessToken)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("accessToken", accessToken);
			Dictionary<string, string> dictionary = AcsAccessToken.PropertyReader.Read(accessToken);
			ArgumentValidator.ThrowIfInvalidValue<Dictionary<string, string>>("parse(accessToken)", dictionary, (Dictionary<string, string> x) => x != null && x.ContainsKey("wrap_access_token"));
			this.AccessToken = dictionary["wrap_access_token"];
			this.ParseAdditionalPropertiesFromAccessToken();
		}

		public string AccessToken { get; private set; }

		public ExDateTime? ExpirationTime { get; private set; }

		public string Issuer { get; private set; }

		public string Audience { get; private set; }

		public string Action { get; private set; }

		public bool IsValid()
		{
			return this.ExpirationTime != null && this.ExpirationTime.Value > ExDateTime.UtcNow;
		}

		public string ToAzureAuthorizationString()
		{
			if (this.toAzureAuthorizationString == null)
			{
				this.toAzureAuthorizationString = string.Format("WRAP access_token=\"{0}\"", this.AccessToken.ToString());
			}
			return this.toAzureAuthorizationString;
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("Issuer:{0}; Action:{1}; Audience:{2}; Expiration:{3}", new object[]
				{
					this.Issuer.ToNullableString(),
					this.Action.ToNullableString(),
					this.Audience.ToNullableString(),
					this.ExpirationTime.ToNullableString<ExDateTime>()
				});
			}
			return this.toString;
		}

		private void ParseAdditionalPropertiesFromAccessToken()
		{
			Dictionary<string, string> dictionary = AcsAccessToken.PropertyReader.Read(this.AccessToken);
			int num;
			if (dictionary.ContainsKey("ExpiresOn") && int.TryParse(dictionary["ExpiresOn"], out num))
			{
				this.ExpirationTime = new ExDateTime?(Constants.EpochBaseTime.AddSeconds((double)num));
			}
			if (dictionary.ContainsKey("Issuer"))
			{
				this.Issuer = dictionary["Issuer"];
			}
			if (dictionary.ContainsKey("net.windows.servicebus.action"))
			{
				this.Action = dictionary["net.windows.servicebus.action"];
			}
			if (dictionary.ContainsKey("Audience"))
			{
				this.Audience = dictionary["Audience"];
			}
		}

		public const string PropertySeparator = "&";

		public const string PropertyValueSeparator = "=";

		private const string ExpirationPropertyName = "ExpiresOn";

		private const string IssuerPropertyName = "Issuer";

		private const string AudiencePropertyName = "Audience";

		private const string ActionPropertyName = "net.windows.servicebus.action";

		private const string WrappedAccessTokenProperty = "wrap_access_token";

		private static readonly PropertyReader PropertyReader = new PropertyReader(new string[]
		{
			"&"
		}, new string[]
		{
			"="
		});

		private string toAzureAuthorizationString;

		private string toString;
	}
}
