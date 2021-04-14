using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Start", "OrganizationPilot", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class StartOrganizationPilotTask : StartOrganizationUpgradeBase
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartOrganizationPilotDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartOrganizationPilot(base.Identity.ToString());
			}
		}

		protected override string TargetOfferId
		{
			get
			{
				if (this.pilotOfferId == null)
				{
					if (ServicePlanConfiguration.GetInstance().IsPilotOffer(this.tenantCU.ProgramId, this.tenantCU.OfferId))
					{
						this.pilotOfferId = this.tenantCU.OfferId;
					}
					else if (!ServicePlanConfiguration.GetInstance().TryGetPilotOfferId(this.tenantCU.ProgramId, this.tenantCU.OfferId, out this.pilotOfferId))
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanDoesNotSupportPilot(this.tenantCU.Name, this.tenantCU.ServicePlan, this.tenantCU.ProgramId)), (ErrorCategory)1000, null);
					}
				}
				return this.pilotOfferId;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new StartOrganizationPilotTaskModuleFactory();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.tenantCU.IsUpgradingOrganization)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotStartPilotFOrOrgBeingUpgraded(this.tenantCU.OrganizationId.OrganizationalUnit.Name)), (ErrorCategory)1002, null);
			}
			string b;
			if (!this.tenantCU.IsPilotingOrganization && (!ServicePlanConfiguration.GetInstance().TryGetAllowedSorceServicePlanForPilot(this.tenantCU.ProgramId, this.TargetOfferId, out b) || this.tenantCU.ServicePlan != b))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanDoesNotSupportPilot(this.tenantCU.OrganizationId.OrganizationalUnit.Name, this.tenantCU.ServicePlan, this.tenantCU.ProgramId)), (ErrorCategory)1000, null);
			}
			base.InternalPilotEnabled = true;
			TaskLogger.LogExit();
		}

		private string pilotOfferId;
	}
}
