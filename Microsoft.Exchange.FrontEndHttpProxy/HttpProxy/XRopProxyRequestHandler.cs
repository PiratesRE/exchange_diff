using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal class XRopProxyRequestHandler : BEServerCookieProxyRequestHandler<RpcHttpService>
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.External;
			}
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			string text = base.ClientRequest.QueryString[Constants.AnchorMailboxHeaderName];
			if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text))
			{
				SmtpAnchorMailbox smtpAnchorMailbox = new SmtpAnchorMailbox(text, this);
				string value = "AnchorMailboxQuery-SMTP";
				if (!base.ClientRequest.GetHttpRequestBase().IsProxyTestProbeRequest())
				{
					smtpAnchorMailbox.IsArchive = new bool?(true);
					value = "AnchorMailboxQuery-Archive-SMTP";
				}
				base.Logger.Set(HttpProxyMetadata.RoutingHint, value);
				return smtpAnchorMailbox;
			}
			return new OrganizationAnchorMailbox(OrganizationId.ForestWideOrgId, this);
		}
	}
}
