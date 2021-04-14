using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class GlobalizeScriptResource : ScriptResource
	{
		public GlobalizeScriptResource(string resourceName, ResourceTarget.Filter targetFilter, string currentVersion, bool hasUserSpecificData = false) : base(resourceName, targetFilter, currentVersion, hasUserSpecificData, true, false)
		{
		}

		protected override string GetScriptDirectory(IPageContext pageContext, string resourceName, bool isBootScriptsDirectory)
		{
			if (this.scriptDirectory == null)
			{
				this.scriptDirectory = ResourcePathBuilderUtilities.GetGlobalizeScriptResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath);
			}
			return pageContext.FormatURIForCDN(this.scriptDirectory, isBootScriptsDirectory);
		}

		private string scriptDirectory;
	}
}
