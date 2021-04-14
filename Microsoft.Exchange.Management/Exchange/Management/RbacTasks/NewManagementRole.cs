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
	[Cmdlet("New", "ManagementRole", SupportsShouldProcess = true, DefaultParameterSetName = "NewDerivedRoleParameterSet")]
	public sealed class NewManagementRole : NewMultitenancySystemConfigurationObjectTask<ExchangeRole>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DataObject.IsUnscopedTopLevel)
				{
					return Strings.ConfirmationMessageNewTopLevelManagementRole(this.DataObject.Name);
				}
				return Strings.ConfirmationMessageNewManagementRole(this.DataObject.Name, this.Parent.ToString());
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "NewDerivedRoleParameterSet")]
		public RoleIdParameter Parent
		{
			get
			{
				return (RoleIdParameter)base.Fields["Parent"];
			}
			set
			{
				base.Fields["Parent"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UnScopedTopLevelRoleParameterSet")]
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
			base.InternalValidate();
			ExchangeRole[] array = this.ConfigurationSession.Find<ExchangeRole>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, base.Name), null, 1);
			if (array.Length > 0)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorRoleNameMustBeUnique(base.Name)), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (ExchangeRole)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			this.DataObject.SetId(this.ConfigurationSession, ExchangeRole.RdnContainer, this.DataObject.Name);
			this.DataObject.Description = this.Description;
			if (base.ParameterSetName.Equals("UnScopedTopLevelRoleParameterSet"))
			{
				if (this.UnScopedTopLevel)
				{
					this.DataObject.RoleType = RoleType.UnScoped;
					this.DataObject.StampImplicitScopes();
					this.DataObject.StampIsEndUserRole();
				}
				else
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNewRoleInvalidValueUnscopedParameter), ErrorCategory.InvalidOperation, null);
				}
			}
			else
			{
				ExchangeRole exchangeRole = (ExchangeRole)base.GetDataObject<ExchangeRole>(this.Parent, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRoleNotFound(this.Parent.ToString())), new LocalizedString?(Strings.ErrorRoleNotUnique(this.Parent.ToString())));
				if (exchangeRole != null && exchangeRole.IsDeprecated)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCannotCreateARoleFromADeprecatedRole(exchangeRole.ToString())), ErrorCategory.InvalidOperation, null);
				}
				if (!this.DataObject.OrganizationId.Equals(exchangeRole.OrganizationId))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCannotCreateRoleAcrossOrganizations(base.CurrentOrganizationId.ToString(), exchangeRole.ToString(), exchangeRole.OrganizationId.ToString())), ErrorCategory.InvalidOperation, null);
				}
				this.DataObject.SetId(exchangeRole.Id.GetChildId(this.DataObject.Name));
				this.DataObject.RoleType = exchangeRole.RoleType;
				this.DataObject[ExchangeRoleSchema.RoleFlags] = exchangeRole[ExchangeRoleSchema.RoleFlags];
				this.DataObject.RoleEntries = exchangeRole.RoleEntries;
				MultiValuedProperty<RoleEntry> multiValuedProperty = (MultiValuedProperty<RoleEntry>)exchangeRole[ExchangeRoleSchema.InternalDownlevelRoleEntries];
				if (multiValuedProperty.Count > 0)
				{
					this.DataObject[ExchangeRoleSchema.InternalDownlevelRoleEntries] = multiValuedProperty;
					this.DataObject[ExchangeRoleSchema.RoleFlags] = exchangeRole[ExchangeRoleSchema.RoleFlags];
				}
				if (!base.CurrentTaskContext.CanBypassRBACScope && !RoleHelper.HasDelegatingHierarchicalRoleAssignmentWithoutScopeRestriction(base.ExecutingUserOrganizationId, base.ExchangeRunspaceConfig.RoleAssignments, exchangeRole.Id))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNewRoleNeedHierarchicalRoleAssignmentWithoutScopeRestriction(exchangeRole.ToString())), ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
			return this.DataObject;
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
			if (this.UnScopedTopLevel)
			{
				try
				{
					if (base.ExecutingUserOrganizationId.Equals(this.DataObject.OrganizationId))
					{
						ADObjectId id;
						RoleAssigneeType roleAssigneeType;
						if (base.TryGetExecutingUserId(out id))
						{
							roleAssigneeType = RoleAssigneeType.User;
						}
						else
						{
							roleAssigneeType = RoleAssigneeType.RoleGroup;
							bool useGlobalCatalog = base.TenantGlobalCatalogSession.UseGlobalCatalog;
							bool useConfigNC = base.TenantGlobalCatalogSession.UseConfigNC;
							bool skipRangedAttributes = base.TenantGlobalCatalogSession.SkipRangedAttributes;
							ADGroup adgroup;
							try
							{
								base.TenantGlobalCatalogSession.UseGlobalCatalog = true;
								base.TenantGlobalCatalogSession.UseConfigNC = false;
								base.TenantGlobalCatalogSession.SkipRangedAttributes = true;
								adgroup = base.TenantGlobalCatalogSession.ResolveWellKnownGuid<ADGroup>(RoleGroup.OrganizationManagement_InitInfo.WellKnownGuid, OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId) ? this.ConfigurationSession.ConfigurationNamingContext : base.TenantGlobalCatalogSession.SessionSettings.CurrentOrganizationId.ConfigurationUnit);
							}
							finally
							{
								base.TenantGlobalCatalogSession.UseGlobalCatalog = useGlobalCatalog;
								base.TenantGlobalCatalogSession.UseConfigNC = useConfigNC;
								base.TenantGlobalCatalogSession.SkipRangedAttributes = skipRangedAttributes;
							}
							if (adgroup != null)
							{
								id = adgroup.Id;
							}
							else
							{
								base.ThrowTerminatingError(new ManagementObjectNotFoundException(DirectoryStrings.ExceptionADTopologyCannotFindWellKnownExchangeGroup), (ErrorCategory)1001, RoleGroup.OrganizationManagement_InitInfo.WellKnownGuid);
							}
						}
						RoleHelper.CreateRoleAssignment(this.DataObject, id, base.ExecutingUserOrganizationId, roleAssigneeType, this.DataObject.OriginatingServer, RoleAssignmentDelegationType.DelegatingOrgWide, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.DataSession as IConfigurationSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
				catch (Exception)
				{
					this.WriteWarning(Strings.WarningFailedToCreateAssignmentForNewRole(this.DataObject.Id.ToString()));
					base.DataSession.Delete(this.DataObject);
					throw;
				}
				if (base.ExchangeRunspaceConfig != null)
				{
					base.ExchangeRunspaceConfig.LoadRoleCmdletInfo();
				}
			}
			TaskLogger.LogExit();
		}

		private const string NewDerivedRoleParameterSet = "NewDerivedRoleParameterSet";

		private const string UnScopedTopLevelRoleParameterSet = "UnScopedTopLevelRoleParameterSet";
	}
}
