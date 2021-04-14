using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Common
{
	public sealed class Canary15Cookie
	{
		private Canary15Cookie(Canary15 canary, Canary15Profile profile)
		{
			this.profile = profile;
			this.Canary = canary;
			this.domain = string.Empty;
			this.HttpCookie = new HttpCookie(this.profile.Name, this.Value);
			this.HttpCookie.Domain = this.Domain;
			this.HttpCookie.Path = this.profile.Path;
			this.NetCookie = new Cookie(this.profile.Name, this.Value, this.profile.Path, this.Domain);
			this.HttpCookie.Secure = true;
			this.NetCookie.Secure = true;
			this.HttpCookie.HttpOnly = false;
			this.NetCookie.HttpOnly = false;
		}

		public Canary15Cookie(string logOnUniqueKey, Canary15Profile profile) : this(new Canary15(logOnUniqueKey), profile)
		{
		}

		public string Value
		{
			get
			{
				return this.Canary.ToString();
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
			set
			{
				this.domain = value;
				this.HttpCookie.Domain = this.domain;
				this.NetCookie.Domain = this.domain;
			}
		}

		public bool IsRenewed
		{
			get
			{
				return this.Canary.IsRenewed;
			}
		}

		public bool IsAboutToExpire
		{
			get
			{
				return this.Canary.IsAboutToExpire;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.Canary.CreationTime;
			}
		}

		public string LogData
		{
			get
			{
				return this.Canary.LogData;
			}
		}

		public HttpCookie HttpCookie { get; private set; }

		public Cookie NetCookie { get; private set; }

		internal Canary15 Canary { get; private set; }

		public static Canary15Cookie TryCreateFromHttpContext(HttpContext httpContext, string logOnUniqueKey, Canary15Profile profile)
		{
			HttpCookie cookie = httpContext.Request.Cookies.Get(profile.Name);
			return Canary15Cookie.TryCreateFromHttpCookie(cookie, logOnUniqueKey, profile);
		}

		public static bool ValidateCanaryInHeaders(HttpContext httpContext, string userSid, Canary15Profile profile, out Canary15Cookie.CanaryValidationResult result)
		{
			string text = httpContext.Request.Headers[profile.Name];
			bool flag = true;
			if (Canary15.RestoreCanary15(text, userSid) != null)
			{
				result = Canary15Cookie.CanaryValidationResult.HeaderMatch;
			}
			else
			{
				string text2;
				try
				{
					string components = httpContext.Request.Url.GetComponents(UriComponents.Query, UriFormat.Unescaped);
					string query = HttpUtility.HtmlDecode(components);
					NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(query);
					text2 = nameValueCollection[profile.Name];
				}
				catch
				{
					text2 = null;
				}
				if (Canary15.RestoreCanary15(text2, userSid) != null)
				{
					result = Canary15Cookie.CanaryValidationResult.UrlParameterMatch;
				}
				else
				{
					string text3 = httpContext.Request.Form[profile.Name];
					if (Canary15.RestoreCanary15(text3, userSid) != null)
					{
						result = Canary15Cookie.CanaryValidationResult.FormParameterMatch;
					}
					else
					{
						flag = false;
						result = Canary15Cookie.CanaryValidationResult.NotFound;
						if (ExTraceGlobals.CoreCallTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int i = 0; i < httpContext.Request.Cookies.Count; i++)
							{
								HttpCookie httpCookie = httpContext.Request.Cookies.Get(i);
								if (string.Equals(httpCookie.Name, profile.Name, StringComparison.OrdinalIgnoreCase))
								{
									stringBuilder.AppendFormat("[{0}]", httpCookie.Value);
								}
							}
							ExTraceGlobals.CoreTracer.TraceDebug(11L, "Canary15Cookie='{0}',HttpHeader.Canary='{1}', UrlParam.Canary='{2}', Form.Canary='{3}', success={4}, result={5}", new object[]
							{
								stringBuilder.ToString(),
								text,
								text2,
								text3,
								flag,
								result.ToString()
							});
						}
					}
				}
			}
			return flag;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}={1}", new object[]
			{
				this.profile.Name,
				this.Value
			});
		}

		public string ToLoggerString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}", this.HttpCookie.Value);
			return stringBuilder.ToString();
		}

		private static Canary15Cookie Create(Canary15 canary, Canary15Profile profile)
		{
			if (canary == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(20L, "Canary == null");
				return null;
			}
			return new Canary15Cookie(canary, profile);
		}

		private static Canary15Cookie TryCreateFromHttpCookie(HttpCookie cookie, string logonUniqueKey, Canary15Profile profile)
		{
			string text = null;
			Canary15 canary = null;
			if (cookie == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(21L, "Http cookie is null, Name={0}", profile.Name);
			}
			else if (string.IsNullOrEmpty(cookie.Value))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, string, string>(21L, "Http cookie value is null, Name={0}, Domain={1}, Path={2}", cookie.Name, cookie.Domain, cookie.Path);
			}
			else if (!Canary15Cookie.TryGetCookieValue(cookie.Value, out text))
			{
				ExTraceGlobals.CoreTracer.TraceDebug(21L, "TryParseCookeValue failed, Name={0}, Domain={1}, Path={2}, Value={3}", new object[]
				{
					cookie.Name,
					cookie.Domain,
					cookie.Path,
					cookie.Value
				});
			}
			else
			{
				canary = Canary15.RestoreCanary15(text, logonUniqueKey);
			}
			if (canary == null)
			{
				if (cookie != null)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(21L, "restoredCanary==null, Name={0}, Domain={1}, Path={2}, Value={3}, canaryString={4}, logonUniqueKey={5}", new object[]
					{
						cookie.Name,
						cookie.Domain,
						cookie.Path,
						cookie.Value,
						text,
						logonUniqueKey
					});
				}
				canary = new Canary15(logonUniqueKey);
				ExTraceGlobals.CoreTracer.TraceDebug<string, string, string>(21L, "Canary is recreated, userContextId={0}, logonUniqueKey={1}, canaryString={2}", canary.UserContextId, canary.LogonUniqueKey, canary.ToString());
			}
			return Canary15Cookie.Create(canary, profile);
		}

		private static bool TryGetCookieValue(string cookieValue, out string canaryString)
		{
			if (string.IsNullOrEmpty(cookieValue) || cookieValue.Length != 76)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, int>(22L, "Invalid format cookie={0}, cookieValue.Length={1}", cookieValue, (cookieValue == null) ? -1 : cookieValue.Length);
				canaryString = null;
				return false;
			}
			canaryString = cookieValue;
			return true;
		}

		internal const int UserContextCanaryLength = 76;

		private const int GuidLength = 32;

		private string domain;

		private Canary15Profile profile;

		public enum CanaryValidationResult
		{
			NotFound,
			HeaderMatch,
			UrlParameterMatch,
			FormParameterMatch
		}
	}
}
