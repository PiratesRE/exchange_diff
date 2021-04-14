using System;
using System.IO;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Metabase
{
	internal static class DotNetFrameworkInfo
	{
		static DotNetFrameworkInfo()
		{
			string name = string.Format("Software\\Microsoft\\ASP.NET\\{0}.0", Environment.Version.ToString(3));
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
			{
				DotNetFrameworkInfo.aspNetIsapiDllPath = Path.GetFullPath((string)registryKey.GetValue("DllFullPath"));
				DotNetFrameworkInfo.frameworkInstallDir = (string)registryKey.GetValue("Path");
			}
		}

		public static string AspNetIsapiDllPath
		{
			get
			{
				return DotNetFrameworkInfo.aspNetIsapiDllPath;
			}
		}

		public static string FrameworkInstallDir
		{
			get
			{
				return DotNetFrameworkInfo.frameworkInstallDir;
			}
		}

		private static string aspNetIsapiDllPath = string.Empty;

		private static string frameworkInstallDir = string.Empty;
	}
}
