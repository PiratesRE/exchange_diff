using System;
using System.Web;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Common
{
	internal static class BrandingUtilities
	{
		internal static bool HasMHCookie()
		{
			return HttpContext.Current.Request.Cookies["MH"] != null;
		}

		internal static bool IsBranded()
		{
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.IsBranded.Enabled && BrandingUtilities.HasMHCookie();
		}

		internal const string MHCookieName = "MH";
	}
}
