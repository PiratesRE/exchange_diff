using System;
using System.DirectoryServices;

namespace Microsoft.Exchange.Management.Metabase
{
	internal static class ActiveSyncIsapiFilter
	{
		public static void Install(DirectoryEntry virtualDirectory)
		{
			IsapiFilterCommon.CreateFilter(virtualDirectory, "Exchange ActiveSync ISAPI Filter", ActiveSyncIsapiFilter.FilterDirectory, ActiveSyncIsapiFilter.ExtensionBinary);
		}

		public static void InstallForCafe(DirectoryEntry virtualDirectory)
		{
			IsapiFilterCommon.CreateFilter(virtualDirectory, "Exchange ActiveSync ISAPI Filter", "FrontEnd\\HttpProxy\\bin", ActiveSyncIsapiFilter.ExtensionBinary);
		}

		public static void Uninstall(DirectoryEntry virtualDirectory)
		{
			IsapiFilterCommon.Uninstall(virtualDirectory, "Exchange ActiveSync ISAPI Filter");
		}

		private const string FilterName = "Exchange ActiveSync ISAPI Filter";

		private const string CafeFilterDirectory = "FrontEnd\\HttpProxy\\bin";

		private static readonly string ExtensionBinary = "AirFilter.dll";

		private static readonly string FilterDirectory = "ClientAccess\\sync\\bin";
	}
}
