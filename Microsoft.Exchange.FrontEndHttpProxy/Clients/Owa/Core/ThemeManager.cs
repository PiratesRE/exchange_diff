using System;
using System.IO;
using Microsoft.Exchange.HttpProxy;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class ThemeManager
	{
		public static void RenderBaseThemeFileUrl(TextWriter writer, ThemeFileId themeFileId)
		{
			ThemeManager.RenderBaseThemeFileUrl(writer, themeFileId, false);
		}

		public static void RenderBaseThemeFileUrl(TextWriter writer, ThemeFileId themeFileId, bool useCDN)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ThemeManager.RenderThemeFileUrl(writer, (int)themeFileId, false, useCDN);
		}

		public static void RenderThemeFileUrl(TextWriter writer, int themeFileIndex, bool isBasicExperience, bool useCDN)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ThemeManager.RenderThemeFilePath(writer, themeFileIndex, isBasicExperience, useCDN);
			writer.Write(ThemeFileList.GetNameFromId(themeFileIndex));
		}

		private static bool RenderThemeFilePath(TextWriter writer, int themeFileIndex, bool isBasicExperience, bool useCDN)
		{
			writer.Write(ThemeManager.themesFolderPath);
			bool flag = ThemeFileList.IsResourceFile(themeFileIndex);
			if (flag)
			{
				writer.Write(ThemeManager.ResourcesFolderName);
			}
			else if (isBasicExperience)
			{
				writer.Write(ThemeManager.BasicFilesFolderName);
			}
			else
			{
				writer.Write(ThemeManager.BaseThemeFolderName);
			}
			writer.Write("/");
			return !flag;
		}

		public static readonly string BaseThemeFolderName = "base";

		public static readonly string BasicFilesFolderName = "basic";

		public static readonly string ResourcesFolderName = "resources";

		public static readonly string DataCenterThemeStorageId = "datacenter";

		private static readonly string ThemesFolderName = "themes";

		private static string themesFolderPath = string.Format("{0}/{1}/", HttpProxyGlobals.ApplicationVersion, ThemeManager.ThemesFolderName);
	}
}
