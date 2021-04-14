using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallMonitoringCfgDataHandler : InstallDatacenterRoleBaseDataHandler
	{
		public InstallMonitoringCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "MonitoringRole", "Install-MonitoringRole", connection)
		{
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}
	}
}
