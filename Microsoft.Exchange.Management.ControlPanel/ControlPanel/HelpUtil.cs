using System;
using System.Threading;
using System.Web;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class HelpUtil
	{
		private static bool IsEACHelpId(string helpId)
		{
			bool result = false;
			if (Enum.IsDefined(typeof(EACHelpId), helpId))
			{
				result = true;
			}
			return result;
		}

		private static string ConstructOwaOptionsHelpUrl(string helpId)
		{
			OrganizationProperties organizationProperties = null;
			OrganizationPropertyCache.TryGetOrganizationProperties(RbacPrincipal.Current.RbacConfiguration.OrganizationId, out organizationProperties);
			return HelpProvider.ConstructHelpRenderingUrl(Thread.CurrentThread.CurrentUICulture.LCID, HelpProvider.OwaHelpExperience.Options, helpId, HelpProvider.RenderingMode.Mouse, null, organizationProperties).ToString();
		}

		private static string BuildEhcHref(string helpId, bool isEACHelpId)
		{
			string result;
			if (isEACHelpId)
			{
				RbacPrincipal rbacPrincipal = HttpContext.Current.User as RbacPrincipal;
				result = ((rbacPrincipal != null) ? HelpProvider.ConstructHelpRenderingUrl(helpId, rbacPrincipal.RbacConfiguration).ToString() : HelpProvider.ConstructHelpRenderingUrl(helpId).ToString());
			}
			else
			{
				result = HelpUtil.ConstructOwaOptionsHelpUrl(helpId);
			}
			return result;
		}

		public static string BuildEhcHref(string helpId)
		{
			HelpUtil.EnsureInit();
			return HelpUtil.BuildEhcHref(helpId, HelpUtil.IsEACHelpId(helpId));
		}

		public static string BuildEhcHref(EACHelpId helpId)
		{
			HelpUtil.EnsureInit();
			return HelpUtil.BuildEhcHref(helpId.ToString(), true);
		}

		public static string BuildEhcHref(OptionsHelpId helpId)
		{
			HelpUtil.EnsureInit();
			return HelpUtil.BuildEhcHref(helpId.ToString(), false);
		}

		public static string BuildFVAEhcHref(string helpId, string controlId)
		{
			HelpUtil.EnsureInit();
			return HelpUtil.BuildEhcHref(controlId, HelpUtil.IsEACHelpId(helpId));
		}

		public static string BuildErrorAssistanceUrl(LocalizedException locEx)
		{
			HelpUtil.EnsureInit();
			Uri uri = null;
			HelpProvider.TryGetErrorAssistanceUrl(locEx, out uri);
			if (!(uri != null))
			{
				return null;
			}
			return uri.ToEscapedString();
		}

		public static string BuildPrivacyStatmentHref()
		{
			HelpUtil.EnsureInit();
			if (Util.IsDataCenter)
			{
				return HelpUtil.AppendLCID(HelpProvider.GetMSOnlinePrivacyStatementUrl().ToString());
			}
			return HelpUtil.AppendLCID(HelpProvider.GetExchange2013PrivacyStatementUrl().ToString());
		}

		public static string BuildCommunitySiteHref()
		{
			Uri uri;
			if (HelpProvider.TryGetCommunityUrl(RbacPrincipal.Current.RbacConfiguration.OrganizationId, out uri))
			{
				return HelpUtil.AppendLCID(uri.ToEscapedString());
			}
			return string.Empty;
		}

		private static string AppendLCID(string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				return string.Format("{0}&clcid={1}", url, Util.GetLCID());
			}
			return url;
		}

		private static void EnsureInit()
		{
			if (!HelpUtil.initialized)
			{
				HelpProvider.Initialize(HelpProvider.HelpAppName.Ecp);
				HelpUtil.initialized = true;
			}
		}

		private static bool initialized;
	}
}
