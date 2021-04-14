using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class LocalizedExtensibilityStringsScriptResource : ScriptResource
	{
		public LocalizedExtensibilityStringsScriptResource(string resourceName, ResourceTarget.Filter targetFilter, string currentOwaVersion) : base(resourceName, targetFilter, currentOwaVersion, true, false, false)
		{
		}

		public string GetLocalizedCultureName()
		{
			return this.GetLocalizedCultureName(this.ResourceName);
		}

		protected override string GetScriptDirectory(IPageContext pageContext, string resourceName, bool isBootScriptsDirectory)
		{
			string localizedCultureName = this.GetLocalizedCultureName(resourceName);
			return pageContext.FormatURIForCDN(ResourcePathBuilderUtilities.GetLocalizedScriptResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath, localizedCultureName), isBootScriptsDirectory);
		}

		private string GetLocalizedCultureName(string resourceName)
		{
			return LocalizedStringsScriptResource.GetLocalizedCultureName(LocalizedExtensibilityStringsScriptResource.threadCultureToLocalizedCultureMap, Thread.CurrentThread.CurrentCulture, (string lang) => this.ResourceExists(resourceName, lang));
		}

		private bool ResourceExists(string resourceName, string languageName)
		{
			string path = Path.Combine(FolderConfiguration.Instance.RootPath, ResourcePathBuilderUtilities.GetLocalizedScriptResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath, languageName), resourceName);
			return File.Exists(path);
		}

		private static readonly ConcurrentDictionary<string, string> threadCultureToLocalizedCultureMap = new ConcurrentDictionary<string, string>();
	}
}
