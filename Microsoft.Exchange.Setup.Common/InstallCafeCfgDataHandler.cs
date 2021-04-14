using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallCafeCfgDataHandler : InstallRoleBaseDataHandler
	{
		public InstallCafeCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "CafeRole", "Install-CafeRole", connection)
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
