using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services
{
	public class EwsBasicAuthenticationHttpModule : IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication application)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled)
			{
				application.PostAuthenticateRequest += EwsBasicAuthenticationHttpModule.OnPostAuthenticateRequestHandler;
			}
		}

		private static void OnPostAuth(object source, EventArgs args)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			if (context.Request.IsAuthenticated && context.User.Identity is WindowsIdentity)
			{
				WindowsIdentity windowsIdentity = context.User.Identity as WindowsIdentity;
				string memberName = context.GetMemberName();
				if (!string.IsNullOrEmpty(memberName))
				{
					ADSessionSettings adsessionSettings = Directory.SessionSettingsFromAddress(memberName);
					if (adsessionSettings.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
					{
						context.User = new GenericPrincipal(new LiveIDIdentity(windowsIdentity.Name, windowsIdentity.User.ToString(), memberName, null, null, null)
						{
							UserOrganizationId = adsessionSettings.CurrentOrganizationId
						}, null);
					}
				}
			}
		}

		private static readonly EventHandler OnPostAuthenticateRequestHandler = new EventHandler(EwsBasicAuthenticationHttpModule.OnPostAuth);
	}
}
