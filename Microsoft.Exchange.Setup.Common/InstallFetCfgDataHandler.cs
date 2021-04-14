using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallFetCfgDataHandler : InstallRoleBaseDataHandler
	{
		public bool StartTransportService
		{
			get
			{
				return this.frontendTransportRoleConfigurationInfo.StartTransportService;
			}
		}

		public InstallFetCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "FrontendTransportRole", "Install-FrontendTransportRole", connection)
		{
			this.frontendTransportRoleConfigurationInfo = (FrontendTransportRoleConfigurationInfo)base.InstallableUnitConfigurationInfo;
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("StartTransportService", this.StartTransportService);
			SetupLogger.TraceExit();
		}

		private FrontendTransportRoleConfigurationInfo frontendTransportRoleConfigurationInfo;
	}
}
