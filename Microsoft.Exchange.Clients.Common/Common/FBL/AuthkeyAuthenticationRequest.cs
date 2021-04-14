using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using Microsoft.Exchange.Common.Net.Cryptography;

namespace Microsoft.Exchange.Clients.Common.FBL
{
	internal class AuthkeyAuthenticationRequest
	{
		public static NameValueCollection DecryptSignedUrl(NameValueCollection encryptedQueryString)
		{
			AuthkeyAuthenticationRequest.SignedUrl signedUrl = new AuthkeyAuthenticationRequest.SignedUrl(encryptedQueryString);
			string query = signedUrl.ValidateHashAndDecryptPayload();
			return HttpUtility.ParseQueryString(query);
		}

		public static string UrlDecodeBase64String(string urlEncodedBase64String)
		{
			if (string.IsNullOrEmpty(urlEncodedBase64String))
			{
				return string.Empty;
			}
			return HttpUtility.UrlDecode(urlEncodedBase64String).Replace('_', '/').Replace('-', '+').Replace('~', '=');
		}

		public static string UrlEncodeBase64String(string base64String)
		{
			return base64String.Replace('+', '-').Replace('/', '_').Replace('=', '~');
		}

		public static string ConstructQueryString(NameValueCollection paramCollection)
		{
			string result = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in paramCollection)
			{
				string text = (string)obj;
				stringBuilder.Append(XssEncode.UrlEncode(text) + "=" + XssEncode.UrlEncode(paramCollection[text]) + "&");
			}
			if (stringBuilder.Length > 0)
			{
				result = stringBuilder.ToString(0, stringBuilder.Length - 1);
			}
			return result;
		}

		public class SignedUrl
		{
			public SignedUrl(CryptoKeyPayloadType payloadType, string plaintextPayload)
			{
				this.PayloadType = payloadType;
				this.UtcTimestamp = DateTime.UtcNow;
				this.Payload = CryptoUtils.Encrypt(Encoding.UTF8.GetBytes(plaintextPayload), payloadType, out this.keyVersion, out this.InitializationVector);
				string[] hashComponents = new string[]
				{
					this.ToQueryStringFragment(null, false)
				};
				this.MessageAuthenticationCode = HashUtility.ComputeHash(hashComponents, this.HashTypeForPayload);
			}

			public SignedUrl(NameValueCollection queryString)
			{
				this.KeyVersion = byte.Parse(queryString["kv"], NumberFormatInfo.InvariantInfo);
				if (this.KeyVersion > 1)
				{
					this.PayloadType = (CryptoKeyPayloadType)int.Parse(queryString["pt"], NumberFormatInfo.InvariantInfo);
				}
				else
				{
					AuthkeyAuthenticationRequest.SignedUrl.LegacyPayloadType legacyPayloadType = (AuthkeyAuthenticationRequest.SignedUrl.LegacyPayloadType)int.Parse(queryString["pt"], NumberFormatInfo.InvariantInfo);
					AuthkeyAuthenticationRequest.SignedUrl.LegacyPayloadType legacyPayloadType2 = legacyPayloadType;
					if (legacyPayloadType2 != AuthkeyAuthenticationRequest.SignedUrl.LegacyPayloadType.SvmFeedback)
					{
						throw new SecurityException("failed to parse query string collection");
					}
					this.PayloadType = CryptoKeyPayloadType.SvmFeedbackEncryption;
				}
				this.UtcTimestamp = DateTime.FromFileTimeUtc(long.Parse(queryString["ts"], NumberFormatInfo.InvariantInfo));
				this.InitializationVector = Convert.FromBase64String(AuthkeyAuthenticationRequest.UrlDecodeBase64String(queryString["iv"]));
				this.Payload = Convert.FromBase64String(AuthkeyAuthenticationRequest.UrlDecodeBase64String(queryString["authKey"]));
				this.MessageAuthenticationCode = Convert.FromBase64String(AuthkeyAuthenticationRequest.UrlDecodeBase64String(queryString["hmac"]));
			}

