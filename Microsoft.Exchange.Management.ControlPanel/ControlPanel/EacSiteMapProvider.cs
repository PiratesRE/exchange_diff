using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EacSiteMapProvider : XmlSiteMapProvider
	{
		public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
		{
			string relativePathToAppRoot = EcpUrl.GetRelativePathToAppRoot(node.Url);
			if (relativePathToAppRoot != null)
			{
				string rewriteUrl = EacFlightProvider.Instance.GetRewriteUrl(relativePathToAppRoot);
				if (!EacFlightProvider.Instance.IsUrlEnabled(rewriteUrl ?? relativePathToAppRoot))
				{
					return false;
				}
				if (rewriteUrl != null)
				{
					node = node.Clone();
					node.Url = EcpUrl.ReplaceRelativePath(node.Url, rewriteUrl, true);
				}
			}
			return base.IsAccessibleToUser(context, node);
		}
	}
}
