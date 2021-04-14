using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class DeprecatedRoleUpgrader : RoleUpgrader
	{
		protected DeprecatedRoleUpgrader(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping) : base(settings, roleNameMapping)
		{
		}

		public override void UpdateRole(RoleDefinition definition)
		{
			throw new NotImplementedException("DeprecateRoleUpgrader feature not implemented");
		}

		public override void UpdateRoles(List<RoleDefinition> rolesDefinitions)
		{
			this.CreateOrUpdateRoles(this.roleNameMapping, rolesDefinitions, (this.settings.ServicePlanSettings == null) ? null : this.settings.ServicePlanSettings.Organization.GetEnabledPermissionFeatures());
		}

		public static bool CanUpgrade(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping, RoleDefinition roleDefinition, out RoleUpgrader roleUpgrader)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20009L, "-->DeprecatedRoleUpgrader.CanUpgrade: roleName = {0}", roleDefinition.RoleName);
			bool flag = false;
			roleUpgrader = null;
			if (settings.Organization == null)
			{
				flag = true;
			}
			else if (!roleDefinition.IsEndUserRole && settings.ServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
			{
				flag = true;
			}
			flag = (flag && roleNameMapping != null && roleNameMapping.IsDeprecatedRole);
			if (flag)
			{
				roleUpgrader = new DeprecatedRoleUpgrader(settings, roleNameMapping);
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(20009L, "<--DeprecatedRoleUpgrader.CanUpgrade: canUpgrade = {0}", flag);
			return flag;
		}

		protected void CreateOrUpdateRoles(RoleNameMapping mapping, List<RoleDefinition> roleDefinitions, List<string> enabledPermissionFeatures)
		{
			this.CreateOrUpdateRoles(mapping, roleDefinitions, enabledPermissionFeatures, null, null);
		}

		protected virtual void CreateOrUpdateRoles(RoleNameMapping mapping, List<RoleDefinition> roleDefinitions, List<string> enabledPermissionFeatures, string suffix, string mailboxPlanIndex)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<int>(20008L, "-->CreateOrUpdateRoles: roleDefinitions count = {0}", roleDefinitions.Count);
			List<ExchangeRole> list = new List<ExchangeRole>(roleDefinitions.Count);
			foreach (RoleDefinition definition in roleDefinitions)
			{
				ExchangeRole exchangeRole = base.CreateOrUpdateRole(null, definition, enabledPermissionFeatures, suffix, mailboxPlanIndex);
				if (exchangeRole != null)
				{
					list.Add(exchangeRole);
				}
			}
			string text = (suffix == null) ? mapping.OldName : (mapping.OldName + suffix);
			ExchangeRole exchangeRole2 = this.settings.ConfigurationSession.Read<ExchangeRole>(this.settings.RolesContainerId.GetChildId(text));
			if (exchangeRole2 == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20008L, "<--CreateOrUpdateRoles: Deprecated role not found. oldRoleName = {0}", text);
				return;
			}
			this.settings.LogReadObject(exchangeRole2);
			foreach (ExchangeRole newRole in list)
			{
				this.CloneRoleAssignments(exchangeRole2, newRole);
			}
			RoleEntry[] deprecatedParentRoleEntries = base.PrepareRoleForUpgradeAndGetOldSortedEntries(exchangeRole2, true);
			int num = 0;
			this.MatchDerivedRoles(exchangeRole2, deprecatedParentRoleEntries, list, ref num);
			base.SaveRoleAndWarnOnFailure(exchangeRole2);
			this.settings.LogWriteObject(exchangeRole2);
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20008L, "<--CreateOrUpdateRoles");
		}

		private void MatchDerivedRoles(ExchangeRole deprecatedParentRole, RoleEntry[] deprecatedParentRoleEntries, List<ExchangeRole> newRoles, ref int recursionCount)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string, int, int>(20008L, "--->MatchDerivedRoles: deprecatedParentRole.Name = {0}, newRoles.Count = {1}, recursionCount = {2}", deprecatedParentRole.Name, newRoles.Count, recursionCount);
			if (++recursionCount >= 1000)
			{
				return;
			}
			ADPagedReader<ExchangeRole> adpagedReader = this.settings.ConfigurationSession.FindPaged<ExchangeRole>(deprecatedParentRole.Id, QueryScope.OneLevel, null, null, 0);
			foreach (ExchangeRole exchangeRole in adpagedReader)
			{
				ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20008L, "----MatchDerivedRoles: childRole = {0}", exchangeRole.Name);
				this.settings.LogReadObject(exchangeRole);
				RoleEntry[] array = base.PrepareRoleForUpgradeAndGetOldSortedEntries(exchangeRole, deprecatedParentRole.RoleState == RoleState.Deprecated);
				List<ExchangeRole> list = new List<ExchangeRole>();
				foreach (ExchangeRole exchangeRole2 in newRoles)
				{
					RoleEntry[] sortedRoleEntries = base.GetSortedRoleEntries(exchangeRole2);
					List<RoleEntry> listOfRoleEntriesForChildRole = base.GetListOfRoleEntriesForChildRole(deprecatedParentRoleEntries, array, sortedRoleEntries, exchangeRole.IsChanged(ADObjectSchema.ExchangeVersion));
					if (this.RoleEntriesMatch(sortedRoleEntries, listOfRoleEntriesForChildRole.ToArray()))
					{
						ExTraceGlobals.AccessCheckTracer.TraceFunction<string, string>(20008L, "----MatchDerivedRoles: Perfect role match. oldRole = {0}, newRole = {1}", exchangeRole.Name, exchangeRole2.Name);
						this.CloneRoleAssignments(exchangeRole, exchangeRole2);
						list.Add(exchangeRole2);
					}
					else
					{
						ExTraceGlobals.AccessCheckTracer.TraceFunction<string, ExchangeRole>(20008L, "----MatchDerivedRoles: Unable to match with role. oldRole = {0}, newRole = {1}", exchangeRole.Name, exchangeRole2);
						if (listOfRoleEntriesForChildRole.Count > 0)
						{
							ExchangeRole exchangeRole3 = this.CreateCustomizedDerivedRole(exchangeRole2, listOfRoleEntriesForChildRole, exchangeRole.Name);
							this.CloneRoleAssignments(exchangeRole, exchangeRole3);
							list.Add(exchangeRole3);
							this.settings.WriteWarning(Strings.WarningCustomRoleCreatedDuringUpgrade(exchangeRole3.Name, exchangeRole.Name, deprecatedParentRole.Name));
						}
					}
				}
				this.MatchDerivedRoles(exchangeRole, array, list, ref recursionCount);
				base.SaveDerivedRoleAndWarnOnValidationErrors(exchangeRole);
			}
			recursionCount--;
			ExTraceGlobals.AccessCheckTracer.TraceFunction<int>(20008L, "<---MatchDerivedRoles: recursionCount = {0}", recursionCount);
		}

		protected ExchangeRole CreateCustomizedDerivedRole(ExchangeRole parentRole, List<RoleEntry> roleEntries, string unmatchedRoleName)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string, string>(20008L, "--->CreateCustomizedDerivedRole: parentRole = {0}, unmatchedRoleName = {1}", parentRole.Name, unmatchedRoleName);
			ADObjectId adobjectId = parentRole.Id;
			if (!parentRole.IsRootRole)
			{
				adobjectId = adobjectId.Parent;
			}
			string text = string.Format("auto_{0}_{1}", unmatchedRoleName, adobjectId.Name).Trim();
			if (text.Length > 64)
			{
				text = text.Substring(0, 64).Trim();
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20008L, "----customRoleName {0}", text);
			ADObjectId childId = adobjectId.GetChildId(text);
			ExchangeRole exchangeRole = this.settings.ConfigurationSession.Read<ExchangeRole>(childId);
			if (exchangeRole != null)
			{
				this.settings.LogReadObject(exchangeRole);
				if (exchangeRole.RoleType != parentRole.RoleType || !exchangeRole.ExchangeVersion.Equals(parentRole.ExchangeVersion) || exchangeRole.RoleState != parentRole.RoleState)
				{
					this.settings.WriteError(new ExRBACFailedToUpdateCustomRole(unmatchedRoleName, text, exchangeRole.ToString()), ErrorCategory.ResourceExists, null);
				}
				exchangeRole.RoleEntries = new MultiValuedProperty<RoleEntry>(roleEntries);
				base.SaveDerivedRoleAndWarnOnValidationErrors(exchangeRole);
				ExTraceGlobals.AccessCheckTracer.TraceFunction(20008L, "<--CreateCustomizedDerivedRole: Role Updated");
			}
			else
			{
				ExchangeRole exchangeRole2 = new ExchangeRole();
				exchangeRole2.ProvisionalClone(parentRole);
				exchangeRole2.RoleEntries = new MultiValuedProperty<RoleEntry>(roleEntries);
				exchangeRole2.SetId(childId);
				base.SaveRoleAndSuggestCleanupOnFailure(exchangeRole2);
				exchangeRole = exchangeRole2;
				ExTraceGlobals.AccessCheckTracer.TraceFunction(20008L, "<--CreateCustomizedDerivedRole: New Custom Role created");
			}
			return exchangeRole;
		}

		protected bool RoleEntriesMatch(RoleEntry[] entriesA, RoleEntry[] entriesB)
		{
			if (entriesA.Length != entriesB.Length)
			{
				return false;
			}
			int i = 0;
			while (i < entriesA.Length)
			{
				RoleEntry roleEntry = entriesA[i];
				int num = Array.BinarySearch<RoleEntry>(entriesB, roleEntry, RoleEntryComparer.Instance);
				bool result;
				if (num >= 0)
				{
					if (roleEntry.Equals(entriesB[num]))
					{
						i++;
						continue;
					}
					result = false;
				}
				else
				{
					result = false;
				}
				return result;
			}
			return true;
		}

		protected void CloneRoleAssignments(ExchangeRole oldRole, ExchangeRole newRole)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string, string>(20008L, "-->TransferRoleAssignments: oldRole = {0}, newRole = {1}", oldRole.Name, newRole.Name);
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in this.settings.ConfigurationSession.FindPaged<ExchangeRoleAssignment>(this.settings.OrgContainerId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, oldRole.Id), null, 0))
			{
				ValidationError[] array = exchangeRoleAssignment.Validate();
				if (array.Length != 0)
				{
					this.settings.WriteWarning(Strings.WarningCannotUpgradeInvalidRoleAssignment(exchangeRoleAssignment.Name));
					foreach (ValidationError validationError in array)
					{
						this.settings.WriteWarning(validationError.Description);
					}
				}
				else if (!this.FindRoleAssignment(newRole, exchangeRoleAssignment.User, exchangeRoleAssignment))
				{
					this.CloneRoleAssignment(newRole, exchangeRoleAssignment);
				}
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20008L, "<--TransferRoleAssignments");
		}

		private bool FindRoleAssignment(ExchangeRole role, ADObjectId userId, ExchangeRoleAssignment roleAssigmentToFind)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string, string>(20008L, "-->FindRoleAssignment: role.Name = {0}, userId.Name = {1}", role.Name, userId.Name);
			List<ComparisonFilter> list = new List<ComparisonFilter>();
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, role.Id));
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.User, userId));
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags, roleAssigmentToFind[ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags]));
			if (roleAssigmentToFind.CustomConfigWriteScope != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomConfigWriteScope, roleAssigmentToFind.CustomConfigWriteScope));
			}
			if (roleAssigmentToFind.CustomRecipientWriteScope != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomRecipientWriteScope, roleAssigmentToFind.CustomRecipientWriteScope));
			}
			using (IEnumerator<ExchangeRoleAssignment> enumerator = this.settings.ConfigurationSession.FindPaged<ExchangeRoleAssignment>(this.settings.OrgContainerId, QueryScope.SubTree, new AndFilter(list.ToArray()), null, 1).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ExchangeRoleAssignment exchangeRoleAssignment = enumerator.Current;
					ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20008L, "<--FindRoleAssignment: Role assignment found. roleAssignment.Name = {0}", exchangeRoleAssignment.Name);
					return true;
				}
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20008L, "<--FindRoleAssignment: Role assignment NOT found");
			return false;
		}

		private void CloneRoleAssignment(ExchangeRole newRole, ExchangeRoleAssignment oldRoleAssignment)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string, string, string>(20008L, "-->CloneRoleAssignment: newRole.Name = {0}, oldRoleAssignment.Name = {1}, oldRoleAssignment.User.Name = {2}", newRole.Name, oldRoleAssignment.Name, oldRoleAssignment.User.Name);
			ExchangeRoleAssignment exchangeRoleAssignment = new ExchangeRoleAssignment();
			exchangeRoleAssignment.ProvisionalClone(oldRoleAssignment);
			exchangeRoleAssignment.SetExchangeVersion(oldRoleAssignment.ExchangeVersion);
			exchangeRoleAssignment.Role = newRole.Id;
			this.settings.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			string text = RoleAssignmentHelper.GenerateUniqueRoleAssignmentName(this.settings.ConfigurationSession, this.settings.OrgContainerId, newRole.Name, oldRoleAssignment.User.Name, oldRoleAssignment.RoleAssignmentDelegationType, this.settings.WriteVerbose);
			exchangeRoleAssignment.SetId(this.settings.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer).GetChildId(text));
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20008L, "----CloneRoleAssignment: newRoleAssignmentName = {0}", text);
			this.settings.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(exchangeRoleAssignment, this.settings.ConfigurationSession, typeof(ExchangeRoleAssignment)));
			this.settings.ConfigurationSession.Save(exchangeRoleAssignment);
			this.settings.LogWriteObject(exchangeRoleAssignment);
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20008L, "<--CloneRoleAssignment");
		}

		private const string CustomRoleNameFormat = "auto_{0}_{1}";
	}
}
