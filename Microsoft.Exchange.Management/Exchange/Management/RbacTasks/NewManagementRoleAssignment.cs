using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("New", "ManagementRoleAssignment", SupportsShouldProcess = true, DefaultParameterSetName = "User")]
	public sealed class NewManagementRoleAssignment : NewMultitenancySystemConfigurationObjectTask<ExchangeRoleAssignment>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewManagementRoleAssignment(this.DataObject.Name, this.DataObject.Role.ToString(), this.DataObject.User.ToString(), this.DataObject.RoleAssignmentDelegationType.ToString(), this.DataObject.RecipientWriteScope.ToString(), this.DataObject.ConfigWriteScope.ToString());
			}
		}

		[Parameter(Mandatory = false, Position = 0)]
		public new string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		public RoleIdParameter Role
		{
			get
			{
				return (RoleIdParameter)base.Fields[RbacCommonParameters.ParameterRole];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterRole] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "User")]
		public UserIdParameter User
		{
			get
			{
				return (UserIdParameter)base.Fields[RbacCommonParameters.ParameterUser];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterUser] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SecurityGroup")]
		public SecurityGroupIdParameter SecurityGroup
		{
			get
			{
				return (SecurityGroupIdParameter)base.Fields[RbacCommonParameters.ParameterSecurityGroup];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterSecurityGroup] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Policy")]
		public MailboxPolicyIdParameter Policy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[RbacCommonParameters.ParameterPolicy];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterPolicy] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Computer")]
		public ComputerIdParameter Computer
		{
			get
			{
				return (ComputerIdParameter)base.Fields[RbacCommonParameters.ParameterComputer];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterComputer] = value;
			}
		}

		[Parameter(ParameterSetName = "SecurityGroup")]
		[Parameter(ParameterSetName = "User")]
		public SwitchParameter Delegating
		{
			get
			{
				return (SwitchParameter)(base.Fields[RbacCommonParameters.ParameterDelegating] ?? false);
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterDelegating] = value;
			}
		}

		[Parameter]
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

		[Parameter]
		[ValidateNotNullOrEmpty]
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

		[ValidateNotNullOrEmpty]
		[Parameter]
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
		[Parameter]
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

		[Parameter]
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

		[ValidateNotNullOrEmpty]
		[Parameter]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter UnScopedTopLevel
		{
			get
			{
				return (SwitchParameter)(base.Fields["UnScopedTopLevel"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UnScopedTopLevel"] = value;
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

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ExchangeOrganizationalUnit exchangeOrganizationalUnit = null;
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			this.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			this.DataObject = (ExchangeRoleAssignment)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (!this.IgnoreDehydratedFlag)
			{
				SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			this.role = (ExchangeRole)base.GetDataObject<ExchangeRole>(this.Role, base.DataSession, null, new LocalizedString?(Strings.ErrorRoleNotFound(this.Role.ToString())), new LocalizedString?(Strings.ErrorRoleNotUnique(this.Role.ToString())));
			RoleHelper.VerifyNoScopeForUnScopedRole(base.Fields, this.role, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.role != null && this.role.IsDeprecated)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotCreateRoleAssignmentToADeprecatedRole(this.role.ToString())), ErrorCategory.InvalidOperation, null);
			}
			RoleAssigneeType roleAssigneeType;
			ADObject adobject;
			if (this.Policy != null)
			{
				RoleAssignmentPolicy roleAssignmentPolicy = (RoleAssignmentPolicy)base.GetDataObject<RoleAssignmentPolicy>(this.Policy, RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId), null, new LocalizedString?(Strings.ErrorRBACPolicyNotFound(this.Policy.ToString())), new LocalizedString?(Strings.ErrorRBACPolicyNotUnique(this.Policy.ToString())));
				if (!this.role.IsEndUserRole)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNonEndUserRoleCannoBeAssignedToPolicy(this.role.Name)), ErrorCategory.InvalidOperation, roleAssignmentPolicy.Id);
				}
				OrganizationId organizationId = OrganizationId.ForestWideOrgId;
				if (this.ConfigurationSession is ITenantConfigurationSession)
				{
					organizationId = TaskHelper.ResolveOrganizationId(this.role.Id, ExchangeRole.RdnContainer, (ITenantConfigurationSession)this.ConfigurationSession);
				}
				ADObjectId adobjectId;
				if (OrganizationId.ForestWideOrgId.Equals(organizationId))
				{
					adobjectId = this.ConfigurationSession.GetOrgContainerId();
				}
				else
				{
					adobjectId = organizationId.ConfigurationUnit;
				}
				if (!roleAssignmentPolicy.Id.IsDescendantOf(adobjectId))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorPolicyOutOfRoleScope(roleAssignmentPolicy.Id.ToString(), adobjectId.Name)), ErrorCategory.InvalidOperation, null);
				}
				roleAssigneeType = RoleAssigneeType.RoleAssignmentPolicy;
				adobject = roleAssignmentPolicy;
			}
			else
			{
				ADRecipient adrecipient = null;
				if (this.User != null)
				{
					adrecipient = (ADUser)base.GetDataObject<ADUser>(this.User, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorAssigneeUserNotFound(this.User.ToString())), new LocalizedString?(Strings.ErrorAssigneeUserNotUnique(this.User.ToString())));
				}
				else if (this.SecurityGroup != null)
				{
					adrecipient = (ADGroup)base.GetDataObject<ADGroup>(this.SecurityGroup, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorAssigneeSecurityGroupNotFound(this.SecurityGroup.ToString())), new LocalizedString?(Strings.ErrorAssigneeSecurityGroupNotUnique(this.SecurityGroup.ToString())));
				}
				else if (this.Computer != null)
				{
					adrecipient = (ADComputerRecipient)base.GetDataObject<ADComputerRecipient>(this.Computer, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorAssigneeComputerNotFound(this.Computer.ToString())), new LocalizedString?(Strings.ErrorAssigneeComputerNotUnique(this.Computer.ToString())));
				}
				RoleHelper.ValidateRoleAssignmentUser(adrecipient, new Task.TaskErrorLoggingDelegate(base.WriteError), false);
				roleAssigneeType = ExchangeRoleAssignment.RoleAssigneeTypeFromADRecipient(adrecipient);
				adobject = adrecipient;
			}
			((IDirectorySession)base.DataSession).LinkResolutionServer = adobject.OriginatingServer;
			RoleHelper.PrepareNewRoleAssignmentWithUniqueNameAndDefaultScopes(this.Name, this.DataObject, this.role, adobject.Id, adobject.OrganizationId, roleAssigneeType, this.Delegating.IsPresent ? RoleAssignmentDelegationType.Delegating : RoleAssignmentDelegationType.Regular, this.ConfigurationSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.role.IsUnscopedTopLevel && this.UnScopedTopLevel)
			{
				this.skipHRoleCheck = true;
				if (this.Delegating)
				{
					this.DataObject.RoleAssignmentDelegationType = RoleAssignmentDelegationType.DelegatingOrgWide;
				}
			}
			else
			{
				RoleHelper.AnalyzeAndStampCustomizedWriteScopes(this, this.DataObject, this.role, this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ManagementScope>), ref this.skipHRoleCheck, ref exchangeOrganizationalUnit, ref this.customRecipientScope, ref this.customConfigScope);
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			RbacRoleAssignmentCommon.CheckMutuallyExclusiveParameters(this);
			base.CheckExclusiveParameters(new object[]
			{
				RbacCommonParameters.ParameterDelegating,
				"ExclusiveRecipientWriteScope"
			});
			base.CheckExclusiveParameters(new object[]
			{
				RbacCommonParameters.ParameterDelegating,
				"ExclusiveConfigWriteScope"
			});
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.UnScopedTopLevel && !this.role.IsUnscopedTopLevel)
			{
				base.WriteError(new InvalidOperationException(Strings.ParameterAllowedOnlyForTopLevelRoleManipulation("UnScopedTopLevel", RoleType.UnScoped.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (!this.skipHRoleCheck && base.ExchangeRunspaceConfig != null)
			{
				RoleHelper.HierarchicalCheckForRoleAssignmentCreation(this, this.DataObject, this.customRecipientScope, this.customConfigScope, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.customConfigScope != null && ScopeRestrictionType.DatabaseScope == this.customConfigScope.ScopeRestrictionType)
			{
				this.WriteWarning(Strings.WarningRoleAssignmentWithDatabaseScopeApplicableOnlyInSP);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			base.WriteResult(this.ConvertDataObjectToPresentationObject(dataObject));
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			ExchangeRoleAssignment exchangeRoleAssignment = (ExchangeRoleAssignment)dataObject;
			ExchangeRoleAssignmentPresentation result = new ExchangeRoleAssignmentPresentation(exchangeRoleAssignment, exchangeRoleAssignment.User, AssignmentMethod.Direct);
			TaskLogger.LogExit();
			return result;
		}

		private ManagementScope customRecipientScope;

		private ManagementScope customConfigScope;

		private bool skipHRoleCheck;

		private ExchangeRole role;
	}
}
