using System;
using System.Net;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	internal class PsgwProxyRequestHandler : ProxyRequestHandler
	{
		protected override bool ProxyKerberosAuthentication
		{
			get
			{
				return true;
			}
		}

		public static bool IsPsgwRequest(HttpRequest request)
		{
			return !string.IsNullOrEmpty(request.Url.AbsolutePath) && (string.Compare(request.Url.AbsolutePath, "/psgw", StringComparison.OrdinalIgnoreCase) == 0 || request.Url.AbsolutePath.IndexOf("/psgw/", StringComparison.OrdinalIgnoreCase) == 0);
		}

		protected override void OnInitializingHandler()
		{
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return true;
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			UriBuilder uriBuilder = new UriBuilder(base.ClientRequest.Url);
			if (uriBuilder.Path.EndsWith("/healthchecktarget.htm"))
			{
				uriBuilder.Path = "/powershell/healthcheck.htm";
			}
			else
			{
				uriBuilder.Path = "/powershell";
			}
			return uriBuilder.Uri;
		}
	}
}
