using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	internal static class RoleHelper
	{
		internal static void LoadScopesByAssignmentsToNewCache(IDictionary<ADObjectId, ManagementScope> cache, IEnumerable<ExchangeRoleAssignment> assignments, IConfigurationSession scSession)
		{
			if (cache == null)
			{
				throw new ArgumentNullException("cache");
			}
			if (assignments == null)
			{
				throw new ArgumentNullException("assignments");
			}
			if (scSession == null)
			{
				throw new ArgumentNullException("scSession");
			}
			List<ADObjectId> list = new List<ADObjectId>();
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in assignments)
			{
				if (exchangeRoleAssignment.CustomRecipientWriteScope != null && !cache.ContainsKey(exchangeRoleAssignment.CustomRecipientWriteScope) && !list.Contains(exchangeRoleAssignment.CustomRecipientWriteScope))
				{
					list.Add(exchangeRoleAssignment.CustomRecipientWriteScope);
				}
				if (exchangeRoleAssignment.CustomConfigWriteScope != null && !cache.ContainsKey(exchangeRoleAssignment.CustomConfigWriteScope) && !list.Contains(exchangeRoleAssignment.CustomConfigWriteScope))
				{
					list.Add(exchangeRoleAssignment.CustomConfigWriteScope);
				}
			}
			if (list.Count != 0)
			{
				ADObjectId[] array = list.ToArray();
				Result<ManagementScope>[] array2 = scSession.ReadMultiple<ManagementScope>(array);
				for (int i = 0; i < array2.Length; i++)
				{
					Result<ManagementScope> result = array2[i];
					if (result.Data == null)
					{
						ExTraceGlobals.ADConfigTracer.TraceError<ADObjectId>(0L, "Management scope '{0}' was not found.", array[i]);
					}
					else
					{
						ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId>(0L, "Read Management scope '{0}].", result.Data.Id);
						cache[array[i]] = result.Data;
					}
				}
			}
		}

		internal static void ValidateRoleAssignmentUser(ADRecipient user, Task.TaskErrorLoggingDelegate writeError, bool isGetTask)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			bool flag = false;
			RecipientTypeDetails recipientTypeDetails = user.RecipientTypeDetails;
			if (recipientTypeDetails <= RecipientTypeDetails.UniversalSecurityGroup)
			{
				if (recipientTypeDetails <= RecipientTypeDetails.EquipmentMailbox)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.LegacyMailbox)
					{
						if (recipientTypeDetails <= RecipientTypeDetails.SharedMailbox)
						{
							if (recipientTypeDetails < RecipientTypeDetails.UserMailbox)
							{
								goto IL_142;
							}
							switch ((int)(recipientTypeDetails - RecipientTypeDetails.UserMailbox))
							{
							case 0:
							case 1:
							case 3:
								goto IL_13C;
							case 2:
								goto IL_142;
							}
						}
						if (recipientTypeDetails != RecipientTypeDetails.LegacyMailbox)
						{
							goto IL_142;
						}
					}
					else
					{
						if (recipientTypeDetails != RecipientTypeDetails.RoomMailbox && recipientTypeDetails != RecipientTypeDetails.EquipmentMailbox)
						{
							goto IL_142;
						}
						flag = isGetTask;
						goto IL_142;
					}
				}
				else if (recipientTypeDetails <= RecipientTypeDetails.MailUniversalSecurityGroup)
				{
					if (recipientTypeDetails != RecipientTypeDetails.MailUser && recipientTypeDetails != RecipientTypeDetails.MailUniversalSecurityGroup)
					{
						goto IL_142;
					}
				}
				else if (recipientTypeDetails != RecipientTypeDetails.User && recipientTypeDetails != RecipientTypeDetails.UniversalSecurityGroup)
				{
					goto IL_142;
				}
			}
			else if (recipientTypeDetails <= (RecipientTypeDetails)((ulong)-2147483648))
			{
				if (recipientTypeDetails <= RecipientTypeDetails.LinkedUser)
				{
					if (recipientTypeDetails != RecipientTypeDetails.DisabledUser && recipientTypeDetails != RecipientTypeDetails.LinkedUser)
					{
						goto IL_142;
					}
				}
				else if (recipientTypeDetails != RecipientTypeDetails.RoleGroup && recipientTypeDetails != (RecipientTypeDetails)((ulong)-2147483648))
				{
					goto IL_142;
				}
			}
			else if (recipientTypeDetails <= RecipientTypeDetails.RemoteSharedMailbox)
			{
				if (recipientTypeDetails != RecipientTypeDetails.Computer && recipientTypeDetails != RecipientTypeDetails.RemoteSharedMailbox)
				{
					goto IL_142;
				}
			}
			else if (recipientTypeDetails != RecipientTypeDetails.TeamMailbox && recipientTypeDetails != RecipientTypeDetails.RemoteTeamMailbox)
			{
				goto IL_142;
			}
			IL_13C:
			flag = true;
			IL_142:
			if (!flag)
			{
				writeError(new ArgumentException(Strings.ErrorWrongRoleAssignmentUserType(user.Id.ToString(), user.RecipientTypeDetails.ToString())), ErrorCategory.InvalidArgument, null);
			}
		}

		internal static bool DoesRoleMatchingNameAndParameters(ExchangeRole role, char typeHint, string name, string[] parameters)
		{
			if (role == null)
			{
				throw new ArgumentNullException("role");
			}
			ManagementRoleEntryType type;
			if (typeHint == 'c')
			{
				type = ManagementRoleEntryType.Cmdlet;
			}
			else if (typeHint == 's')
			{
				type = ManagementRoleEntryType.Script;
			}
			else
			{
				if (typeHint != 'a')
				{
					throw new ArgumentOutOfRangeException("typeHint");
				}
				type = ManagementRoleEntryType.ApplicationPermission;
			}
			foreach (RoleEntry roleEntry in role.RoleEntries)
			{
				if (RBACHelper.DoesRoleEntryMatchNameAndParameters(roleEntry, type, name, parameters, null))
				{
					return true;
				}
			}
			return false;
		}

		private static List<RoleEntry> GetRoleEntriesMatchingName(ExchangeRole role, ManagementRoleEntryType type, string name, string snapinName, int maxResult)
		{
			if (role == null)
			{
				throw new ArgumentNullException("role");
			}
			List<RoleEntry> list = new List<RoleEntry>();
			foreach (RoleEntry roleEntry in role.RoleEntries)
			{
				if (RBACHelper.DoesRoleEntryMatchNameAndParameters(roleEntry, type, name, null, snapinName))
				{
					list.Add(roleEntry);
					if (list.Count >= maxResult)
					{
						break;
					}
				}
			}
			return list;
		}

		internal static RoleEntry GetRoleEntry(ExchangeRole role, string cmdlet, string snapinName, ManagementRoleEntryType type, Task.TaskErrorLoggingDelegate writeError)
		{
			if (role == null)
			{
				throw new ArgumentNullException("role");
			}
			if (string.IsNullOrEmpty(cmdlet))
			{
				throw new ArgumentNullException("cmdlet");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			List<RoleEntry> roleEntriesMatchingName = RoleHelper.GetRoleEntriesMatchingName(role, type, cmdlet, snapinName, 2);
			if (1 < roleEntriesMatchingName.Count)
			{
				writeError(new ManagementObjectAmbiguousException(Strings.ErrorRoleEntryNotUnique(role.Id.ToString(), cmdlet)), ErrorCategory.InvalidArgument, role.Id);
			}
			if (0 >= roleEntriesMatchingName.Count)
			{
				return null;
			}
			return roleEntriesMatchingName[0];
		}

		internal static RoleEntry GetMandatoryRoleEntry(ExchangeRole role, string cmdletOrScriptName, string snapinName, Task.TaskErrorLoggingDelegate writeError)
		{
			RoleEntry roleEntry = RoleHelper.GetRoleEntry(role, cmdletOrScriptName, snapinName, ManagementRoleEntryType.All, writeError);
			if (null == roleEntry)
			{
				writeError(new ManagementObjectNotFoundException(Strings.ErrorRoleEntryNotFound(role.Id.ToString(), cmdletOrScriptName)), ErrorCategory.InvalidArgument, null);
			}
			return roleEntry;
		}

		internal static void ValidateChangeAgainstParentAndChildren(ExchangeRole role, ExchangeRole parentRole, IEnumerable<ExchangeRole> childRoles, Task.TaskErrorLoggingDelegate writeError)
		{
			if (role == null)
			{
				throw new ArgumentNullException("role");
			}
			if (parentRole == null)
			{
				throw new ArgumentNullException("parentRole");
			}
			if (childRoles == null)
			{
				throw new ArgumentNullException("childRoles");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			foreach (RoleEntry roleEntry in role.RoleEntries.Added)
			{
				bool flag = false;
				MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
				foreach (string item in roleEntry.Parameters)
				{
					multiValuedProperty.Add(item);
				}
				foreach (RoleEntry roleEntry2 in parentRole.RoleEntries)
				{
					if (RoleEntry.CompareRoleEntriesByName(roleEntry2, roleEntry) == 0)
					{
						flag = true;
						foreach (string item2 in roleEntry2.Parameters)
						{
							multiValuedProperty.Remove(item2);
							if (multiValuedProperty.Count == 0)
							{
								break;
							}
						}
					}
				}
				if (!flag)
				{
					writeError(new InvalidOperationException(Strings.ErrorRoleEntryNotExistOnParent(role.Id.ToString(), roleEntry.ToString())), ErrorCategory.InvalidOperation, role.Id);
				}
				else if (0 < multiValuedProperty.Count)
				{
					writeError(new InvalidOperationException(Strings.ErrorRoleEntryParametersNotExistOnParent(role.Id.ToString(), roleEntry.ToString(), string.Join(",", multiValuedProperty.ToArray()))), ErrorCategory.InvalidOperation, role.Id);
				}
			}
			Dictionary<string, RoleHelper.RemovedRoleEntry> dictionary = new Dictionary<string, RoleHelper.RemovedRoleEntry>();
			foreach (RoleEntry roleEntry3 in role.RoleEntries.Removed)
			{
				bool flag2 = true;
				MultiValuedProperty<string> multiValuedProperty2 = new MultiValuedProperty<string>();
				foreach (string item3 in roleEntry3.Parameters)
				{
					multiValuedProperty2.Add(item3);
				}
				foreach (RoleEntry roleEntry4 in role.RoleEntries.Added)
				{
					if (RoleEntry.CompareRoleEntriesByName(roleEntry3, roleEntry4) == 0)
					{
						flag2 = false;
						foreach (string item4 in roleEntry4.Parameters)
						{
							multiValuedProperty2.Remove(item4);
							if (multiValuedProperty2.Count == 0)
							{
								break;
							}
						}
					}
				}
				if (flag2 || multiValuedProperty2.Count > 0)
				{
					dictionary[roleEntry3.Name] = new RoleHelper.RemovedRoleEntry(roleEntry3, flag2, multiValuedProperty2);
				}
			}
			foreach (ExchangeRole exchangeRole in childRoles)
			{
				foreach (RoleEntry roleEntry5 in exchangeRole.RoleEntries)
				{
					if (dictionary.ContainsKey(roleEntry5.Name))
					{
						RoleHelper.RemovedRoleEntry removedRoleEntry = dictionary[roleEntry5.Name];
						if (RoleEntry.CompareRoleEntriesByName(roleEntry5, removedRoleEntry.RoleEntry) == 0)
						{
							if (removedRoleEntry.IsRemoved)
							{
								writeError(new InvalidOperationException(Strings.ErrorRoleEntryExistOnChildren(role.Id.ToString(), removedRoleEntry.RoleEntry.ToString())), ErrorCategory.InvalidOperation, role.Id);
							}
							foreach (string text in removedRoleEntry.RemovedParameters)
							{
								if (roleEntry5.ContainsParameter(text))
								{
									writeError(new InvalidOperationException(Strings.ErrorRoleEntryParameterExistOnChildren(text, removedRoleEntry.RoleEntry.Name, role.Id.ToString(), exchangeRole.Id.ToString())), ErrorCategory.InvalidOperation, role.Id);
								}
							}
						}
					}
				}
			}
		}

		internal static bool HasDelegatingHierarchicalRoleAssignmentWithoutScopeRestriction(OrganizationId executingUserOrganizationId, IEnumerable<ExchangeRoleAssignment> roleAssignments, ADObjectId idRole)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction(0L, "-->HasDelegatingHierarchicalRoleAssignmentWithoutScopeRestriction");
			if (RoleHelper.HasFullRbacDelegationPermission(executingUserOrganizationId, idRole))
			{
				return true;
			}
			if (roleAssignments != null)
			{
				foreach (ExchangeRoleAssignment exchangeRoleAssignment in roleAssignments)
				{
					if (exchangeRoleAssignment.RoleAssignmentDelegationType == RoleAssignmentDelegationType.DelegatingOrgWide && exchangeRoleAssignment.Enabled && idRole.IsDescendantOf(exchangeRoleAssignment.Role))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal static bool HasFullRbacDelegationPermission(OrganizationId executingUserOrganizationId, ADObjectId idRole)
		{
			if (ADObjectId.IsNullOrEmpty(idRole))
			{
				throw new ArgumentNullException("idRole");
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20000L, "-->HasFullRbacDelegationPermission");
			if (OrganizationId.ForestWideOrgId.Equals(executingUserOrganizationId))
			{
				ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				if (!idRole.IsDescendantOf(rootOrgContainerIdForLocalForest))
				{
					ExTraceGlobals.AccessCheckTracer.TraceFunction(20000L, "<--HasFullRbacDelegationPermission: return true for enterprise administrator working against hosted org role");
					return true;
				}
				ExTraceGlobals.AccessCheckTracer.TraceFunction(20000L, "<--HasFullRbacDelegationPermission: return false for enterprise administrator working against enterprise role");
			}
			else
			{
				if (!idRole.IsDescendantOf(executingUserOrganizationId.ConfigurationUnit) && idRole.IsDescendantOf(executingUserOrganizationId.ConfigurationUnit.Parent))
				{
					ExTraceGlobals.AccessCheckTracer.TraceFunction(20000L, "<--HasFullRbacDelegationPermission: return true for hosted administrator working against child org's role");
					return true;
				}
				ExTraceGlobals.AccessCheckTracer.TraceFunction(20000L, "<--HasFullRbacDelegationPermission: return false for hosted administrator working against his org's role");
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20000L, "<--HasFullRbacDelegationPermission: return false");
			return false;
		}

		internal static void ApplyDefaultScopeForTheRoleAssignment(OrganizationId executingUserOrganizationId, IEnumerable<ExchangeRoleAssignment> existingRoleAssignmentsForAssigner, ExchangeRoleAssignment newRoleAssignment, IDictionary<ADObjectId, ManagementScope> scopeCache, Task.TaskErrorLoggingDelegate writeError, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			if (newRoleAssignment == null)
			{
				throw new ArgumentNullException("newRoleAssignment");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (writeVerbose == null)
			{
				throw new ArgumentNullException("writeVerbose");
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20001L, "-->ApplyDefaultScopeForTheRoleAssignment");
			if (!RoleHelper.HasFullRbacDelegationPermission(executingUserOrganizationId, newRoleAssignment.Role))
			{
				writeVerbose(Strings.VerboseGenerateDefaultScopeByExistingAssignments(newRoleAssignment.Id.ToString()));
				if (existingRoleAssignmentsForAssigner != null)
				{
					ExTraceGlobals.AccessCheckTracer.TraceDebug(20001L, "ApplyDefaultScopeForTheRoleAssignment: Finding the closest management role in the hierarchy, which has role assignment");
					ADObjectId adobjectId = null;
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in existingRoleAssignmentsForAssigner)
					{
						if ((exchangeRoleAssignment.RoleAssignmentDelegationType != RoleAssignmentDelegationType.Delegating && exchangeRoleAssignment.RoleAssignmentDelegationType != RoleAssignmentDelegationType.DelegatingOrgWide) || !exchangeRoleAssignment.Enabled)
						{
							writeVerbose(Strings.VerboseIgnoringAssignment(exchangeRoleAssignment.Id.ToString()));
						}
						else if (!newRoleAssignment.Role.IsDescendantOf(exchangeRoleAssignment.Role))
						{
							writeVerbose(Strings.VerboseIgnoringAssignment(exchangeRoleAssignment.Id.ToString()));
						}
						else if (adobjectId == null || exchangeRoleAssignment.Role.IsDescendantOf(adobjectId))
						{
							adobjectId = exchangeRoleAssignment.Role;
							if (adobjectId.Equals(newRoleAssignment.Role))
							{
								break;
							}
						}
					}
					if (adobjectId != null)
					{
						ExTraceGlobals.AccessCheckTracer.TraceDebug<ADObjectId>(20001L, "ApplyDefaultScopeForTheRoleAssignment: the nearest role which has role assignment is '{0}'", adobjectId);
						List<ExchangeRoleAssignment> list = new List<ExchangeRoleAssignment>();
						foreach (ExchangeRoleAssignment exchangeRoleAssignment2 in existingRoleAssignmentsForAssigner)
						{
							if ((exchangeRoleAssignment2.RoleAssignmentDelegationType == RoleAssignmentDelegationType.Delegating || exchangeRoleAssignment2.RoleAssignmentDelegationType == RoleAssignmentDelegationType.DelegatingOrgWide) && exchangeRoleAssignment2.Enabled && adobjectId.Equals(exchangeRoleAssignment2.Role))
							{
								writeVerbose(Strings.VerboseUsingAssignmentAsTemplate(exchangeRoleAssignment2.Id.ToString()));
								list.Add(exchangeRoleAssignment2);
								ExTraceGlobals.AccessCheckTracer.TraceDebug<ADObjectId, int>(20001L, "ApplyDefaultScopeForTheRoleAssignment: role assignment '{0}' will be taken as template to clone scope. Count = {1}", exchangeRoleAssignment2.Id, list.Count);
							}
						}
						RoleHelper.CalculateDefaultWriteScope(newRoleAssignment, list, scopeCache, writeError);
						ExTraceGlobals.AccessCheckTracer.TraceFunction(20001L, "<--ApplyDefaultScopeForTheRoleAssignment: completed");
						return;
					}
				}
				else
				{
					ExTraceGlobals.AccessCheckTracer.TraceFunction(20001L, "<--ApplyDefaultScopeForTheRoleAssignment: no role assignment for current user");
				}
				writeError(new InvalidOperationException(Strings.ErrorNewRemoveRoleAssignmentNeedHierarchicalRoleAssignment(newRoleAssignment.Id.ToString(), newRoleAssignment.Role.ToString())), ErrorCategory.InvalidOperation, newRoleAssignment.Id);
				return;
			}
			if (newRoleAssignment.RoleAssignmentDelegationType == RoleAssignmentDelegationType.Delegating && newRoleAssignment.CustomRecipientWriteScope == null && (newRoleAssignment.RecipientWriteScope == RecipientWriteScopeType.NotApplicable || newRoleAssignment.RecipientWriteScope == RecipientWriteScopeType.Organization) && (newRoleAssignment.ConfigWriteScope == ConfigWriteScopeType.NotApplicable || newRoleAssignment.ConfigWriteScope == ConfigWriteScopeType.OrganizationConfig) && newRoleAssignment.CustomConfigWriteScope == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug(20001L, "ApplyDefaultScopeForTheRoleAssignment: Executing user is a Datacenter admin and the assignment is delegating without restrictions - apply DelegatingOrgWide scope");
				newRoleAssignment.RoleAssignmentDelegationType = RoleAssignmentDelegationType.DelegatingOrgWide;
				return;
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug(20001L, "ApplyDefaultScopeForTheRoleAssignment: Executing user is a Datacenter admin and the assignment is not delegating or has restrictions - do not apply DelegatingOrgWide scope");
		}

		private static void CalculateDefaultWriteScope(ExchangeRoleAssignment newRoleAssignment, List<ExchangeRoleAssignment> existingDelegatingRoleAssignmentsForAssigner, IDictionary<ADObjectId, ManagementScope> scopeCache, Task.TaskErrorLoggingDelegate writeError)
		{
			if (newRoleAssignment == null)
			{
				throw new ArgumentNullException("newRoleAssignment");
			}
			if (existingDelegatingRoleAssignmentsForAssigner == null)
			{
				throw new ArgumentNullException("existingDelegatingRoleAssignmentsForAssigner");
			}
			if (existingDelegatingRoleAssignmentsForAssigner.Count == 0)
			{
				throw new ArgumentOutOfRangeException("existingDelegatingRoleAssignmentsForAssigner");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<ADObjectId, int, int>(20002L, "-->CalculateDefaultWriteScope: newRoleAssignment = {0}, existingDelegatingRoleAssignmentsForAssigner Count = {1} (scopeCache Count = {2})", newRoleAssignment.Id, existingDelegatingRoleAssignmentsForAssigner.Count, (scopeCache == null) ? 0 : scopeCache.Count);
			LocalizedString value;
			ExchangeRoleAssignment exchangeRoleAssignment = ExchangeRoleAssignment.FindOneWithMaximumWriteScope(existingDelegatingRoleAssignmentsForAssigner, scopeCache, true, out value);
			if (exchangeRoleAssignment == null)
			{
				writeError(new InvalidOperationException(Strings.ErrorCannotCalculateDefaultScope(newRoleAssignment.Id.ToString(), value)), ErrorCategory.InvalidOperation, newRoleAssignment.Id);
			}
			newRoleAssignment.RecipientWriteScope = exchangeRoleAssignment.RecipientWriteScope;
			newRoleAssignment.CustomRecipientWriteScope = exchangeRoleAssignment.CustomRecipientWriteScope;
			ExchangeRoleAssignment exchangeRoleAssignment2 = ExchangeRoleAssignment.FindOneWithMaximumWriteScope(existingDelegatingRoleAssignmentsForAssigner, scopeCache, false, out value);
			if (exchangeRoleAssignment2 == null)
			{
				writeError(new InvalidOperationException(Strings.ErrorCannotCalculateDefaultScope(newRoleAssignment.Id.ToString(), value)), ErrorCategory.InvalidOperation, newRoleAssignment.Id);
			}
			newRoleAssignment.ConfigWriteScope = exchangeRoleAssignment2.ConfigWriteScope;
			newRoleAssignment.CustomConfigWriteScope = exchangeRoleAssignment2.CustomConfigWriteScope;
			if (exchangeRoleAssignment2.RoleAssignmentDelegationType == RoleAssignmentDelegationType.DelegatingOrgWide && exchangeRoleAssignment.RoleAssignmentDelegationType == RoleAssignmentDelegationType.DelegatingOrgWide && newRoleAssignment.RoleAssignmentDelegationType == RoleAssignmentDelegationType.Delegating)
			{
				newRoleAssignment.RoleAssignmentDelegationType = RoleAssignmentDelegationType.DelegatingOrgWide;
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20002L, "<--CalculateDefaultWriteScope: completed");
		}

		internal static bool HasDelegatingHierarchicalRoleAssignment(OrganizationId executingUserOrganizationId, IEnumerable<ExchangeRoleAssignment> existingRoleAssignmentsForAssigner, IDictionary<ADObjectId, ManagementScope> scopesCache, ExchangeRoleAssignment affectedRoleAssignment, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			if (affectedRoleAssignment == null)
			{
				throw new ArgumentNullException("affectedRoleAssignment");
			}
			if (writeVerbose == null)
			{
				throw new ArgumentNullException("writeVerbose");
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(20002L, "-->HasDelegatingHierarchicalRoleAssignment");
			bool flag;
			if (!(flag = RoleHelper.HasFullRbacDelegationPermission(executingUserOrganizationId, affectedRoleAssignment.Role)))
			{
				writeVerbose(Strings.VerboseValidateScopeAgainstExistingAssignments(affectedRoleAssignment.Id.ToString()));
				if (existingRoleAssignmentsForAssigner != null)
				{
					bool flag2 = false;
					bool flag3 = false;
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in existingRoleAssignmentsForAssigner)
					{
						if ((exchangeRoleAssignment.RoleAssignmentDelegationType != RoleAssignmentDelegationType.Delegating && exchangeRoleAssignment.RoleAssignmentDelegationType != RoleAssignmentDelegationType.DelegatingOrgWide) || !exchangeRoleAssignment.Enabled)
						{
							writeVerbose(Strings.VerboseIgnoringAssignment(exchangeRoleAssignment.Id.ToString()));
						}
						else if (!affectedRoleAssignment.Role.IsDescendantOf(exchangeRoleAssignment.Role))
						{
							writeVerbose(Strings.VerboseIgnoringAssignment(exchangeRoleAssignment.Id.ToString()));
						}
						else
						{
							ExTraceGlobals.AccessCheckTracer.TraceDebug<ADObjectId>(20002L, "ApplyDefaultScopeForTheRoleAssignment: Found role assignment '{0}' for a role in the hierarchy", exchangeRoleAssignment.Id);
							if (!flag2)
							{
								writeVerbose(Strings.VerboseCheckingRecipientWriteScope(exchangeRoleAssignment.Id.ToString()));
								LocalizedString value;
								if (affectedRoleAssignment.IsRecipientWriteScopeSmallerOrEqualThan(exchangeRoleAssignment, scopesCache, out value))
								{
									writeVerbose(Strings.VerboseCheckingScopePassed);
									ExTraceGlobals.AccessCheckTracer.TraceDebug<ADObjectId, ADObjectId>(20002L, "ApplyDefaultScopeForTheRoleAssignment: affected role assignment '{0}' has smaller or same recipient write scope as assignment '{1}'", affectedRoleAssignment.Id, exchangeRoleAssignment.Id);
									flag2 = true;
									if (flag3)
									{
										break;
									}
								}
								else
								{
									writeVerbose(Strings.VerboseCheckingScopeFailed(value));
								}
							}
							if (!flag3)
							{
								writeVerbose(Strings.VerboseCheckingConfigWriteScope(exchangeRoleAssignment.Id.ToString()));
								LocalizedString value;
								if (affectedRoleAssignment.IsConfigWriteScopeSmallerOrEqualThan(exchangeRoleAssignment, scopesCache, out value))
								{
									writeVerbose(Strings.VerboseCheckingScopePassed);
									ExTraceGlobals.AccessCheckTracer.TraceDebug<ADObjectId, ADObjectId>(20002L, "ApplyDefaultScopeForTheRoleAssignment: affected role assignment '{0}' has smaller or same config write scope as assignment '{1}'", affectedRoleAssignment.Id, exchangeRoleAssignment.Id);
									flag3 = true;
									if (flag2)
									{
										break;
									}
								}
								else
								{
									writeVerbose(Strings.VerboseCheckingScopeFailed(value));
								}
							}
						}
					}
					flag = (flag3 && flag2);
				}
				else
				{
					ExTraceGlobals.AccessCheckTracer.TraceDebug(20002L, "<--ApplyDefaultScopeForTheRoleAssignment: no role assignment for current user");
				}
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(20002L, "<--HasDelegatingHierarchicalRoleAssignment: return {0}", flag);
			return flag;
		}

		internal static void HierarchicalCheckForRoleAssignmentCreation(Task task, ExchangeRoleAssignment roleAssignment, ManagementScope recipientScope, ManagementScope configScope, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskErrorLoggingDelegate writeError)
		{
			IDictionary<ADObjectId, ManagementScope> dictionary = task.ExchangeRunspaceConfig.ScopesCache;
			if (recipientScope != null || configScope != null)
			{
				IDictionary<ADObjectId, ManagementScope> dictionary2 = dictionary;
				if (recipientScope != null && !dictionary2.ContainsKey(recipientScope.Id))
				{
					if (object.ReferenceEquals(dictionary2, dictionary))
					{
						dictionary2 = new Dictionary<ADObjectId, ManagementScope>(dictionary2);
					}
					dictionary2.Add(recipientScope.Id, recipientScope);
				}
				if (configScope != null && !dictionary2.ContainsKey(configScope.Id))
				{
					if (object.ReferenceEquals(dictionary2, dictionary))
					{
						dictionary2 = new Dictionary<ADObjectId, ManagementScope>(dictionary2);
					}
					dictionary2.Add(configScope.Id, configScope);
				}
				dictionary = dictionary2;
			}
			if (!RoleHelper.HasDelegatingHierarchicalRoleAssignment(task.ExecutingUserOrganizationId, task.ExchangeRunspaceConfig.RoleAssignments, dictionary, roleAssignment, writeVerbose))
			{
				writeError(new InvalidOperationException(Strings.ErrorNewRemoveRoleAssignmentNeedHierarchicalRoleAssignment(roleAssignment.Id.ToString(), roleAssignment.Role.ToString())), ErrorCategory.InvalidOperation, roleAssignment.Id);
			}
		}

		internal static void PrepareNewRoleAssignmentWithUniqueNameAndDefaultScopes(string name, ExchangeRoleAssignment roleAssignment, ExchangeRole role, ADObjectId roleAssigneeId, OrganizationId roleAssigneeOrganizationId, RoleAssigneeType roleAssigneeType, RoleAssignmentDelegationType delegationType, IConfigurationSession configurationSession, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskErrorLoggingDelegate writeError)
		{
			roleAssignment.Role = role.Id;
			roleAssignment.RoleAssignmentDelegationType = delegationType;
			roleAssignment.RecipientReadScope = role.ImplicitRecipientReadScope;
			roleAssignment.ConfigReadScope = role.ImplicitConfigReadScope;
			roleAssignment.RecipientWriteScope = (RecipientWriteScopeType)role.ImplicitRecipientWriteScope;
			roleAssignment.ConfigWriteScope = (ConfigWriteScopeType)role.ImplicitConfigWriteScope;
			roleAssignment.Enabled = true;
			OrganizationId organizationId = OrganizationId.ForestWideOrgId;
			if (configurationSession is ITenantConfigurationSession)
			{
				organizationId = TaskHelper.ResolveOrganizationId(role.Id, ExchangeRole.RdnContainer, (ITenantConfigurationSession)ADSession.RescopeSessionToTenantSubTree(configurationSession));
			}
			ADObjectId adobjectId;
			if (OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				adobjectId = configurationSession.GetOrgContainerId();
			}
			else
			{
				adobjectId = organizationId.ConfigurationUnit;
			}
			if (string.IsNullOrEmpty(name))
			{
				configurationSession.SessionSettings.IsSharedConfigChecked = true;
				name = RoleAssignmentHelper.GenerateUniqueRoleAssignmentName(configurationSession, adobjectId, role.Name, roleAssigneeId.Name, delegationType, writeVerbose);
			}
			roleAssignment.SetId(adobjectId.GetDescendantId(ExchangeRoleAssignment.RdnContainer).GetChildId(name));
			roleAssignment.OrganizationId = roleAssigneeOrganizationId;
			roleAssignment.RoleAssigneeType = roleAssigneeType;
			roleAssignment.User = roleAssigneeId;
			if (!roleAssigneeOrganizationId.Equals(OrganizationId.ForestWideOrgId) && !roleAssigneeOrganizationId.Equals(organizationId) && (OrganizationId.ForestWideOrgId.Equals(organizationId) || !organizationId.OrganizationalUnit.IsDescendantOf(roleAssigneeOrganizationId.OrganizationalUnit)))
			{
				writeError(new InvalidOperationException(Strings.ErrorOrgUserBeAssignedToParentOrg(roleAssigneeId.ToString())), ErrorCategory.InvalidOperation, roleAssignment.Id);
			}
		}

		internal static void AnalyzeAndStampCustomizedWriteScopes(Task task, ExchangeRoleAssignment roleAssignment, ExchangeRole role, IConfigurationSession configurationSession, DataAccessHelper.GetDataObjectDelegate getOuDataObject, DataAccessHelper.GetDataObjectDelegate getManagementScopeDataObject, ref bool skipHRoleCheck, ref ExchangeOrganizationalUnit ou, ref ManagementScope customRecipientScope, ref ManagementScope customConfigScope)
		{
			bool flag = true;
			RecipientWriteScopeType recipientWriteScope = roleAssignment.RecipientWriteScope;
			if (task.Fields.IsModified(RbacCommonParameters.ParameterRecipientRelativeWriteScope))
			{
				roleAssignment.RecipientWriteScope = (RecipientWriteScopeType)task.Fields[RbacCommonParameters.ParameterRecipientRelativeWriteScope];
				roleAssignment.CustomRecipientWriteScope = null;
			}
			else if (task.Fields.IsModified(RbacCommonParameters.ParameterRecipientOrganizationalUnitScope))
			{
				roleAssignment.RecipientWriteScope = RecipientWriteScopeType.OU;
				if (ou == null)
				{
					bool useConfigNC = configurationSession.UseConfigNC;
					bool useGlobalCatalog = configurationSession.UseGlobalCatalog;
					try
					{
						OrganizationalUnitIdParameter organizationalUnitIdParameter = (OrganizationalUnitIdParameter)task.Fields[RbacCommonParameters.ParameterRecipientOrganizationalUnitScope];
						configurationSession.UseConfigNC = false;
						configurationSession.UseGlobalCatalog = true;
						ou = (ExchangeOrganizationalUnit)getOuDataObject(organizationalUnitIdParameter, configurationSession, roleAssignment.OrganizationalUnitRoot, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(organizationalUnitIdParameter.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(organizationalUnitIdParameter.ToString())));
					}
					finally
					{
						configurationSession.UseConfigNC = useConfigNC;
						configurationSession.UseGlobalCatalog = useGlobalCatalog;
					}
				}
				roleAssignment.CustomRecipientWriteScope = ou.Id;
			}
			else if (task.Fields.IsModified(RbacCommonParameters.ParameterCustomRecipientWriteScope))
			{
				if (customRecipientScope == null)
				{
					customRecipientScope = RoleHelper.GetAndValidateDomainScope((ManagementScopeIdParameter)task.Fields[RbacCommonParameters.ParameterCustomRecipientWriteScope], configurationSession, getManagementScopeDataObject, roleAssignment, new Task.TaskErrorLoggingDelegate(task.WriteError));
					if (customRecipientScope.Exclusive)
					{
						task.WriteError(new ArgumentException(Strings.ErrorScopeExclusive(customRecipientScope.Id.ToString(), RbacCommonParameters.ParameterCustomRecipientWriteScope)), ErrorCategory.InvalidArgument, null);
					}
				}
				roleAssignment.CustomRecipientWriteScope = customRecipientScope.Id;
				roleAssignment.RecipientWriteScope = RecipientWriteScopeType.CustomRecipientScope;
			}
			else if (task.Fields.IsModified("ExclusiveRecipientWriteScope"))
			{
				if (customRecipientScope == null)
				{
					customRecipientScope = RoleHelper.GetAndValidateDomainScope((ManagementScopeIdParameter)task.Fields["ExclusiveRecipientWriteScope"], configurationSession, getManagementScopeDataObject, roleAssignment, new Task.TaskErrorLoggingDelegate(task.WriteError));
					if (!customRecipientScope.Exclusive)
					{
						task.WriteError(new ArgumentException(Strings.ErrorScopeNotExclusive(customRecipientScope.Id.ToString(), "ExclusiveRecipientWriteScope")), ErrorCategory.InvalidArgument, null);
					}
				}
				roleAssignment.CustomRecipientWriteScope = customRecipientScope.Id;
				roleAssignment.RecipientWriteScope = RecipientWriteScopeType.ExclusiveRecipientScope;
			}
			else
			{
				flag = false;
			}
			if (flag && role.ImplicitRecipientWriteScope != (ScopeType)roleAssignment.RecipientWriteScope && !RbacScope.IsScopeTypeSmaller((ScopeType)roleAssignment.RecipientWriteScope, role.ImplicitRecipientWriteScope))
			{
				task.WriteWarning(Strings.WriteScopeGreaterThanRoleScope((roleAssignment.CustomRecipientWriteScope == null) ? roleAssignment.RecipientWriteScope.ToString() : roleAssignment.CustomRecipientWriteScope.ToString(), role.Name, role.ImplicitRecipientWriteScope.ToString()));
				roleAssignment.CustomRecipientWriteScope = null;
				roleAssignment.RecipientWriteScope = recipientWriteScope;
				flag = false;
			}
			bool flag2 = true;
			ConfigWriteScopeType configWriteScope = roleAssignment.ConfigWriteScope;
			if (task.Fields.IsModified(RbacCommonParameters.ParameterCustomConfigWriteScope))
			{
				if (customConfigScope == null)
				{
					customConfigScope = RoleHelper.GetAndValidateConfigScope((ManagementScopeIdParameter)task.Fields[RbacCommonParameters.ParameterCustomConfigWriteScope], configurationSession, getManagementScopeDataObject, roleAssignment, new Task.TaskErrorLoggingDelegate(task.WriteError));
					if (customConfigScope.Exclusive)
					{
						task.WriteError(new ArgumentException(Strings.ErrorScopeExclusive(customConfigScope.Id.ToString(), RbacCommonParameters.ParameterCustomConfigWriteScope)), ErrorCategory.InvalidArgument, null);
					}
				}
				roleAssignment.CustomConfigWriteScope = customConfigScope.Id;
				roleAssignment.ConfigWriteScope = ((customConfigScope.ScopeRestrictionType == ScopeRestrictionType.PartnerDelegatedTenantScope) ? ConfigWriteScopeType.PartnerDelegatedTenantScope : ConfigWriteScopeType.CustomConfigScope);
			}
			else if (task.Fields.IsModified("ExclusiveConfigWriteScope"))
			{
				if (customConfigScope == null)
				{
					customConfigScope = RoleHelper.GetAndValidateConfigScope((ManagementScopeIdParameter)task.Fields["ExclusiveConfigWriteScope"], configurationSession, getManagementScopeDataObject, roleAssignment, new Task.TaskErrorLoggingDelegate(task.WriteError));
					if (!customConfigScope.Exclusive)
					{
						task.WriteError(new ArgumentException(Strings.ErrorScopeNotExclusive(customConfigScope.Id.ToString(), "ExclusiveConfigWriteScope")), ErrorCategory.InvalidArgument, null);
					}
				}
				roleAssignment.CustomConfigWriteScope = customConfigScope.Id;
				roleAssignment.ConfigWriteScope = ConfigWriteScopeType.ExclusiveConfigScope;
			}
			else
			{
				flag2 = false;
			}
			if (flag2 && role.ImplicitConfigWriteScope != (ScopeType)roleAssignment.ConfigWriteScope && !RbacScope.IsScopeTypeSmaller((ScopeType)roleAssignment.ConfigWriteScope, role.ImplicitConfigWriteScope))
			{
				task.WriteWarning(Strings.WriteScopeGreaterThanRoleScope((roleAssignment.CustomConfigWriteScope == null) ? roleAssignment.ConfigWriteScope.ToString() : roleAssignment.CustomConfigWriteScope.ToString(), role.Name, role.ImplicitConfigWriteScope.ToString()));
				roleAssignment.CustomConfigWriteScope = null;
				roleAssignment.ConfigWriteScope = configWriteScope;
				flag2 = false;
			}
			bool flag3 = flag || flag2;
			if (task.ExchangeRunspaceConfig != null)
			{
				if (!flag3)
				{
					RoleHelper.ApplyDefaultScopeForTheRoleAssignment(task.ExecutingUserOrganizationId, task.ExchangeRunspaceConfig.RoleAssignments, roleAssignment, task.ExchangeRunspaceConfig.ScopesCache, new Task.TaskErrorLoggingDelegate(task.WriteError), new Task.TaskVerboseLoggingDelegate(task.WriteVerbose));
					skipHRoleCheck = true;
					return;
				}
			}
			else if (!flag3 && roleAssignment.RoleAssignmentDelegationType == RoleAssignmentDelegationType.Delegating)
			{
				roleAssignment.RoleAssignmentDelegationType = RoleAssignmentDelegationType.DelegatingOrgWide;
			}
		}

		internal static ManagementScope GetAndValidateDomainScope(ManagementScopeIdParameter scopeId, IConfigurationSession configurationSession, DataAccessHelper.GetDataObjectDelegate getDataObject, ExchangeRoleAssignment assignment, Task.TaskErrorLoggingDelegate writeError)
		{
			ManagementScope managementScope = null;
			if (scopeId != null)
			{
				managementScope = (ManagementScope)getDataObject(scopeId, configurationSession, assignment.OrganizationId.ConfigurationUnit, null, new LocalizedString?(Strings.ErrorScopeNotFound(scopeId.ToString())), new LocalizedString?(Strings.ErrorScopeNotUnique(scopeId.ToString())));
				RoleHelper.ValidateManagementScope(managementScope, assignment.OrganizationId, true, configurationSession, writeError);
			}
			return managementScope;
		}

		internal static ManagementScope GetAndValidateConfigScope(ManagementScopeIdParameter scopeId, IConfigurationSession configurationSession, DataAccessHelper.GetDataObjectDelegate getDataObject, ExchangeRoleAssignment assignment, Task.TaskErrorLoggingDelegate writeError)
		{
			ManagementScope managementScope = null;
			if (scopeId != null)
			{
				if (!assignment.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					writeError(new InvalidOperationException(Strings.ErrorInvalidConfigScopeOnNonHosterRoleAssignment(assignment.Id.ToString())), ErrorCategory.InvalidOperation, assignment.Id);
				}
				managementScope = (ManagementScope)getDataObject(scopeId, configurationSession, assignment.OrganizationId.ConfigurationUnit, null, new LocalizedString?(Strings.ErrorScopeNotFound(scopeId.ToString())), new LocalizedString?(Strings.ErrorScopeNotUnique(scopeId.ToString())));
				RoleHelper.ValidateManagementScope(managementScope, assignment.OrganizationId, false, configurationSession, writeError);
			}
			return managementScope;
		}

		internal static void ValidateManagementScope(ManagementScope scope, OrganizationId orgId, bool isDomainWriteRestriction, IConfigurationSession scSession, Task.TaskErrorLoggingDelegate writeError)
		{
			if (scope == null)
			{
				throw new ArgumentNullException("scope");
			}
			if (null == orgId)
			{
				throw new ArgumentNullException("orgId");
			}
			if (scSession == null)
			{
				throw new ArgumentNullException("scSession");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			ADObjectId adobjectId;
			if (OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				adobjectId = scSession.GetOrgContainerId();
			}
			else
			{
				adobjectId = orgId.ConfigurationUnit;
			}
			if (!scope.Id.IsDescendantOf(adobjectId))
			{
				writeError(new InvalidOperationException(Strings.ErrorScopeOutOfRoleScope(scope.Id.ToString(), adobjectId.Name)), ErrorCategory.InvalidOperation, null);
			}
			switch (scope.ScopeRestrictionType)
			{
			case ScopeRestrictionType.RecipientScope:
				if (!isDomainWriteRestriction)
				{
					writeError(new ArgumentException(Strings.ErrorUnsupportedConfigScopeType(scope.Id.ToString(), scope.ScopeRestrictionType.ToString())), ErrorCategory.InvalidArgument, null);
					return;
				}
				break;
			case ScopeRestrictionType.ServerScope:
			case ScopeRestrictionType.PartnerDelegatedTenantScope:
				if (isDomainWriteRestriction)
				{
					writeError(new ArgumentException(Strings.ErrorUnsupportedRecipientScopeType(scope.Id.ToString(), scope.ScopeRestrictionType.ToString())), ErrorCategory.InvalidArgument, null);
				}
				break;
			default:
				return;
			}
		}

		internal static void ValidateAssignmentMethod(RoleAssignmentIdParameter assignmentId, ADObjectId user, ADObjectId role, ADObjectId realAssignmentPrincipal, RoleHelper.ErrorRoleAssignmentDelegate strErrorGroupRoleAssignment, RoleHelper.ErrorRoleAssignmentDelegate strErrorMailboxPlanRoleAssignment, RoleHelper.ErrorRoleAssignmentDelegate strErrorPolicyRoleAssignment, Task.TaskErrorLoggingDelegate writeError)
		{
			AssignmentMethod assignmentMethod = assignmentId.AssignmentMethod;
			switch (assignmentMethod)
			{
			case AssignmentMethod.Direct:
				return;
			case AssignmentMethod.SecurityGroup:
				writeError(new InvalidOperationException(strErrorGroupRoleAssignment(user.ToString(), role.ToString(), realAssignmentPrincipal.ToString())), ErrorCategory.InvalidOperation, assignmentId);
				return;
			case AssignmentMethod.Direct | AssignmentMethod.SecurityGroup:
				break;
			case AssignmentMethod.RoleAssignmentPolicy:
				writeError(new InvalidOperationException(strErrorPolicyRoleAssignment(user.ToString(), role.ToString(), realAssignmentPrincipal.ToString())), ErrorCategory.InvalidOperation, assignmentId);
				return;
			default:
				if (assignmentMethod == AssignmentMethod.MailboxPlan)
				{
					writeError(new InvalidOperationException(strErrorMailboxPlanRoleAssignment(user.ToString(), role.ToString(), realAssignmentPrincipal.ToString())), ErrorCategory.InvalidOperation, assignmentId);
					return;
				}
				break;
			}
			writeError(new InvalidOperationException(Strings.ErrorUnsupportedAssignmentMethod(assignmentId.AssignmentMethod.ToString())), ErrorCategory.InvalidOperation, assignmentId);
		}

		internal static void HierarchyRoleAssignmentChecking(ExchangeRoleAssignment roleAssignment, ExchangeRunspaceConfiguration exchangeRunspaceConfig, IConfigurationSession configurationSession, OrganizationId executingUserOrganizationId, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, bool disableOrRemove)
		{
			if (roleAssignment.Role == null || roleAssignment.User == null)
			{
				return;
			}
			if (exchangeRunspaceConfig != null)
			{
				bool flag = false;
				ICollection<ExchangeRoleAssignment> collection = null;
				if (exchangeRunspaceConfig.RoleAssignments != null)
				{
					collection = new List<ExchangeRoleAssignment>(exchangeRunspaceConfig.RoleAssignments.Count);
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in exchangeRunspaceConfig.RoleAssignments)
					{
						if (exchangeRoleAssignment.Id.Equals(roleAssignment.Id))
						{
							flag = (exchangeRoleAssignment.RoleAssignmentDelegationType == RoleAssignmentDelegationType.Delegating || exchangeRoleAssignment.RoleAssignmentDelegationType == RoleAssignmentDelegationType.DelegatingOrgWide);
						}
						else
						{
							collection.Add(exchangeRoleAssignment);
						}
					}
				}
				Dictionary<ADObjectId, ManagementScope> dictionary = new Dictionary<ADObjectId, ManagementScope>(exchangeRunspaceConfig.ScopesCache);
				RoleHelper.LoadScopesByAssignmentsToNewCache(dictionary, new ExchangeRoleAssignment[]
				{
					roleAssignment
				}, configurationSession);
				if (!RoleHelper.HasDelegatingHierarchicalRoleAssignment(executingUserOrganizationId, collection, dictionary, roleAssignment, writeVerbose))
				{
					if (flag)
					{
						if (disableOrRemove)
						{
							writeWarning(Strings.WarningRemoveRoleAssignmentToBlockSelf(roleAssignment.Id.ToString(), roleAssignment.Role.ToString()));
							return;
						}
					}
					else
					{
						writeError(new InvalidOperationException(Strings.ErrorNewRemoveRoleAssignmentNeedHierarchicalRoleAssignment(roleAssignment.Id.ToString(), roleAssignment.Role.ToString())), ErrorCategory.InvalidOperation, roleAssignment.Id);
					}
				}
			}
		}

		internal static void CreateRoleAssignment(ExchangeRole role, ADObjectId assigneeId, OrganizationId assigneeOrganizationId, RoleAssigneeType roleAssigneeType, string originatingServer, RoleAssignmentDelegationType delegationType, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, IConfigurationSession configurationSession, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskErrorLoggingDelegate writeError)
		{
			if (role == null)
			{
				throw new ArgumentNullException("role");
			}
			if (assigneeId == null)
			{
				throw new ArgumentNullException("assigneeId");
			}
			ExchangeRoleAssignment exchangeRoleAssignment = new ExchangeRoleAssignment();
			RoleHelper.PrepareNewRoleAssignmentWithUniqueNameAndDefaultScopes(null, exchangeRoleAssignment, role, assigneeId, assigneeOrganizationId, roleAssigneeType, delegationType, configurationSession, writeVerbose, writeError);
			writeVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(exchangeRoleAssignment, configurationSession, typeof(ExchangeRoleAssignment)));
			if (!string.IsNullOrEmpty(originatingServer))
			{
				configurationSession.LinkResolutionServer = originatingServer;
			}
			configurationSession.Save(exchangeRoleAssignment);
		}

		internal static bool IsScopeSpecified(PropertyBag fields)
		{
			return fields.IsModified(RbacCommonParameters.ParameterRecipientRelativeWriteScope) || fields.IsModified(RbacCommonParameters.ParameterRecipientOrganizationalUnitScope) || fields.IsModified(RbacCommonParameters.ParameterCustomRecipientWriteScope) || fields.IsModified(RbacCommonParameters.ParameterCustomConfigWriteScope) || fields.IsModified("ExclusiveRecipientWriteScope") || fields.IsModified("ExclusiveConfigWriteScope");
		}

		internal static void VerifyNoScopeForUnScopedRole(PropertyBag fields, ExchangeRole role, Task.TaskErrorLoggingDelegate writeError)
		{
			bool flag = RoleHelper.IsScopeSpecified(fields);
			if (role.IsUnscoped && flag)
			{
				writeError(new InvalidOperationException(Strings.ScopeIsNotAllowedForRole(RoleType.UnScoped.ToString())), ErrorCategory.InvalidArgument, null);
			}
		}

		private struct RemovedRoleEntry
		{
			internal RemovedRoleEntry(RoleEntry roleEntry, bool isRemoved, MultiValuedProperty<string> removedParameters)
			{
				this.RoleEntry = roleEntry;
				this.IsRemoved = isRemoved;
				this.RemovedParameters = removedParameters;
			}

			internal RoleEntry RoleEntry;

			internal bool IsRemoved;

			internal MultiValuedProperty<string> RemovedParameters;
		}

		internal delegate LocalizedString ErrorRoleAssignmentDelegate(string user, string role, string assignee);
	}
}
