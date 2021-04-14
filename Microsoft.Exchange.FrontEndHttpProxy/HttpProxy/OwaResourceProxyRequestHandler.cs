using System;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OwaResourceProxyRequestHandler : ProxyRequestHandler
	{
		protected override bool WillAddProtocolSpecificCookiesToClientResponse
		{
			get
			{
				return true;
			}
		}

		internal static bool CanHandle(HttpRequest httpRequest)
		{
			HttpCookie httpCookie = httpRequest.Cookies[Constants.AnonResource];
			return httpCookie != null && string.Compare(httpCookie.Value, "true", CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0 && BEResourceRequestHandler.IsResourceRequest(httpRequest.Url.LocalPath);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			HttpCookie httpCookie = base.ClientRequest.Cookies[Constants.AnonResourceBackend];
			if (httpCookie != null)
			{
				this.savedBackendServer = httpCookie.Value;
			}
			if (!string.IsNullOrEmpty(this.savedBackendServer))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, Constants.AnonResourceBackend + "-Cookie");
				ExTraceGlobals.VerboseTracer.TraceDebug<HttpCookie, int>((long)this.GetHashCode(), "[OwaResourceProxyRequestHandler::ResolveAnchorMailbox]: AnonResourceBackend cookie used: {0}; context {1}.", httpCookie, base.TraceContext);
				return new ServerInfoAnchorMailbox(BackEndServer.FromString(this.savedBackendServer), this);
			}
			return new AnonymousAnchorMailbox(this);
		}

		protected override bool ShouldBackendRequestBeAnonymous()
		{
			return true;
		}

		protected override void CopySupplementalCookiesToClientResponse()
		{
			string text = null;
			if (base.AnchoredRoutingTarget != null && base.AnchoredRoutingTarget.BackEndServer != null)
			{
				text = base.AnchoredRoutingTarget.BackEndServer.ToString();
			}
			if (!string.IsNullOrEmpty(text) && this.savedBackendServer != text)
			{
				HttpCookie httpCookie = new HttpCookie(Constants.AnonResourceBackend, text);
				httpCookie.HttpOnly = true;
				httpCookie.Secure = base.ClientRequest.IsSecureConnection;
				base.ClientResponse.Cookies.Add(httpCookie);
			}
			base.CopySupplementalCookiesToClientResponse();
		}

		private string savedBackendServer;
	}
}
