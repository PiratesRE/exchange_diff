using System;
using System.Configuration;
using System.Web;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class CrossPremiseUtil
	{
		private static string DefaultRealmParameter
		{
			get
			{
				if (CrossPremiseUtil.defaultRealmParameter == null)
				{
					CrossPremiseUtil.defaultRealmParameter = (ConfigurationManager.AppSettings["CrossPremiseDefaultRealmParameter"] ?? "microsoftonline.com");
				}
				return CrossPremiseUtil.defaultRealmParameter;
			}
		}

		public static string UserFeatureAtCurrentOrg
		{
			get
			{
				RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
				return string.Format("{0}{1}", '0', rbacPrincipal.IsInRole("UserOptions+OrgMgmControlPanel") ? '1' : '0');
			}
		}

		public static string OnPremiseLinkToOffice365
		{
			get
			{
				return CrossPremiseUtil.InternalOnPremiseLinkToOffice365(OrganizationCache.ServiceInstance);
			}
		}

		public static string OnPremiseLinkToOffice365WorldWide
		{
			get
			{
				return CrossPremiseUtil.InternalOnPremiseLinkToOffice365("0");
			}
		}

		public static string OnPremiseLinkToOffice365Gallatin
		{
			get
			{
				return CrossPremiseUtil.InternalOnPremiseLinkToOffice365("1");
			}
		}

		public static string GetLinkToCrossPremise(HttpContext context, HttpRequest request)
		{
			return CrossPremiseUtil.InternalGetLinkToCrossPremise(context, request, OrganizationCache.ServiceInstance);
		}

		private static string InternalOnPremiseLinkToOffice365(string serviceInstance)
		{
			if (EacEnvironment.Instance.IsDataCenter)
			{
				return string.Empty;
			}
			string url = CrossPremiseUtil.InternalGetLinkToCrossPremise(HttpContext.Current, HttpContext.Current.Request, serviceInstance);
			return EcpUrl.AppendQueryParameter(url, "ov", "1");
		}

		private static string InternalGetLinkToCrossPremise(HttpContext context, HttpRequest request, string serviceInstance)
		{
			string text = request.QueryString["cross"];
			bool flag = (!string.IsNullOrEmpty(text) && text != "0") || request.IsAuthenticatedByAdfs();
			string text2;
			if (flag)
			{
				text2 = RbacPrincipal.Current.RbacConfiguration.ExecutingUserPrincipalName;
				text2 = text2.Substring(text2.IndexOf('@') + 1);
			}
			else if (OrganizationCache.EntHasTargetDeliveryDomain)
			{
				text2 = OrganizationCache.EntTargetDeliveryDomain;
			}
			else
			{
				text2 = CrossPremiseUtil.DefaultRealmParameter;
			}
			string format;
			if (serviceInstance != null)
			{
				if (serviceInstance == "0")
				{
					format = OrganizationCache.CrossPremiseUrlFormatWorldWide;
					goto IL_A7;
				}
				if (serviceInstance == "1")
				{
					format = OrganizationCache.CrossPremiseUrlFormatGallatin;
					goto IL_A7;
				}
			}
			format = OrganizationCache.CrossPremiseUrlFormat;
			IL_A7:
			return string.Format(format, context.GetRequestUrl().Host, CrossPremiseUtil.UserFeatureAtCurrentOrg, text2);
		}

		private const string CrossPremiseDefaultRealmParameterKey = "CrossPremiseDefaultRealmParameter";

		private const string CrossPremiseDefaultRealmValue = "microsoftonline.com";

		private static string defaultRealmParameter;
	}
}
