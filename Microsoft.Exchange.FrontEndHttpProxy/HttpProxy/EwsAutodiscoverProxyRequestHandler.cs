using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class EwsAutodiscoverProxyRequestHandler : BEServerCookieProxyRequestHandler<WebServicesService>
	{
		protected bool PreferAnchorMailboxHeader
		{
			get
			{
				return this.preferAnchorMailboxHeader;
			}
			set
			{
				this.preferAnchorMailboxHeader = value;
			}
		}

		protected bool SkipTargetBackEndCalculation
		{
			get
			{
				return this.skipTargetBackEndCalculation;
			}
			set
			{
				this.skipTargetBackEndCalculation = value;
			}
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			if (this.skipTargetBackEndCalculation)
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "OrgRelationship-Anonymous");
				return new AnonymousAnchorMailbox(this);
			}
			string text;
			if (ProtocolHelper.IsAutodiscoverV2PreviewRequest(base.ClientRequest.Url.AbsolutePath))
			{
				text = base.ClientRequest.Params["Email"];
			}
			else if (ProtocolHelper.IsAutodiscoverV2Version1Request(base.ClientRequest.Url.AbsolutePath))
			{
				int num = base.ClientRequest.Url.AbsolutePath.LastIndexOf('/');
				text = base.ClientRequest.Url.AbsolutePath.Substring(num + 1);
			}
			else
			{
				text = this.TryGetExplicitLogonNode(ExplicitLogonNode.Second);
			}
			string text2;
			if (ProtocolHelper.TryGetValidNormalizedExplicitLogonAddress(text, out text2))
			{
				this.isExplicitLogonRequest = true;
				this.explicitLogonAddress = text;
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP");
				SmtpAnchorMailbox smtpAnchorMailbox = new SmtpAnchorMailbox(text2, this);
				if (this.preferAnchorMailboxHeader)
				{
					string text3 = base.ClientRequest.Headers[Constants.AnchorMailboxHeaderName];
					if (!string.IsNullOrEmpty(text3) && !StringComparer.OrdinalIgnoreCase.Equals(text3, text2) && SmtpAddress.IsValidSmtpAddress(text3))
					{
						base.Logger.Set(HttpProxyMetadata.RoutingHint, "AnchorMailboxHeader-SMTP");
						smtpAnchorMailbox = new SmtpAnchorMailbox(text3, this);
					}
				}
				if (ProtocolHelper.IsAutodiscoverV2Request(base.ClientRequest.Url.AbsolutePath))
				{
					smtpAnchorMailbox.FailOnDomainNotFound = false;
				}
				return smtpAnchorMailbox;
			}
			return base.ResolveAnchorMailbox();
		}

		protected override bool ShouldExcludeFromExplicitLogonParsing()
		{
			return false;
		}

		protected override bool IsValidExplicitLogonNode(string node, bool nodeIsLast)
		{
			if (nodeIsLast)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[AutodiscoverProxyRequestHandler::IsValidExplicitLogonNode]: Context {0}; rejected explicit logon node: {1}", base.TraceContext, node);
				return false;
			}
			return true;
		}

		protected override UriBuilder GetClientUrlForProxy()
		{
			string text = base.ClientRequest.Url.ToString();
			string uri = text;
			if (this.isExplicitLogonRequest && !ProtocolHelper.IsAutodiscoverV2Request(base.ClientRequest.Url.AbsoluteUri))
			{
				string text2 = "/" + this.explicitLogonAddress;
				int num = text.IndexOf(text2);
				if (num != -1)
				{
					uri = text.Substring(0, num) + text.Substring(num + text2.Length);
				}
			}
			return new UriBuilder(uri);
		}

		private bool preferAnchorMailboxHeader;

		private bool skipTargetBackEndCalculation;

		private bool isExplicitLogonRequest;

		private string explicitLogonAddress;
	}
}
