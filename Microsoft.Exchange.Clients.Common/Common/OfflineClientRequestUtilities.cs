using System;
using System.Web;

namespace Microsoft.Exchange.Clients.Common
{
	public class OfflineClientRequestUtilities
	{
		public static bool? IsRequestForOfflineAppcacheManifest(HttpRequest request)
		{
			HttpCookie httpCookie = request.Cookies.Get("offline");
			if (httpCookie == null || httpCookie.Value == null)
			{
				return null;
			}
			string value = httpCookie.Value;
			if (value == "1")
			{
				return new bool?(true);
			}
			if (value == "0")
			{
				return new bool?(false);
			}
			return null;
		}

		public static bool IsRequestFromOfflineClient(HttpRequest request)
		{
			HttpCookie httpCookie = request.Cookies.Get("X-Offline");
			return (httpCookie != null && httpCookie.Value == "1") || OfflineClientRequestUtilities.IsRequestFromMOWAClient(request, request.UserAgent);
		}

		public static bool IsRequestFromMOWAClient(HttpRequest request, string userAgent)
		{
			if (request != null && request.Cookies != null && request.Cookies["PALEnabled"] != null)
			{
				return request.Cookies["PALEnabled"].Value != "-1";
			}
			return request.QueryString["palenabled"] == "1" || (userAgent != null && userAgent.Contains("MSAppHost")) || request.Headers["X-OWA-Protocol"] == "MOWA";
		}

		public static bool IsRequestForAppCachedVersion(HttpContext context)
		{
			HttpCookie httpCookie = context.Request.Cookies["IsClientAppCacheEnabled"];
			return httpCookie != null && httpCookie.Value.Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		public const string PalEnabledCookie = "PALEnabled";

		public const string IsClientAppCacheEnabledCookieName = "IsClientAppCacheEnabled";

		internal const string OfflineManifestCookie = "offline";

		internal const string OfflineCookie = "X-Offline";

		internal const string PALEnabledQueryStringConstant = "palenabled";

		internal const string MSAppHostConstant = "MSAppHost";

		internal const string OwaProtocolHeaderName = "X-OWA-Protocol";

		internal const string MOWA = "MOWA";
	}
}
