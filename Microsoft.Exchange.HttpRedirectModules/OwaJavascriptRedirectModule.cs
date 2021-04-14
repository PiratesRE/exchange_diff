using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.HttpProxy;

namespace Microsoft.Exchange.HttpRedirect
{
	public class OwaJavascriptRedirectModule : IHttpModule
	{
		public static bool TryGetOwa302PathAndQuery(string requestPath, string requestPathAndQuery, out string owaPathAndQuery)
		{
			requestPath = (requestPath ?? string.Empty);
			requestPathAndQuery = (requestPathAndQuery ?? string.Empty);
			owaPathAndQuery = string.Empty;
			if (Regex.IsMatch(requestPath, "^/?[^./]+[.][^/]+([/][^./@]+[@][^./]+[.][^/]+)?/?$"))
			{
				string str = requestPathAndQuery.StartsWith("/") ? "/owa" : "/owa/";
				owaPathAndQuery = str + requestPathAndQuery;
			}
			return !string.IsNullOrEmpty(owaPathAndQuery);
		}

		public void Init(HttpApplication application)
		{
			if (!OwaJavascriptRedirectModule.staticInitialized)
			{
				bool flag = false;
				if (WebConfigurationManager.AppSettings["GallatinSplashPageEnabled"] != null)
				{
					string value = WebConfigurationManager.AppSettings["GallatinSplashPageEnabled"];
					bool.TryParse(value, out flag);
				}
				if (flag)
				{
					string text = WebConfigurationManager.AppSettings["GallatinSplashPagePath"];
					OwaJavascriptRedirectModule.gallatinSplashPageRequiredForUrl = (text ?? string.Empty);
				}
				OwaJavascriptRedirectModule.gallatinSplashPageEnabled = flag;
				OwaJavascriptRedirectModule.staticInitialized = true;
			}
			application.BeginRequest += this.OnBeginRequest;
		}

		public void Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				this.OnBeginRequestInternal((HttpApplication)sender);
			});
		}

		private void OnBeginRequestInternal(HttpApplication httpApplication)
		{
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			if (context.Request.IsSecureConnection)
			{
				string text = request.Url.AbsolutePath ?? string.Empty;
				string requestPathAndQuery = request.Url.PathAndQuery ?? string.Empty;
				string text2 = request.Url.AbsoluteUri ?? string.Empty;
				HttpResponse response = context.Response;
				if (OwaJavascriptRedirectModule.gallatinSplashPageEnabled && OwaJavascriptRedirectModule.gallatinSplashPageRequiredForUrl.Equals(text2, StringComparison.OrdinalIgnoreCase))
				{
					context.Server.TransferRequest("/owa/auth/outlookcn.aspx");
					return;
				}
				if (string.IsNullOrEmpty(text) || text == "/")
				{
					if (OfflineClientRequestUtilities.IsRequestForAppCachedVersion(context))
					{
						string arg = Uri.EscapeUriString(Uri.UnescapeDataString("/owa/" + context.Request.Url.Query));
						response.StatusCode = 200;
						response.ContentType = "text/html";
						response.Write(string.Format("<!DOCTYPE html><html><head><script type=\"text/javascript\">window.location.replace(\"{0}\" + window.location.hash);</script></head><body></body></html>", arg));
						response.AppendToLog("OwaJavascriptRedirectUri=/owa/");
						httpApplication.CompleteRequest();
						return;
					}
					HttpRedirectCommon.RedirectRequestToNewUri(httpApplication, HttpRedirectCommon.HttpRedirectType.Permanent, new UriBuilder(text2)
					{
						Path = "/owa/"
					}.Uri, "Owa301RedirectUri=");
					return;
				}
				else
				{
					string empty = string.Empty;
					if (OwaJavascriptRedirectModule.TryGetOwa302PathAndQuery(text, requestPathAndQuery, out empty))
					{
						response.AppendToLog("Owa302RedirectUri=" + empty);
						response.Redirect(empty);
					}
				}
			}
		}

		internal const string IISLogFieldPrefixForJavascriptRedirects = "OwaJavascriptRedirectUri=";

		internal const string IISLogFieldPrefixFor301Redirects = "Owa301RedirectUri=";

		internal const string IISLogFieldPrefixFor302Redirects = "Owa302RedirectUri=";

		internal const string RedirectPath = "/owa/";

		private const string ClientSideRedirectBody = "<!DOCTYPE html><html><head><script type=\"text/javascript\">window.location.replace(\"{0}\" + window.location.hash);</script></head><body></body></html>";

		private const string GallatinSplashPageEnabledAppSetting = "GallatinSplashPageEnabled";

		private const string GallatinSplashPagePathAppSetting = "GallatinSplashPagePath";

		private const string SplashPage = "/owa/auth/outlookcn.aspx";

		private static bool staticInitialized = false;

		private static bool gallatinSplashPageEnabled;

		private static string gallatinSplashPageRequiredForUrl;
	}
}
