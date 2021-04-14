using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[DataContract]
	internal sealed class WnsAccessToken
	{
		public WnsAccessToken(string accessToken, string tokenType)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("accessToken", accessToken);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("tokenType", tokenType);
			this.AccessToken = accessToken;
			this.TokenType = tokenType;
		}

		[DataMember(Name = "access_token", IsRequired = true, EmitDefaultValue = false)]
		public string AccessToken
		{
			get
			{
				return this.accessToken;
			}
			private set
			{
				this.accessToken = value;
				this.issuingTime = ExDateTime.UtcNow;
			}
		}

		[DataMember(Name = "token_type", IsRequired = true, EmitDefaultValue = false)]
		public string TokenType { get; private set; }

		public int GetUsageTimeInMinutes()
		{
			double totalMinutes = ExDateTime.UtcNow.Subtract(this.issuingTime).TotalMinutes;
			if (totalMinutes <= 2147483647.0)
			{
				return (int)totalMinutes;
			}
			return int.MaxValue;
		}

		public string ToWnsAuthorizationString()
		{
			if (this.toWnsAuthorizationString == null)
			{
				this.toWnsAuthorizationString = string.Format("Bearer {0}", this.AccessToken.ToNullableString());
			}
			return this.toWnsAuthorizationString;
		}

		private string accessToken;

		private ExDateTime issuingTime;

		private string toWnsAuthorizationString;
	}
}
