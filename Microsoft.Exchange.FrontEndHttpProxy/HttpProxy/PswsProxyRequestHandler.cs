using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.HttpProxy
{
	internal class PswsProxyRequestHandler : RwsPswsProxyRequestHandlerBase<WebServicesService>
	{
		protected override string ServiceName
		{
			get
			{
				return "PowerShell Web Service";
			}
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return string.Equals(headerName, "client-request-id", StringComparison.OrdinalIgnoreCase) || (!string.Equals(headerName, WellKnownHeader.CmdletProxyIsOn, StringComparison.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName));
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			headers.Add("public-server-uri", base.ClientRequest.Url.GetLeftPart(UriPartial.Authority));
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		protected override void DoProtocolSpecificBeginProcess()
		{
			base.DoProtocolSpecificBeginProcess();
			string message;
			if (!this.AuthorizeOAuthRequest(out message))
			{
				throw new HttpException(403, message);
			}
			string domain;
			if (base.TryGetTenantDomain("organization", out domain))
			{
				base.IsDomainBasedRequest = true;
				base.Domain = domain;
			}
		}

		private bool AuthorizeOAuthRequest(out string errorMsg)
		{
			OAuthIdentity oauthIdentity = base.HttpContext.User.Identity as OAuthIdentity;
			string empty = string.Empty;
			errorMsg = string.Empty;
			if (oauthIdentity != null && base.TryGetTenantDomain("organization", out empty))
			{
				string text = string.Empty;
				if (oauthIdentity.OrganizationId != null)
				{
					text = oauthIdentity.OrganizationId.ConfigurationUnit.ToString();
				}
				if (!string.IsNullOrEmpty(text) && string.Compare(text, empty, true) != 0)
				{
					errorMsg = string.Format("{0} is not a authorized tenant. The authorized tenant is {1}", empty, text);
					return false;
				}
			}
			return true;
		}

		private const string TenantParameterName = "organization";
	}
}
