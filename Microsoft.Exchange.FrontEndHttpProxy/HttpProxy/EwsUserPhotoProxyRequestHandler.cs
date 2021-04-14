using System;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class EwsUserPhotoProxyRequestHandler : EwsProxyRequestHandler
	{
		protected override bool UseBackEndCacheForDownLevelServer
		{
			get
			{
				return false;
			}
		}

		internal static bool IsUserPhotoRequest(HttpRequest request)
		{
			return ProtocolHelper.IsEwsGetUserPhotoRequest(request.Path);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			string text = this.TryGetExplicitLogonNode(ExplicitLogonNode.Second);
			if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP");
				return new SmtpAnchorMailbox(text, this);
			}
			return base.ResolveAnchorMailbox();
		}

		protected override string TryGetExplicitLogonNode(ExplicitLogonNode node)
		{
			return base.ClientRequest.QueryString["email"];
		}

		protected override BackEndServer GetDownLevelClientAccessServer(AnchorMailbox anchorMailbox, BackEndServer mailboxServer)
		{
			BackEndServer deterministicBackEndServer = HttpProxyBackEndHelper.GetDeterministicBackEndServer<WebServicesService>(mailboxServer, anchorMailbox.ToCookieKey(), this.ClientAccessType);
			ExTraceGlobals.VerboseTracer.TraceDebug<int, BackEndServer, BackEndServer>((long)this.GetHashCode(), "[EwsUserPhotoProxyRequestHandler::GetDownLevelClientAccessServer] Context {0}; Overriding down level target {0} with latest version backend {1}.", base.TraceContext, mailboxServer, deterministicBackEndServer);
			return deterministicBackEndServer;
		}
	}
}
