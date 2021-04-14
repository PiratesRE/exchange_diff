using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Update", "ServicePlan", SupportsShouldProcess = true, DefaultParameterSetName = "IdentityParameterSet")]
	public sealed class UpdateServicePlanTask : ManageServicePlanMigrationBase
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.UpdateServicePlanDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateServicePlan(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrateServicePlanParameterSet")]
		public string ProgramId
		{
			get
			{
				return (string)base.Fields["TenantProgramId"];
			}
			set
			{
				base.Fields["TenantProgramId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrateServicePlanParameterSet")]
		public string OfferId
		{
			get
			{
				return (string)base.Fields["TenantOfferId"];
			}
			set
			{
				base.Fields["TenantOfferId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ConfigOnly
		{
			get
			{
				return this.configOnly;
			}
			set
			{
				this.configOnly = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Conservative
		{
			get
			{
				return this.conservative;
			}
			set
			{
				this.conservative = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeUserUpdatePhase
		{
			get
			{
				return this.includeUserUpdatePhase;
			}
			set
			{
				this.includeUserUpdatePhase = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			base.LoadTenantCU();
			TaskLogger.LogExit();
		}

		protected override void ResolveTargetOffer()
		{
			if (!string.IsNullOrEmpty(this.OfferId) && !string.IsNullOrEmpty(this.ProgramId))
			{
				this.targetProgramId = this.ProgramId;
				this.targetOfferId = this.OfferId;
				return;
			}
			this.targetProgramId = this.tenantCU.ProgramId;
			this.targetOfferId = this.tenantCU.OfferId;
		}
	}
}
