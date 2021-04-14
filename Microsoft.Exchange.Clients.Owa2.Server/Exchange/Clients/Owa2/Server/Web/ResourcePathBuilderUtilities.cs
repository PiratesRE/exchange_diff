using System;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class ResourcePathBuilderUtilities
	{
		public static string GetManifestDiskRelativeFolderPath(string owaVersion)
		{
			return Path.Combine(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(owaVersion), "manifests");
		}

		public static string GetResourcesRelativeFolderPath(string owaVersion)
		{
			return string.Format("prem/{0}", owaVersion);
		}

		public static string GetScriptResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return resourcesRelativeFolderPath + "/scripts";
		}

		public static string GetGlobalizeScriptResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "/globalize";
		}

		public static string GetLocalizedScriptResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "/ext";
		}

		public static string GetLocalizedScriptResourcesRelativeFolderPath(string resourcesRelativeFolderPath, string cultureName)
		{
			return ResourcePathBuilderUtilities.GetLocalizedScriptResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "/" + cultureName;
		}

		public static string GetBootResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return resourcesRelativeFolderPath + "/resources";
		}

		public static string GetStyleResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return ResourcePathBuilderUtilities.GetBootResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "/styles";
		}

		public static string GetBootImageResourcesRelativeFolderPath(string resourcesRelativeFolderPath, bool isRtl)
		{
			return string.Format("{0}/resources/images/{1}", resourcesRelativeFolderPath, isRtl ? "rtl" : "0");
		}

		public static string GetThemeResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return ResourcePathBuilderUtilities.GetBootResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "/themes/{0}/";
		}

		public static string GetStyleResourcesRelativeFolderPathWithSlash(string resourcesRelativeFolderPath)
		{
			return ResourcePathBuilderUtilities.GetStyleResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "/";
		}

		public static string GetThemeImageResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return ResourcePathBuilderUtilities.GetThemeResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "images/";
		}

		public static string GetImageResourcesRelativeFolderPath(string resourcesRelativeFolderPath)
		{
			return ResourcePathBuilderUtilities.GetBootResourcesRelativeFolderPath(resourcesRelativeFolderPath) + "/images/";
		}

		public static string GetBootThemeImageResourcesRelativeFolderPath(string version, string resourcesRelativeFolderPath, bool isRtl, bool shouldSkipThemeFolder)
		{
			if (!shouldSkipThemeFolder)
			{
				return ResourcePathBuilderUtilities.GetThemedLocaleImageResourcesRelativeFolderPath(resourcesRelativeFolderPath, isRtl);
			}
			return string.Format("{0}/resources/images/{1}", resourcesRelativeFolderPath, isRtl ? "rtl" : "0");
		}

		public static string GetThemedLocaleImageResourcesRelativeFolderPath(string resourcesRelativeFolderPath, bool isRtl)
		{
			return string.Format("{0}/resources/themes/{1}/images/{2}", resourcesRelativeFolderPath, "{0}", isRtl ? "rtl" : "0");
		}

		public static string GetBootStyleResourcesRelativeFolderPath(string version, string resourcesRelativeFolderPath, string stylesFolderCulturePlaceHolder, bool shouldSkipThemeFolder)
		{
			if (!shouldSkipThemeFolder)
			{
				return string.Format("{0}/resources/themes/{1}/{2}", resourcesRelativeFolderPath, "{0}", stylesFolderCulturePlaceHolder);
			}
			return string.Format("{0}/resources/styles/{1}", resourcesRelativeFolderPath, stylesFolderCulturePlaceHolder);
		}

		public static string GetScriptResourcesRootFolderPath(string exchangeInstallPath, string resourcesRelativeFolderPath)
		{
			return Path.Combine(exchangeInstallPath, string.Format("ClientAccess\\Owa\\{0}", ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(resourcesRelativeFolderPath)));
		}
	}
}
