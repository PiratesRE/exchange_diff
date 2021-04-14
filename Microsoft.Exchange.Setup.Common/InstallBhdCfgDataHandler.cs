using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallBhdCfgDataHandler : InstallRoleBaseDataHandler
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

		public InstallBhdCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "BridgeheadRole", "Install-BridgeheadRole", connection)
		{
			this.bridgeheadRoleConfigurationInfo = (BridgeheadRoleConfigurationInfo)base.InstallableUnitConfigurationInfo;
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("StartTransportService", this.StartTransportService);
			base.Parameters.AddWithValue("DisableAMFiltering", this.DisableAMFiltering);
			SetupLogger.TraceExit();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.UpdatePreCheckTaskDataHandler();
			PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			SetupLogger.TraceExit();
		}

		private BridgeheadRoleConfigurationInfo bridgeheadRoleConfigurationInfo;
	}
}
