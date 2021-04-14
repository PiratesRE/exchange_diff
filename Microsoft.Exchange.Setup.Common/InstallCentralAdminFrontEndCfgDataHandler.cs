using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallCentralAdminFrontEndCfgDataHandler : InstallDatacenterRoleBaseDataHandler
	{
		public InstallCentralAdminFrontEndCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "CentralAdminFrontEndRole", "Install-CentralAdminFrontEndRole", connection)
		{
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}
	}
}
