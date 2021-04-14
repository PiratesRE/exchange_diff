using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "RoleAssignmentPolicy", SupportsShouldProcess = true)]
	public sealed class NewRoleAssignmentPolicy : NewMailboxPolicyBase<RoleAssignmentPolicy>
	{
		[Parameter]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefault
		{
			get
			{
				return this.DataObject.IsDefault;
			}
			set
			{
				this.DataObject.IsDefault = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public RoleIdParameter[] Roles
		{
			get
			{
				return (RoleIdParameter[])base.Fields["Roles"];
			}
			set
			{
				base.Fields["Roles"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string text = RoleGroupCommon.NamesFromObjects(this.roles);
				return Strings.ConfirmationMessageNewRBACDefaultPolicy(base.Name, text);
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		private bool UpdateExistingDefaultPolicies
		{
			get
			{
				return this.existingDefaultPolicies != null && this.existingDefaultPolicies.Count > 0;
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (RoleAssignmentPolicy)base.PrepareDataObject();
			this.DataObject.Description = this.Description;
			if (!base.HasErrors)
			{
				ADObjectId orgContainerId = ((IConfigurationSession)base.DataSession).GetOrgContainerId();
				ADObjectId descendantId = orgContainerId.GetDescendantId(RoleAssignmentPolicy.RdnContainer);
				this.DataObject.SetId(descendantId.GetChildId(base.Name));
				this.PrepareRolesAndRoleAssignments();
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		private void PrepareRolesAndRoleAssignments()
		{
			RoleAssigneeType assigneeType = RoleAssigneeType.RoleAssignmentPolicy;
			if (base.Fields.IsChanged("Roles") && this.Roles != null)
			{
				this.roles = new MultiValuedProperty<ExchangeRole>();
				this.roleAssignments = new List<ExchangeRoleAssignment>();
				this.PrepareRoles();
				this.PrepareRoleAssignments(assigneeType);
			}
		}

		private void PrepareRoles()
		{
			bool flag = false;
			foreach (RoleIdParameter roleIdParameter in this.Roles)
			{
				ExchangeRole exchangeRole = (ExchangeRole)base.GetDataObject<ExchangeRole>(roleIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRoleNotFound(roleIdParameter.ToString())), new LocalizedString?(Strings.ErrorRoleNotUnique(roleIdParameter.ToString())));
				if (exchangeRole.RoleType == RoleType.MyBaseOptions)
				{
					flag = true;
				}
				this.roles.Add(exchangeRole);
			}
			if (!flag)
			{
				this.WriteWarning(Strings.WarningNoMyBaseOptionsRole(RoleType.MyBaseOptions.ToString()));
			}
		}

		private void PrepareRoleAssignments(RoleAssigneeType assigneeType)
		{
			foreach (ExchangeRole role in this.roles)
			{
				bool flag = false;
				ExchangeRoleAssignment exchangeRoleAssignment = new ExchangeRoleAssignment();
				RoleHelper.PrepareNewRoleAssignmentWithUniqueNameAndDefaultScopes(null, exchangeRoleAssignment, role, this.DataObject.Id, this.DataObject.OrganizationId, assigneeType, RoleAssignmentDelegationType.Regular, this.ConfigurationSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
				RoleHelper.AnalyzeAndStampCustomizedWriteScopes(this, exchangeRoleAssignment, role, this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ManagementScope>), ref flag, ref this.ou, ref this.customRecipientScope, ref this.customConfigScope);
				if (!flag && base.ExchangeRunspaceConfig != null)
				{
					RoleHelper.HierarchicalCheckForRoleAssignmentCreation(this, exchangeRoleAssignment, this.customRecipientScope, this.customConfigScope, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				this.roleAssignments.Add(exchangeRoleAssignment);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			IList<RoleAssignmentPolicy> policies = RoleAssignmentPolicyHelper.GetPolicies((IConfigurationSession)base.DataSession, null);
			this.CheckFirstPolicyIsDefault(policies);
			this.CheckForExistingDefaultPolicies(policies);
			this.CheckForAdminRoles();
		}

		private void CheckForAdminRoles()
		{
			if (this.roles == null)
			{
				return;
			}
			foreach (ExchangeRole exchangeRole in this.roles)
			{
				if (!exchangeRole.IsEndUserRole)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNonEndUserRoleCannoBeAssignedToPolicy(exchangeRole.Name)), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
		}

		private void CheckForExistingDefaultPolicies(IList<RoleAssignmentPolicy> allPolicies)
		{
			if (this.DataObject.IsDefault)
			{
				this.existingDefaultPolicies = new List<RoleAssignmentPolicy>();
				foreach (RoleAssignmentPolicy roleAssignmentPolicy in allPolicies)
				{
					if (roleAssignmentPolicy.IsDefault)
					{
						this.existingDefaultPolicies.Add(roleAssignmentPolicy);
					}
				}
			}
		}

		private void CheckFirstPolicyIsDefault(IList<RoleAssignmentPolicy> allPolicies)
		{
			if (allPolicies.Count == 0)
			{
				if (!this.DataObject.IsDefault && this.DataObject.IsModified(RoleAssignmentPolicySchema.IsDefault))
				{
					this.WriteWarning(Strings.FirstRoleAssignmentPolicyMustBeDefault);
				}
				this.DataObject.IsDefault = true;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.UpdateExistingDefaultPolicies && !base.ShouldContinue(Strings.ConfirmationMessageSwitchMailboxPolicy("RoleAssignmentPolicy", this.DataObject.Name)))
			{
				return;
			}
			base.InternalProcessRecord();
			if (this.UpdateExistingDefaultPolicies)
			{
				try
				{
					RoleAssignmentPolicyHelper.ClearIsDefaultOnPolicies((IConfigurationSession)base.DataSession, this.existingDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
			this.ProcessRoleAssignments();
		}

		private void ProcessRoleAssignments()
		{
			if (this.roleAssignments == null)
			{
				return;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 359, "ProcessRoleAssignments", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxPolicies\\RoleAssignmentPolicyTasks.cs");
			tenantOrTopologyConfigurationSession.LinkResolutionServer = this.DataObject.OriginatingServer;
			List<ExchangeRoleAssignment> createdRoleAssignments = new List<ExchangeRoleAssignment>();
			string empty = string.Empty;
			try
			{
				this.WriteRoleAssignments(tenantOrTopologyConfigurationSession, createdRoleAssignments, ref empty);
			}
			catch (Exception)
			{
				this.WriteWarning(Strings.WarningCouldNotCreateRoleAssignment(empty, base.Name));
				this.RollbackChanges(tenantOrTopologyConfigurationSession, createdRoleAssignments);
				throw;
			}
		}

		private void WriteRoleAssignments(IConfigurationSession writableConfigSession, List<ExchangeRoleAssignment> createdRoleAssignments, ref string currentRoleAssignmentName)
		{
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in this.roleAssignments)
			{
				currentRoleAssignmentName = exchangeRoleAssignment.Id.Name;
				writableConfigSession.Save(exchangeRoleAssignment);
				createdRoleAssignments.Add(exchangeRoleAssignment);
			}
		}

		private void RollbackChanges(IConfigurationSession writableConfigSession, List<ExchangeRoleAssignment> createdRoleAssignments)
		{
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in createdRoleAssignments)
			{
				base.WriteVerbose(Strings.VerboseRemovingRoleAssignment(exchangeRoleAssignment.Id.ToString()));
				writableConfigSession.Delete(exchangeRoleAssignment);
				base.WriteVerbose(Strings.VerboseRemovedRoleAssignment(exchangeRoleAssignment.Id.ToString()));
			}
			base.WriteVerbose(Strings.VerboseRemovingRoleAssignmentPolicy(this.DataObject.Id.ToString()));
			base.DataSession.Delete(this.DataObject);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			RoleAssignmentPolicy roleAssignmentPolicy = (RoleAssignmentPolicy)dataObject;
			roleAssignmentPolicy.PopulateRoles(this.FetchRoleAssignments());
			base.WriteResult(roleAssignmentPolicy);
		}

		private Result<ExchangeRoleAssignment>[] FetchRoleAssignments()
		{
			Result<ExchangeRoleAssignment>[] array = null;
			if (this.roleAssignments != null)
			{
				array = new Result<ExchangeRoleAssignment>[this.roleAssignments.Count];
				for (int i = 0; i < this.roleAssignments.Count; i++)
				{
					array[i] = new Result<ExchangeRoleAssignment>(this.roleAssignments[i], null);
				}
			}
			return array;
		}

		private MultiValuedProperty<ExchangeRole> roles;

		private List<ExchangeRoleAssignment> roleAssignments;

		private ExchangeOrganizationalUnit ou;

		private ManagementScope customRecipientScope;

		private ManagementScope customConfigScope;
	}
}
