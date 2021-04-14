using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("New", "RoleGroup", SupportsShouldProcess = true, DefaultParameterSetName = "default")]
	public sealed class NewRoleGroup : NewGeneralRecipientObjectTask<ADGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string managedBy = RoleGroupCommon.NamesFromObjects(this.managedByRecipients);
				string text = RoleGroupCommon.NamesFromObjects(this.roles);
				string recipientWriteScope = string.Empty;
				string configWriteScope = string.Empty;
				if (this.ou != null)
				{
					recipientWriteScope = this.ou.Name;
				}
				else if (this.customRecipientScope != null)
				{
					recipientWriteScope = this.customRecipientScope.Name;
				}
				if (this.customConfigScope != null)
				{
					configWriteScope = this.customConfigScope.Name;
				}
				return Strings.ConfirmationMessageNewRoleGroup(this.Name, text, managedBy, recipientWriteScope, configWriteScope);
			}
		}

		public new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return null;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0)]
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

		[ValidateNotNullOrEmpty]
		[Parameter]
		public string SamAccountName
		{
			get
			{
				return this.DataObject.SamAccountName;
			}
			set
			{
				this.DataObject.SamAccountName = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<SecurityPrincipalIdParameter> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<SecurityPrincipalIdParameter>)base.Fields[DistributionGroupSchema.ManagedBy];
			}
			set
			{
				base.Fields[DistributionGroupSchema.ManagedBy] = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<SecurityPrincipalIdParameter> Members
		{
			get
			{
				return (MultiValuedProperty<SecurityPrincipalIdParameter>)base.Fields[RoleGroupParameters.ParameterMembers];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterMembers] = value;
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

		[ValidateNotNullOrEmpty]
		[Parameter]
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

		[Parameter]
		[ValidateNotNullOrEmpty]
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

		[Parameter(Mandatory = true, ParameterSetName = "linkedpartnergroup")]
		[ValidateNotNullOrEmpty]
		public string LinkedPartnerGroupId
		{
			get
			{
				return (string)base.Fields[RoleGroupParameters.ParameterLinkedPartnerGroupId];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedPartnerGroupId] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "linkedpartnergroup")]
		[ValidateNotNullOrEmpty]
		public string LinkedPartnerOrganizationId
		{
			get
			{
				return (string)base.Fields[RoleGroupParameters.PartnerLinkedPartnerOrganizationId];
			}
			set
			{
				base.Fields[RoleGroupParameters.PartnerLinkedPartnerOrganizationId] = value;
			}
		}

		[Parameter]
		public SwitchParameter PartnerManaged
		{
			get
			{
				return (SwitchParameter)(base.Fields[RoleGroupParameters.PartnerLinkedPartnerManaged] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields[RoleGroupParameters.PartnerLinkedPartnerManaged] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "crossforest")]
		public UniversalSecurityGroupIdParameter LinkedForeignGroup
		{
			get
			{
				return (UniversalSecurityGroupIdParameter)base.Fields[RoleGroupParameters.ParameterLinkedForeignGroup];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedForeignGroup] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "crossforest")]
		[ValidateNotNull]
		public string LinkedDomainController
		{
			get
			{
				return (string)base.Fields[RoleGroupParameters.ParameterLinkedDomainController];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedDomainController] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "crossforest")]
		public PSCredential LinkedCredential
		{
			get
			{
				return (PSCredential)base.Fields[RoleGroupParameters.ParameterLinkedCredential];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedCredential] = value;
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

		[Parameter(Mandatory = false)]
		public string ValidationOrganization
		{
			get
			{
				return (string)base.Fields["ValidationOrganization"];
			}
			set
			{
				base.Fields["ValidationOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid WellKnownObjectGuid
		{
			get
			{
				if (base.Fields["WellKnownObjectGuid"] == null)
				{
					return Guid.Empty;
				}
				return (Guid)base.Fields["WellKnownObjectGuid"];
			}
			set
			{
				base.Fields["WellKnownObjectGuid"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			bool flag = false;
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.ValidationOrganization != null && !string.Equals(this.ValidationOrganization, base.CurrentOrganizationId.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				base.ThrowTerminatingError(new ValidationOrgCurrentOrgNotMatchException(this.ValidationOrganization, base.CurrentOrganizationId.ToString()), ExchangeErrorCategory.Client, null);
			}
			if ("crossforest" == base.ParameterSetName)
			{
				try
				{
					NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
					this.linkedGroupSid = MailboxTaskHelper.GetSidFromAnotherForest<ADGroup>(this.LinkedForeignGroup, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADGroup>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new MailboxTaskHelper.OneStringErrorDelegate(Strings.ErrorLinkedGroupInTheCurrentForest), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotFoundOnGlobalCatalog), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotFoundOnDomainController), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotUniqueOnGlobalCatalog), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotUniqueOnDomainController), new MailboxTaskHelper.OneStringErrorDelegate(Strings.ErrorVerifyLinkedGroupForest));
				}
				catch (PSArgumentException exception)
				{
					base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.LinkedCredential);
				}
			}
			if (!base.Fields.IsModified(ADGroupSchema.ManagedBy) && base.CurrentOrganizationId.Equals(base.ExecutingUserOrganizationId))
			{
				List<SecurityPrincipalIdParameter> list = new List<SecurityPrincipalIdParameter>(2);
				flag = true;
				bool useGlobalCatalog = base.TenantGlobalCatalogSession.UseGlobalCatalog;
				bool useConfigNC = base.TenantGlobalCatalogSession.UseConfigNC;
				bool skipRangedAttributes = base.TenantGlobalCatalogSession.SkipRangedAttributes;
				try
				{
					base.TenantGlobalCatalogSession.UseGlobalCatalog = true;
					base.TenantGlobalCatalogSession.UseConfigNC = false;
					base.TenantGlobalCatalogSession.SkipRangedAttributes = true;
					ADGroup adgroup = base.TenantGlobalCatalogSession.ResolveWellKnownGuid<ADGroup>(RoleGroup.OrganizationManagement_InitInfo.WellKnownGuid, OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId) ? this.ConfigurationSession.ConfigurationNamingContext : base.TenantGlobalCatalogSession.SessionSettings.CurrentOrganizationId.ConfigurationUnit);
					if (adgroup == null)
					{
						base.ThrowTerminatingError(new ManagementObjectNotFoundException(DirectoryStrings.ExceptionADTopologyCannotFindWellKnownExchangeGroup), (ErrorCategory)1001, RoleGroup.OrganizationManagement_InitInfo.WellKnownGuid);
					}
					list.Add(new SecurityPrincipalIdParameter(adgroup.DistinguishedName));
				}
				finally
				{
					base.TenantGlobalCatalogSession.UseGlobalCatalog = useGlobalCatalog;
					base.TenantGlobalCatalogSession.UseConfigNC = useConfigNC;
					base.TenantGlobalCatalogSession.SkipRangedAttributes = skipRangedAttributes;
				}
				ADObjectId adObjectId;
				if (base.TryGetExecutingUserId(out adObjectId))
				{
					list.Add(new SecurityPrincipalIdParameter(adObjectId));
				}
				this.ManagedBy = new MultiValuedProperty<SecurityPrincipalIdParameter>(list);
			}
			if (this.ManagedBy == null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorManagedByCannotBeEmpty), (ErrorCategory)1000, null);
			}
			if (!flag || !DatacenterRegistry.IsForefrontForOffice())
			{
				MailboxTaskHelper.CheckAndResolveManagedBy<ADGroup>(this, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), ExchangeErrorCategory.Client, this.ManagedBy.ToArray(), out this.managedByRecipients);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			base.CheckExclusiveParameters(NewRoleGroup.mutuallyExclusiveParameters);
			base.CheckExclusiveParameters(new object[]
			{
				RoleGroupParameters.ParameterMembers,
				RoleGroupParameters.ParameterLinkedForeignGroup
			});
			base.CheckExclusiveParameters(new object[]
			{
				RoleGroupParameters.ParameterMembers,
				RoleGroupParameters.ParameterLinkedPartnerGroupId
			});
			TaskLogger.LogExit();
		}

		protected override void PrepareRecipientObject(ADGroup group)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(group);
			group.GroupType = (GroupTypeFlags.Universal | GroupTypeFlags.SecurityEnabled);
			group[ADRecipientSchema.Description] = new MultiValuedProperty<string>(this.Description);
			if (string.Equals(this.Description, CoreStrings.MsoManagedTenantAdminGroupDescription, StringComparison.Ordinal))
			{
				group[ADGroupSchema.RoleGroupTypeId] = 23;
			}
			else if (string.Equals(this.Description, CoreStrings.MsoMailTenantAdminGroupDescription, StringComparison.Ordinal))
			{
				group[ADGroupSchema.RoleGroupTypeId] = 24;
			}
			else if (string.Equals(this.Description, CoreStrings.MsoManagedTenantHelpdeskGroupDescription, StringComparison.Ordinal))
			{
				group[ADGroupSchema.RoleGroupTypeId] = 25;
			}
			if (base.CurrentOrganizationId == OrganizationId.ForestWideOrgId)
			{
				ADObjectId adobjectId = RoleGroupCommon.RoleGroupContainerId(base.TenantGlobalCatalogSession, this.ConfigurationSession);
				group.SetId(adobjectId.GetChildId(this.Name));
			}
			MailboxTaskHelper.StampOnManagedBy(this.DataObject, this.managedByRecipients, new Task.ErrorLoggerDelegate(base.WriteError));
			this.DataObject.RecipientTypeDetails = RecipientTypeDetails.RoleGroup;
			MailboxTaskHelper.ValidateGroupManagedBy(base.TenantGlobalCatalogSession, group, this.managedByRecipients, RoleGroupCommon.OwnerRecipientTypeDetails, true, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError));
			if (string.IsNullOrEmpty(group.SamAccountName))
			{
				IRecipientSession[] recipientSessions = new IRecipientSession[]
				{
					base.RootOrgGlobalCatalogSession
				};
				if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled && base.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
				{
					recipientSessions = new IRecipientSession[]
					{
						base.RootOrgGlobalCatalogSession,
						base.PartitionOrRootOrgGlobalCatalogSession
					};
				}
				group.SamAccountName = RecipientTaskHelper.GenerateUniqueSamAccountName(recipientSessions, group.Id.DomainId, group.Name, true, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), false);
			}
			else
			{
				RecipientTaskHelper.IsSamAccountNameUnique(group, group.SamAccountName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			if ("crossforest" == base.ParameterSetName)
			{
				group.ForeignGroupSid = this.linkedGroupSid;
			}
			if ("linkedpartnergroup" == base.ParameterSetName)
			{
				group.LinkedPartnerGroupId = this.LinkedPartnerGroupId;
				group.LinkedPartnerOrganizationId = this.LinkedPartnerOrganizationId;
			}
			if (this.PartnerManaged.IsPresent)
			{
				group.RawCapabilities.Add(Capability.Partner_Managed);
			}
			if (base.Fields.IsChanged(RoleGroupParameters.ParameterMembers) && this.Members != null)
			{
				foreach (SecurityPrincipalIdParameter member in this.Members)
				{
					MailboxTaskHelper.ValidateAndAddMember(base.TenantGlobalCatalogSession, group, member, false, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
				}
			}
			MailboxTaskHelper.ValidateAddedMembers(base.TenantGlobalCatalogSession, group, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADGroup result = (ADGroup)base.PrepareDataObject();
			if (!this.PartnerManaged.IsPresent)
			{
				SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			RoleAssigneeType roleAssigneeType = RoleAssigneeType.RoleGroup;
			if ("crossforest" == base.ParameterSetName)
			{
				roleAssigneeType = RoleAssigneeType.LinkedRoleGroup;
			}
			if (base.Fields.IsChanged("Roles") && this.Roles != null)
			{
				this.roles = new MultiValuedProperty<ExchangeRole>();
				this.roleAssignments = new List<ExchangeRoleAssignment>();
				foreach (RoleIdParameter roleIdParameter in this.Roles)
				{
					ExchangeRole item = (ExchangeRole)base.GetDataObject<ExchangeRole>(roleIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRoleNotFound(roleIdParameter.ToString())), new LocalizedString?(Strings.ErrorRoleNotUnique(roleIdParameter.ToString())));
					this.roles.Add(item);
				}
				this.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
				foreach (ExchangeRole role in this.roles)
				{
					bool flag = false;
					ExchangeRoleAssignment exchangeRoleAssignment = new ExchangeRoleAssignment();
					RoleHelper.PrepareNewRoleAssignmentWithUniqueNameAndDefaultScopes(null, exchangeRoleAssignment, role, this.DataObject.Id, this.DataObject.OrganizationId, roleAssigneeType, RoleAssignmentDelegationType.Regular, this.ConfigurationSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
					RoleHelper.AnalyzeAndStampCustomizedWriteScopes(this, exchangeRoleAssignment, role, this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ManagementScope>), ref flag, ref this.ou, ref this.customRecipientScope, ref this.customConfigScope);
					if (!flag && base.ExchangeRunspaceConfig != null)
					{
						RoleHelper.HierarchicalCheckForRoleAssignmentCreation(this, exchangeRoleAssignment, this.customRecipientScope, this.customConfigScope, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
					this.roleAssignments.Add(exchangeRoleAssignment);
				}
			}
			TaskLogger.LogExit();
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			IConfigurationSession configurationSession = null;
			base.InternalProcessRecord();
			if (this.WellKnownObjectGuid != Guid.Empty || this.roleAssignments != null)
			{
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 676, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleGroup\\NewRoleGroup.cs");
				configurationSession.LinkResolutionServer = this.DataObject.OriginatingServer;
			}
			if (this.WellKnownObjectGuid != Guid.Empty)
			{
				try
				{
					RoleGroupCommon.StampWellKnownObjectGuid(configurationSession, this.DataObject.OrganizationId, this.DataObject.DistinguishedName, this.WellKnownObjectGuid);
				}
				catch (Exception)
				{
					this.DataObject.ExternalDirectoryObjectId = null;
					base.DataSession.Save(this.DataObject);
					base.DataSession.Delete(this.DataObject);
					throw;
				}
			}
			if (this.roleAssignments != null)
			{
				List<ExchangeRoleAssignment> list = new List<ExchangeRoleAssignment>();
				string id = string.Empty;
				try
				{
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in this.roleAssignments)
					{
						exchangeRoleAssignment.User = this.DataObject.Id;
						id = exchangeRoleAssignment.Id.Name;
						configurationSession.Save(exchangeRoleAssignment);
						list.Add(exchangeRoleAssignment);
					}
				}
				catch (Exception)
				{
					this.WriteWarning(Strings.WarningCouldNotCreateRoleAssignment(id, this.Name));
					foreach (ExchangeRoleAssignment exchangeRoleAssignment2 in list)
					{
						base.WriteVerbose(Strings.VerboseRemovingRoleAssignment(exchangeRoleAssignment2.Id.ToString()));
						configurationSession.Delete(exchangeRoleAssignment2);
						base.WriteVerbose(Strings.VerboseRemovedRoleAssignment(exchangeRoleAssignment2.Id.ToString()));
					}
					base.WriteVerbose(Strings.VerboseRemovingRoleGroup(this.DataObject.Id.ToString()));
					base.DataSession.Delete(this.DataObject);
					throw;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			base.WriteVerbose(TaskVerboseStringHelper.GetReadObjectVerboseString(this.DataObject.Identity, base.DataSession, typeof(RoleGroup)));
			ADGroup adgroup = null;
			try
			{
				adgroup = (ADGroup)base.DataSession.Read<ADGroup>(this.DataObject.Identity);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			if (adgroup == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.ResolveIdentityString(this.DataObject.Identity), typeof(RoleGroup).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, this.DataObject.Identity);
			}
			Result<ExchangeRoleAssignment>[] array = null;
			if (this.roleAssignments != null)
			{
				array = new Result<ExchangeRoleAssignment>[this.roleAssignments.Count];
				for (int i = 0; i < this.roleAssignments.Count; i++)
				{
					array[i] = new Result<ExchangeRoleAssignment>(this.roleAssignments[i], null);
				}
			}
			if (null != adgroup.ForeignGroupSid)
			{
				adgroup.LinkedGroup = SecurityPrincipalIdParameter.GetFriendlyUserName(adgroup.ForeignGroupSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				adgroup.ResetChangeTracking();
			}
			RoleGroup result = new RoleGroup(adgroup, array);
			base.WriteResult(result);
			TaskLogger.LogExit();
		}

		private MultiValuedProperty<ADRecipient> managedByRecipients;

		private MultiValuedProperty<ExchangeRole> roles;

		private List<ExchangeRoleAssignment> roleAssignments;

		private ExchangeOrganizationalUnit ou;

		private ManagementScope customRecipientScope;

		private ManagementScope customConfigScope;

		private SecurityIdentifier linkedGroupSid;

		private static string[] mutuallyExclusiveParameters = new string[]
		{
			RbacCommonParameters.ParameterCustomRecipientWriteScope,
			RbacCommonParameters.ParameterRecipientOrganizationalUnitScope
		};
	}
}
