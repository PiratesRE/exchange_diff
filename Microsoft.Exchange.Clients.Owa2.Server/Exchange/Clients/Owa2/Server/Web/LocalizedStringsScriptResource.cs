using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class LocalizedStringsScriptResource : ScriptResource
	{
		public LocalizedStringsScriptResource(string resourceName, ResourceTarget.Filter targetFilter, string currentOwaVersion) : base(resourceName, targetFilter, currentOwaVersion, true, false, false)
		{
			this.resourceName = resourceName;
		}

		public string GetLocalizedCultureName()
		{
			return this.GetLocalizedCultureName(this.resourceName);
		}

		public string GetResoucePath(IPageContext pageContext, string cultureName, bool isBootResourcePath)
		{
			return this.GetScriptDirectoryFromCultureName(pageContext, cultureName, isBootResourcePath) + "/" + this.resourceName.ToLowerInvariant();
		}

		internal static string GetLocalizedCultureName(ConcurrentDictionary<string, string> cultureMap, CultureInfo culture, Func<string, bool> existsFilter)
		{
			return cultureMap.GetOrAdd(culture.Name, (string x) => LocalizedStringsScriptResource.GetLocalizedCultureName(culture, existsFilter));
		}

		protected override string GetScriptDirectory(IPageContext pageContext, string resourceName, bool isBootScriptsDirectory)
		{
			return pageContext.FormatURIForCDN(this.GetScriptDirectory(pageContext, Thread.CurrentThread.CurrentCulture, resourceName, isBootScriptsDirectory), isBootScriptsDirectory);
		}

		private string GetScriptDirectory(IPageContext pageContext, CultureInfo cultureInfo, string resourceName, bool isBootScriptsDirectory)
		{
			return this.GetScriptDirectoryFromCultureName(pageContext, LocalizedStringsScriptResource.GetLocalizedCultureName(LocalizedStringsScriptResource.threadCultureToLocalizedCultureMap, cultureInfo, (string lang) => this.ResourceExists(resourceName, lang)), isBootScriptsDirectory);
		}

		private static string GetLocalizedCultureName(CultureInfo culture, Func<string, bool> existsFilter)
		{
			string result = "en";
			CultureInfo cultureInfo = culture;
			while (cultureInfo != null && !string.IsNullOrEmpty(cultureInfo.Name))
			{
				string name = cultureInfo.Name;
				if (existsFilter(name))
				{
					result = name;
					break;
				}
				cultureInfo = cultureInfo.Parent;
			}
			return result;
		}

		private string GetScriptDirectoryFromCultureName(IPageContext handler, string cultureName, bool isBootScriptsDirectory)
		{
			if (this.scriptDirectoryFormat == null)
			{
				this.scriptDirectoryFormat = ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath) + "/{0}";
			}
			return handler.FormatURIForCDN(string.Format(this.scriptDirectoryFormat, cultureName), isBootScriptsDirectory);
		}

		private string GetLocalizedCultureName(string resourceName)
		{
			return LocalizedStringsScriptResource.GetLocalizedCultureName(LocalizedStringsScriptResource.threadCultureToLocalizedCultureMap, Thread.CurrentThread.CurrentUICulture, (string lang) => this.ResourceExists(resourceName, lang));
		}

		private bool ResourceExists(string resourceName, string languageName)
		{
			string path = Path.Combine(FolderConfiguration.Instance.RootPath, ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath), languageName, resourceName);
			return File.Exists(path);
		}

		private const string LocalizedStringsScriptPath = "/{0}";

		private const string DefaultCulture = "en";

		private readonly string resourceName;

		private static ConcurrentDictionary<string, string> threadCultureToLocalizedCultureMap = new ConcurrentDictionary<string, string>();

		private string scriptDirectoryFormat;
	}
}
