using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.PswsClient;
using Microsoft.IdentityModel.S2S.Web;

namespace Microsoft.Exchange.Management.ControlPanel.Psws
{
	internal sealed class PswsAuthenticator : IAuthenticator
	{
		internal static PswsAuthenticator Create()
		{
			string text = RbacPrincipal.Current.RbacConfiguration.ExecutingUserOrganizationId.ToExternalDirectoryOrganizationId();
			string text2 = (string)HttpContext.Current.Items[TokenIssuer.ItemTagUpn];
			string text3 = (string)HttpContext.Current.Items[TokenIssuer.ItemTagPuid];
			return new PswsAuthenticator(text, text2, text3);
		}

		internal PswsAuthenticator(string tenantId, string upn, string puid)
		{
			if (string.IsNullOrWhiteSpace(tenantId))
			{
				throw new ArgumentNullException("tenantId cannot be null or empty");
			}
			if (string.IsNullOrWhiteSpace(upn))
			{
				throw new ArgumentNullException("upn cannot be null or empty");
			}
			if (string.IsNullOrWhiteSpace(puid))
			{
				throw new ArgumentNullException("puid cannot be null or empty");
			}
			this.tenantId = tenantId;
			this.upn = upn;
			this.puid = puid;
		}

		public IDisposable Authenticate(HttpWebRequest request)
		{
			TokenIssuer tokenIssuer = new TokenIssuer();
			string token = tokenIssuer.GetToken(this.tenantId, new Dictionary<string, string>
			{
				{
					TokenIssuer.ClaimTypeUpn,
					this.upn
				},
				{
					TokenIssuer.ClaimTypeNameId,
					this.puid
				}
			});
			request.Headers.Add(TokenIssuer.AuthorizationHeader, OAuth2ProtectedResourceUtility.WriteAuthorizationHeader(token));
			return null;
		}

		private readonly string tenantId;

		private readonly string upn;

		private readonly string puid;
	}
}
