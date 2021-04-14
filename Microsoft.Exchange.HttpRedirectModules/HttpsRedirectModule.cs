using System;
using System.Web;
using Microsoft.Exchange.HttpProxy;

namespace Microsoft.Exchange.HttpRedirect
{
	public class HttpsRedirectModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.BeginRequest += this.OnBeginRequest;
		}

		public void Dispose()
		{
		}

		internal static bool TryGetRedirectUri(Uri uri, out Uri redirectUri)
		{
			redirectUri = null;
			if (HttpRedirectCommon.UriIsHttps(uri))
			{
				return false;
			}
			redirectUri = new UriBuilder(uri)
			{
				Scheme = "https",
				Port = 443
			}.Uri;
			return true;
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
			Uri url = context.Request.Url;
			Uri redirectUri = null;
			if (HttpsRedirectModule.TryGetRedirectUri(url, out redirectUri))
			{
				HttpRedirectCommon.RedirectRequestToNewUri(httpApplication, HttpRedirectCommon.HttpRedirectType.Permanent, redirectUri, "HttpsRedirectUri=");
			}
		}

		internal const string IISLogFieldPrefix = "HttpsRedirectUri=";
	}
}
