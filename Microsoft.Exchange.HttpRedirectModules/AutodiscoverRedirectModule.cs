using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpRedirect
{
	public class AutodiscoverRedirectModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.BeginRequest += this.OnBeginRequest;
		}

		public void Dispose()
		{
		}

		internal static bool TryGetRedirectUri(Uri uri, string canonicalSecureHostName, out Uri redirectUri)
		{
			redirectUri = null;
			if (HttpRedirectCommon.UriIsHttps(uri))
			{
				return false;
			}
			redirectUri = new UriBuilder(uri)
			{
				Host = canonicalSecureHostName,
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
			if (string.IsNullOrEmpty(AutodiscoverRedirectModule.CanonicalSecureHostNameSetting.Value))
			{
				return;
			}
			if (AutodiscoverRedirectModule.TryGetRedirectUri(url, AutodiscoverRedirectModule.CanonicalSecureHostNameSetting.Value, out redirectUri))
			{
				HttpRedirectCommon.RedirectRequestToNewUri(httpApplication, HttpRedirectCommon.HttpRedirectType.Temporary, redirectUri, "AutodiscoverRedirectUri=");
			}
		}

		internal const string IISLogFieldPrefix = "AutodiscoverRedirectUri=";

		internal const string AutodiscoverCanonicalSecureHostNameKey = "AutodiscoverCanonicalSecureHostName";

		private static readonly StringAppSettingsEntry CanonicalSecureHostNameSetting = new StringAppSettingsEntry("AutodiscoverCanonicalSecureHostName", string.Empty, ExTraceGlobals.VerboseTracer);
	}
}
