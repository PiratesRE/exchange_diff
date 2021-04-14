using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class ExCookieContainer
	{
		public string GetCookieHeader(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			CookieCollection cookieCollection = this.GetCookies(uri);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in cookieCollection)
			{
				Cookie cookie = (Cookie)obj;
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append("; ");
				}
				if (cookie.Version != 0)
				{
					stringBuilder.AppendFormat("$Version={0}; ", cookie.Version);
				}
				stringBuilder.AppendFormat("{0}={1}", cookie.Name, cookie.Value);
				if (cookie.Path != null)
				{
					stringBuilder.AppendFormat("; $Path={0}", cookie.Path);
				}
			}
			return stringBuilder.ToString();
		}

		public CookieCollection GetCookies(Uri uri)
		{
			CookieCollection cookieCollection = new CookieCollection();
			foreach (Cookie cookie in this.cookies.Values)
			{
				if (uri.Host.EndsWith(cookie.Domain, StringComparison.OrdinalIgnoreCase) && !cookie.Expired && (cookie.Path == null || uri.AbsolutePath.StartsWith(cookie.Path, StringComparison.OrdinalIgnoreCase)))
				{
					cookieCollection.Add(cookie);
				}
			}
			return cookieCollection;
		}

		public void StoreCookies(HttpWebResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			List<Cookie> list = this.ParseCookies(response.ResponseUri, response.Headers["Set-Cookie"]);
			if (list == null)
			{
				return;
			}
			foreach (Cookie cookie in list)
			{
				this.Add(cookie);
			}
		}

		public void Add(Uri requestUri, Cookie cookie)
		{
			cookie.Domain = requestUri.Host;
			cookie.Path = requestUri.AbsolutePath;
			this.Add(cookie);
		}

		public void Add(Cookie cookie)
		{
			string cookieKey = this.GetCookieKey(cookie);
			if (this.cookies.ContainsKey(cookieKey))
			{
				this.cookies[cookieKey] = cookie;
				return;
			}
			this.cookies.Add(cookieKey, cookie);
		}

		private List<Cookie> ParseCookies(Uri uri, string setCookieHeader)
		{
			if (string.IsNullOrEmpty(setCookieHeader))
			{
				return null;
			}
			List<Cookie> list = new List<Cookie>();
			try
			{
				MatchCollection matchCollection = ExCookieContainer.cookieParsingRegex.Matches(setCookieHeader);
				if (matchCollection != null)
				{
					foreach (object obj in matchCollection)
					{
						Match match = (Match)obj;
						list.Add(this.ParseCookie(uri, match));
					}
				}
			}
			catch (Exception innerException)
			{
				throw new Exception(string.Format("ExCookieContainer failed to parse cookies. The Set-Cookie header of Uri '{0}' is: '{1}'.", uri, setCookieHeader), innerException);
			}
			return list;
		}

		private Cookie ParseCookie(Uri uri, Match match)
		{
			Cookie cookie = new Cookie();
			Func<string, string, string> parsingDelegate = (string s, string d) => s;
			Func<string, DateTime, DateTime> parsingDelegate2 = delegate(string s, DateTime d)
			{
				DateTime result;
				if (!DateTime.TryParse(s, out result))
				{
					return d;
				}
				return result;
			};
			Func<string, bool, bool> parsingDelegate3 = delegate(string s, bool d)
			{
				bool result;
				if (!bool.TryParse(s, out result))
				{
					return d;
				}
				return result;
			};
			Func<string, int, int> parsingDelegate4 = delegate(string s, int d)
			{
				int result;
				if (!int.TryParse(s, out result))
				{
					return d;
				}
				return result;
			};
			cookie.Name = this.ParseValueFromMatch<string>(match, "Name", null, parsingDelegate, null);
			cookie.Value = this.ParseValueFromMatch<string>(match, "Value", null, parsingDelegate, null);
			cookie.Domain = this.ParseValueFromMatch<string>(match, "Domain", uri.Host, parsingDelegate, null);
			cookie.Path = this.ParseValueFromMatch<string>(match, "Path", null, parsingDelegate, null);
			cookie.Version = this.ParseValueFromMatch<int>(match, "Version", 0, parsingDelegate4, 0);
			cookie.Expires = this.ParseValueFromMatch<DateTime>(match, "Expires", DateTime.MinValue, parsingDelegate2, default(DateTime));
			cookie.Secure = this.ParseValueFromMatch<bool>(match, "Secure", false, parsingDelegate3, true);
			cookie.HttpOnly = this.ParseValueFromMatch<bool>(match, "HttpOnly", false, parsingDelegate3, true);
			return cookie;
		}

		private T ParseValueFromMatch<T>(Match match, string matchResultName, T defaultValue, Func<string, T, T> parsingDelegate, T parameterPresentValue = default(T))
		{
			string text = match.Result("${" + matchResultName + "}");
			text = text.Trim();
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			if (typeof(T) != typeof(bool))
			{
				return parsingDelegate(text, defaultValue);
			}
			string text2 = match.Result("${" + matchResultName + "Value}");
			text2 = text2.Trim();
			if (string.IsNullOrEmpty(text2))
			{
				return parameterPresentValue;
			}
			text2 = text2.Substring(1);
			if (string.IsNullOrEmpty(text2))
			{
				return defaultValue;
			}
			return parsingDelegate(text2, defaultValue);
		}

		private string GetCookieKey(Cookie cookie)
		{
			return cookie.Name + "_" + cookie.Domain;
		}

		private const string cookieParsingRegexPattern = "(?<Name>[^\\s,][^=]*)=(?<Value>[^;]*)\\s*[;]*((\\s*expires=(?<Expires>[^;,]*,[^;,]*)[;,]?)|(\\s*domain=(?<Domain>[^;,]*)[;,]?)|(\\s*version=(?<Version>[^;,]*)[;,]?)|(\\s*path=(?<Path>[^;,]*)[;,]?)|(\\s*(?<HttpOnly>HTTPOnly)(?<HttpOnlyValue>=?[^;,]*)[;,]?)|(\\s*(?<Secure>Secure)(?<SecureValue>=?[^;,]*)[;,]?))*";

		private Dictionary<string, Cookie> cookies = new Dictionary<string, Cookie>(StringComparer.OrdinalIgnoreCase);

		private static readonly Regex cookieParsingRegex = new Regex("(?<Name>[^\\s,][^=]*)=(?<Value>[^;]*)\\s*[;]*((\\s*expires=(?<Expires>[^;,]*,[^;,]*)[;,]?)|(\\s*domain=(?<Domain>[^;,]*)[;,]?)|(\\s*version=(?<Version>[^;,]*)[;,]?)|(\\s*path=(?<Path>[^;,]*)[;,]?)|(\\s*(?<HttpOnly>HTTPOnly)(?<HttpOnlyValue>=?[^;,]*)[;,]?)|(\\s*(?<Secure>Secure)(?<SecureValue>=?[^;,]*)[;,]?))*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
