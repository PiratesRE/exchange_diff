using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal sealed class AzureSasToken : IAzureSasTokenProvider
	{
		public AzureSasToken(string resourceUri, string expirationInSeconds, string signature, string keyName, string targetResourceUri = null)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("resourceUri", resourceUri);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("expirationInSeconds", expirationInSeconds);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("signature", signature);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("keyName", keyName);
			ArgumentValidator.ThrowIfInvalidValue<string>("resourceUri", resourceUri, (string x) => Uri.IsWellFormedUriString(x, UriKind.Absolute));
			ArgumentValidator.ThrowIfInvalidValue<string>("expirationInSeconds", expirationInSeconds, (string x) => double.TryParse(x, out this.expirationValue));
			this.ResourceUri = resourceUri;
			this.ExpirationInSeconds = expirationInSeconds;
			this.Signature = signature;
			this.KeyName = keyName;
			this.targetResourceUri = targetResourceUri;
		}

		[DataMember(Name = "sr", IsRequired = true)]
		public string ResourceUri { get; private set; }

		[DataMember(Name = "se", IsRequired = true)]
		public string ExpirationInSeconds { get; private set; }

		[DataMember(Name = "sig", IsRequired = true)]
		public string Signature { get; private set; }

		[DataMember(Name = "skn", IsRequired = true)]
		public string KeyName { get; private set; }

		public bool IsExpired()
		{
			if (this.expirationValue == 0.0)
			{
				double.TryParse(this.ExpirationInSeconds, out this.expirationValue);
			}
			return Constants.EpochBaseTime.AddSeconds(this.expirationValue) < ExDateTime.UtcNow;
		}

		public bool IsValid()
		{
			return (this.targetResourceUri == null || this.targetResourceUri.IndexOf(this.ResourceUri) == 0) && !this.IsExpired();
		}

		public AzureSasToken CreateSasToken(string resourceUri)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("resourceUri", resourceUri);
			ArgumentValidator.ThrowIfInvalidValue<string>("resourceUri", resourceUri, (string x) => Uri.IsWellFormedUriString(x, UriKind.Absolute));
			return new AzureSasToken(this.ResourceUri, this.ExpirationInSeconds, this.Signature, this.KeyName, resourceUri);
		}

		public AzureSasToken CreateSasToken(string resourceUri, int expirationInSeconds)
		{
			throw new NotImplementedException("CreateSasToken(string, int) is not supported by the AzureSasToken.");
		}

		public string ToJson()
		{
			return JsonConverter.Serialize<AzureSasToken>(this, null);
		}

		public string ToAzureAuthorizationString()
		{
			if (this.sasToken == null)
			{
				this.sasToken = string.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", new object[]
				{
					HttpUtility.UrlEncode(this.ResourceUri),
					HttpUtility.UrlEncode(this.Signature),
					this.ExpirationInSeconds,
					this.KeyName
				});
			}
			return this.sasToken;
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("{{ sr:{0}; se:{1}; skn:{2}; str:{3}; exp:{4}; val:{5}}}", new object[]
				{
					this.ResourceUri,
					this.ExpirationInSeconds,
					this.KeyName,
					this.targetResourceUri,
					this.IsExpired(),
					this.IsValid()
				});
			}
			return this.toString;
		}

		private readonly string targetResourceUri;

		private string sasToken;

		private double expirationValue;

		private string toString;
	}
}
