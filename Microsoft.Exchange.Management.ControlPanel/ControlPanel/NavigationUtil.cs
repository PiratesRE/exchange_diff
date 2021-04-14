using System;
using System.Web;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class NavigationUtil
	{
		internal static bool ShouldRenderOwaLink(RbacPrincipal rbacPrincipal, bool showAdminFeature)
		{
			return !showAdminFeature && !NavigationUtil.LaunchedFromOutlook && rbacPrincipal.IsInRole("Mailbox+OWA+MailboxFullAccess");
		}

		private static bool LaunchedFromOutlook
		{
			get
			{
				bool result = false;
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					string value = httpContext.Request.QueryString["rfr"];
					result = "olk".Equals(value, StringComparison.OrdinalIgnoreCase);
				}
				return result;
			}
		}

		internal static bool ShouldRenderLogoutLink(RbacPrincipal rbacPrincipal)
		{
			return rbacPrincipal.IsInRole("MailboxFullAccess+!DelegatedAdmin+!ByoidAdmin");
		}
	}
}
