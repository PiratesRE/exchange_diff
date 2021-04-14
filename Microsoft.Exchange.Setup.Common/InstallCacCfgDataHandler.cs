using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallCacCfgDataHandler : InstallRoleBaseDataHandler
	{
		public InstallCacCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "ClientAccessRole", "Install-ClientAccessRole", connection)
		{
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			SetupLogger.TraceExit();
		}
	}
}
