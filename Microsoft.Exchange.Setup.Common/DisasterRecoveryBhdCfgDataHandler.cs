using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DisasterRecoveryBhdCfgDataHandler : DisasterRecoveryCfgDataHandler
	{
		public bool StartTransportService
		{
			get
			{
				return this.bridgeheadRoleConfigurationInfo.StartTransportService;
			}
		}

		public bool DisableAMFiltering
		{
			get
			{
				return this.bridgeheadRoleConfigurationInfo.DisableAMFiltering;
			}
			set
			{
				this.bridgeheadRoleConfigurationInfo.DisableAMFiltering = value;
			}
		}

		public DisasterRecoveryBhdCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, RoleManager.GetRoleByName("BridgeheadRole"), connection)
		{
			this.bridgeheadRoleConfigurationInfo = (base.InstallableUnitConfigurationInfo as BridgeheadRoleConfigurationInfo);
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("StartTransportService", this.StartTransportService);
			base.Parameters.AddWithValue("DisableAMFiltering", this.DisableAMFiltering);
			SetupLogger.TraceExit();
		}

		private BridgeheadRoleConfigurationInfo bridgeheadRoleConfigurationInfo;
	}
}
