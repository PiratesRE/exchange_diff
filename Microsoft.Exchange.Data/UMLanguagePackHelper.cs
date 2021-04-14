using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	public sealed class UMLanguagePackHelper
	{
		public static bool IsUmLanguagePack(string pathname)
		{
			string input = Path.GetFileName(pathname).ToLower();
			string pattern = string.Format("[{0}][a-z][a-z]-[a-z][a-z][{1}]", "UMLanguagePack.".ToLower(), ".exe");
			string pattern2 = string.Format("[{0}][a-z][a-z][a-z]-[a-z][a-z][{1}]", "UMLanguagePack.".ToLower(), ".exe");
			string pattern3 = string.Format("[{0}][a-z][a-z]-[a-z][a-z][a-z][a-z]-[a-z][a-z][{1}]", "UMLanguagePack.".ToLower(), ".exe");
			return Regex.IsMatch(input, pattern) | Regex.IsMatch(input, pattern2) | Regex.IsMatch(input, pattern3);
		}

		public static LongPath GetUMLanguagePackFilename(string pathDirectory, CultureInfo culture)
		{
			LongPath result = null;
			string path = Path.Combine(pathDirectory, "UMLanguagePack." + culture.ToString() + ".exe");
			if (!LongPath.TryParse(path, out result))
			{
				result = null;
			}
			return result;
		}

		public static LocalLongFullPath GetAddUMLanguageLogPath(string setupLoggingPath, CultureInfo culture)
		{
			string path = string.Format("add-{0}{1}.msilog", "UMLanguagePack.", culture.ToString());
			string path2 = Path.Combine(setupLoggingPath, path);
			LocalLongFullPath result = null;
			if (!LocalLongFullPath.TryParse(path2, out result))
			{
				result = null;
			}
			return result;
		}

		private const string languagePackFilePrefix = "UMLanguagePack.";

		private const string languagePackExtension = ".exe";
	}
}
