using System;
using System.DirectoryServices;
using System.IO;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Metabase
{
	internal static class IsapiFilterCommon
	{
		internal static bool CreateFilter(DirectoryEntry virtualDirectory, string filterName, string filterDirectory, string extensionBinary)
		{
			bool result = false;
			string iiswebsitePath = IsapiFilterCommon.GetIISWebsitePath(virtualDirectory);
			IsapiFilterCommon.CreateFilter(iiswebsitePath, filterName, filterDirectory, extensionBinary, true, out result);
			return result;
		}

		internal static void CreateFilter(string adsiWebSitePath, string filterName, string filterDirectory, string extensionBinary, bool enable, out bool filterCreated)
		{
			filterCreated = false;
			string iisserverName = IsapiFilterCommon.GetIISServerName(adsiWebSitePath);
			using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, iisserverName))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
				{
					string text = Path.Combine((string)registryKey2.GetValue("MsiInstallPath"), filterDirectory);
					text = Path.Combine(text, extensionBinary);
					using (DirectoryEntry directoryEntry = IsapiFilter.CreateIsapiFilter(adsiWebSitePath, text, filterName, out filterCreated))
					{
						if (filterCreated)
						{
							directoryEntry.Properties["FilterFlags"].Value = MetabasePropertyTypes.FilterFlags.NotifyOrderMedium;
							directoryEntry.Properties["FilterEnabled"].Value = enable;
							directoryEntry.CommitChanges();
							IisUtility.CommitMetabaseChanges(iisserverName);
						}
					}
				}
			}
		}

		internal static void Uninstall(DirectoryEntry virtualDirectory, string filterName)
		{
			string iisserverName = IsapiFilterCommon.GetIISServerName(virtualDirectory);
			string iislocalPath = IsapiFilterCommon.GetIISLocalPath(virtualDirectory);
			string text = null;
			string str = null;
			string text2 = null;
			IisUtility.ParseApplicationRootPath(iislocalPath, ref text, ref str, ref text2);
			IsapiFilter.RemoveIsapiFilter("IIS://" + iisserverName + str, filterName);
		}

		internal static string GetIISServerName(string path)
		{
			return path.Substring("IIS://".Length, path.IndexOf("/", "IIS://".Length) - "IIS://".Length);
		}

		internal static string GetIISServerName(DirectoryEntry virtualDirectory)
		{
			return IsapiFilterCommon.GetIISServerName(virtualDirectory.Path);
		}

		internal static string GetIISWebsitePath(DirectoryEntry virtualDirectory)
		{
			return virtualDirectory.Path.Substring(0, virtualDirectory.Path.IndexOf("/ROOT/", StringComparison.OrdinalIgnoreCase));
		}

		internal static string GetIISLocalPath(DirectoryEntry virtualDirectory)
		{
			return virtualDirectory.Path.Substring(virtualDirectory.Path.IndexOf("/", "IIS://".Length, StringComparison.OrdinalIgnoreCase));
		}

		private const string MetabaseRoot = "/LM";

		private const string AdsiPrefix = "IIS://";
	}
}
