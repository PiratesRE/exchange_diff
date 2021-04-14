using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Remove", "ManagementRoleAssignment", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveManagementRoleAssignment : RemoveSystemConfigurationObjectTask<RoleAssignmentIdParameter, ExchangeRoleAssignment>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveManagementRoleAssignment(this.Identity.ToString(), (base.DataObject.Role == null) ? "<null>" : base.DataObject.Role.ToString(), (base.DataObject.User == null) ? "<null>" : base.DataObject.User.ToString(), base.DataObject.RoleAssignmentDelegationType.ToString(), base.DataObject.RecipientWriteScope.ToString(), base.DataObject.ConfigWriteScope.ToString());
			}
		}

		[Parameter(Mandatory = false)]
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			RoleHelper.ValidateAssignmentMethod(this.Identity, this.Identity.User, base.DataObject.Role, base.DataObject.User, new RoleHelper.ErrorRoleAssignmentDelegate(Strings.ErrorRemoveGroupRoleAssignment), new RoleHelper.ErrorRoleAssignmentDelegate(Strings.ErrorRemoveMailboxPlanRoleAssignment), new RoleHelper.ErrorRoleAssignmentDelegate(Strings.ErrorRemovePolicyRoleAssignment), new Task.TaskErrorLoggingDelegate(base.WriteError));
			RoleAssignmentsGlobalConstraints roleAssignmentsGlobalConstraints = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
			roleAssignmentsGlobalConstraints.ValidateIsSafeToRemoveAssignment(base.DataObject);
			RoleHelper.HierarchyRoleAssignmentChecking(base.DataObject, base.ExchangeRunspaceConfig, this.ConfigurationSession, base.ExecutingUserOrganizationId, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), true);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.ResolveDataObject();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(base.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(base.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
