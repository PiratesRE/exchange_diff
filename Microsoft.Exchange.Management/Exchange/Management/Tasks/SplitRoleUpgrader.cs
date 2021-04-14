using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class SplitRoleUpgrader : DeprecatedRoleUpgrader
	{
		protected SplitRoleUpgrader(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping) : base(settings, roleNameMapping)
		{
		}

		public new static bool CanUpgrade(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping, RoleDefinition roleDefinition, out RoleUpgrader roleUpgrader)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20009L, "-->SplitRoleUpgrader.CanUpgrade: roleName = {0}", roleDefinition.RoleName);
			bool flag = false;
			roleUpgrader = null;
			if (!roleDefinition.IsEndUserRole)
			{
				flag = true;
			}
			flag = (flag && roleNameMapping != null && roleNameMapping.IsSplitting);
			if (flag)
			{
				roleUpgrader = new SplitRoleUpgrader(settings, roleNameMapping);
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(20009L, "<--SplitRoleUpgrader.CanUpgrade: canUpgrade = {0}", flag);
			return flag;
		}

		protected override void CreateOrUpdateRoles(RoleNameMapping mapping, List<RoleDefinition> roleDefinitions, List<string> enabledPermissionFeatures, string suffix, string mailboxPlanIndex)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<int>(20009L, "-->CreateOrUpdateRoles: roleDefinitions count = {0}", roleDefinitions.Count);
			RoleDefinition item = roleDefinitions.First((RoleDefinition x) => x.RoleName.Equals(mapping.OldName));
			roleDefinitions.Remove(item);
			List<ExchangeRole> list = new List<ExchangeRole>(roleDefinitions.Count);
			foreach (RoleDefinition definition in roleDefinitions)
			{
				ExchangeRole exchangeRole = base.CreateOrUpdateRole(null, definition, enabledPermissionFeatures, suffix, mailboxPlanIndex);
				if (exchangeRole != null)
				{
					list.Add(exchangeRole);
				}
			}
			ExchangeRole exchangeRole2 = item.GenerateRole(enabledPermissionFeatures, this.settings.RolesContainerId, suffix, mailboxPlanIndex);
			string unescapedCommonName = (suffix == null) ? mapping.OldName : (mapping.OldName + suffix);
			ExchangeRole exchangeRole3 = this.settings.ConfigurationSession.Read<ExchangeRole>(this.settings.RolesContainerId.GetChildId(unescapedCommonName));
			if (exchangeRole3 == null)
			{
				if (exchangeRole2.RoleEntries.Count > 0)
				{
					exchangeRole2.OrganizationId = this.settings.OrganizationId;
					base.SaveRoleAndSuggestCleanupOnFailure(exchangeRole2);
					ExTraceGlobals.AccessCheckTracer.TraceFunction(20009L, "<--CreateOrUpdateRole: New Role created");
				}
				return;
			}
			this.settings.LogReadObject(exchangeRole3);
			this.UpdateCannedRole(exchangeRole3, exchangeRole2, list);
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20009L, "<--CreateOrUpdateRoles");
		}

		protected void UpdateCannedRole(ExchangeRole existingRole, ExchangeRole cannedRole, List<ExchangeRole> newRoles)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20009L, "-->UpdateCannedRole: existingRole = {0}, existingRoleVersion = {1}, existingRole-RoleType = {2}, cannedRole = {3}, cannedRoleVersion = {4}, cannedRole-RoleType = {5}", new object[]
			{
				existingRole.Name,
				existingRole.ExchangeVersion.ToString(),
				existingRole.RoleType.ToString(),
				cannedRole.Name,
				cannedRole.ExchangeVersion.ToString(),
				cannedRole.RoleType.ToString()
			});
			bool flag = existingRole.ExchangeVersion == ExchangeRoleSchema.Exchange2009_R3;
			RoleEntry[] array = base.PrepareRoleForUpgradeAndGetOldSortedEntries(existingRole, false);
			if (existingRole.IsModified(ADObjectSchema.ExchangeVersion))
			{
				existingRole.RoleType = cannedRole.RoleType;
			}
			foreach (ExchangeRole newRole in newRoles)
			{
				base.CloneRoleAssignments(existingRole, newRole);
			}
			existingRole.RoleEntries = (flag ? cannedRole.RoleEntries : base.CalculateUpdatedRoleEntries(cannedRole, array));
			int num = 0;
			this.FindAndMatchDerivedRoles(existingRole, array, newRoles, ref num);
			existingRole.OrganizationId = this.settings.OrganizationId;
			base.SaveRoleAndWarnOnFailure(existingRole);
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20009L, "<---UpdateCannedRole");
		}

		protected void FindAndMatchDerivedRoles(ExchangeRole parentRole, RoleEntry[] oldParentRoleEntries, List<ExchangeRole> newRoles, ref int recursionCount)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string, int, int>(20009L, "--->FindAndMatchDerivedRoles: parentRole.Name = {0}, newRoles.Count = {1}, recursionCount = {2}", parentRole.Name, newRoles.Count, recursionCount);
			if (++recursionCount >= 1000)
			{
				return;
			}
			ADPagedReader<ExchangeRole> adpagedReader = this.settings.ConfigurationSession.FindPaged<ExchangeRole>(parentRole.Id, QueryScope.OneLevel, null, null, 0);
			foreach (ExchangeRole exchangeRole in adpagedReader)
			{
				ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20009L, "----MatchDerivedRoles: childRole = {0}", exchangeRole.Name);
				this.settings.LogReadObject(exchangeRole);
				RoleEntry[] array = base.PrepareRoleForUpgradeAndGetOldSortedEntries(exchangeRole, false);
				exchangeRole.RoleState = parentRole.RoleState;
				exchangeRole.RoleType = parentRole.RoleType;
				List<RoleEntry> listOfRoleEntriesForChildRole = base.GetListOfRoleEntriesForChildRole(oldParentRoleEntries, array, parentRole.RoleEntries.ToArray<RoleEntry>(), exchangeRole.IsChanged(ADObjectSchema.ExchangeVersion));
				exchangeRole.RoleEntries = new MultiValuedProperty<RoleEntry>(listOfRoleEntriesForChildRole);
				List<ExchangeRole> list = new List<ExchangeRole>();
				foreach (ExchangeRole exchangeRole2 in newRoles)
				{
					RoleEntry[] sortedRoleEntries = base.GetSortedRoleEntries(exchangeRole2);
					listOfRoleEntriesForChildRole = base.GetListOfRoleEntriesForChildRole(oldParentRoleEntries, array, sortedRoleEntries, exchangeRole.IsChanged(ADObjectSchema.ExchangeVersion));
					if (base.RoleEntriesMatch(sortedRoleEntries, listOfRoleEntriesForChildRole.ToArray()))
					{
						ExTraceGlobals.AccessCheckTracer.TraceFunction<string, string>(20009L, "----FindAndMatchDerivedRoles: Perfect role match. splittingRole = {0}, newRole = {1}", exchangeRole.Name, exchangeRole2.Name);
						base.CloneRoleAssignments(exchangeRole, exchangeRole2);
						list.Add(exchangeRole2);
					}
					else
					{
						ExTraceGlobals.AccessCheckTracer.TraceFunction<string, ExchangeRole>(20009L, "----FindAndMatchDerivedRoles: Unable to match with role. splittingRole = {0}, newRole = {1}", exchangeRole.Name, exchangeRole2);
						if (listOfRoleEntriesForChildRole.Count > 0)
						{
							ExchangeRole exchangeRole3 = base.CreateCustomizedDerivedRole(exchangeRole2, listOfRoleEntriesForChildRole, exchangeRole.Name);
							base.CloneRoleAssignments(exchangeRole, exchangeRole3);
							list.Add(exchangeRole3);
							this.settings.WriteWarning(Strings.WarningCustomRoleCreatedDuringUpgradeForSplittingRole(exchangeRole3.Name, exchangeRole.Name, parentRole.Name));
						}
					}
				}
				this.FindAndMatchDerivedRoles(exchangeRole, array, list, ref recursionCount);
				base.SaveDerivedRoleAndWarnOnValidationErrors(exchangeRole);
			}
			recursionCount--;
			ExTraceGlobals.AccessCheckTracer.TraceFunction<int>(20009L, "<---FindAndMatchDerivedRoles: recursionCount = {0}", recursionCount);
		}
	}
}
