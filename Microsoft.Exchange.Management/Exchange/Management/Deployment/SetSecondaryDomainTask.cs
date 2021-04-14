using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "SecondaryDomain", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class SetSecondaryDomainTask : SecondaryDomainTaskBase
	{
		public SetSecondaryDomainTask()
		{
			base.Fields["InstallationMode"] = InstallationModes.BuildToBuildUpgrade;
		}

		protected override bool ShouldExecuteComponentTasks()
		{
			return true;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.SetSecondaryDomainDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSecondaryDomain(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public AcceptedDomainIdParameter Identity
		{
			get
			{
				return (AcceptedDomainIdParameter)base.Fields["SecondaryDomainIdentity"];
			}
			set
			{
				base.Fields["SecondaryDomainIdentity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public AuthenticationType AuthenticationType
		{
			get
			{
				return (AuthenticationType)(base.Fields["AuthenticationType"] ?? AuthenticationType.Managed);
			}
			set
			{
				base.Fields["AuthenticationType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OutBoundOnly
		{
			get
			{
				return (bool)(base.Fields["OutBoundOnly"] ?? false);
			}
			set
			{
				base.Fields["OutBoundOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MakeDefault
		{
			get
			{
				return (bool)(base.Fields["MakeDefault"] ?? false);
			}
			set
			{
				base.Fields["MakeDefault"] = value;
			}
		}

		private bool PartnerMode
		{
			get
			{
				return (bool)(base.Fields["PartnerMode"] ?? false);
			}
			set
			{
				base.Fields["PartnerMode"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (base.ExchangeRunspaceConfig != null)
			{
				this.PartnerMode = base.ExchangeRunspaceConfig.PartnerMode;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.acceptedDomain = OrganizationTaskHelper.GetAcceptedDomain(this.Identity, base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
			if (this.acceptedDomain.PendingRemoval)
			{
				base.WriteError(new CannotOperateOnAcceptedDomainPendingRemovalException(this.acceptedDomain.DomainName.ToString()), ErrorCategory.InvalidOperation, null);
			}
			this.orgOuDN = this.acceptedDomain.OrganizationId.OrganizationalUnit.DistinguishedName;
			this.organizationIdParam = new OrganizationIdParameter(this.orgOuDN);
			if (!OrganizationTaskHelper.CanProceedWithOrganizationTask(this.organizationIdParam, base.Session, SetSecondaryDomainTask.IgnorableFlagsOnStatusTimeout, new Task.TaskErrorLoggingDelegate(base.WriteError)))
			{
				base.WriteError(new OrganizationPendingOperationException(Strings.ErrorCannotOperateOnOrgInCurrentState), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Fields["PrimaryOrganization"] = this.orgOuDN;
			base.Fields["SecondarySmtpDomainName"] = this.acceptedDomain.DomainName.ToString();
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private OrganizationIdParameter organizationIdParam;

		private AcceptedDomain acceptedDomain;

		private string orgOuDN;

		private static readonly OrganizationStatus[] IgnorableFlagsOnStatusTimeout = new OrganizationStatus[]
		{
			OrganizationStatus.PendingAcceptedDomainAddition,
			OrganizationStatus.PendingAcceptedDomainRemoval,
			OrganizationStatus.Suspended,
			OrganizationStatus.LockedOut
		};
	}
}
