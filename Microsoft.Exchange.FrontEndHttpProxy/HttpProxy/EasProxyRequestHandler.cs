using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.HttpProxy
{
	internal class EasProxyRequestHandler : OwaEcpProxyRequestHandler<MobileSyncService>
	{
		protected override string ProxyLogonUri
		{
			get
			{
				return "Proxy/";
			}
		}

		protected override string ProxyLogonQueryString
		{
			get
			{
				return "cmd=ProxyLogin";
			}
		}

		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.Internal;
			}
		}

		protected override DatacenterRedirectStrategy CreateDatacenterRedirectStrategy()
		{
			return new DefaultRedirectStrategy(this);
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			return new UriBuilder(base.GetTargetBackEndServerUrl())
			{
				Path = base.ClientRequest.ApplicationPath + "/Proxy" + base.ClientRequest.Url.AbsolutePath.Substring(base.ClientRequest.ApplicationPath.Length)
			}.Uri;
		}

		protected override void DoProtocolSpecificBeginRequestLogging()
		{
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			IIdentity identity = base.HttpContext.User.Identity;
			if (identity is WindowsIdentity || identity is ClientSecurityContextIdentity)
			{
				headers["X-EAS-Proxy"] = identity.GetSecurityIdentifier().ToString() + "," + identity.GetSafeName(true);
			}
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !string.Equals(headerName, "X-EAS-Proxy", StringComparison.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName);
		}

		protected override void SetProtocolSpecificProxyLogonRequestParameters(HttpWebRequest request)
		{
			request.ContentType = "text/xml";
		}

		protected override bool TryHandleProtocolSpecificRequestErrors(Exception ex)
		{
			HttpException ex2 = ex as HttpException;
			if (ex2 != null && ex2.WebEventCode == 3004)
			{
				string text = base.ClientRequest.Headers["MS-ASProtocolVersion"];
				base.Logger.AppendGenericError("RuntimeErrorPostTooLarge", ex.ToString());
				if (!string.IsNullOrEmpty(text) && (text == "14.0" || text == "14.1"))
				{
					base.ClientResponse.StatusCode = 200;
					if (base.ClientResponse.IsClientConnected)
					{
						base.ClientResponse.ContentType = "application/vnd.ms-sync.wbxml";
						base.ClientResponse.OutputStream.Write(EasProxyRequestHandler.easRequestSizeTooLargeResponseBytes, 0, EasProxyRequestHandler.easRequestSizeTooLargeResponseBytes.Length);
					}
				}
				else
				{
					base.ClientResponse.StatusCode = 500;
				}
				base.Complete();
				return true;
			}
			return base.TryHandleProtocolSpecificRequestErrors(ex);
		}

		protected override void RedirectIfNeeded(BackEndServer mailboxServer)
		{
		}

		private const string ProxyHeader = "X-EAS-Proxy";

		private const string EASProtocolVersion = "MS-ASProtocolVersion";

		private const string WbXmlContentType = "application/vnd.ms-sync.wbxml";

		private const string EasProxyLogonUri = "Proxy/";

		private const string EasProxyLogonQueryString = "cmd=ProxyLogin";

		private static byte[] easRequestSizeTooLargeResponseBytes = new byte[]
		{
			3,
			1,
			106,
			0,
			0,
			21,
			69,
			82,
			3,
			49,
			49,
			53,
			0,
			1,
			1
		};
	}
}
