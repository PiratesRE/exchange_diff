using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class BEResourceRequestHandler : ProxyRequestHandler
	{
		internal static bool CanHandle(HttpRequest httpRequest)
		{
			return !string.IsNullOrEmpty(BEResourceRequestHandler.GetBEResouceCookie(httpRequest)) && BEResourceRequestHandler.IsResourceRequest(httpRequest.Url.LocalPath);
		}

		internal static bool IsResourceRequest(string localPath)
		{
			return localPath.EndsWith(Constants.ExtensionAxd, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionChromeWebApp, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionCss, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionEot, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionGif, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionJpg, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionJs, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionHtm, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionHtml, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionICO, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionManifest, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionMp3, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionMSI, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionPng, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionSvg, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionTtf, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionWav, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(Constants.ExtensionWoff, StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".bin", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".dat", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".flt", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".mui", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".xap", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".skin", StringComparison.OrdinalIgnoreCase);
		}

		protected override bool ShouldBackendRequestBeAnonymous()
		{
			return true;
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			string beresouceCookie = BEResourceRequestHandler.GetBEResouceCookie(base.ClientRequest);
			if (!string.IsNullOrEmpty(beresouceCookie))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, Constants.BEResource + "-Cookie");
				ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[BEResourceRequestHanlder::ResolveAnchorMailbox]: BEResource cookie used: {0}; context {1}.", beresouceCookie, base.TraceContext);
				return new ServerInfoAnchorMailbox(BackEndServer.FromString(beresouceCookie), this);
			}
			return base.ResolveAnchorMailbox();
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			base.AddProtocolSpecificHeadersToServerRequest(headers);
			if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && HttpProxyGlobals.ProtocolType == ProtocolType.Ecp && base.ProxyToDownLevel)
			{
				EcpProxyRequestHandler.AddDownLevelProxyHeaders(headers, base.HttpContext);
			}
		}

		private static string GetBEResouceCookie(HttpRequest httpRequest)
		{
			string result = null;
			HttpCookie httpCookie = httpRequest.Cookies[Constants.BEResource];
			if (httpCookie != null)
			{
				result = httpCookie.Value;
			}
			return result;
		}
	}
}
