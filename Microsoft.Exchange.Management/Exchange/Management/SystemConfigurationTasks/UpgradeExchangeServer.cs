using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Upgrade", "ExchangeServer", DefaultParameterSetName = "Identity")]
	public sealed class UpgradeExchangeServer : SetTopologySystemConfigurationObjectTask<ServerIdParameter, Server>
	{
		protected override IConfigurable PrepareDataObject()
		{
			Server server = (Server)base.PrepareDataObject();
			server.AdminDisplayVersion = ConfigurationContext.Setup.InstalledVersion;
			server.VersionNumber = SystemConfigurationTasksHelper.GenerateVersionNumber(ConfigurationContext.Setup.InstalledVersion);
			return server;
		}
	}
}
