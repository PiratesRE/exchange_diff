using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DisasterRecoveryFetCfgDataHandler : DisasterRecoveryCfgDataHandler
	{
		public bool StartTransportService
		{
			get
			{
				return this.frontendTransportRoleConfigurationInfo.StartTransportService;
			}
		}

		public DisasterRecoveryFetCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, RoleManager.GetRoleByName("FrontendTransportRole"), connection)
		{
			this.frontendTransportRoleConfigurationInfo = (base.InstallableUnitConfigurationInfo as FrontendTransportRoleConfigurationInfo);
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
