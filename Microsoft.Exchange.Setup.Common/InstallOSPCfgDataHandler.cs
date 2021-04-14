using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallOSPCfgDataHandler : InstallRoleBaseDataHandler
	{
		public InstallOSPCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "OSPRole", "Install-OSPRole", connection)
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
