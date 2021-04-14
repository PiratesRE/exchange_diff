using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DisasterRecoveryGwyCfgDataHandler : DisasterRecoveryCfgDataHandler
	{
		public bool StartTransportService
		{
			get
			{
				return this.gatewayRoleConfigurationInfo.StartTransportService;
			}
		}

		public DisasterRecoveryGwyCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, RoleManager.GetRoleByName("GatewayRole"), connection)
		{
			this.gatewayRoleConfigurationInfo = (base.InstallableUnitConfigurationInfo as GatewayRoleConfigurationInfo);
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("StartTransportService", this.StartTransportService);
			SetupLogger.TraceExit();
		}

		private GatewayRoleConfigurationInfo gatewayRoleConfigurationInfo;
	}
}
