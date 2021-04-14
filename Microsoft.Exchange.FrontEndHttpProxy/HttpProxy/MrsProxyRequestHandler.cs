using System;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class MrsProxyRequestHandler : BEServerCookieProxyRequestHandler<WebServicesService>
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.InternalNLBBypass;
			}
		}

		protected override bool UseBackEndCacheForDownLevelServer
		{
			get
			{
				return false;
			}
		}

		internal static bool IsMrsRequest(HttpRequest request)
		{
			string[] segments = request.Url.Segments;
			if (segments == null || segments.Length != 3)
			{
				return false;
			}
			string text = segments[2].TrimEnd(new char[]
			{
				'/'
			});
			if (!text.Equals("MRSProxy.svc", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (!MrsProxyRequestHandler.IsMrsProxyEnabled())
			{
				throw new HttpException(403, "MRS proxy service is disabled");
			}
			return true;
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			string text = base.ClientRequest.Headers[Constants.TargetDatabaseHeaderName];
			if (!string.IsNullOrEmpty(text))
			{
				Guid databaseGuid;
				if (Guid.TryParse(text, out databaseGuid))
				{
					base.Logger.Set(HttpProxyMetadata.RoutingHint, "TargetDatabase-GUID");
					return new DatabaseGuidAnchorMailbox(databaseGuid, this);
				}
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "TargetDatabase-Name");
				return new DatabaseNameAnchorMailbox(text, this);
			}
			else
			{
				AnchorMailbox anchorMailbox = base.CreateAnchorMailboxFromRoutingHint();
				if (anchorMailbox != null)
				{
					return anchorMailbox;
				}
				if (Utilities.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					base.Logger.Set(HttpProxyMetadata.RoutingHint, "ClientVersionHeader");
					return base.GetServerVersionAnchorMailbox(base.ClientRequest.Headers[Constants.ClientVersionHeaderName]);
				}
				string text2 = base.ClientRequest.Headers[WellKnownHeader.GenericAnchorHint];
				if (!string.IsNullOrEmpty(text2))
				{
					return new PstProviderAnchorMailbox(text2, this);
				}
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "ForestWideOrganization");
				return new OrganizationAnchorMailbox(OrganizationId.ForestWideOrgId, this);
			}
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri targetBackEndServerUrl = base.GetTargetBackEndServerUrl();
			UriBuilder uriBuilder = new UriBuilder(targetBackEndServerUrl);
			if (targetBackEndServerUrl.Port == 444)
			{
				uriBuilder.Port = 443;
			}
			uriBuilder.Path = "/Microsoft.Exchange.MailboxReplicationService.ProxyService";
			return uriBuilder.Uri;
		}

		protected override BackEndServer GetDownLevelClientAccessServer(AnchorMailbox anchorMailbox, BackEndServer mailboxServer)
		{
			BackEndServer deterministicBackEndServer = HttpProxyBackEndHelper.GetDeterministicBackEndServer<WebServicesService>(mailboxServer, anchorMailbox.ToCookieKey(), this.ClientAccessType);
			ExTraceGlobals.VerboseTracer.TraceDebug<int, BackEndServer, BackEndServer>((long)this.GetHashCode(), "[MrsProxyRequestHandler::GetDownLevelClientAccessServer] Context {0}; Overriding down level target {0} with latest version backend {1}.", base.TraceContext, mailboxServer, deterministicBackEndServer);
			return deterministicBackEndServer;
		}

		private static bool IsMrsProxyEnabled()
		{
			bool? flag = null;
			ADWebServicesVirtualDirectory adwebServicesVirtualDirectory = (ADWebServicesVirtualDirectory)HttpProxyGlobals.VdirObject.Member;
			flag = new bool?(adwebServicesVirtualDirectory.MRSProxyEnabled);
			if (flag == null)
			{
				ExTraceGlobals.VerboseTracer.TraceError(0L, "[MrsProxyRequestHandler::IsMrsProxyEnabled] Can not find vdir.");
			}
			return flag != null && flag.Value;
		}

		private const string BackEndMrsProxyPath = "/Microsoft.Exchange.MailboxReplicationService.ProxyService";

		private const string FrontEndMrsProxyPath = "MRSProxy.svc";
	}
}
