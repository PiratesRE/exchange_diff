using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	internal static class LoginUtil
	{
		static LoginUtil()
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("securityTrimmingEnabled", "true");
			LoginUtil.urlAuthSiteProvider.Initialize("urlAuthSiteProvider", nameValueCollection);
		}

		public static bool CheckUrlAccess(string path)
		{
			if (!path.StartsWith("~"))
			{
				Uri uri = new Uri(HttpContext.Current.Request.Url, new Uri(path, UriKind.RelativeOrAbsolute));
				path = uri.PathAndQuery;
			}
			SiteMapNode siteMapNode = new SiteMapNode(LoginUtil.urlAuthSiteProvider, "authCheck", path);
			return siteMapNode.IsAccessibleToUser(HttpContext.Current);
		}

		public static bool IsInRoles(IPrincipal user, string[] roles)
		{
			for (int i = 0; i < roles.Length; i++)
			{
				if (user.IsInRole(roles[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static SiteMapProvider urlAuthSiteProvider = new EacSiteMapProvider();
	}
}
