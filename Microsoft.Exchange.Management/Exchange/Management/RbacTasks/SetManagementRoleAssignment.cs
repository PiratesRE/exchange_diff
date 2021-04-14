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
	[Cmdlet("Set", "ManagementRoleAssignment", SupportsShouldProcess = true, DefaultParameterSetName = "RelativeRecipientWriteScope")]
	public sealed class SetManagementRoleAssignment : SystemConfigurationObjectActionTask<RoleAssignmentIdParameter, ExchangeRoleAssignment>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetManagementRoleAssignment(this.Identity.ToString(), (this.DataObject.Role == null) ? "<null>" : this.DataObject.Role.ToString(), (this.DataObject.User == null) ? "<null>" : this.DataObject.User.ToString(), this.DataObject.RoleAssignmentDelegationType.ToString(), this.DataObject.RecipientWriteScope.ToString(), this.DataObject.ConfigWriteScope.ToString());
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

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override RoleAssignmentIdParameter Identity
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

		[Parameter(ParameterSetName = "RelativeRecipientWriteScope")]
		public RecipientWriteScopeType RecipientRelativeWriteScope
		{
			get
			{
				return (RecipientWriteScopeType)(base.Fields[RbacCommonParameters.ParameterRecipientRelativeWriteScope] ?? -1);
			}
			set
			{
				base.VerifyValues<RecipientWriteScopeType>(RbacRoleAssignmentCommon.AllowedRecipientRelativeWriteScope, value);
				base.Fields[RbacCommonParameters.ParameterRecipientRelativeWriteScope] = value;
			}
		}

		[Parameter(ParameterSetName = "CustomRecipientWriteScope")]
		public ManagementScopeIdParameter CustomRecipientWriteScope
		{
			get
			{
				return (ManagementScopeIdParameter)base.Fields[RbacCommonParameters.ParameterCustomRecipientWriteScope];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterCustomRecipientWriteScope] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "RecipientOrganizationalUnitScope")]
		public OrganizationalUnitIdParameter RecipientOrganizationalUnitScope
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields[RbacCommonParameters.ParameterRecipientOrganizationalUnitScope];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterRecipientOrganizationalUnitScope] = value;
			}
		}

		[Parameter(ParameterSetName = "RelativeRecipientWriteScope")]
		[Parameter(ParameterSetName = "RecipientOrganizationalUnitScope")]
		[Parameter(ParameterSetName = "CustomRecipientWriteScope")]
		public ManagementScopeIdParameter CustomConfigWriteScope
		{
			get
			{
				return (ManagementScopeIdParameter)base.Fields[RbacCommonParameters.ParameterCustomConfigWriteScope];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterCustomConfigWriteScope] = value;
			}
		}

		[Parameter(ParameterSetName = "ExclusiveScope")]
		[ValidateNotNullOrEmpty]
		public ManagementScopeIdParameter ExclusiveRecipientWriteScope
		{
			get
			{
				return (ManagementScopeIdParameter)base.Fields["ExclusiveRecipientWriteScope"];
			}
			set
			{
				base.Fields["ExclusiveRecipientWriteScope"] = value;
			}
		}

		[Parameter(ParameterSetName = "ExclusiveScope")]
		[ValidateNotNullOrEmpty]
		public ManagementScopeIdParameter ExclusiveConfigWriteScope
		{
			get
			{
				return (ManagementScopeIdParameter)base.Fields["ExclusiveConfigWriteScope"];
			}
			set
			{
				base.Fields["ExclusiveConfigWriteScope"] = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return (bool)(base.Fields[RbacCommonParameters.ParameterEnabled] ?? false);
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterEnabled] = value;
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
			RoleHelper.ValidateAssignmentMethod(this.Identity, this.Identity.User, this.DataObject.Role, this.DataObject.User, new RoleHelper.ErrorRoleAssignmentDelegate(Strings.ErrorSetGroupRoleAssignment), new RoleHelper.ErrorRoleAssignmentDelegate(Strings.ErrorSetMailboxPlanRoleAssignment), new RoleHelper.ErrorRoleAssignmentDelegate(Strings.ErrorSetPolicyRoleAssignment), new Task.TaskErrorLoggingDelegate(base.WriteError));
			bool flag = false;
			if (base.Fields.IsModified(RbacCommonParameters.ParameterEnabled))
			{
				if (this.Enabled && this.DataObject.Enabled)
				{
					this.WriteWarning(Strings.WarningEnableEnabledRoleAssignment(this.DataObject.Id.ToString()));
				}
				else if (!this.Enabled && !this.DataObject.Enabled)
				{
					this.WriteWarning(Strings.WarningDisableDisabledRoleAssignment(this.DataObject.Id.ToString()));
				}
				else if (!this.Enabled && this.DataObject.Enabled)
				{
					flag = true;
				}
			}
			if (RoleAssignmentsGlobalConstraints.IsValidCannedRoleToGroupAssignment(this.DataObject) && (flag || RoleHelper.IsScopeSpecified(base.Fields)))
			{
				RoleAssignmentsGlobalConstraints roleAssignmentsGlobalConstraints = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
				roleAssignmentsGlobalConstraints.ValidateIsSafeToModifyAssignment(this.DataObject, flag);
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.ResolveDataObject();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterEnabled))
			{
				this.DataObject.Enabled = this.Enabled;
			}
			ExchangeRole exchangeRole = (ExchangeRole)base.GetDataObject<ExchangeRole>(new RoleIdParameter(this.DataObject.Role), this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRoleNotFound(this.DataObject.Role.ToString())), new LocalizedString?(Strings.ErrorRoleNotUnique(this.DataObject.Role.ToString())));
			if (base.Fields.IsModified(RbacCommonParameters.ParameterRecipientRelativeWriteScope))
			{
				this.DataObject.RecipientWriteScope = this.RecipientRelativeWriteScope;
				this.DataObject.CustomRecipientWriteScope = null;
			}
			else
			{
				if (base.Fields.IsModified(RbacCommonParameters.ParameterRecipientOrganizationalUnitScope))
				{
					this.DataObject.RecipientWriteScope = RecipientWriteScopeType.OU;
					bool useConfigNC = this.ConfigurationSession.UseConfigNC;
					bool useGlobalCatalog = this.ConfigurationSession.UseGlobalCatalog;
					try
					{
						this.ConfigurationSession.UseConfigNC = false;
						this.ConfigurationSession.UseGlobalCatalog = true;
						ExchangeOrganizationalUnit exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)base.GetDataObject<ExchangeOrganizationalUnit>(this.RecipientOrganizationalUnitScope, this.ConfigurationSession, this.DataObject.OrganizationalUnitRoot, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.RecipientOrganizationalUnitScope.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(this.RecipientOrganizationalUnitScope.ToString())));
						this.DataObject.CustomRecipientWriteScope = exchangeOrganizationalUnit.Id;
						goto IL_321;
					}
					finally
					{
						this.ConfigurationSession.UseConfigNC = useConfigNC;
						this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog;
					}
				}
				if (base.Fields.IsModified(RbacCommonParameters.ParameterCustomRecipientWriteScope))
				{
					if (this.CustomRecipientWriteScope == null)
					{
						this.DataObject.CustomRecipientWriteScope = null;
						this.DataObject.RecipientWriteScope = (RecipientWriteScopeType)exchangeRole.ImplicitRecipientWriteScope;
					}
					else
					{
						ManagementScope andValidateDomainScope = RoleHelper.GetAndValidateDomainScope(this.CustomRecipientWriteScope, this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ManagementScope>), this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
						if (andValidateDomainScope.Exclusive)
						{
							base.WriteError(new ArgumentException(Strings.ErrorScopeExclusive(andValidateDomainScope.Id.ToString(), RbacCommonParameters.ParameterCustomRecipientWriteScope)), ErrorCategory.InvalidArgument, null);
						}
						this.DataObject.CustomRecipientWriteScope = andValidateDomainScope.Id;
						this.DataObject.RecipientWriteScope = RecipientWriteScopeType.CustomRecipientScope;
					}
				}
				else if (base.Fields.IsModified("ExclusiveRecipientWriteScope"))
				{
					ManagementScope andValidateDomainScope2 = RoleHelper.GetAndValidateDomainScope(this.ExclusiveRecipientWriteScope, this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ManagementScope>), this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
					if (!andValidateDomainScope2.Exclusive)
					{
						base.WriteError(new ArgumentException(Strings.ErrorScopeNotExclusive(andValidateDomainScope2.Id.ToString(), "ExclusiveRecipientWriteScope")), ErrorCategory.InvalidArgument, null);
					}
					this.DataObject.CustomRecipientWriteScope = andValidateDomainScope2.Id;
					this.DataObject.RecipientWriteScope = RecipientWriteScopeType.ExclusiveRecipientScope;
				}
			}
			IL_321:
			RoleHelper.VerifyNoScopeForUnScopedRole(base.Fields, exchangeRole, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (exchangeRole.ImplicitRecipientWriteScope != (ScopeType)this.DataObject.RecipientWriteScope && !RbacScope.IsScopeTypeSmaller((ScopeType)this.DataObject.RecipientWriteScope, exchangeRole.ImplicitRecipientWriteScope))
			{
				this.WriteWarning(Strings.WriteScopeGreaterThanRoleScope(this.DataObject.RecipientWriteScope.ToString(), exchangeRole.Name, exchangeRole.ImplicitRecipientWriteScope.ToString()));
				this.DataObject.CustomRecipientWriteScope = null;
				this.DataObject.RecipientWriteScope = (RecipientWriteScopeType)exchangeRole.ImplicitRecipientWriteScope;
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterCustomConfigWriteScope))
			{
				if (this.CustomConfigWriteScope == null)
				{
					this.DataObject.ConfigWriteScope = (ConfigWriteScopeType)exchangeRole.ImplicitConfigWriteScope;
					this.DataObject.CustomConfigWriteScope = null;
				}
				else
				{
					this.DataObject.ConfigWriteScope = ConfigWriteScopeType.CustomConfigScope;
					ManagementScope andValidateConfigScope = RoleHelper.GetAndValidateConfigScope(this.CustomConfigWriteScope, this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ManagementScope>), this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
					if (andValidateConfigScope.Exclusive)
					{
						base.WriteError(new ArgumentException(Strings.ErrorScopeExclusive(andValidateConfigScope.Id.ToString(), RbacCommonParameters.ParameterCustomConfigWriteScope)), ErrorCategory.InvalidArgument, null);
					}
					this.DataObject.CustomConfigWriteScope = andValidateConfigScope.Id;
					this.DataObject.ConfigWriteScope = ((andValidateConfigScope.ScopeRestrictionType == ScopeRestrictionType.PartnerDelegatedTenantScope) ? ConfigWriteScopeType.PartnerDelegatedTenantScope : ConfigWriteScopeType.CustomConfigScope);
				}
			}
			else if (base.Fields.IsModified("ExclusiveConfigWriteScope"))
			{
				ManagementScope andValidateConfigScope2 = RoleHelper.GetAndValidateConfigScope(this.ExclusiveConfigWriteScope, this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ManagementScope>), this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
				if (!andValidateConfigScope2.Exclusive)
				{
					base.WriteError(new ArgumentException(Strings.ErrorScopeNotExclusive(andValidateConfigScope2.Id.ToString(), "ExclusiveConfigWriteScope")), ErrorCategory.InvalidArgument, null);
				}
				this.DataObject.CustomConfigWriteScope = andValidateConfigScope2.Id;
				this.DataObject.ConfigWriteScope = ConfigWriteScopeType.ExclusiveConfigScope;
			}
			if (exchangeRole.ImplicitConfigWriteScope != (ScopeType)this.DataObject.ConfigWriteScope && !RbacScope.IsScopeTypeSmaller((ScopeType)this.DataObject.ConfigWriteScope, exchangeRole.ImplicitConfigWriteScope))
			{
				this.WriteWarning(Strings.WriteScopeGreaterThanRoleScope(this.DataObject.CustomConfigWriteScope.ToString(), exchangeRole.Name, exchangeRole.ImplicitConfigWriteScope.ToString()));
				this.DataObject.CustomConfigWriteScope = null;
				this.DataObject.ConfigWriteScope = (ConfigWriteScopeType)exchangeRole.ImplicitConfigWriteScope;
			}
			RoleHelper.HierarchyRoleAssignmentChecking(this.DataObject, base.ExchangeRunspaceConfig, this.ConfigurationSession, base.ExecutingUserOrganizationId, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), !this.Enabled);
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
