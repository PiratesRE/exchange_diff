using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewMigrationBatch : EcpWizardForm
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			string a = base.Request.QueryString["migration"];
			if (string.Equals(a, "localmove", StringComparison.OrdinalIgnoreCase))
			{
				this.wizardFormSheet.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=MigrationBatch&workflow=LocalMoveGetObjectForNew");
				base.Caption = Strings.NewLocalMoveCaption;
				return;
			}
			if (string.Equals(a, "crossforest", StringComparison.OrdinalIgnoreCase))
			{
				this.wizardFormSheet.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=MigrationBatch&workflow=OnboardingGetObjectForNew");
				base.Caption = Strings.NewCrossForestMoveCaption;
				return;
			}
			if (string.Equals(a, "onboarding", StringComparison.OrdinalIgnoreCase))
			{
				this.wizardFormSheet.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=MigrationBatch&workflow=OnboardingGetObjectForNew");
				base.Caption = Strings.NewMigrationBatchCaption;
				return;
			}
			if (string.Equals(a, "offboarding", StringComparison.OrdinalIgnoreCase))
			{
				this.wizardFormSheet.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=MigrationBatch&workflow=OffboardingGetObjectForNew");
				base.Caption = Strings.NewMigrationBatchCaption;
				return;
			}
			throw new BadQueryParameterException("migration");
		}

		private const string MigrationScenario = "migration";

		private const string LocalMove = "localmove";

		private const string CrossForestMove = "crossforest";

		private const string Onboarding = "onboarding";

		private const string Offboarding = "offboarding";

		protected WizardFormSheet wizardFormSheet;
	}
}
