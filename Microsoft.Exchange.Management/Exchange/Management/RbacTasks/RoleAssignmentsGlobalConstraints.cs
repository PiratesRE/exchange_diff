using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	internal sealed class RoleAssignmentsGlobalConstraints
	{
		internal List<ExchangeRole> CacheRoles { get; private set; }

		internal RoleAssignmentsGlobalConstraints(IConfigurationSession configurationSession, IRecipientSession recipientSession, Task.ErrorLoggerDelegate writeError)
		{
			this.configurationSession = configurationSession;
			this.configurationSession.SessionSettings.IsSharedConfigChecked = true;
			this.recipientSession = recipientSession;
			this.writeError = writeError;
			this.CacheRoles = new List<ExchangeRole>();
		}

		private void InitializeContextVariables()
		{
			this.excludedFromAssignmentSearch = null;
			this.excludedFromAssignmentSearch = new List<ADObjectId>();
			this.excludedFromEmptinessValidation = null;
			this.excludedFromEmptinessValidation = new List<ADObjectId>();
			this.roleGroups = null;
			this.roleGroups = new List<ADGroup>();
			this.expandedRoleGroups = null;
			this.expandedRoleGroups = new Dictionary<ADObjectId, int>();
			this.isObjectRemovalValidation = false;
			this.powerShellEnabledUserStatus = new Dictionary<ADObjectId, bool>();
			this.includeOnlyPowerShellEnabledRecipients = false;
		}

		private ExchangeRole GetRole(ADObjectId roleId)
		{
			ExchangeRole exchangeRole = this.CacheRoles.FirstOrDefault((ExchangeRole x) => x.Id.Equals(roleId));
			if (exchangeRole == null)
			{
				exchangeRole = this.configurationSession.Read<ExchangeRole>(roleId);
				if (exchangeRole == null)
				{
					this.writeError(new ManagementObjectNotFoundException(Strings.ErrorObjectNotFound(roleId.ToString())), ExchangeErrorCategory.Client, null);
				}
				this.CacheRoles.Add(exchangeRole);
			}
			return exchangeRole;
		}

		private ADGroup GetGroup(ADObjectId groupId)
		{
			ADGroup adgroup = this.roleGroups.FirstOrDefault((ADGroup x) => x.Id.Equals(groupId));
			if (adgroup == null)
			{
				ADRecipient adrecipient = this.recipientSession.Read(groupId);
				if (adrecipient == null)
				{
					this.writeError(new ManagementObjectNotFoundException(Strings.ErrorObjectNotFound(groupId.ToString())), ExchangeErrorCategory.Client, null);
				}
				adgroup = (adrecipient as ADGroup);
				if (adgroup == null)
				{
					this.writeError(new TaskException(Strings.ErrorObjectNotFound(groupId.ToString())), ExchangeErrorCategory.Client, null);
				}
				this.roleGroups.Add(adgroup);
			}
			return adgroup;
		}

		private bool IsPowerShellEnabled(ADObjectId userId)
		{
			bool flag = false;
			if (!this.powerShellEnabledUserStatus.TryGetValue(userId, out flag))
			{
				ADRecipient[] array = this.recipientSession.Find(userId, QueryScope.Base, RoleAssignmentsGlobalConstraints.powerShellEnabledUserFilter, null, 1);
				flag = (array != null && array.Length == 1);
				this.powerShellEnabledUserStatus.Add(userId, flag);
			}
			return flag;
		}

		private bool IsOrganizationBeingRemoved(OrganizationId organizationId)
		{
			if (!this.organizationStatusMap.ContainsKey(organizationId))
			{
				ExchangeConfigurationUnit exchangeConfigurationUnit = this.configurationSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
				this.organizationStatusMap[organizationId] = exchangeConfigurationUnit.OrganizationStatus;
			}
			return ExchangeConfigurationUnit.IsBeingDeleted(this.organizationStatusMap[organizationId]);
		}

		private bool IsSafeToRemoveDisableAssignmentFromGroup(ExchangeRoleAssignment roleAssignment)
		{
			if (!RoleAssignmentsGlobalConstraints.IsValidCannedRoleToGroupAssignment(roleAssignment))
			{
				return true;
			}
			ExchangeRole role = this.GetRole(roleAssignment.Role);
			if (!role.IsValid)
			{
				return true;
			}
			bool flag = true;
			bool verifyGroupEmptiness = false;
			if (roleAssignment.RoleAssignmentDelegationType.Equals(RoleAssignmentDelegationType.DelegatingOrgWide) && role.IsRootRole && !role.IsUnscoped)
			{
				flag = false;
			}
			if (roleAssignment.RoleAssignmentDelegationType.Equals(RoleAssignmentDelegationType.Regular) && role.IsRootRole && RoleAssignmentsGlobalConstraints.RoleTypesWithRegularAssignment.Contains(role.RoleType))
			{
				flag = false;
				verifyGroupEmptiness = true;
			}
			flag = (flag || !role.GetImplicitScopeSet().Equals(roleAssignment.GetSimpleScopeSet()));
			if (!flag)
			{
				flag = this.ExistDistinctRoleAssignmentForGroup(roleAssignment, role, verifyGroupEmptiness);
			}
			return flag;
		}

		private bool ExistDistinctRoleAssignmentForGroup(ExchangeRoleAssignment roleAssignment, ExchangeRole role, bool verifyGroupEmptiness)
		{
			ADObjectId adobjectId = roleAssignment.OrganizationId.Equals(OrganizationId.ForestWideOrgId) ? this.configurationSession.GetOrgContainerId() : roleAssignment.OrganizationId.ConfigurationUnit;
			adobjectId = adobjectId.GetDescendantId(ExchangeRoleAssignment.RdnContainer);
			List<QueryFilter> list = new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, roleAssignment.Id),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, roleAssignment.Role),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RecipientWriteScope, (RecipientWriteScopeType)role.ImplicitRecipientWriteScope),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, (ConfigWriteScopeType)role.ImplicitConfigWriteScope),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, roleAssignment.RoleAssignmentDelegationType),
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssigneeType, RoleAssigneeType.RoleGroup),
					new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssigneeType, RoleAssigneeType.SecurityGroup)
				})
			};
			if (this.excludedFromAssignmentSearch != null)
			{
				foreach (ADObjectId propertyValue in this.excludedFromAssignmentSearch)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeRoleAssignmentSchema.User, propertyValue));
				}
			}
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in this.configurationSession.FindPaged<ExchangeRoleAssignment>(adobjectId, QueryScope.OneLevel, new AndFilter(list.ToArray()), null, 0))
			{
				if (exchangeRoleAssignment.IsValid && exchangeRoleAssignment.Enabled && exchangeRoleAssignment.RecipientReadScope.Equals(role.ImplicitRecipientReadScope) && exchangeRoleAssignment.ConfigReadScope.Equals(role.ImplicitConfigReadScope) && (!verifyGroupEmptiness || !this.IsGroupEmpty(exchangeRoleAssignment.User)))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsUserRequiredForAssignment(ExchangeRoleAssignment roleAssignment)
		{
			if (!RoleAssignmentsGlobalConstraints.IsValidCannedRoleToGroupAssignment(roleAssignment))
			{
				return false;
			}
			ExchangeRole role = this.GetRole(roleAssignment.Role);
			return role.IsValid && (roleAssignment.RoleAssignmentDelegationType.Equals(RoleAssignmentDelegationType.Regular) && role.IsRootRole && RoleAssignmentsGlobalConstraints.RoleTypesWithRegularAssignment.Contains(role.RoleType)) && role.GetImplicitScopeSet().Equals(roleAssignment.GetSimpleScopeSet());
		}

		private bool IsGroupEmpty(ADObjectId groupId)
		{
			ADObjectId roleGroupIdForEmptinessCheck = this.GetRoleGroupIdForEmptinessCheck(groupId);
			return roleGroupIdForEmptinessCheck != null && this.IsGroupEmpty(this.GetGroup(roleGroupIdForEmptinessCheck));
		}

		private bool IsGroupEmpty(ADGroup group)
		{
			if (group.Members.Count == 0)
			{
				if (!this.expandedRoleGroups.ContainsKey(group.Id))
				{
					this.expandedRoleGroups.Add(group.Id, 0);
				}
				return true;
			}
			if (this.expandedRoleGroups.ContainsKey(group.Id))
			{
				return this.expandedRoleGroups[group.Id] == 0;
			}
			int numberOfUsers = 0;
			ADRecipientExpansion adrecipientExpansion = new ADRecipientExpansion(this.recipientSession, true);
			adrecipientExpansion.Expand(group, delegate(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
			{
				if (group.Id.Equals(recipient.Id))
				{
					return ExpansionControl.Continue;
				}
				if (this.excludedFromEmptinessValidation != null && this.excludedFromEmptinessValidation.Contains(recipient.Id) && (this.isObjectRemovalValidation || (parent != null && parent.Id.Equals(group.Id))))
				{
					return ExpansionControl.Skip;
				}
				if (recipientExpansionType == ExpansionType.None)
				{
					if (this.includeOnlyPowerShellEnabledRecipients)
					{
						if (this.IsPowerShellEnabled(recipient.Id))
						{
							numberOfUsers++;
						}
					}
					else
					{
						numberOfUsers++;
					}
				}
				if (this.expandedRoleGroups.ContainsKey(recipient.Id))
				{
					numberOfUsers += this.expandedRoleGroups[recipient.Id];
					if (numberOfUsers == 0)
					{
						return ExpansionControl.Skip;
					}
				}
				if (numberOfUsers > 0)
				{
					return ExpansionControl.Terminate;
				}
				return ExpansionControl.Continue;
			}, delegate(ExpansionFailure failure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
			{
				if (failure.Equals(ExpansionFailure.NoMembers))
				{
					return ExpansionControl.Continue;
				}
				return ExpansionControl.Skip;
			});
			this.expandedRoleGroups.Add(group.Id, numberOfUsers);
			return numberOfUsers == 0;
		}

		private Result<ExchangeRoleAssignment>[] GetInheritedRoleAssignments(ADGroup group)
		{
			List<string> tokenSids = this.recipientSession.GetTokenSids(group, AssignmentMethod.SecurityGroup);
			if (tokenSids == null || (tokenSids != null && tokenSids.Count == 0))
			{
				return null;
			}
			bool useGlobalCatalog = this.recipientSession.UseGlobalCatalog;
			ADObjectId[] array = null;
			try
			{
				this.recipientSession.UseGlobalCatalog = true;
				array = this.recipientSession.ResolveSidsToADObjectIds(tokenSids.ToArray());
				if (this.sharedConfig != null)
				{
					array = this.sharedConfig.GetSharedRoleGroupIds(array);
				}
			}
			finally
			{
				this.recipientSession.UseGlobalCatalog = useGlobalCatalog;
			}
			if (array == null)
			{
				return null;
			}
			Result<ExchangeRoleAssignment>[] result = null;
			useGlobalCatalog = this.configurationSession.UseGlobalCatalog;
			try
			{
				this.configurationSession.UseGlobalCatalog = true;
				result = this.configurationSession.FindRoleAssignmentsByUserIds(array, new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeRoleAssignmentSchema.RecipientWriteScope, RecipientWriteScopeType.CustomRecipientScope),
					new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.CustomConfigScope)
				}));
			}
			finally
			{
				this.configurationSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return result;
		}

		private Result<ExchangeRoleAssignment>[] GetDirectRoleAssignmentsForGroup(ADGroup group)
		{
			ADObjectId roleGroupIdForRoleAssignementCheck = this.GetRoleGroupIdForRoleAssignementCheck(group.Id);
			if (roleGroupIdForRoleAssignementCheck != null)
			{
				return this.configurationSession.FindRoleAssignmentsByUserIds(new ADObjectId[]
				{
					roleGroupIdForRoleAssignementCheck
				}, RoleAssignmentsGlobalConstraints.RoleAssignmentsFilter);
			}
			return new Result<ExchangeRoleAssignment>[0];
		}

		private bool HierarchicalCheckForGroupEmptiness(ADGroup group, out ExchangeRoleAssignment roleAssignment)
		{
			roleAssignment = null;
			Result<ExchangeRoleAssignment>[] inheritedRoleAssignments = this.GetInheritedRoleAssignments(group);
			if (inheritedRoleAssignments == null)
			{
				return true;
			}
			this.excludedFromEmptinessValidation.Add(group.Id);
			List<ADGroup> list = new List<ADGroup>();
			Result<ExchangeRoleAssignment>[] array = inheritedRoleAssignments;
			for (int i = 0; i < array.Length; i++)
			{
				Result<ExchangeRoleAssignment> assignment = array[i];
				Result<ExchangeRoleAssignment> assignment8 = assignment;
				if (RoleAssignmentsGlobalConstraints.IsValidCannedRoleToGroupAssignment(assignment8.Data))
				{
					if (!list.Exists(delegate(ADGroup x)
					{
						ADObjectId id = x.Id;
						Result<ExchangeRoleAssignment> assignment7 = assignment;
						return id.Equals(assignment7.Data.User);
					}))
					{
						Result<ExchangeRoleAssignment> assignment2 = assignment;
						if (this.IsUserRequiredForAssignment(assignment2.Data))
						{
							Result<ExchangeRoleAssignment> assignment3 = assignment;
							if (!this.IsGroupEmpty(assignment3.Data.User))
							{
								list.Add(group);
							}
							else
							{
								Result<ExchangeRoleAssignment> assignment4 = assignment;
								ExchangeRoleAssignment data = assignment4.Data;
								Result<ExchangeRoleAssignment> assignment5 = assignment;
								if (!this.ExistDistinctRoleAssignmentForGroup(data, this.GetRole(assignment5.Data.Role), true))
								{
									Result<ExchangeRoleAssignment> assignment6 = assignment;
									roleAssignment = assignment6.Data;
									return false;
								}
							}
						}
					}
				}
			}
			return true;
		}

		private bool HierarchicalCheckForCannedRoleGroups(ADGroup group, out ADGroup cannedRoleGroup)
		{
			cannedRoleGroup = null;
			if (group.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				cannedRoleGroup = this.recipientSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EoaWkGuid, this.configurationSession.ConfigurationNamingContext);
				if (cannedRoleGroup == null)
				{
					this.writeError(new ExOrgAdminSGroupNotFoundException(WellKnownGuid.EoaWkGuid), ExchangeErrorCategory.ServerOperation, null);
				}
				this.excludedFromEmptinessValidation.Add(group.Id);
				return !cannedRoleGroup.ContainsMember(group.Id, false) || !this.IsGroupEmpty(cannedRoleGroup);
			}
			return true;
		}

		public void ValidateIsSafeToRemoveAssignment(ExchangeRoleAssignment roleAssignment)
		{
			if (roleAssignment == null)
			{
				return;
			}
			this.InitializeContextVariables();
			if (!this.IsSafeToRemoveDisableAssignmentFromGroup(roleAssignment))
			{
				this.writeError(new TaskInvalidOperationException(Strings.ErrorRoleAssignmentConstraintViolation(roleAssignment.Name, roleAssignment.Role.Name, roleAssignment.User.Name)), ExchangeErrorCategory.Client, null);
			}
		}

		public void ValidateIsSafeToModifyAssignment(ExchangeRoleAssignment roleAssignment, bool disablingAssignment)
		{
			if (roleAssignment == null)
			{
				return;
			}
			this.InitializeContextVariables();
			if (!this.IsSafeToRemoveDisableAssignmentFromGroup(roleAssignment))
			{
				if (disablingAssignment)
				{
					this.writeError(new TaskInvalidOperationException(Strings.ErrorRoleAssignmentConstraintViolation(roleAssignment.Name, roleAssignment.Role.Name, roleAssignment.User.Name)), ExchangeErrorCategory.Client, null);
					return;
				}
				this.writeError(new TaskInvalidOperationException(Strings.ErrorRoleAssignmentConstraintViolationOnScopes(roleAssignment.Name, roleAssignment.Role.Name)), ExchangeErrorCategory.Client, null);
			}
		}

		public void ValidateIsSafeToRemoveRoleGroup(ADGroup group, Result<ExchangeRoleAssignment>[] roleAssignments, Task task)
		{
			if (group == null)
			{
				return;
			}
			if (group.OrganizationId != OrganizationId.ForestWideOrgId && !this.ShouldPreventLastAdminRemoval(task, group.OrganizationId))
			{
				return;
			}
			this.InitializeContextVariables();
			this.excludedFromEmptinessValidation.Add(group.Id);
			this.excludedFromAssignmentSearch.Add(group.Id);
			this.isObjectRemovalValidation = true;
			if (roleAssignments != null)
			{
				foreach (Result<ExchangeRoleAssignment> result in roleAssignments)
				{
					if (!this.IsSafeToRemoveDisableAssignmentFromGroup(result.Data))
					{
						this.writeError(new TaskInvalidOperationException(Strings.ErrorCannotDeleteGroupRoleAssignmentConstraint(group.Name, result.Data.Name, result.Data.Role.Name)), ExchangeErrorCategory.Client, null);
					}
				}
			}
			if (!this.IsGroupEmpty(group))
			{
				ExchangeRoleAssignment exchangeRoleAssignment;
				if (!this.HierarchicalCheckForGroupEmptiness(group, out exchangeRoleAssignment))
				{
					this.writeError(new TaskInvalidOperationException(Strings.ErrorCannotDeleteGroupRoleAssignmentConstraint(group.Name, exchangeRoleAssignment.Name, exchangeRoleAssignment.Role.Name)), ExchangeErrorCategory.Client, null);
				}
				ADGroup adgroup = null;
				if (!this.HierarchicalCheckForCannedRoleGroups(group, out adgroup))
				{
					this.writeError(new TaskInvalidOperationException(Strings.ErrorCannedRoleGroupCannotBeEmpty(adgroup.Name, group.Name)), ExchangeErrorCategory.Client, null);
				}
			}
		}

		public void ValidateIsSafeToRemoveRoleGroupMember(ADGroup group, List<ADObjectId> membersToRemove)
		{
			if (group == null || membersToRemove == null)
			{
				return;
			}
			this.InitializeContextVariables();
			this.excludedFromEmptinessValidation.AddRange(membersToRemove);
			this.excludedFromAssignmentSearch.Add(group.Id);
			if (!this.IsGroupEmpty(group))
			{
				return;
			}
			this.excludedFromEmptinessValidation.Add(group.Id);
			string membersToRemove2 = RoleGroupCommon.NamesFromObjects(membersToRemove);
			bool flag = RoleGroupCommon.IsPrecannedRoleGroup(group, this.configurationSession, new Guid[]
			{
				RoleGroup.OrganizationManagement_InitInfo.WellKnownGuid
			});
			if (flag)
			{
				this.writeError(new TaskInvalidOperationException(Strings.ErrorCannedRoleGroupCannotBeEmpty(group.Name, membersToRemove2)), ExchangeErrorCategory.Client, null);
			}
			SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(group.OrganizationId);
			if (sharedConfiguration != null)
			{
				return;
			}
			Result<ExchangeRoleAssignment>[] directRoleAssignmentsForGroup = this.GetDirectRoleAssignmentsForGroup(group);
			if (directRoleAssignmentsForGroup != null)
			{
				foreach (Result<ExchangeRoleAssignment> result in directRoleAssignmentsForGroup)
				{
					if (!this.IsSafeToRemoveDisableAssignmentFromGroup(result.Data))
					{
						this.writeError(new TaskInvalidOperationException(Strings.ErrorGroupCannotBeEmptyRoleAssignmentConstraint(group.Name, membersToRemove2, result.Data.Name, result.Data.Role.Name)), ExchangeErrorCategory.Client, null);
					}
				}
			}
			ExchangeRoleAssignment exchangeRoleAssignment;
			if (!this.HierarchicalCheckForGroupEmptiness(group, out exchangeRoleAssignment))
			{
				this.writeError(new TaskInvalidOperationException(Strings.ErrorGroupCannotBeEmptyRoleAssignmentConstraint(group.Name, membersToRemove2, exchangeRoleAssignment.Name, exchangeRoleAssignment.Role.Name)), ExchangeErrorCategory.Client, null);
			}
			ADGroup adgroup = null;
			if (!this.HierarchicalCheckForCannedRoleGroups(group, out adgroup))
			{
				this.writeError(new TaskInvalidOperationException(Strings.ErrorCannedRoleGroupCannotBeEmpty(adgroup.Name, membersToRemove2)), ExchangeErrorCategory.Client, null);
			}
		}

		public bool IsLastAdmin(ADUser adUser)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			IRecipientSession recipientSession = this.recipientSession;
			IConfigurationSession configurationSession = this.configurationSession;
			bool result;
			try
			{
				this.InitializeContextVariables();
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), adUser.OrganizationId, adUser.OrganizationId, false);
				this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 1045, "IsLastAdmin", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleAssignment\\RoleAssignmentsGlobalConstraints.cs");
				this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 1046, "IsLastAdmin", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleAssignment\\RoleAssignmentsGlobalConstraints.cs");
				this.excludedFromEmptinessValidation.Add(adUser.Id);
				this.excludedFromAssignmentSearch.Add(adUser.Id);
				this.isObjectRemovalValidation = true;
				this.includeOnlyPowerShellEnabledRecipients = this.IsPowerShellEnabled(adUser.Id);
				List<string> tokenSids = this.recipientSession.GetTokenSids(adUser, AssignmentMethod.SecurityGroup | AssignmentMethod.RoleGroup);
				if (tokenSids == null || tokenSids.Count == 0)
				{
					result = false;
				}
				else
				{
					ADObjectId[] entryIds = this.recipientSession.ResolveSidsToADObjectIds(tokenSids.ToArray());
					this.sharedConfig = SharedConfiguration.GetSharedConfiguration(adUser.OrganizationId);
					if (this.sharedConfig != null)
					{
						this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.configurationSession.ConsistencyMode, this.sharedConfig.GetSharedConfigurationSessionSettings(), 1065, "IsLastAdmin", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleAssignment\\RoleAssignmentsGlobalConstraints.cs");
					}
					Result<ADGroup>[] array = this.recipientSession.ReadMultipleADGroups(entryIds);
					foreach (Result<ADGroup> result2 in array)
					{
						bool flag = this.IsGroupEmpty(result2.Data);
						if (flag)
						{
							Result<ExchangeRoleAssignment>[] directRoleAssignmentsForGroup = this.GetDirectRoleAssignmentsForGroup(result2.Data);
							if (directRoleAssignmentsForGroup != null)
							{
								foreach (Result<ExchangeRoleAssignment> result3 in directRoleAssignmentsForGroup)
								{
									if (!this.IsSafeToRemoveDisableAssignmentFromGroup(result3.Data))
									{
										return true;
									}
								}
							}
							ExchangeRoleAssignment exchangeRoleAssignment;
							if (!this.HierarchicalCheckForGroupEmptiness(result2.Data, out exchangeRoleAssignment))
							{
								return true;
							}
						}
					}
					result = false;
				}
			}
			finally
			{
				this.recipientSession = recipientSession;
				this.configurationSession = configurationSession;
			}
			return result;
		}

		internal bool ShouldPreventLastAdminRemoval(Task task, OrganizationId organization)
		{
			return task.ExecutingUserOrganizationId != OrganizationId.ForestWideOrgId && Datacenter.IsMultiTenancyEnabled() && !this.IsOrganizationBeingRemoved(organization);
		}

		internal static bool IsValidCannedRoleToGroupAssignment(ExchangeRoleAssignment roleAssignment)
		{
			return roleAssignment != null && roleAssignment.IsValid && (roleAssignment.RoleAssigneeType == RoleAssigneeType.RoleGroup || roleAssignment.RoleAssigneeType == RoleAssigneeType.SecurityGroup) && roleAssignment.RecipientWriteScope != RecipientWriteScopeType.ExclusiveRecipientScope && roleAssignment.RecipientWriteScope != RecipientWriteScopeType.CustomRecipientScope && roleAssignment.ConfigWriteScope != ConfigWriteScopeType.ExclusiveConfigScope && roleAssignment.ConfigWriteScope != ConfigWriteScopeType.CustomConfigScope && roleAssignment.IsAssignmentToRootRole;
		}

		private ADObjectId GetRoleGroupIdForRoleAssignementCheck(ADObjectId roleGroupId)
		{
			if (this.sharedConfig == null || !roleGroupId.IsDescendantOf(this.sharedConfig.TinyTenantId.OrganizationalUnit))
			{
				return roleGroupId;
			}
			ADObjectId[] sharedRoleGroupIds = this.sharedConfig.GetSharedRoleGroupIds(new ADObjectId[]
			{
				roleGroupId
			});
			if (!sharedRoleGroupIds.IsNullOrEmpty<ADObjectId>())
			{
				return sharedRoleGroupIds[0];
			}
			return null;
		}

		private ADObjectId GetRoleGroupIdForEmptinessCheck(ADObjectId roleGroupId)
		{
			if (this.sharedConfig == null || !roleGroupId.IsDescendantOf(this.sharedConfig.SharedConfigId.OrganizationalUnit))
			{
				return roleGroupId;
			}
			ADObjectId[] tinyTenantGroupIds = this.sharedConfig.GetTinyTenantGroupIds(new ADObjectId[]
			{
				roleGroupId
			});
			if (!tinyTenantGroupIds.IsNullOrEmpty<ADObjectId>())
			{
				return tinyTenantGroupIds[0];
			}
			return null;
		}

		private static readonly List<RoleType> RoleTypesWithRegularAssignment = new List<RoleType>(1)
		{
			RoleType.RoleManagement
		};

		private IConfigurationSession configurationSession;

		private IRecipientSession recipientSession;

		private Task.ErrorLoggerDelegate writeError;

		private SharedConfiguration sharedConfig;

		private static readonly QueryFilter RoleAssignmentsFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeRoleAssignmentSchema.RecipientWriteScope, RecipientWriteScopeType.CustomRecipientScope),
			new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.CustomConfigScope)
		});

		private static readonly ComparisonFilter powerShellEnabledUserFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RemotePowerShellEnabled, true);

		private readonly Dictionary<OrganizationId, OrganizationStatus> organizationStatusMap = new Dictionary<OrganizationId, OrganizationStatus>();

		private List<ADObjectId> excludedFromAssignmentSearch;

		private List<ADObjectId> excludedFromEmptinessValidation;

		private List<ADGroup> roleGroups;

		private Dictionary<ADObjectId, int> expandedRoleGroups;

		private Dictionary<ADObjectId, bool> powerShellEnabledUserStatus;

		private bool isObjectRemovalValidation;

		private bool includeOnlyPowerShellEnabledRecipients;
	}
}
