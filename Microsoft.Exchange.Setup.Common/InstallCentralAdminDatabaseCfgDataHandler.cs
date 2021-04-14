using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallCentralAdminDatabaseCfgDataHandler : InstallDatacenterRoleBaseDataHandler
	{
		public InstallCentralAdminDatabaseCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "CentralAdminDatabaseRole", "Install-CentralAdminDatabaseRole", connection)
		{
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}
	}
}
