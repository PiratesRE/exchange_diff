using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class StyleResource : ResourceBase
	{
		public StyleResource(string resourceName, ResourceTarget.Filter targetFilter, string currentOwaVersion, bool hasUserSpecificData) : base(resourceName, targetFilter, currentOwaVersion, hasUserSpecificData)
		{
		}

		public override string GetResourcePath(IPageContext pageContext, bool isBootResource)
		{
			string text = ResourceBase.CombinePath(new string[]
			{
				this.GetStyleDirectory(pageContext, pageContext.Theme, isBootResource),
				this.ResourceName
			});
			return text.ToLowerInvariant();
		}

		protected virtual string GetStyleDirectory(IPageContext pageContext, string theme, bool isBootStylesDirectory)
		{
			return pageContext.FormatURIForCDN(ResourcePathBuilderUtilities.GetStyleResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath), isBootStylesDirectory);
		}
	}
}
