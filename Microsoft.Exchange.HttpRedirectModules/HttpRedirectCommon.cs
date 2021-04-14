using System;
using System.Net;
using System.Web;

namespace Microsoft.Exchange.HttpRedirect
{
	internal class HttpRedirectCommon
	{
		static HttpRedirectCommon()
		{
			if (!string.IsNullOrWhiteSpace(HttpRuntime.AppDomainAppVirtualPath))
			{
				HttpRedirectCommon.VirtualDirectoryNameLeadingSlash = HttpRuntime.AppDomainAppVirtualPath.Replace("'", "''");
				if (!HttpRedirectCommon.VirtualDirectoryNameLeadingSlash.StartsWith("/"))
				{
					HttpRedirectCommon.VirtualDirectoryNameLeadingSlash = "/" + HttpRedirectCommon.VirtualDirectoryNameLeadingSlash;
				}
			}
		}

		public static string VirtualDirectoryNameLeadingSlash { get; private set; }

		public static bool UriIsHttps(Uri uri)
		{
			return uri.Scheme == "https" && uri.Port == 443;
		}

		public static void RedirectRequestToNewUri(HttpApplication httpApplication, HttpRedirectCommon.HttpRedirectType httpRedirectType, Uri redirectUri, string logFieldPrefix)
		{
			HttpContext context = httpApplication.Context;
			HttpResponse response = context.Response;
			string text = redirectUri.ToString();
			HttpStatusCode statusCode = (httpRedirectType == HttpRedirectCommon.HttpRedirectType.Permanent) ? HttpStatusCode.MovedPermanently : HttpStatusCode.Found;
			string statusDescription = (httpRedirectType == HttpRedirectCommon.HttpRedirectType.Permanent) ? "Moved Permanently" : "Moved Temporarily";
			response.Clear();
			response.StatusCode = (int)statusCode;
			response.StatusDescription = statusDescription;
			response.AddHeader("Location", text);
			response.AddHeader("Connection", "close");
			response.AddHeader("Cache-Control", "no-cache");
			response.AddHeader("Pragma", "no-cache");
			response.AppendToLog(logFieldPrefix + text);
			httpApplication.CompleteRequest();
		}

		public const string MovedPermanentlyHttpStatusText = "Moved Permanently";

		public const string MovedTemporarilyHttpStatusText = "Moved Temporarily";

		public const string HttpsScheme = "https";

		public const string LocationHttpHeaderName = "Location";

		public const string ConnectionHttpHeaderName = "Connection";

		public const string CacheControlHttpHeaderName = "Cache-Control";

		public const string PragmaHttpHeaderName = "Pragma";

		public const string NoCacheHttpHeaderValue = "no-cache";

		public const string CloseHttpHeaderValue = "close";

		public const int DefaultWebSiteSslPort = 443;

		public const string ForwardSlash = "/";

		public enum HttpRedirectType
		{
			Permanent,
			Temporary
		}
	}
}
