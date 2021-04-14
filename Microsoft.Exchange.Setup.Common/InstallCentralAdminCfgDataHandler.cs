using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallCentralAdminCfgDataHandler : InstallDatacenterRoleBaseDataHandler
	{
		public InstallCentralAdminCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "CentralAdminRole", "Install-CentralAdminRole", connection)
		{
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}
	}
}
