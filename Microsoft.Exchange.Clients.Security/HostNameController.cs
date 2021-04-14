using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Security
{
	public class HostNameController
	{
		public HostNameController(NameValueCollection appSettings)
		{
			this.deprecatedToNewHostNameMap = new Dictionary<string, string>();
			string text = appSettings["FlightedOwaEcpCanonicalHostName"];
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = appSettings["DeprecatedOwaEcpCanonicalHostName"];
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					text = text.ToLowerInvariant().Trim();
					foreach (string text3 in array)
					{
						this.deprecatedToNewHostNameMap[text3.ToLowerInvariant().Trim()] = text;
					}
				}
			}
		}

		public bool IsDeprecatedHostName(string hostName, out string newHostName)
		{
			newHostName = null;
			return !string.IsNullOrEmpty(hostName) && this.deprecatedToNewHostNameMap.TryGetValue(hostName.ToLowerInvariant(), out newHostName);
		}

		public bool TrySwitchOwaHostNameAndReturnPermanentRedirect(HttpContext context)
		{
			HttpRequest request = context.Request;
			Uri uri = request.GetRequestUrlEvenIfProxied();
			HttpResponse response = context.Response;
			HttpCookie httpCookie = request.Cookies.Get("HostSwitch");
			bool flag = httpCookie != null && httpCookie.Value == "1";
			string host = uri.Host;
			string host2 = null;
			if (flag && request.RequestType == "GET" && this.IsDeprecatedHostName(host, out host2) && this.IsOwaStartPageRequest(uri) && !this.HasNonRedirectableQueryParams(request.QueryString.AllKeys))
			{
				bool flag2 = OfflineClientRequestUtilities.IsRequestForAppCachedVersion(context);
				bool flag3 = OfflineClientRequestUtilities.IsRequestFromMOWAClient(request, request.UserAgent);
				if (!flag2 && !flag3)
				{
					uri = new UriBuilder(uri)
					{
						Host = host2
					}.Uri;
					this.RedirectPermanent(context, uri);
					return true;
				}
			}
			return false;
		}

		public void AddHostSwitchFlightEnabledCookie(HttpResponse response)
		{
			HttpCookie httpCookie = new HttpCookie("HostSwitch");
			httpCookie.HttpOnly = true;
			httpCookie.Path = "/";
			httpCookie.Value = "1";
			httpCookie.Expires = DateTime.UtcNow.AddMonths(1);
			response.Cookies.Add(httpCookie);
		}

		public virtual bool IsUserAgentExcludedFromHostNameSwitchFlight(HttpRequest request)
		{
			UserAgent userAgent = new UserAgent(request.UserAgent, false, request.Cookies);
			return OfflineClientRequestUtilities.IsRequestFromMOWAClient(request, request.UserAgent) || userAgent.IsIos || userAgent.IsAndroid;
		}

		protected virtual void RedirectPermanent(HttpContext context, Uri requestUri)
		{
			string text = requestUri.ToString();
			context.Response.Clear();
			context.Response.Cache.SetCacheability(HttpCacheability.Private);
			context.Response.Cache.SetMaxAge(HostNameController.PermanentRedirectToNewHostNameMaxAge);
			context.Response.AppendToLog("OwaHostChange301RedirectUri=" + text);
			context.Response.RedirectPermanent(text, true);
		}

		protected bool IsOwaStartPageRequest(Uri requestUri)
		{
			string[] segments = requestUri.Segments;
			int num = segments.Length;
			return num > 1 && num < 5 && segments[1].IndexOf("owa", StringComparison.OrdinalIgnoreCase) != -1 && (num <= 2 || !string.IsNullOrEmpty(UrlUtilities.ValidateFederatedDomainInURL(requestUri)) || segments[2].Contains("@")) && (num <= 3 || segments[3].Contains("@"));
		}

		protected bool HasNonRedirectableQueryParams(string[] queryParams)
		{
			foreach (string key in queryParams)
			{
				if (HostNameController.NonRedirectableQueryParams.ContainsKey(key))
				{
					return true;
				}
			}
			return false;
		}

		public const string DeprecatedHostNameConfigKey = "DeprecatedOwaEcpCanonicalHostName";

		public const string FlightedHostNameConfigKey = "FlightedOwaEcpCanonicalHostName";

		public const string HostNameFlightEnabledCookieName = "HostSwitch";

		public const string HostNameFlightEnabledCookieValue = "1";

		public const int HostNameChangeCookieValidityInMonths = 1;

		internal const string IISLogFieldPrefixFor301Redirects = "OwaHostChange301RedirectUri=";

		public static readonly TimeSpan PermanentRedirectToNewHostNameMaxAge = TimeSpan.FromHours(720.0);

		internal static readonly Dictionary<string, string> NonRedirectableQueryParams = new Dictionary<string, string>
		{
			{
				"authRedirect",
				"true"
			},
			{
				"ver",
				"true"
			},
			{
				"cver",
				"true"
			},
			{
				"bO",
				"true"
			},
			{
				"aC",
				"true"
			},
			{
				"flights",
				"true"
			}
		};

		private Dictionary<string, string> deprecatedToNewHostNameMap;
	}
}
