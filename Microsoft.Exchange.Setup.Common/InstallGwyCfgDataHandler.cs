using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallGwyCfgDataHandler : InstallRoleBaseDataHandler
	{
		public bool StartTransportService
		{
			get
			{
				return this.gatewayRoleConfigurationInfo.StartTransportService;
			}
		}

		public InstallGwyCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "GatewayRole", "Install-GatewayRole", connection)
		{
			this.gatewayRoleConfigurationInfo = (GatewayRoleConfigurationInfo)base.InstallableUnitConfigurationInfo;
			this.isADDependentRole = false;
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("AdamLdapPort", this.AdamLdapPort);
			base.Parameters.AddWithValue("AdamSslPort", this.AdamSslPort);
			base.Parameters.AddWithValue("StartTransportService", this.StartTransportService);
			base.Parameters.AddWithValue("Industry", base.SetupContext.Industry);
			SetupLogger.TraceExit();
		}

		public ushort AdamLdapPort
		{
			get
			{
				return this.gatewayRoleConfigurationInfo.AdamLdapPort;
			}
			set
			{
				this.gatewayRoleConfigurationInfo.AdamLdapPort = value;
			}
		}

		public ushort AdamSslPort
		{
			get
			{
				return this.gatewayRoleConfigurationInfo.AdamSslPort;
			}
			set
			{
				this.gatewayRoleConfigurationInfo.AdamSslPort = value;
			}
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			base.UpdatePreCheckTaskDataHandler();
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			instance.AdamLdapPort = this.AdamLdapPort;
			instance.AdamSslPort = this.AdamSslPort;
		}

		private GatewayRoleConfigurationInfo gatewayRoleConfigurationInfo;
	}
}
