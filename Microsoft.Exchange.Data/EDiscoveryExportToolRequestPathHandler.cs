using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.Exchange.Data
{
	internal static class EDiscoveryExportToolRequestPathHandler
	{
		public static bool IsEDiscoveryExportToolRequest(HttpRequest request)
		{
			string absolutePath = request.Url.AbsolutePath;
			if (string.IsNullOrEmpty(absolutePath))
			{
				return false;
			}
			if (absolutePath.IndexOf("/exporttool/", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return false;
			}
			EDiscoveryExportToolRequestPathHandler.EnsureRegexInit();
			return EDiscoveryExportToolRequestPathHandler.applicationPathRegex.IsMatch(absolutePath);
		}

		public static Match GetPathMatch(HttpRequest request)
		{
			EDiscoveryExportToolRequestPathHandler.EnsureRegexInit();
			return EDiscoveryExportToolRequestPathHandler.applicationPathRegex.Match(request.Url.AbsolutePath);
		}

		private static void EnsureRegexInit()
		{
			if (EDiscoveryExportToolRequestPathHandler.applicationPathRegex == null)
			{
				lock (EDiscoveryExportToolRequestPathHandler.Monitor)
				{
					if (EDiscoveryExportToolRequestPathHandler.applicationPathRegex == null)
					{
						EDiscoveryExportToolRequestPathHandler.applicationPathRegex = new Regex(EDiscoveryExportToolRequestPathHandler.applicationPathPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
					}
				}
			}
		}

		private static readonly object Monitor = new object();

		private static readonly string applicationPathPattern = "/ecp/(?<major>\\d{2})\\.(?<minor>\\d{1,})\\.(?<build>\\d{1,})\\.(?<revision>\\d{1,})/exporttool/.*$";

		private static Regex applicationPathRegex;
	}
}
