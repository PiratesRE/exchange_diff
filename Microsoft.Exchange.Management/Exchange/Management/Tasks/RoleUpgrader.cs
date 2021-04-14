using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Management.Tasks
{
	internal abstract class RoleUpgrader
	{
		protected RoleUpgrader(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping)
		{
			this.settings = settings;
			this.roleNameMapping = roleNameMapping;
		}

		public abstract void UpdateRole(RoleDefinition definition);

		public abstract void UpdateRoles(List<RoleDefinition> rolesDefinitions);

		protected ExchangeRole CreateOrUpdateRole(RoleNameMapping mapping, RoleDefinition definition, List<string> enabledPermissionFeatures)
		{
			return this.CreateOrUpdateRole(mapping, definition, enabledPermissionFeatures, null, null);
		}

		protected ExchangeRole CreateOrUpdateRole(RoleNameMapping mapping, RoleDefinition definition, List<string> enabledPermissionFeatures, string suffix, string mailboxPlanIndex)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "-->CreateOrUpdateRole: RoleDefinition = {0}, enabledPermissionFeatures is Null = {1}, suffix = {2}, mailboxPlanIndex = {3}", new object[]
			{
				definition.RoleName,
				enabledPermissionFeatures == null,
				string.IsNullOrEmpty(suffix) ? string.Empty : suffix,
				string.IsNullOrEmpty(mailboxPlanIndex) ? string.Empty : mailboxPlanIndex
			});
			this.RenameExistingRole(mapping, suffix);
			ExchangeRole exchangeRole = definition.GenerateRole(enabledPermissionFeatures, this.settings.RolesContainerId, suffix, mailboxPlanIndex);
			if (exchangeRole.RoleEntries.Count > 0)
			{
				ExchangeRole exchangeRole2 = this.settings.ConfigurationSession.Read<ExchangeRole>(exchangeRole.Id);
				if (exchangeRole2 != null)
				{
					this.settings.LogReadObject(exchangeRole2);
					this.UpdateCannedRole(exchangeRole2, exchangeRole, definition);
					exchangeRole = exchangeRole2;
					ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "<--CreateOrUpdateRole: Role Updated");
				}
				else
				{
					exchangeRole.OrganizationId = this.settings.OrganizationId;
					this.SaveRoleAndSuggestCleanupOnFailure(exchangeRole);
					ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "<--CreateOrUpdateRole: New Role created");
					this.CreateDCSafeRoleIfNeeded(exchangeRole, definition);
				}
			}
			else
			{
				ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "<--CreateOrUpdateRole: No Role created");
				this.settings.RemoveRoleAndAssignments(exchangeRole.Id);
				exchangeRole = null;
			}
			return exchangeRole;
		}

		protected void RenameExistingRole(RoleNameMapping mapping, string suffix)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(20005L, "-->RenameExistingRole: Mapping Is Null = {0}", mapping == null);
			if (mapping == null)
			{
				return;
			}
			string text = string.IsNullOrEmpty(suffix) ? mapping.OldName : (mapping.OldName + suffix);
			ExchangeRole exchangeRole = this.settings.ConfigurationSession.Read<ExchangeRole>(this.settings.RolesContainerId.GetChildId(text));
			if (exchangeRole != null)
			{
				this.settings.LogReadObject(exchangeRole);
				string text2 = string.IsNullOrEmpty(suffix) ? mapping.NewName : (mapping.NewName + suffix);
				exchangeRole.SetId(this.settings.RolesContainerId.GetChildId(text2));
				this.SaveRoleAndSuggestCleanupOnFailure(exchangeRole);
				ExTraceGlobals.AccessCheckTracer.TraceFunction<string, string>(20005L, "<--RenameExistingRole: Role Renamed. oldName = {0}, NewName = {1}", text, text2);
			}
		}

		protected void UpdateCannedRole(ExchangeRole existingRole, ExchangeRole cannedRole, RoleDefinition roleDefinition)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "-->UpdateCannedRole: existingRole = {0}, existingRoleVersion = {1}, existingRole-RoleType = {2}, cannedRole = {3}, cannedRoleVersion = {4}, cannedRole-RoleType = {5}", new object[]
			{
				existingRole.Name,
				existingRole.ExchangeVersion.ToString(),
				existingRole.RoleType.ToString(),
				cannedRole.Name,
				cannedRole.ExchangeVersion.ToString(),
				cannedRole.RoleType.ToString()
			});
			bool flag = existingRole.ExchangeVersion == ExchangeRoleSchema.Exchange2009_R3;
			RoleEntry[] array = this.PrepareRoleForUpgradeAndGetOldSortedEntries(existingRole, false);
			existingRole.RoleEntries = (flag ? cannedRole.RoleEntries : this.CalculateUpdatedRoleEntries(cannedRole, array));
			int num = 0;
			this.FindAndUpdateDerivedRoles(existingRole, array, roleDefinition, ref num);
			existingRole.OrganizationId = this.settings.OrganizationId;
			this.SaveRoleAndSuggestCleanupOnFailure(existingRole);
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "<---UpdateCannedRole");
		}

		protected MultiValuedProperty<RoleEntry> CalculateUpdatedRoleEntries(ExchangeRole cannedRole, RoleEntry[] sortedOldRoleEntries)
		{
			List<RoleEntry> list = new List<RoleEntry>();
			RoleEntry[] array = cannedRole.RoleEntries.ToArray();
			Array.Sort<RoleEntry>(array, RoleEntryComparer.Instance);
			int num = 0;
			int num2 = 0;
			for (;;)
			{
				RoleEntry roleEntry = (num2 < sortedOldRoleEntries.Length) ? sortedOldRoleEntries[num2] : null;
				RoleEntry roleEntry2 = (num < array.Length) ? array[num] : null;
				RoleUpgrader.EntryComparisonState entryComparisonState = this.AnalyzeEntries(roleEntry, roleEntry2);
				switch (entryComparisonState)
				{
				case RoleUpgrader.EntryComparisonState.NoEntriesFound:
					goto IL_102;
				case RoleUpgrader.EntryComparisonState.OnlyExistsInOld:
				{
					RoleEntry roleEntry3 = roleEntry.FindAndRemoveMatchingParameters(this.settings.AvailableRoleEntries);
					if (!(roleEntry3 == null))
					{
						list.Add(roleEntry3);
					}
					num2++;
					goto IL_102;
				}
				case RoleUpgrader.EntryComparisonState.OnlyExistsInNew:
					list.Add(roleEntry2);
					num++;
					goto IL_102;
				case RoleUpgrader.EntryComparisonState.ExistsInBoth:
				{
					RoleEntry item = roleEntry2;
					RoleEntry roleEntry4 = roleEntry.FindAndRemoveMatchingParameters(this.settings.AvailableRoleEntries);
					if (roleEntry4 != null)
					{
						item = RoleEntry.MergeParameters(new RoleEntry[]
						{
							roleEntry2,
							roleEntry4
						});
					}
					list.Add(item);
					num2++;
					num++;
					goto IL_102;
				}
				}
				break;
				IL_102:
				if (entryComparisonState == RoleUpgrader.EntryComparisonState.NoEntriesFound)
				{
					goto Block_6;
				}
			}
			throw new ArgumentOutOfRangeException("entryComparisonState");
			Block_6:
			return new MultiValuedProperty<RoleEntry>(list);
		}

		private RoleUpgrader.EntryComparisonState AnalyzeEntries(RoleEntry curOld, RoleEntry curNew)
		{
			RoleUpgrader.EntryComparisonState entryComparisonState = RoleUpgrader.EntryComparisonState.NoEntriesFound;
			if (curOld != null)
			{
				entryComparisonState |= RoleUpgrader.EntryComparisonState.OnlyExistsInOld;
			}
			if (curNew != null)
			{
				entryComparisonState |= RoleUpgrader.EntryComparisonState.OnlyExistsInNew;
			}
			if (entryComparisonState == RoleUpgrader.EntryComparisonState.ExistsInBoth)
			{
				int num = RoleEntry.NameComparer.Compare(curOld, curNew);
				if (num < 0)
				{
					entryComparisonState = RoleUpgrader.EntryComparisonState.OnlyExistsInOld;
				}
				else if (num > 0)
				{
					entryComparisonState = RoleUpgrader.EntryComparisonState.OnlyExistsInNew;
				}
			}
			return entryComparisonState;
		}

		protected void FindAndUpdateDerivedRoles(ExchangeRole updatedParentRole, RoleEntry[] oldParentRoleEntries, RoleDefinition roleDefinition, ref int recursionCount)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "-->FindAndUpdateDerivedRoles: updatedParentRole.Name = {0}, updatedParentRole.RoleEntries.Count = {1}, oldParentRoleEntries.Length = {2}, recursionCount = {3}", new object[]
			{
				updatedParentRole.Name,
				updatedParentRole.RoleEntries.Count,
				oldParentRoleEntries.Length,
				recursionCount
			});
			if (++recursionCount >= 1000)
			{
				return;
			}
			bool flag = false;
			bool flag2 = this.settings.Organization == null && (Datacenter.IsMicrosoftHostedOnly(false) || Datacenter.IsDatacenterDedicated(false)) && roleDefinition.ContainsProhibitedActions(InstallCannedRbacRoles.DCProhibitedActions);
			ADPagedReader<ExchangeRole> adpagedReader = this.settings.ConfigurationSession.FindPaged<ExchangeRole>(updatedParentRole.Id, QueryScope.OneLevel, null, null, 0);
			foreach (ExchangeRole exchangeRole in adpagedReader)
			{
				this.settings.LogReadObject(exchangeRole);
				RoleEntry[] array = this.PrepareRoleForUpgradeAndGetOldSortedEntries(exchangeRole, false);
				List<RoleEntry> value;
				if (1 == recursionCount && flag2 && exchangeRole.Name.Equals(RoleDefinition.GetDCSafeNameForRole(updatedParentRole.Name), StringComparison.OrdinalIgnoreCase))
				{
					value = roleDefinition.GetRoleEntriesFilteringProhibitedActions(null, InstallCannedRbacRoles.DCProhibitedActions);
					flag = true;
				}
				else
				{
					value = this.GetListOfRoleEntriesForChildRole(oldParentRoleEntries, array, updatedParentRole.RoleEntries.ToArray(), exchangeRole.IsChanged(ADObjectSchema.ExchangeVersion));
				}
				exchangeRole.RoleEntries = new MultiValuedProperty<RoleEntry>(value);
				this.FindAndUpdateDerivedRoles(exchangeRole, array, roleDefinition, ref recursionCount);
				this.SaveDerivedRoleAndWarnOnValidationErrors(exchangeRole);
				ExTraceGlobals.AccessCheckTracer.TraceFunction<string, int>(20005L, "----FindAndUpdateDerivedRoles: role.Name = {0}, role.RoleEntries.Count = {1}", exchangeRole.Name, exchangeRole.RoleEntries.Count);
			}
			if (1 == recursionCount && !flag)
			{
				this.CreateDCSafeRoleIfNeeded(updatedParentRole, roleDefinition);
			}
			recursionCount--;
			ExTraceGlobals.AccessCheckTracer.TraceFunction<int>(20005L, "<--FindAndUpdateDerivedRoles: recursionCount = {0}", recursionCount);
		}

		protected List<RoleEntry> GetListOfRoleEntriesForChildRole(RoleEntry[] oldParentRoleEntries, RoleEntry[] oldChildRoleEntries, RoleEntry[] updatedParentRoleEntries, bool versionBumped)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "-->GetListOfRoleEntriesForChildRole: oldParentRoleEntries.Length = {0}, oldChildRoleEntries.Length = {1}, updatedParentRoleEntries.Length = {2}, versionBumped = {3}", new object[]
			{
				oldParentRoleEntries.Length,
				oldChildRoleEntries.Length,
				updatedParentRoleEntries.Length,
				versionBumped
			});
			List<RoleEntry> list = new List<RoleEntry>(updatedParentRoleEntries.Length);
			foreach (RoleEntry roleEntry in updatedParentRoleEntries)
			{
				RoleEntry value = versionBumped ? roleEntry.MapToPreviousVersion() : roleEntry;
				int num = Array.BinarySearch<RoleEntry>(oldChildRoleEntries, value, RoleEntryComparer.Instance);
				if (num >= 0)
				{
					List<string> list2 = new List<string>();
					foreach (string text in roleEntry.Parameters)
					{
						string parameterToCheck = versionBumped ? roleEntry.MapParameterToPreviousVersion(text) : text;
						if (oldChildRoleEntries[num].ContainsParameter(parameterToCheck))
						{
							list2.Add(text);
						}
						else
						{
							int num2 = Array.BinarySearch<RoleEntry>(oldParentRoleEntries, value, RoleEntryComparer.Instance);
							if (!oldParentRoleEntries[num2].ContainsParameter(parameterToCheck))
							{
								list2.Add(text);
							}
						}
					}
					list.Add(roleEntry.Clone(list2));
				}
				else
				{
					int num3 = Array.BinarySearch<RoleEntry>(oldParentRoleEntries, value, RoleEntryComparer.Instance);
					if (num3 < 0)
					{
						list.Add(roleEntry);
					}
				}
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<int>(20005L, "<--GetListOfRoleEntriesForChildRole: updatedRoleEntries.Count = {0}", list.Count);
			return list;
		}

		protected RoleEntry[] GetSortedRoleEntries(ExchangeRole role)
		{
			RoleEntry[] array;
			if (role.ExchangeVersion == ExchangeRoleSchema.Exchange2009_R3)
			{
				array = ((MultiValuedProperty<RoleEntry>)role[ExchangeRoleSchema.InternalDownlevelRoleEntries]).ToArray();
			}
			else
			{
				array = role.RoleEntries.ToArray();
			}
			Array.Sort<RoleEntry>(array, RoleEntryComparer.Instance);
			return array;
		}

		protected RoleEntry[] PrepareRoleForUpgradeAndGetOldSortedEntries(ExchangeRole roleToUpgrade, bool isDeprecated)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20005L, "-->PrepareRoleForUpgradeAndGetOldSortedEntries: roleToUpgrade = {0}", roleToUpgrade.Name);
			RoleEntry[] sortedRoleEntries = this.GetSortedRoleEntries(roleToUpgrade);
			roleToUpgrade[ExchangeRoleSchema.RoleFlags] = 0;
			roleToUpgrade.StampImplicitScopes();
			roleToUpgrade.StampIsEndUserRole();
			if (isDeprecated)
			{
				roleToUpgrade.RoleState = RoleState.Deprecated;
			}
			if (roleToUpgrade.ExchangeVersion.IsOlderThan(ExchangeRoleSchema.CurrentRoleVersion) && roleToUpgrade.RoleState != RoleState.Deprecated)
			{
				roleToUpgrade.SetExchangeVersion(ExchangeRoleSchema.CurrentRoleVersion);
				ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20005L, "----PrepareRoleForUpgradeAndGetOldSortedEntries: Version bumped to '{0}'.", roleToUpgrade.ExchangeVersion.ToString());
			}
			if (roleToUpgrade.ExchangeVersion == ExchangeRoleSchema.Exchange2009_R3)
			{
				roleToUpgrade.RoleEntries = null;
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20005L, "<--PrepareRoleForUpgradeAndGetOldSortedEntries");
			return sortedRoleEntries;
		}

		protected void SaveRoleAndSuggestCleanupOnFailure(ExchangeRole role)
		{
			Exception ex = null;
			try
			{
				this.settings.ConfigurationSession.Save(role);
				this.settings.LogWriteObject(role);
				this.settings.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(role, this.settings.ConfigurationSession, typeof(ExchangeRole)));
			}
			catch (DataSourceOperationException ex2)
			{
				ex = ex2;
			}
			catch (DataValidationException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				CorruptedRoleNeedsCleanupException ex4 = new CorruptedRoleNeedsCleanupException(role.Identity.ToString(), ex.Message, ex);
				if (!this.settings.Task.Stopping)
				{
					TaskLogger.SendWatsonReport(ex4);
				}
				this.settings.WriteError(ex4, ErrorCategory.InvalidOperation, null);
			}
		}

		protected void SaveDerivedRoleAndWarnOnValidationErrors(ExchangeRole role)
		{
			if (role.IsRootRole)
			{
				throw new ArgumentNullException("Only derive roles should be");
			}
			try
			{
				int count = role.RoleEntries.Count;
				ValidationError[] array = role.Validate();
				role.AllowEmptyRole = true;
				this.settings.ConfigurationSession.Save(role);
				this.settings.LogWriteObject(role);
				this.settings.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(role, this.settings.ConfigurationSession, typeof(ExchangeRole)));
				if (array != null && array.Length != 0)
				{
					this.settings.WriteWarning(Strings.WarningInvalidRoleAfterUpgrade(role.Identity.ToString(), MultiValuedPropertyBase.FormatMultiValuedProperty(array)));
				}
			}
			catch (DataSourceOperationException ex)
			{
				this.settings.WriteWarning(Strings.WarningCannotUpgradeRole(role.Identity.ToString(), ex.Message));
			}
			catch (DataValidationException ex2)
			{
				this.settings.WriteWarning(Strings.WarningCannotUpgradeRole(role.Identity.ToString(), ex2.Message));
			}
		}

		protected void SaveRoleAndWarnOnFailure(ExchangeRole role)
		{
			try
			{
				this.settings.ConfigurationSession.Save(role);
				this.settings.LogWriteObject(role);
				this.settings.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(role, this.settings.ConfigurationSession, typeof(ExchangeRole)));
			}
			catch (DataSourceOperationException ex)
			{
				this.settings.WriteWarning(Strings.WarningCannotUpgradeRole(role.Identity.ToString(), ex.Message));
			}
			catch (DataValidationException ex2)
			{
				this.settings.WriteWarning(Strings.WarningCannotUpgradeRole(role.Identity.ToString(), ex2.Message));
			}
		}

		protected void CreateDCSafeRoleIfNeeded(ExchangeRole cannedRole, RoleDefinition roleDefinition)
		{
			if (this.settings.Organization != null || !Datacenter.IsMicrosoftHostedOnly(false))
			{
				return;
			}
			if (!roleDefinition.ContainsProhibitedActions(InstallCannedRbacRoles.DCProhibitedActions))
			{
				return;
			}
			ExchangeRole exchangeRole = roleDefinition.GenerateRole(null, cannedRole.Id, null, null);
			exchangeRole.Name = RoleDefinition.GetDCSafeNameForRole(cannedRole.Name);
			exchangeRole.RoleEntries = new MultiValuedProperty<RoleEntry>(roleDefinition.GetRoleEntriesFilteringProhibitedActions(null, InstallCannedRbacRoles.DCProhibitedActions));
			exchangeRole.OrganizationId = this.settings.OrganizationId;
			if (exchangeRole.RoleEntries.Count != 0)
			{
				this.SaveRoleAndWarnOnFailure(exchangeRole);
			}
		}

		protected RoleUpgradeConfigurationSettings settings;

		protected RoleNameMapping roleNameMapping;

		private enum EntryComparisonState
		{
			NoEntriesFound,
			OnlyExistsInOld,
			OnlyExistsInNew,
			ExistsInBoth
		}
	}
}
