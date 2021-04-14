using System;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageExsetdataRegistryMarkers : Task
	{
		protected void WriteRegistryMarkers()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", true))
			{
				string path = (string)registryKey.GetValue("MsiInstallPath");
				int num = (int)registryKey.GetValue("MsiBuildMajor");
				int num2 = (int)registryKey.GetValue("MsiBuildMinor");
				registryKey.SetValue("Services", Path.GetDirectoryName(path));
				registryKey.SetValue("NewestBuild", num + 10000);
				registryKey.SetValue("CurrentBuild", (num + 10000 & 65535) << 16 | (num2 & 65535));
			}
		}

		protected void DeleteRegistryMarkers()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", true))
			{
				registryKey.DeleteValue("Services", false);
				registryKey.DeleteValue("NewestBuild", false);
				registryKey.DeleteValue("CurrentBuild", false);
			}
		}
	}
}
