using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "SecondaryDomain", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveSecondaryDomainTask : SecondaryDomainTaskBase
	{
		public RemoveSecondaryDomainTask()
		{
			base.Fields["InstallationMode"] = InstallationModes.Uninstall;
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

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipRecipients
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipRecipients"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipRecipients"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override bool ShouldExecuteComponentTasks()
		{
			return true;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.RemoveSecondaryDomainDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveSecondaryDomain(this.Identity.ToString());
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new RemoveSecondaryDomainTaskModuleFactory();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.acceptedDomain = OrganizationTaskHelper.GetAcceptedDomain(this.Identity, base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
			this.organizationIdParam = new OrganizationIdParameter(this.acceptedDomain.OrganizationId);
			if (!OrganizationTaskHelper.CanProceedWithOrganizationTask(this.organizationIdParam, base.Session, RemoveSecondaryDomainTask.IgnorableFlagsOnStatusTimeout, new Task.TaskErrorLoggingDelegate(base.WriteError)))
			{
				base.WriteError(new OrganizationPendingOperationException(Strings.ErrorCannotOperateOnOrgInCurrentState), ErrorCategory.InvalidOperation, null);
			}
			RemoveAcceptedDomain.CheckDomainForRemoval(this.acceptedDomain, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void FilterComponents()
		{
			base.FilterComponents();
			if (this.SkipRecipients)
			{
				foreach (SetupComponentInfo setupComponentInfo in base.ComponentInfoList)
				{
					setupComponentInfo.Tasks.RemoveAll(delegate(TaskInfo taskInfo)
					{
						OrgTaskInfo orgTaskInfo = taskInfo as OrgTaskInfo;
						return orgTaskInfo != null && orgTaskInfo.Uninstall != null && orgTaskInfo.Uninstall.Tenant != null && orgTaskInfo.Uninstall.Tenant.RecipientOperation;
					});
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Fields["PrimaryOrganization"] = this.acceptedDomain.OrganizationId.OrganizationalUnit.DistinguishedName;
			base.Fields["SecondarySmtpDomainName"] = this.acceptedDomain.DomainName.ToString();
			ExchangeConfigurationUnit exchangeConfigUnitFromOrganizationId = OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId(this.organizationIdParam, base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
			base.Fields["TenantExternalDirectoryOrganizationId"] = exchangeConfigUnitFromOrganizationId.ExternalDirectoryOrganizationId;
			IConfigurationSession session = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(base.Session, exchangeConfigUnitFromOrganizationId.OrganizationId, true);
			if (exchangeConfigUnitFromOrganizationId.OrganizationStatus == OrganizationStatus.PendingAcceptedDomainAddition || exchangeConfigUnitFromOrganizationId.OrganizationStatus == OrganizationStatus.PendingAcceptedDomainRemoval)
			{
				OrganizationTaskHelper.SetOrganizationStatus(this.organizationIdParam, session, OrganizationStatus.Active, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private OrganizationIdParameter organizationIdParam;

		private AcceptedDomain acceptedDomain;

		private static readonly OrganizationStatus[] IgnorableFlagsOnStatusTimeout = new OrganizationStatus[]
		{
			OrganizationStatus.PendingAcceptedDomainAddition,
			OrganizationStatus.PendingAcceptedDomainRemoval,
			OrganizationStatus.ReadyForRemoval,
			OrganizationStatus.SoftDeleted
		};
	}
}
