using System;
using System.DirectoryServices;

namespace Microsoft.Exchange.Management.Metabase
{
	internal static class AuthModuleIsapiFilter
	{
		public static void Install(DirectoryEntry virtualDirectory)
		{
			string iiswebsitePath = IsapiFilterCommon.GetIISWebsitePath(virtualDirectory);
			bool flag;
			IsapiFilterCommon.CreateFilter(iiswebsitePath, "Microsoft.Exchange.AuthModuleFilter ISAPI Filter", AuthModuleIsapiFilter.FilterDirectory, AuthModuleIsapiFilter.ExtensionBinary, true, out flag);
		}

		public static void Uninstall(DirectoryEntry virtualDirectory)
		{
			IsapiFilterCommon.Uninstall(virtualDirectory, "Microsoft.Exchange.AuthModuleFilter ISAPI Filter");
		}

		private const string FilterName = "Microsoft.Exchange.AuthModuleFilter ISAPI Filter";

		private static readonly string ExtensionBinary = "Microsoft.Exchange.AuthModuleFilter.dll";

		private static readonly string FilterDirectory = "bin";
	}
}
