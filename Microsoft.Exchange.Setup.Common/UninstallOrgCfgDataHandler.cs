using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UninstallOrgCfgDataHandler : OrgCfgDataHandler
	{
		public UninstallOrgCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "Uninstall-ExchangeOrganization", connection)
		{
			base.WorkUnit.Text = Strings.OrganizationInstallText;
			this.setRemoveOrganization = false;
		}

		private void DetermineParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			this.setRemoveOrganization = false;
			SetupLogger.Log(Strings.DeterminingOrgPrepParameters);
			if (base.SetupContext.CanOrgBeRemoved)
			{
				this.setRemoveOrganization = true;
				SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("RemoveOrganization"));
			}
			SetupLogger.TraceExit();
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			this.DetermineParameters();
			if (this.setRemoveOrganization)
			{
				base.Parameters.AddWithValue("RemoveOrganization", true);
			}
			SetupLogger.TraceExit();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			instance.AddRole("Global");
		}

		public override bool WillDataHandlerDoAnyWork()
		{
			bool result = false;
			this.DetermineParameters();
			if (this.setRemoveOrganization)
			{
				result = true;
			}
			return result;
		}

		private const string removeOrganizationArgument = "RemoveOrganization";

		private bool setRemoveOrganization;
	}
}
