using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Get", "ManagementRoleAssignment", DefaultParameterSetName = "Identity")]
	public sealed class GetManagementRoleAssignment : GetMultitenancySystemConfigurationObjectTask<RoleAssignmentIdParameter, ExchangeRoleAssignment>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		public static ADScope GetRecipientWriteADScope(ExchangeRoleAssignmentPresentation roleAssignment, ADRawEntry assignee, ManagementScope managementScope)
		{
			Dictionary<ADObjectId, ManagementScope> dictionary = new Dictionary<ADObjectId, ManagementScope>();
			if (managementScope != null)
			{
				dictionary.Add(managementScope.Id, managementScope);
			}
			if (roleAssignment.RecipientWriteScope == RecipientWriteScopeType.MyGAL)
			{
				throw new ArgumentException("Role assignment with MyGAL scope is not supported");
			}
			ExchangeRoleAssignment exchangeRoleAssignment = (ExchangeRoleAssignment)roleAssignment.DataObject;
			RbacScope recipientWriteRbacScope = ExchangeRoleAssignment.GetRecipientWriteRbacScope(roleAssignment.RecipientWriteScope, roleAssignment.CustomRecipientWriteScope, dictionary, null, exchangeRoleAssignment.IsFromEndUserRole);
			if (recipientWriteRbacScope != null)
			{
				recipientWriteRbacScope.PopulateRootAndFilter((OrganizationId)assignee[ADObjectSchema.OrganizationId], assignee);
			}
			return recipientWriteRbacScope;
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.sharedConfiguration != null)
				{
					return this.sharedConfiguration.SharedConfigurationCU.Id;
				}
				if (this.Organization == null && GetManagementRoleAssignment.IsDatacenter && this.assignee != null)
				{
					return this.assignee.OrganizationId.ConfigurationUnit;
				}
				if (this.Organization == null && GetManagementRoleAssignment.IsDatacenter && this.anyRole != null)
				{
					return this.anyRole.OrganizationId.ConfigurationUnit;
				}
				return base.RootId;
			}
		}

		[Parameter(ValueFromPipeline = true, ParameterSetName = "RoleAssignee")]
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

		[Parameter(ParameterSetName = "RoleAssignee")]
		public RoleAssigneeIdParameter RoleAssignee
		{
			get
			{
				return (RoleAssigneeIdParameter)base.Fields[RbacCommonParameters.ParameterRoleAssignee];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterRoleAssignee] = value;
			}
		}

		[Parameter(ParameterSetName = "RoleAssignee")]
		public AssignmentMethod[] AssignmentMethod
		{
			get
			{
				return (AssignmentMethod[])base.Fields[RbacCommonParameters.ParameterAssignmentMethod];
			}
			set
			{
				base.VerifyValues<AssignmentMethod>(GetManagementRoleAssignment.AllowedAssignmentMethods, value);
				base.Fields[RbacCommonParameters.ParameterAssignmentMethod] = value;
			}
		}

		[Parameter]
		public RoleAssigneeType RoleAssigneeType
		{
			get
			{
				return (RoleAssigneeType)base.Fields[RbacCommonParameters.ParameterRoleAssigneeType];
			}
			set
			{
				base.VerifyValues<RoleAssigneeType>((RoleAssigneeType[])Enum.GetValues(typeof(RoleAssigneeType)), value);
				base.Fields[RbacCommonParameters.ParameterRoleAssigneeType] = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields[RbacCommonParameters.ParameterEnabled];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterEnabled] = value;
			}
		}

		[Parameter]
		public bool Delegating
		{
			get
			{
				return (bool)base.Fields[RbacCommonParameters.ParameterDelegating];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterDelegating] = value;
			}
		}

		[Parameter]
		public bool Exclusive
		{
			get
			{
				return (bool)base.Fields["Exclusive"];
			}
			set
			{
				base.Fields["Exclusive"] = value;
			}
		}

		[Parameter]
		public RecipientWriteScopeType RecipientWriteScope
		{
			get
			{
				return (RecipientWriteScopeType)(base.Fields[RbacCommonParameters.ParameterRecipientWriteScope] ?? -1);
			}
			set
			{
				base.VerifyValues<RecipientWriteScopeType>((RecipientWriteScopeType[])Enum.GetValues(typeof(RecipientWriteScopeType)), value);
				base.Fields[RbacCommonParameters.ParameterRecipientWriteScope] = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
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

		[Parameter]
		public ConfigWriteScopeType ConfigWriteScope
		{
			get
			{
				return (ConfigWriteScopeType)(base.Fields[RbacCommonParameters.ParameterConfigWriteScope] ?? -1);
			}
			set
			{
				base.VerifyValues<ConfigWriteScopeType>((ConfigWriteScopeType[])Enum.GetValues(typeof(ConfigWriteScopeType)), value);
				base.Fields[RbacCommonParameters.ParameterConfigWriteScope] = value;
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

		[ValidateNotNullOrEmpty]
		[Parameter]
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

		[Parameter]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter GetEffectiveUsers
		{
			get
			{
				return (SwitchParameter)(base.Fields["GetEffectiveUsers"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["GetEffectiveUsers"] = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public GeneralRecipientIdParameter WritableRecipient
		{
			get
			{
				return (GeneralRecipientIdParameter)base.Fields["WritableRecipient"];
			}
			set
			{
				base.Fields["WritableRecipient"] = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter WritableServer
		{
			get
			{
				return (ServerIdParameter)base.Fields["WritableServer"];
			}
			set
			{
				base.Fields["WritableServer"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public DatabaseIdParameter WritableDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["ParameterWritableDatabase"];
			}
			set
			{
				base.Fields["ParameterWritableDatabase"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.sharedConfiguration != null)
			{
				return this.sharedSystemConfigSession;
			}
			IConfigurationSession configurationSession;
			if (this.RoleAssignee != null || this.Role != null)
			{
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 408, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleAssignment\\GetManagementRoleAssignment.cs");
			}
			else
			{
				configurationSession = (IConfigurationSession)base.CreateSession();
			}
			configurationSession.SessionSettings.IsSharedConfigChecked = true;
			return configurationSession;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.ReScopeSharedConfigAndSharedSessionIfNecessary(base.CurrentOrganizationId);
			if (base.Fields.IsModified(RbacCommonParameters.ParameterAssignmentMethod) && !base.Fields.IsModified(RbacCommonParameters.ParameterRoleAssignee))
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorAssignmentMethodWithoutRoleAssignee), ErrorCategory.InvalidArgument, null);
			}
			RbacRoleAssignmentCommon.CheckMutuallyExclusiveParameters(this);
			List<QueryFilter> list = new List<QueryFilter>();
			if (this.RoleAssignee != null)
			{
				AssignmentMethod assignmentMethod = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.None;
				if (base.Fields.IsModified(RbacCommonParameters.ParameterAssignmentMethod))
				{
					foreach (AssignmentMethod assignmentMethod3 in this.AssignmentMethod)
					{
						assignmentMethod |= assignmentMethod3;
					}
					this.assigneeIds = new List<ADObjectId>();
				}
				else
				{
					assignmentMethod = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.All;
				}
				if (this.sharedConfiguration != null)
				{
					this.assignee = RoleAssigneeIdParameter.GetRawRoleAssignee(this.RoleAssignee, this.sharedSystemConfigSession, base.TenantGlobalCatalogSession);
				}
				else
				{
					this.assignee = RoleAssigneeIdParameter.GetRawRoleAssignee(this.RoleAssignee, this.ConfigurationSession, base.TenantGlobalCatalogSession);
				}
				if ((base.CurrentOrganizationId == null || base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId)) && this.assignee != null && this.assignee.OrganizationId != null && !this.assignee.OrganizationId.Equals(base.CurrentOrganizationId))
				{
					base.CurrentOrganizationId = this.assignee.OrganizationId;
					this.ReScopeSharedConfigAndSharedSessionIfNecessary(this.assignee.OrganizationId);
				}
				if (!(this.assignee is RoleAssignmentPolicy))
				{
					base.WriteVerbose(Strings.VerboseResolvingSecurityPrinciplals);
					if (((ADRecipient)this.assignee).RecipientTypeDetails != RecipientTypeDetails.MailboxPlan)
					{
						RoleHelper.ValidateRoleAssignmentUser((ADRecipient)this.assignee, new Task.TaskErrorLoggingDelegate(base.ThrowTerminatingError), true);
					}
					else
					{
						assignmentMethod &= ~Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.MailboxPlan;
					}
					ADObjectId roleAssignmentPolicy = ((ADRecipient)this.assignee).RoleAssignmentPolicy;
					if (this.AssignmentMethod != null && this.AssignmentMethod.Length == 1 && this.AssignmentMethod[0] == Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleAssignmentPolicy && roleAssignmentPolicy == null)
					{
						base.ThrowTerminatingError(new ArgumentException(Strings.ErrorUserNotHaveRoleAssignmentPolicy(this.RoleAssignee.ToString())), ErrorCategory.InvalidArgument, null);
					}
					List<string> tokenSids = base.TenantGlobalCatalogSession.GetTokenSids((ADRecipient)this.assignee, assignmentMethod);
					if (tokenSids == null || tokenSids.Count == 0)
					{
						if (this.AssignmentMethod != null && this.AssignmentMethod.Length == 1)
						{
							if (this.AssignmentMethod[0] == Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleGroup)
							{
								base.ThrowTerminatingError(new ArgumentException(Strings.ErrorUserNotInRoleGroups(this.RoleAssignee.ToString())), ErrorCategory.InvalidArgument, null);
							}
							else if (this.AssignmentMethod[0] == Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.SecurityGroup)
							{
								base.ThrowTerminatingError(new ArgumentException(Strings.ErrorUserNotInSecurityGroups(this.RoleAssignee.ToString())), ErrorCategory.InvalidArgument, null);
							}
							else if (this.AssignmentMethod[0] == Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.MailboxPlan)
							{
								base.ThrowTerminatingError(new ArgumentException(Strings.ErrorUserNotHaveMailboxPlan(this.RoleAssignee.ToString())), ErrorCategory.InvalidArgument, null);
							}
						}
					}
					else
					{
						ADObjectId[] array = base.TenantGlobalCatalogSession.ResolveSidsToADObjectIds(tokenSids.ToArray());
						if (this.sharedConfiguration != null)
						{
							ADObjectId[] sharedRoleGroupIds = this.sharedConfiguration.GetSharedRoleGroupIds(array);
							this.assigneeIds = new List<ADObjectId>();
							if (!sharedRoleGroupIds.IsNullOrEmpty<ADObjectId>())
							{
								this.assigneeIds.AddRange(sharedRoleGroupIds);
							}
						}
						else
						{
							this.assigneeIds = new List<ADObjectId>(array);
						}
					}
					if ((this.sharedConfiguration != null || roleAssignmentPolicy != null) && (assignmentMethod & Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleAssignmentPolicy) != Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.None)
					{
						if (this.assigneeIds == null)
						{
							this.assigneeIds = new List<ADObjectId>(1);
						}
						if (this.sharedConfiguration == null && roleAssignmentPolicy != null)
						{
							this.assigneeIds.Add(roleAssignmentPolicy);
						}
						else
						{
							this.assigneeIds.Add(this.sharedConfiguration.GetSharedRoleAssignmentPolicy());
						}
					}
				}
				else if ((assignmentMethod & Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.Direct) != Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.None)
				{
					if (this.assigneeIds == null)
					{
						this.assigneeIds = new List<ADObjectId>(1);
					}
					if (this.sharedConfiguration == null)
					{
						this.assigneeIds.Add(this.assignee.Id);
					}
					else
					{
						this.assigneeIds.Add(this.sharedConfiguration.GetSharedRoleAssignmentPolicy());
					}
				}
			}
			if (this.AssignmentMethod != null && this.AssignmentMethod.Contains(Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleGroup) && !this.AssignmentMethod.Contains(Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.SecurityGroup))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssigneeType, RoleAssigneeType.RoleGroup));
			}
			if (this.CustomConfigWriteScope != null)
			{
				ManagementScope managementScope = (ManagementScope)base.GetDataObject<ManagementScope>(this.CustomConfigWriteScope, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorScopeNotFound(this.CustomConfigWriteScope.ToString())), new LocalizedString?(Strings.ErrorScopeNotUnique(this.CustomConfigWriteScope.ToString())));
				if (managementScope.Exclusive)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorScopeExclusive(managementScope.Id.ToString(), RbacCommonParameters.ParameterCustomConfigWriteScope)), ErrorCategory.InvalidArgument, null);
				}
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.CustomConfigScope));
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomConfigWriteScope, managementScope.Id));
			}
			if (this.ExclusiveConfigWriteScope != null)
			{
				ManagementScope managementScope2 = (ManagementScope)base.GetDataObject<ManagementScope>(this.ExclusiveConfigWriteScope, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorScopeNotFound(this.ExclusiveConfigWriteScope.ToString())), new LocalizedString?(Strings.ErrorScopeNotUnique(this.ExclusiveConfigWriteScope.ToString())));
				if (!managementScope2.Exclusive)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorScopeNotExclusive(managementScope2.Id.ToString(), "ExclusiveConfigWriteScope")), ErrorCategory.InvalidArgument, null);
				}
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.ExclusiveConfigScope));
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomConfigWriteScope, managementScope2.Id));
			}
			if (this.CustomRecipientWriteScope != null)
			{
				base.WriteVerbose(Strings.VerboseResolvingCustomRecipientWriteScope);
				ManagementScope managementScope3 = (ManagementScope)base.GetDataObject<ManagementScope>(this.CustomRecipientWriteScope, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorScopeNotFound(this.CustomRecipientWriteScope.ToString())), new LocalizedString?(Strings.ErrorScopeNotUnique(this.CustomRecipientWriteScope.ToString())));
				if (managementScope3.Exclusive)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorScopeExclusive(managementScope3.Id.ToString(), RbacCommonParameters.ParameterCustomRecipientWriteScope)), ErrorCategory.InvalidArgument, null);
				}
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RecipientWriteScope, RecipientWriteScopeType.CustomRecipientScope));
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomRecipientWriteScope, managementScope3.Id));
			}
			if (this.ExclusiveRecipientWriteScope != null)
			{
				base.WriteVerbose(Strings.VerboseResolvingExclusiveRecipientWriteScope);
				ManagementScope managementScope4 = (ManagementScope)base.GetDataObject<ManagementScope>(this.ExclusiveRecipientWriteScope, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorScopeNotFound(this.ExclusiveRecipientWriteScope.ToString())), new LocalizedString?(Strings.ErrorScopeNotUnique(this.ExclusiveRecipientWriteScope.ToString())));
				if (!managementScope4.Exclusive)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorScopeNotExclusive(managementScope4.Id.ToString(), "ExclusiveRecipientWriteScope")), ErrorCategory.InvalidArgument, null);
				}
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RecipientWriteScope, RecipientWriteScopeType.ExclusiveRecipientScope));
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomRecipientWriteScope, managementScope4.Id));
			}
			if (this.RecipientOrganizationalUnitScope != null)
			{
				base.WriteVerbose(Strings.VerboseResolvingRecipientOrganizationalUnitScope);
				bool useConfigNC = this.ConfigurationSession.UseConfigNC;
				bool useGlobalCatalog = this.ConfigurationSession.UseGlobalCatalog;
				try
				{
					this.ConfigurationSession.UseConfigNC = false;
					this.ConfigurationSession.UseGlobalCatalog = true;
					ExchangeOrganizationalUnit exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)base.GetDataObject<ExchangeOrganizationalUnit>(this.RecipientOrganizationalUnitScope, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.RecipientOrganizationalUnitScope.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.RecipientOrganizationalUnitScope.ToString())));
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RecipientWriteScope, RecipientWriteScopeType.OU));
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomRecipientWriteScope, exchangeOrganizationalUnit.Id));
				}
				finally
				{
					this.ConfigurationSession.UseConfigNC = useConfigNC;
					this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog;
				}
			}
			this.InitializeWritableReportingObjectIfNecessary();
			if (base.Fields.IsModified(RbacCommonParameters.ParameterDelegating))
			{
				if (!this.Delegating)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, RoleAssignmentDelegationType.Regular));
				}
				else
				{
					list.Add(new OrFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, RoleAssignmentDelegationType.Delegating),
						new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, RoleAssignmentDelegationType.DelegatingOrgWide)
					}));
				}
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterRecipientWriteScope))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RecipientWriteScope, this.RecipientWriteScope));
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterConfigWriteScope))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, this.ConfigWriteScope));
			}
			if (base.Fields.IsModified("Exclusive"))
			{
				if (this.Exclusive)
				{
					list.Add(new OrFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RecipientWriteScope, RecipientWriteScopeType.ExclusiveRecipientScope),
						new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.ExclusiveConfigScope)
					}));
				}
				else
				{
					list.Add(new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RecipientWriteScope, RecipientWriteScopeType.ExclusiveRecipientScope)));
					list.Add(new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.ExclusiveConfigScope)));
				}
			}
			else if (base.Fields.IsModified(RbacCommonParameters.ParameterCustomConfigWriteScope))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.CustomConfigScope));
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterRoleAssigneeType))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssigneeType, this.RoleAssigneeType));
			}
			if (1 < list.Count)
			{
				this.internalFilterForNonPipelineParameter = new AndFilter(list.ToArray());
			}
			else if (1 == list.Count)
			{
				this.internalFilterForNonPipelineParameter = list[0];
			}
			else
			{
				this.internalFilterForNonPipelineParameter = null;
			}
			TaskLogger.LogExit();
		}

		protected override IEnumerable<ExchangeRoleAssignment> GetPagedData()
		{
			List<ADObjectId> sharedTenantAssigneeIds = this.assigneeIds;
			if (sharedTenantAssigneeIds == null && this.sharedConfiguration != null)
			{
				sharedTenantAssigneeIds = this.GetSharedTenantAssigneeIds();
			}
			IEnumerable<ExchangeRoleAssignment> enumerable;
			if (sharedTenantAssigneeIds == null)
			{
				enumerable = base.GetPagedData();
			}
			else
			{
				if (sharedTenantAssigneeIds.Count == 0)
				{
					return null;
				}
				IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
				if (this.sharedConfiguration == null)
				{
					configurationSession = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(configurationSession, this.assignee.OrganizationId, true);
				}
				List<ExchangeRoleAssignment> list = new List<ExchangeRoleAssignment>();
				Result<ExchangeRoleAssignment>[] array = configurationSession.FindRoleAssignmentsByUserIds(sharedTenantAssigneeIds.ToArray(), this.InternalFilter);
				foreach (Result<ExchangeRoleAssignment> result in array)
				{
					list.Add(result.Data);
				}
				enumerable = list;
			}
			if (GetManagementRoleAssignment.WritableObjectType.NotApplicable != this.writableObjectType)
			{
				this.InitializeManagementReportingIfNecessary();
			}
			switch (this.writableObjectType)
			{
			case GetManagementRoleAssignment.WritableObjectType.Recipient:
				enumerable = this.managementReporting.FindRoleAssignmentsWithWritableRecipient(this.writableObject, enumerable);
				break;
			case GetManagementRoleAssignment.WritableObjectType.Server:
				enumerable = this.managementReporting.FindRoleAssignmentsWithWritableServer((Server)this.writableObject, enumerable);
				break;
			case GetManagementRoleAssignment.WritableObjectType.Database:
				enumerable = this.managementReporting.FindRoleAssignmentsWithWritableDatabase((Database)this.writableObject, enumerable);
				break;
			}
			return enumerable;
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			this.anyRole = null;
			if (this.Role != null)
			{
				ADObjectId rootID = null;
				IConfigurationSession configurationSession;
				if (this.sharedSystemConfigSession != null)
				{
					configurationSession = this.sharedSystemConfigSession;
					rootID = SharedConfiguration.GetSharedConfiguration(base.CurrentOrganizationId).SharedConfigurationCU.ConfigurationUnit;
				}
				else
				{
					configurationSession = this.ConfigurationSession;
					if (GetManagementRoleAssignment.IsDatacenter)
					{
						if (this.assignee != null)
						{
							rootID = this.assignee.OrganizationId.ConfigurationUnit;
						}
						else if ((base.CurrentOrganizationId == null || base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId)) && this.Role.InternalADObjectId == null)
						{
							throw new ArgumentException("In datacenter you can not search for role assignments based on a role name in the root organization. Please search for the role and use the identity in this cmdlet.");
						}
					}
				}
				IEnumerable<ExchangeRole> dataObjects = base.GetDataObjects<ExchangeRole>(this.Role, configurationSession, rootID);
				List<QueryFilter> list = new List<QueryFilter>();
				foreach (ExchangeRole exchangeRole in dataObjects)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, exchangeRole.Id));
					this.anyRole = exchangeRole;
				}
				if (list.Count == 0)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorRoleNotFound(this.Role.ToString())), (ErrorCategory)1000, null);
				}
				this.internalFilterForPipelineParameter = QueryFilter.OrTogether(list.ToArray());
			}
			else
			{
				this.internalFilterForPipelineParameter = null;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null)
			{
				this.Identity.InternalFilter = this.InternalFilter;
			}
			base.InternalValidate();
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				List<QueryFilter> list = new List<QueryFilter>();
				if (base.InternalFilter != null)
				{
					list.Add(base.InternalFilter);
				}
				if (this.internalFilterForNonPipelineParameter != null)
				{
					list.Add(this.internalFilterForNonPipelineParameter);
				}
				if (this.internalFilterForPipelineParameter != null)
				{
					list.Add(this.internalFilterForPipelineParameter);
				}
				if (1 < list.Count)
				{
					return new AndFilter(list.ToArray());
				}
				if (1 == list.Count)
				{
					return list[0];
				}
				return null;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ExchangeRoleAssignment exchangeRoleAssignment = (ExchangeRoleAssignment)dataObject;
			bool flag = false;
			if (base.Fields.IsModified(RbacCommonParameters.ParameterEnabled))
			{
				flag |= (this.Enabled != exchangeRoleAssignment.Enabled);
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterDelegating))
			{
				if (this.Delegating)
				{
					flag |= ((exchangeRoleAssignment.RoleAssignmentDelegationType & RoleAssignmentDelegationType.Delegating) == (RoleAssignmentDelegationType)0);
				}
				else
				{
					flag |= ((exchangeRoleAssignment.RoleAssignmentDelegationType & RoleAssignmentDelegationType.Regular) == (RoleAssignmentDelegationType)0);
				}
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterRecipientWriteScope))
			{
				flag |= (this.RecipientWriteScope != exchangeRoleAssignment.RecipientWriteScope);
			}
			if (base.Fields.IsModified(RbacCommonParameters.ParameterConfigWriteScope))
			{
				flag |= (this.ConfigWriteScope != exchangeRoleAssignment.ConfigWriteScope);
			}
			if (flag)
			{
				base.WriteVerbose(Strings.VerboseSkipObject(exchangeRoleAssignment.DistinguishedName));
			}
			else
			{
				ExchangeRoleAssignmentPresentation exchangeRoleAssignmentPresentation = (ExchangeRoleAssignmentPresentation)this.ConvertDataObjectToPresentationObject(dataObject);
				base.WriteResult(exchangeRoleAssignmentPresentation);
				if ((exchangeRoleAssignmentPresentation.RoleAssigneeType == RoleAssigneeType.SecurityGroup || exchangeRoleAssignmentPresentation.RoleAssigneeType == RoleAssigneeType.RoleGroup) && this.GetEffectiveUsers)
				{
					this.HandleEffectiveUsersWriteResult(dataObject, exchangeRoleAssignment, this.GetAssignmentMethod(exchangeRoleAssignment));
				}
			}
			TaskLogger.LogExit();
		}

		private void HandleEffectiveUsersWriteResult(IConfigurable dataObject, ExchangeRoleAssignment roleAssignement, AssignmentMethod assignmentMethod)
		{
			if (this.roleAssignmentExpansion == null)
			{
				this.roleAssignmentExpansion = new RoleAssignmentExpansion(base.TenantGlobalCatalogSession, base.CurrentOrganizationId);
			}
			List<ADObjectId> effectiveUsersForRoleAssignment = this.roleAssignmentExpansion.GetEffectiveUsersForRoleAssignment(roleAssignement);
			foreach (ADObjectId adobjectId in effectiveUsersForRoleAssignment)
			{
				if (this.assignee == null || (this.assignee != null && this.assignee.Id.Equals(adobjectId)))
				{
					MultiValuedProperty<FormattedADObjectIdCollection> assignmentChains = this.roleAssignmentExpansion.GetAssignmentChains(roleAssignement.User, adobjectId);
					ExchangeRoleAssignmentPresentation exchangeRoleAssignmentPresentation = (ExchangeRoleAssignmentPresentation)this.ConvertDataObjectToPresentationObject(dataObject);
					exchangeRoleAssignmentPresentation.UpdatePresentationObjectWithEffectiveUser(adobjectId, assignmentChains, this.GetEffectiveUsers, assignmentMethod);
					base.WriteResult(exchangeRoleAssignmentPresentation);
				}
			}
		}

		private AssignmentMethod GetAssignmentMethod(ExchangeRoleAssignment roleAssignment)
		{
			AssignmentMethod result = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.Direct;
			RoleAssigneeType roleAssigneeType = roleAssignment.RoleAssigneeType;
			switch (roleAssigneeType)
			{
			case RoleAssigneeType.SecurityGroup:
				result = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.SecurityGroup;
				break;
			case (RoleAssigneeType)3:
			case (RoleAssigneeType)5:
				break;
			case RoleAssigneeType.RoleAssignmentPolicy:
				result = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleAssignmentPolicy;
				break;
			case RoleAssigneeType.MailboxPlan:
				result = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.MailboxPlan;
				break;
			default:
				switch (roleAssigneeType)
				{
				case RoleAssigneeType.RoleGroup:
				case RoleAssigneeType.LinkedRoleGroup:
					result = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleGroup;
					break;
				}
				break;
			}
			return result;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			AssignmentMethod assignmentMethod = Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.Direct;
			ExchangeRoleAssignment exchangeRoleAssignment = (ExchangeRoleAssignment)dataObject;
			ADObjectId adobjectId = exchangeRoleAssignment.User;
			ADObjectId adobjectId2 = exchangeRoleAssignment.User;
			if (this.sharedConfiguration != null && adobjectId2 != null)
			{
				RoleAssigneeType roleAssigneeType = exchangeRoleAssignment.RoleAssigneeType;
				switch (roleAssigneeType)
				{
				case RoleAssigneeType.SecurityGroup:
				case (RoleAssigneeType)3:
					break;
				case RoleAssigneeType.RoleAssignmentPolicy:
					adobjectId2 = (adobjectId = this.GetTinyTenantLocalRap());
					goto IL_A1;
				default:
					if (roleAssigneeType != RoleAssigneeType.RoleGroup)
					{
					}
					break;
				}
				ADObjectId[] tinyTenantGroupIds = this.sharedConfiguration.GetTinyTenantGroupIds(new ADObjectId[]
				{
					adobjectId2
				});
				if (!tinyTenantGroupIds.IsNullOrEmpty<ADObjectId>())
				{
					adobjectId2 = (adobjectId = tinyTenantGroupIds[0]);
				}
				else if (this.sharedConfiguration.GetSharedRoleAssignmentPolicy().Equals(adobjectId2))
				{
					adobjectId2 = (adobjectId = this.GetTinyTenantLocalRap());
				}
			}
			IL_A1:
			if (this.RoleAssignee != null && !this.assignee.Id.Equals(adobjectId))
			{
				adobjectId = this.assignee.Id;
				if (exchangeRoleAssignment.RoleAssigneeType == RoleAssigneeType.User)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorUnknownAssignmentMethod(exchangeRoleAssignment.RoleAssigneeType.ToString())), ErrorCategory.InvalidData, null);
				}
				else
				{
					assignmentMethod = this.GetAssignmentMethod(exchangeRoleAssignment);
				}
			}
			string userName = string.Empty;
			RoleAssigneeType roleAssigneeType2 = exchangeRoleAssignment.RoleAssigneeType;
			switch (roleAssigneeType2)
			{
			case RoleAssigneeType.SecurityGroup:
				break;
			case (RoleAssigneeType)3:
				goto IL_177;
			case RoleAssigneeType.RoleAssignmentPolicy:
				userName = Strings.AllPolicyAssignees;
				goto IL_18C;
			default:
				switch (roleAssigneeType2)
				{
				case RoleAssigneeType.ForeignSecurityPrincipal:
					userName = Strings.AllForeignAccounts;
					goto IL_18C;
				case (RoleAssigneeType)9:
				case RoleAssigneeType.PartnerLinkedRoleGroup:
					goto IL_177;
				case RoleAssigneeType.RoleGroup:
					break;
				case RoleAssigneeType.LinkedRoleGroup:
					userName = Strings.AllLinkedGroupMembers;
					goto IL_18C;
				default:
					goto IL_177;
				}
				break;
			}
			userName = Strings.AllGroupMembers;
			goto IL_18C;
			IL_177:
			if (exchangeRoleAssignment.User != null)
			{
				userName = exchangeRoleAssignment.User.Name;
			}
			IL_18C:
			ExchangeRoleAssignmentPresentation result = new ExchangeRoleAssignmentPresentation(exchangeRoleAssignment, adobjectId, assignmentMethod, userName, (this.sharedConfiguration != null) ? adobjectId2 : null, (this.sharedConfiguration != null) ? this.sharedConfiguration.SharedConfigurationCU.OrganizationId : null);
			TaskLogger.LogExit();
			return result;
		}

		private void InitializeWritableReportingObjectIfNecessary()
		{
			if (this.WritableServer != null)
			{
				this.writableObject = (Server)base.GetDataObject<Server>(this.WritableServer, this.ConfigurationSession, null, new LocalizedString?(Strings.WritableServerNotFound(this.WritableServer.ToString())), new LocalizedString?(Strings.WritableServerNotUnique(this.WritableServer.ToString())));
				this.writableObjectType = GetManagementRoleAssignment.WritableObjectType.Server;
			}
			if (this.WritableDatabase != null)
			{
				this.writableObject = (Database)base.GetDataObject<Database>(this.WritableDatabase, this.ConfigurationSession, null, new LocalizedString?(Strings.WritableDatabaseNotFound(this.WritableDatabase.ToString())), new LocalizedString?(Strings.WritableDatabaseNotUnique(this.WritableDatabase.ToString())));
				this.writableObjectType = GetManagementRoleAssignment.WritableObjectType.Database;
			}
			if (this.WritableRecipient != null)
			{
				this.writableObject = (ADRecipient)base.GetDataObject<ADRecipient>(this.WritableRecipient, base.TenantGlobalCatalogSession, null, GetManagementRoleAssignment.AllowedRecipientTypes, new LocalizedString?(Strings.WritableRecipientNotFound(this.WritableRecipient.ToString())), new LocalizedString?(Strings.WritableRecipientNotUnique(this.WritableRecipient.ToString())));
				this.writableObjectType = GetManagementRoleAssignment.WritableObjectType.Recipient;
			}
		}

		private void InitializeManagementReportingIfNecessary()
		{
			if (this.managementReporting == null)
			{
				this.managementReporting = new ManagementReporting(base.DataSession as IConfigurationSession, base.TenantGlobalCatalogSession, base.CurrentOrganizationId, this.sharedConfiguration, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
		}

		private List<ADObjectId> GetSharedTenantAssigneeIds()
		{
			List<ADObjectId> list = new List<ADObjectId>(this.sharedConfiguration.SharedConfigurationCU.OtherWellKnownObjects.Count + 1);
			foreach (DNWithBinary dnwithBinary in this.sharedConfiguration.SharedConfigurationCU.OtherWellKnownObjects)
			{
				list.Add(new ADObjectId(dnwithBinary.DistinguishedName));
			}
			list.Add(this.sharedConfiguration.GetSharedRoleAssignmentPolicy());
			return list;
		}

		private ADObjectId GetTinyTenantLocalRap()
		{
			if (this.tinyTenantLocalRAP == null)
			{
				RoleAssignmentPolicy[] array = (this.sharedSystemConfigSession ?? this.ConfigurationSession).Find<RoleAssignmentPolicy>(null, QueryScope.SubTree, null, null, 1);
				this.tinyTenantLocalRAP = array[0].Id;
			}
			return this.tinyTenantLocalRAP;
		}

		private void ReScopeSharedConfigAndSharedSessionIfNecessary(OrganizationId organizationId)
		{
			if (this.IgnoreDehydratedFlag)
			{
				return;
			}
			this.sharedConfiguration = SharedConfiguration.GetSharedConfiguration(organizationId);
			if (this.sharedConfiguration != null)
			{
				this.sharedSystemConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, this.sharedConfiguration.GetSharedConfigurationSessionSettings(), 1367, "ReScopeSharedConfigAndSharedSessionIfNecessary", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleAssignment\\GetManagementRoleAssignment.cs");
			}
		}

		private const string ParameterSetRole = "Role";

		private const string ParameterSetRoleAssignee = "RoleAssignee";

		private static bool IsDatacenter = Datacenter.IsMicrosoftHostedOnly(true);

		private QueryFilter internalFilterForNonPipelineParameter;

		private QueryFilter internalFilterForPipelineParameter;

		private ADObject assignee;

		private List<ADObjectId> assigneeIds;

		private ADObject anyRole;

		private ManagementReporting managementReporting;

		private ADObject writableObject;

		private GetManagementRoleAssignment.WritableObjectType writableObjectType = GetManagementRoleAssignment.WritableObjectType.NotApplicable;

		private RoleAssignmentExpansion roleAssignmentExpansion;

		private SharedConfiguration sharedConfiguration;

		private IConfigurationSession sharedSystemConfigSession;

		private ADObjectId tinyTenantLocalRAP;

		private static readonly AssignmentMethod[] AllowedAssignmentMethods = new AssignmentMethod[]
		{
			Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.Direct,
			Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.SecurityGroup,
			Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.MailboxPlan,
			Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleAssignmentPolicy,
			Microsoft.Exchange.Data.Directory.SystemConfiguration.AssignmentMethod.RoleGroup
		};

		private static readonly OptionalIdentityData AllowedRecipientTypes = new OptionalIdentityData
		{
			AdditionalFilter = QueryFilter.OrTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.User),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.Contact),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailContact),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.SystemMailbox)
			})
		};

		private enum WritableObjectType
		{
			NotApplicable = 1,
			Recipient = 3,
			Server,
			Database = 6
		}
	}
}
