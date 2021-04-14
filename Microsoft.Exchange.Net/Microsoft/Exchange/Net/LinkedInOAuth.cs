using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedInOAuth
	{
		public LinkedInOAuth(ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.tracer = tracer;
		}

		public string GetAuthorizationHeader(string url, string httpMethod, NameValueCollection queryParameters, string accessToken, string accessSecret, string appId, string appSecret)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("url", url);
			ArgumentValidator.ThrowIfNullOrEmpty("httpMethod", httpMethod);
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			ArgumentValidator.ThrowIfNullOrEmpty("appSecret", appSecret);
			Uri url2 = new Uri(url);
			NameValueCollection oauthParameters = this.GetOAuthParameters(url2, queryParameters, httpMethod, appId, appSecret, null, accessToken, accessSecret, null);
			return this.BuildOAuthHeader(oauthParameters, string.Empty);
		}

		private static string NormalizeRequestParameters(NameValueCollection parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<LinkedInOAuth.QueryParameter> list = new List<LinkedInOAuth.QueryParameter>();
			foreach (string name in parameters.AllKeys)
			{
				LinkedInOAuth.QueryParameter item = new LinkedInOAuth.QueryParameter
				{
					Name = name,
					Value = parameters[name]
				};
				list.Add(item);
			}
			list.Sort(new LinkedInOAuth.QueryParameterComparer());
			for (int j = 0; j < list.Count; j++)
			{
				LinkedInOAuth.QueryParameter queryParameter = list[j];
				stringBuilder.AppendFormat("{0}={1}", queryParameter.Name, queryParameter.Value);
				if (j < list.Count - 1)
				{
					stringBuilder.Append("&");
				}
			}
			return stringBuilder.ToString();
		}

		private static string UrlEncode(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in value)
			{
				if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~".IndexOf(c) != -1)
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append('%');
					stringBuilder.AppendFormat("{0:X2}", (int)c);
				}
			}
			return stringBuilder.ToString();
		}

		private string CreateSignature(Uri uri, string httpMethod, NameValueCollection requestParameters, string consumerSecret, string tokenSecret)
		{
			string value = LinkedInOAuth.NormalizeUrl(uri);
			string value2 = LinkedInOAuth.NormalizeRequestParameters(requestParameters);
			string text = string.Format("{0}&{1}&{2}", httpMethod.ToUpperInvariant(), LinkedInOAuth.UrlEncode(value), LinkedInOAuth.UrlEncode(value2));
			this.tracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "LinkedIn OAuth: signature base: {0} with consumer secret (hash): {1:X8}; token secret (hash): {2:X8}", text, (consumerSecret != null) ? consumerSecret.GetHashCode() : 0, (tokenSecret != null) ? tokenSecret.GetHashCode() : 0);
			string result;
			using (HMACSHA1 hmacsha = new HMACSHA1())
			{
				hmacsha.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", LinkedInOAuth.UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : LinkedInOAuth.UrlEncode(tokenSecret)));
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				byte[] inArray = hmacsha.ComputeHash(bytes);
				result = Convert.ToBase64String(inArray);
			}
			return result;
		}

		private static string GetTimeStamp()
		{
			return Convert.ToInt64((DateTime.UtcNow - LinkedInOAuth.EpochUtc).TotalSeconds).ToString();
		}

		private static string GetNonce()
		{
			Random random = new Random();
			return random.Next().ToString();
		}

		private static string NormalizeUrl(Uri url)
		{
			string text = string.Format("{0}://{1}", url.Scheme, url.Host);
			if ((!string.Equals(url.Scheme, "http", StringComparison.OrdinalIgnoreCase) || url.Port != 80) && (!string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase) || url.Port != 443))
			{
				text = text + ":" + url.Port;
			}
			return text + url.AbsolutePath;
		}

		internal NameValueCollection GetOAuthParameters(Uri url, NameValueCollection queryParameters, string httpMethod, string consumerKey, string consumerSecret, string tokenVerifier, string token, string tokenSecret, string callbackEndPoint)
		{
			if (url == null)
			{
				throw new ArgumentException("url");
			}
			if (string.IsNullOrEmpty(httpMethod))
			{
				throw new ArgumentNullException("httpMethod");
			}
			if (string.IsNullOrEmpty(consumerKey))
			{
				throw new ArgumentNullException("consumerKey");
			}
			if (string.IsNullOrEmpty(consumerSecret))
			{
				throw new ArgumentNullException("consumerSecret");
			}
			new NameValueCollection();
			string timeStamp = LinkedInOAuth.GetTimeStamp();
			string nonce = LinkedInOAuth.GetNonce();
			NameValueCollection nameValueCollection;
			if (queryParameters != null)
			{
				nameValueCollection = new NameValueCollection(queryParameters);
			}
			else
			{
				nameValueCollection = new NameValueCollection();
			}
			nameValueCollection.Add("oauth_timestamp", timeStamp);
			nameValueCollection.Add("oauth_nonce", nonce);
			nameValueCollection.Add("oauth_version", "1.0");
			nameValueCollection.Add("oauth_signature_method", "HMAC-SHA1");
			nameValueCollection.Add("oauth_consumer_key", consumerKey);
			if (!string.IsNullOrEmpty(tokenVerifier))
			{
				nameValueCollection.Add("oauth_verifier", tokenVerifier);
			}
			if (!string.IsNullOrEmpty(token))
			{
				nameValueCollection.Add("oauth_token", token);
			}
			if (!string.IsNullOrEmpty(callbackEndPoint))
			{
				string value = LinkedInOAuth.UrlEncode(callbackEndPoint);
				nameValueCollection.Add("oauth_callback", value);
			}
			string text = this.CreateSignature(url, httpMethod, nameValueCollection, consumerSecret, tokenSecret);
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "LinkedIn OAuth: signature computed BEFORE URL-encoding: {0}", text);
			text = LinkedInOAuth.UrlEncode(text);
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "LinkedIn OAuth: signature AFTER URL-encoding: {0}", text);
			nameValueCollection.Add("oauth_signature", text);
			return nameValueCollection;
		}

		internal string BuildOAuthHeader(NameValueCollection oauthParameters, string realm)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("OAuth ");
			if (!string.IsNullOrEmpty(realm))
			{
				stringBuilder.Append("realm=\"" + realm + "\" ");
			}
			for (int i = 0; i < oauthParameters.Count; i++)
			{
				string key = oauthParameters.GetKey(i);
				if (key.StartsWith("oauth_"))
				{
					string value = key + "=\"" + oauthParameters[key] + "\"";
					stringBuilder.Append(value);
					if (i < oauthParameters.Count - 1)
					{
						stringBuilder.Append(",");
					}
				}
			}
			return stringBuilder.ToString();
		}

		private const string OAuthVersion = "1.0";

		private const string OAuthParameterPrefix = "oauth_";

		private const string OAuthConsumerKeyKey = "oauth_consumer_key";

		private const string OAuthVersionKey = "oauth_version";

		private const string OAuthSignatureMethodKey = "oauth_signature_method";

		private const string OAuthSignatureKey = "oauth_signature";

		private const string OAuthTimestampKey = "oauth_timestamp";

		private const string OAuthNonceKey = "oauth_nonce";

		private const string OAuthTokenKey = "oauth_token";

		private const string OAuthTokenVerifier = "oauth_verifier";

		private const string OAuthCallback = "oauth_callback";

		private const string HMACSHA1SignatureType = "HMAC-SHA1";

		private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		private readonly ITracer tracer;

		private static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private class QueryParameter
		{
			public string Name { get; set; }

			public string Value { get; set; }
		}

		private class QueryParameterComparer : IComparer<LinkedInOAuth.QueryParameter>
		{
			public int Compare(LinkedInOAuth.QueryParameter x, LinkedInOAuth.QueryParameter y)
			{
				if (x.Name == y.Name)
				{
					return string.Compare(x.Value, y.Value);
				}
				return string.Compare(x.Name, y.Name);
			}
		}
	}
}