			public byte KeyVersion
			{
				get
				{
					return this.keyVersion;
				}
				set
				{
					this.keyVersion = value;
				}
			}

			public byte[] MessageAuthenticationCode { get; set; }

			private CryptoKeyPayloadType HashTypeForPayload
			{
				get
				{
					CryptoKeyPayloadType payloadType = this.PayloadType;
					if (payloadType == CryptoKeyPayloadType.SvmFeedbackEncryption)
					{
						return CryptoKeyPayloadType.SvmFeedbackHash;
					}
					throw new InvalidOperationException("Didn't find matching hash algorithm for PayloadType: " + Enum.GetName(typeof(CryptoKeyPayloadType), this.PayloadType));
				}
			}

			public string ToQueryStringFragment(NameValueCollection additionalParameters, bool includeHMAC = true)
			{
				NameValueCollection nameValueCollection = new NameValueCollection();
				if (this.KeyVersion > 1)
				{
					nameValueCollection.Add("pt", ((int)this.PayloadType).ToString(NumberFormatInfo.InvariantInfo));
				}
				else
				{
					CryptoKeyPayloadType payloadType = this.PayloadType;
					if (payloadType != CryptoKeyPayloadType.SvmFeedbackEncryption)
					{
						throw new InvalidOperationException(string.Format("Key of type {0} is not supported by legacy payloads, Key version must be greater than 1", Enum.GetName(typeof(CryptoKeyPayloadType), this.PayloadType)));
					}
					AuthkeyAuthenticationRequest.SignedUrl.LegacyPayloadType legacyPayloadType = AuthkeyAuthenticationRequest.SignedUrl.LegacyPayloadType.SvmFeedback;
					NameValueCollection nameValueCollection2 = nameValueCollection;
					string name = "pt";
					int num = (int)legacyPayloadType;
					nameValueCollection2.Add(name, num.ToString(NumberFormatInfo.InvariantInfo));
				}
				nameValueCollection.Add("kv", this.KeyVersion.ToString(NumberFormatInfo.InvariantInfo));
				nameValueCollection.Add("ts", this.UtcTimestamp.ToFileTimeUtc().ToString(NumberFormatInfo.InvariantInfo));
				nameValueCollection.Add("iv", AuthkeyAuthenticationRequest.UrlEncodeBase64String(Convert.ToBase64String(this.InitializationVector)));
				nameValueCollection.Add("authKey", AuthkeyAuthenticationRequest.UrlEncodeBase64String(Convert.ToBase64String(this.Payload)));
				if (includeHMAC && this.MessageAuthenticationCode != null)
				{
					nameValueCollection.Add("hmac", AuthkeyAuthenticationRequest.UrlEncodeBase64String(Convert.ToBase64String(this.MessageAuthenticationCode)));
				}
				if (additionalParameters != null)
				{
					nameValueCollection.Add(additionalParameters);
				}
				return AuthkeyAuthenticationRequest.ConstructQueryString(nameValueCollection);
			}

			public string ValidateHashAndDecryptPayload()
			{
				string[] hashComponents = new string[]
				{
					this.ToQueryStringFragment(null, false)
				};
				byte[] array = HashUtility.ComputeHash(hashComponents, this.HashTypeForPayload);
				if (array.Length == this.MessageAuthenticationCode.Length)
				{
					if (!array.Where((byte t, int i) => t != this.MessageAuthenticationCode[i]).Any<byte>())
					{
						return Encoding.UTF8.GetString(CryptoUtils.Decrypt(this.Payload, this.PayloadType, this.KeyVersion, this.InitializationVector));
					}
				}
				throw new SecurityException("failed to validate hash and decrypt payload");
			}

			public readonly byte SerializationVersion = 1;

			public readonly CryptoKeyPayloadType PayloadType;

			public readonly DateTime UtcTimestamp;

			public readonly byte[] InitializationVector;

			public readonly byte[] Payload;

			private byte keyVersion;

			public enum LegacyPayloadType
			{
				SvmFeedback
			}
		}
	}
}
