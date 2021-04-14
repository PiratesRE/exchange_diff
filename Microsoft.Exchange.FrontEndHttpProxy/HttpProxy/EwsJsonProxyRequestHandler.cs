using System;
using System.Net;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.HttpProxy
{
	internal class EwsJsonProxyRequestHandler : OwaProxyRequestHandler
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.Internal;
			}
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			headers["RPSPUID"] = (string)base.HttpContext.Items["RPSPUID"];
			headers["RPSOrgIdPUID"] = (string)base.HttpContext.Items["RPSOrgIdPUID"];
			base.AddProtocolSpecificHeadersToServerRequest(headers);
			if (base.ClientRequest != null && string.Equals(base.ClientRequest.QueryString["action"], "GetWacIframeUrl", StringComparison.OrdinalIgnoreCase))
			{
				OwaProxyRequestHandler.AddProxyUriHeader(base.ClientRequest, headers);
			}
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !string.Equals(headerName, "X-OWA-ProxyUri", StringComparison.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName);
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri targetBackEndServerUrl = base.GetTargetBackEndServerUrl();
			return UrlUtilities.FixIntegratedAuthUrlForBackEnd(targetBackEndServerUrl);
		}

		private const string LiveIdPuid = "RPSPUID";

		private const string OrgIdPuid = "RPSOrgIdPUID";
	}
}
