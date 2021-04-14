using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Upgrade", "MSExchangeReplService")]
	public class UpgradeReplService : ManageReplService
	{
		private bool ShouldUpgradeService()
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("system\\currentcontrolset\\services\\msexchangerepl");
			bool result;
			if (registryKey == null)
			{
				result = true;
			}
			else
			{
				using (registryKey)
				{
					string input = (string)registryKey.GetValue("imagepath");
					string str = Regex.Replace(input, "\"", "");
					string str2 = base.ServiceInstallContext.Parameters["assemblypath"];
					result = !SharedHelper.StringIEquals(str, str2);
				}
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.ShouldUpgradeService())
			{
				base.Uninstall();
				base.Install();
			}
			base.UninstallEventManifest();
			base.InstallEventManifest();
			TaskLogger.LogExit();
		}
	}
}
