using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal class EDiscoveryExportToolProxyRequestHandler : ProxyRequestHandler
	{
		internal static bool IsEDiscoveryExportToolProxyRequest(HttpRequest request)
		{
			return EDiscoveryExportToolRequestPathHandler.IsEDiscoveryExportToolRequest(request);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<string, Uri>((long)this.GetHashCode(), "[EDiscoveryExportToolProxyRequestHandler::ResolveAnchorMailbox]: Method {0}; Url {1};", base.ClientRequest.HttpMethod, base.ClientRequest.Url);
			string[] array = base.ClientRequest.Url.AbsolutePath.Split(new char[]
			{
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 5 && array[2] == "exporttool" && array[4].StartsWith("microsoft.exchange."))
			{
				this.serverFqdn = array[3];
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "EDiscoveryExportTool-ServerInfo");
				return new ServerInfoAnchorMailbox(this.serverFqdn, this);
			}
			this.serverFqdn = null;
			Match pathMatch = EDiscoveryExportToolRequestPathHandler.GetPathMatch(base.ClientRequest);
			ServerVersion serverVersion;
			if (pathMatch.Success && RegexUtilities.TryGetServerVersionFromRegexMatch(pathMatch, out serverVersion))
			{
				AnchorMailbox result = new ServerVersionAnchorMailbox<EcpService>(serverVersion, ClientAccessType.Internal, true, this);
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "EDiscoveryExportTool-ServerVersion");
				return result;
			}
			throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.ServerNotFound, string.Format("Unable to find target server for url: {0}", base.ClientRequest.Url));
		}

		protected override UriBuilder GetClientUrlForProxy()
		{
			UriBuilder uriBuilder = new UriBuilder(base.ClientRequest.Url);
			if (!string.IsNullOrEmpty(this.serverFqdn))
			{
				uriBuilder.Path = base.ClientRequest.Url.AbsolutePath.Replace("/" + this.serverFqdn + "/", "/");
			}
			return uriBuilder;
		}

		private string serverFqdn;
	}
}
