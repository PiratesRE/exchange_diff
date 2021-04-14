using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallRoleBaseDataHandler : ConfigurationDataHandler
	{
		public InstallRoleBaseDataHandler(ISetupContext context, string installableUnitName, string commandText, MonadConnection connection) : base(context, installableUnitName, commandText, connection)
		{
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			if (base.SetupContext.IsDatacenter && base.SetupContext.IsFfo)
			{
				base.Parameters.AddWithValue("IsFfo", true);
			}
			base.Parameters.AddWithValue("updatesdir", this.UpdatesDir);
			if (InstallableUnitConfigurationInfoManager.IsServerRoleBasedConfigurableInstallableUnit(base.InstallableUnitName) && base.SetupContext.ServerCustomerFeedbackEnabled != base.SetupContext.OriginalServerCustomerFeedbackEnabled)
			{
				base.Parameters.AddWithValue("CustomerFeedbackEnabled", base.SetupContext.ServerCustomerFeedbackEnabled);
			}
			base.Parameters.AddWithValue("LanguagePacksPath", base.GetMsiSourcePath());
			SetupLogger.TraceExit();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			base.UpdatePreCheckTaskDataHandler();
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			instance.CustomerFeedbackEnabled = base.SetupContext.ServerCustomerFeedbackEnabled;
			instance.ActiveDirectorySplitPermissions = base.SetupContext.ActiveDirectorySplitPermissions;
		}

		private LongPath UpdatesDir
		{
			get
			{
				return base.SetupContext.UpdatesDir;
			}
			set
			{
				base.SetupContext.UpdatesDir = value;
			}
		}
	}
}
