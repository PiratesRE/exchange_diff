using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallAtCfgDataHandler : InstallRoleBaseDataHandler
	{
		public InstallAtCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "AdminToolsRole", "Install-AdminToolsRole", connection)
		{
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}
	}
}
