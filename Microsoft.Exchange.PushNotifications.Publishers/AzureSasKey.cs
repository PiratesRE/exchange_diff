using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureSasKey : IAzureSasTokenProvider
	{
		public AzureSasKey(string keyName, SecureString keyValue, AzureSasKey.ClaimType? claims = null)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("keyName", keyName);
			ArgumentValidator.ThrowIfNull("keyValue", keyValue);
			ArgumentValidator.ThrowIfZeroOrNegative("keyValue.Length", keyValue.Length);
			this.KeyName = keyName;
			this.KeyValue = keyValue;
			this.Claims = ((claims != null) ? claims.Value : AzureSasKey.ClaimType.None);
		}

		public string KeyName { get; private set; }

		public SecureString KeyValue { get; private set; }

		public AzureSasKey.ClaimType Claims { get; private set; }

		public static AzureSasKey GenerateRandomKey(AzureSasKey.ClaimType claims, string keyName = null)
		{
			byte[] array = new byte[32];
			AzureSasKey.RandomGenerator.GetBytes(array);
			return new AzureSasKey(keyName ?? "TenantSasKey", Convert.ToBase64String(array).ConvertToSecureString(), new AzureSasKey.ClaimType?(claims));
		}

		public AzureSasToken CreateSasToken(string resourceUri)
		{
			return this.CreateSasToken(resourceUri, 300);
		}

		public AzureSasToken CreateSasToken(string resourceUri, int expirationInSeconds)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("resourceUri", resourceUri);
			ArgumentValidator.ThrowIfInvalidValue<string>("resourceUri", resourceUri, (string x) => Uri.IsWellFormedUriString(resourceUri, UriKind.Absolute));
			ArgumentValidator.ThrowIfZeroOrNegative("expirationInSeconds", expirationInSeconds);
			string text = Convert.ToString((int)(ExDateTime.UtcNow - Constants.EpochBaseTime).TotalSeconds + expirationInSeconds);
			string arg = HttpUtility.UrlEncode(resourceUri);
			string s = string.Format("{0}\n{1}", arg, text);
			AzureSasToken result;
			using (HMACSHA256Cng hmacsha256Cng = new HMACSHA256Cng(Encoding.UTF8.GetBytes(this.KeyValue.AsUnsecureString())))
			{
				string signature = Convert.ToBase64String(hmacsha256Cng.ComputeHash(Encoding.UTF8.GetBytes(s)));
				result = new AzureSasToken(resourceUri, text, signature, this.KeyName, null);
			}
			return result;
		}

		public AzureSasKey ChangeClaims(AzureSasKey.ClaimType claims)
		{
			return new AzureSasKey(this.KeyName, this.KeyValue, new AzureSasKey.ClaimType?(claims));
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("{0} [{1}]", this.KeyName, this.Claims);
			}
			return this.toString;
		}

		public const string RandomSasKeyDefaultName = "TenantSasKey";

		public const int DefaultTokenExpirationInSeconds = 300;

		private const int SasKeySizeInBytes = 32;

		private static readonly RandomNumberGenerator RandomGenerator = RandomNumberGenerator.Create();

		private string toString;

		[Flags]
		public enum ClaimType
		{
			None = 0,
			Send = 1,
			Listen = 2,
			Manage = 4
		}
	}
}
