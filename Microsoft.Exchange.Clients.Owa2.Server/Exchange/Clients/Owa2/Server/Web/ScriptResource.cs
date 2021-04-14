using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class ScriptResource : ResourceBase
	{
		public ScriptResource(string resourceName, ResourceTarget.Filter targetFilter, string currentOwaVersion, bool hasUserSpecificData = false, bool isExternalDrop = false, bool isFullPath = false) : base(resourceName, targetFilter, currentOwaVersion, hasUserSpecificData)
		{
			this.isFullPath = isFullPath;
			this.isExternalDrop = isExternalDrop;
		}

		public bool IsExternalDropped
		{
			get
			{
				return this.isExternalDrop;
			}
		}

		public override string GetResourcePath(IPageContext pageContext, bool isBootResourcePath)
		{
			string text = ResourceBase.CombinePath(new string[]
			{
				this.GetScriptDirectory(pageContext, this.ResourceName, isBootResourcePath),
				this.ResourceName
			});
			return text.ToLowerInvariant();
		}

		protected virtual string GetScriptDirectory(IPageContext pageContext, string resourceName, bool isBootScriptsDirectory)
		{
			if (resourceName.StartsWith("http://") || resourceName.StartsWith("https://") || this.isFullPath)
			{
				return string.Empty;
			}
			return pageContext.FormatURIForCDN(ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath), isBootScriptsDirectory);
		}

		private readonly bool isFullPath;

		private readonly bool isExternalDrop;
	}
}
