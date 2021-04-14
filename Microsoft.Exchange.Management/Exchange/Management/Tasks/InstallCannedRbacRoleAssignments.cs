using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "CannedRbacRoleAssignments", DefaultParameterSetName = "Default")]
	public sealed class InstallCannedRbacRoleAssignments : InstallCannedRbacObjectsTaskBase
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Organization")]
		public ServicePlan PreviousServicePlanSettings
		{
			get
			{
				return (ServicePlan)base.Fields["PreviousServicePlanSettings"];
			}
			set
			{
				base.Fields["PreviousServicePlanSettings"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsFfo
		{
			get
			{
				return (SwitchParameter)base.Fields["IsFfo"];
			}
			set
			{
				base.Fields["IsFfo"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.adSplitPermissionMode = base.GetADSplitPermissionMode(null, null);
		}

		protected override void InternalProcessRecord()
		{
			InstallCannedRbacRoleAssignments.isFfoEnvironment = (base.Fields.Contains("IsFfo") && this.IsFfo);
			this.configurationSession.SessionSettings.IsSharedConfigChecked = true;
			base.InternalProcessRecord();
			this.RemoveInvalidRoleAssignments();
			this.UpdateRoleAssignments();
			List<string> enabledFeatures = (base.ServicePlanSettings == null) ? null : base.ServicePlanSettings.Organization.GetEnabledRoleGroupRoleAssignmentFeatures();
			List<string> enabledFeatures2 = (this.PreviousServicePlanSettings == null) ? null : this.PreviousServicePlanSettings.Organization.GetEnabledRoleGroupRoleAssignmentFeatures();
			RoleGroupRoleMapping[] roleGroupAssignmentsDefinition = this.GetRoleGroupAssignmentsDefinition();
			List<string> cannedRoleNames = this.GetCannedRoleNames();
			List<ExchangeRole> list = new List<ExchangeRole>();
			foreach (ExchangeRole exchangeRole in this.configurationSession.FindPaged<ExchangeRole>(this.rolesContainerId, QueryScope.OneLevel, null, null, 0))
			{
				if (cannedRoleNames.Contains(exchangeRole.Name))
				{
					list.Add(exchangeRole);
				}
			}
			RbacContainer rbacContainer = this.configurationSession.GetRbacContainer();
			ExchangeBuild currentRBACConfigVersion = base.GetCurrentRBACConfigVersion(rbacContainer);
			foreach (RoleGroupRoleMapping roleGroupRoleMapping in roleGroupAssignmentsDefinition)
			{
				ADGroup adgroup = null;
				foreach (RoleAssignmentDefinition roleAssignmentDefinition in roleGroupRoleMapping.Assignments)
				{
					bool flag = false;
					if (roleAssignmentDefinition.SatisfyCondition(enabledFeatures))
					{
						switch (base.InvocationMode)
						{
						case InvocationMode.Install:
							flag = true;
							break;
						case InvocationMode.BuildToBuildUpgrade:
							flag = (roleAssignmentDefinition.IntroducedInBuild > currentRBACConfigVersion);
							break;
						case InvocationMode.ServicePlanUpdate:
							flag = (!roleAssignmentDefinition.SatisfyCondition(enabledFeatures2) || roleAssignmentDefinition.IntroducedInBuild > currentRBACConfigVersion);
							break;
						}
					}
					if (InstallCannedRbacRoleAssignments.MonitoredDCOnlyRoleGroups.Contains(roleGroupRoleMapping.RoleGroup))
					{
						flag = true;
					}
					if (flag)
					{
						if (adgroup == null)
						{
							adgroup = this.FindCannedRoleGroupByName(roleGroupRoleMapping.RoleGroup);
						}
						this.CreateRoleAssignmentDefinition(roleAssignmentDefinition, adgroup, list);
					}
					else if (!roleAssignmentDefinition.SatisfyCondition(enabledFeatures, roleGroupAssignmentsDefinition))
					{
						if (this.Organization == null)
						{
							throw new InvalidOperationException(roleGroupRoleMapping.RoleGroup.ToString());
						}
						this.RemoveRoleAssignmentDefinition(roleAssignmentDefinition, list);
					}
				}
				if (InstallCannedRbacRoleAssignments.MonitoredDCOnlyRoleGroups.Contains(roleGroupRoleMapping.RoleGroup))
				{
					if (adgroup == null)
					{
						adgroup = this.FindCannedRoleGroupByName(roleGroupRoleMapping.RoleGroup);
					}
					this.PurgeInvalidAssignmentsFromRoleGroup(roleGroupRoleMapping, adgroup, list);
				}
			}
			ADGroup adgroup2 = this.ResolveWellKnownGuid(RoleGroup.OrganizationManagement_InitInfo.WellKnownGuid);
			if (adgroup2 == null)
			{
				base.WriteError(new ExRbacRoleGroupNotFoundException(RoleGroup.OrganizationManagement_InitInfo.WellKnownGuid, "Organization Management"), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(adgroup2);
			if ((base.ServicePlanSettings != null && base.ServicePlanSettings.Organization.PermissionManagementEnabled) || this.Organization == null)
			{
				using (List<ExchangeRole>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ExchangeRole exchangeRole2 = enumerator2.Current;
						if (exchangeRole2.IsEndUserRole && !exchangeRole2.IsDeprecated && !this.FindRoleAssignment(exchangeRole2, adgroup2, RoleAssignmentDelegationType.DelegatingOrgWide))
						{
							this.CreateRoleAssignment(exchangeRole2, adgroup2, RoleAssignmentDelegationType.DelegatingOrgWide);
						}
					}
					return;
				}
			}
			foreach (ExchangeRole exchangeRole3 in list)
			{
				if (exchangeRole3.IsEndUserRole && !exchangeRole3.IsDeprecated)
				{
					this.RemoveRoleAssignmentsFromGroup(exchangeRole3, adgroup2, RoleAssignmentDelegationType.DelegatingOrgWide);
				}
			}
		}

		private RoleGroupRoleMapping[] GetRoleGroupAssignmentsDefinition()
		{
			if (this.Organization == null)
			{
				if (Datacenter.IsMicrosoftHostedOnly(false))
				{
					return Datacenter_RoleGroupDefinition.Definition;
				}
				if (Datacenter.IsPartnerHostedOnly(false))
				{
					return Hosting_RoleGroupDefinition.Definition;
				}
				if (Datacenter.IsDatacenterDedicated(false))
				{
					return Dedicated_RoleGroupDefinition.Definition;
				}
				return Enterprise_RoleGroupDefinition.Definition;
			}
			else
			{
				if (Datacenter.IsPartnerHostedOnly(false))
				{
					return HostedTenant_RoleGroupDefinition.Definition;
				}
				return Tenant_RoleGroupDefinition.Definition;
			}
		}

		private void CreateRoleAssignmentDefinition(RoleAssignmentDefinition roleAssignmentDefinition, ADGroup roleGroup, List<ExchangeRole> precannedRoles)
		{
			List<ExchangeRole> list = precannedRoles.FindAll((ExchangeRole x) => x.RoleType.Equals(roleAssignmentDefinition.RoleType));
			if (roleAssignmentDefinition.UseSafeRole)
			{
				List<ExchangeRole> list2 = new List<ExchangeRole>(list.Count);
				foreach (ExchangeRole cannedRole in list)
				{
					ExchangeRole exchangeRole = this.TryFindSafeDCRoleOrUseDefault(cannedRole);
					if (exchangeRole != null)
					{
						list2.Add(exchangeRole);
					}
				}
				list = list2;
			}
			foreach (ExchangeRole role in list)
			{
				if (!this.FindRoleAssignment(role, roleGroup, roleAssignmentDefinition.DelegationType))
				{
					this.CreateRoleAssignment(role, roleGroup, roleAssignmentDefinition.DelegationType);
				}
			}
		}

		private void RemoveRoleAssignmentDefinition(RoleAssignmentDefinition roleAssignmentDefinition, List<ExchangeRole> precannedRoles)
		{
			List<ExchangeRole> list = precannedRoles.FindAll((ExchangeRole x) => x.RoleType.Equals(roleAssignmentDefinition.RoleType));
			foreach (ExchangeRole exchangeRole in list)
			{
				this.RemoveAllRoleAssignmentsForOneRole(exchangeRole, roleAssignmentDefinition.DelegationType);
				foreach (ExchangeRole role in this.configurationSession.FindPaged<ExchangeRole>(exchangeRole.Id, QueryScope.SubTree, null, null, 0))
				{
					this.RemoveAllRoleAssignmentsForOneRole(role, roleAssignmentDefinition.DelegationType);
				}
			}
		}

		private void RemoveAllRoleAssignmentsForOneRole(ExchangeRole role, RoleAssignmentDelegationType delegationType)
		{
			QueryFilter queryFilter;
			if (delegationType == RoleAssignmentDelegationType.Regular)
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, delegationType);
			}
			else
			{
				queryFilter = new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, RoleAssignmentDelegationType.Delegating),
					new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, RoleAssignmentDelegationType.DelegatingOrgWide)
				});
			}
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer), QueryScope.OneLevel, new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, role.Id),
				queryFilter
			}), null, 0))
			{
				this.configurationSession.Delete(exchangeRoleAssignment);
				base.LogWriteObject(exchangeRoleAssignment);
			}
		}

		private void RemoveRoleAssignmentsFromGroup(ExchangeRole role, ADGroup group, RoleAssignmentDelegationType delegationType)
		{
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer), QueryScope.OneLevel, new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, role.Id),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.User, group.Id),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, delegationType)
			}), null, 0))
			{
				this.configurationSession.Delete(exchangeRoleAssignment);
				base.LogWriteObject(exchangeRoleAssignment);
			}
		}

		private bool FindRoleAssignment(ExchangeRole role, ADRecipient user)
		{
			return this.FindRoleAssignment(role, user, RoleAssignmentDelegationType.Regular);
		}

		private bool FindRoleAssignment(ExchangeRole role, ADRecipient user, RoleAssignmentDelegationType delegationType)
		{
			using (IEnumerator<ExchangeRoleAssignment> enumerator = this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId, QueryScope.SubTree, new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, role.Id),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.User, user.Id),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, delegationType)
			}), null, 1).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ExchangeRoleAssignment exchangeRoleAssignment = enumerator.Current;
					return true;
				}
			}
			return false;
		}

		private ExchangeRole TryFindSafeDCRoleOrUseDefault(ExchangeRole cannedRole)
		{
			string dcsafeNameForRole = RoleDefinition.GetDCSafeNameForRole(cannedRole.Name);
			ExchangeRole[] array = this.configurationSession.Find<ExchangeRole>(cannedRole.Id, QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, dcsafeNameForRole), null, 1);
			if (array != null && array.Length == 1)
			{
				return array[0];
			}
			return cannedRole;
		}

		private void CreateRoleAssignment(ExchangeRole role, ADRecipient recipient)
		{
			this.CreateRoleAssignment(role, recipient, RoleAssignmentDelegationType.Regular);
		}

		private void CreateRoleAssignment(ExchangeRole role, ADRecipient recipient, RoleAssignmentDelegationType delegationType)
		{
			if (this.adSplitPermissionMode && delegationType == RoleAssignmentDelegationType.Regular && InstallCannedRbacRoleAssignments.invalidRoleTypesInADSplitPermissionMode.Contains(role.RoleType))
			{
				base.WriteVerbose(Strings.VerboseSkipCreatingRoleAssignment(recipient.Id.ToString(), role.Id.ToString(), delegationType.ToString()));
				return;
			}
			RoleAssigneeType roleAssigneeType = ExchangeRoleAssignment.RoleAssigneeTypeFromADRecipient(recipient);
			RoleHelper.CreateRoleAssignment(role, recipient.Id, recipient.OrganizationId, roleAssigneeType, recipient.OriginatingServer, delegationType, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, this.configurationSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		private void UpdateRoleAssignments()
		{
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeVersion, ExchangeRoleAssignmentSchema.Exchange2009_R3),
				new NotFilter(new ExistsFilter(ADObjectSchema.OrganizationalUnitRoot)),
				new NotFilter(new ExistsFilter(ADObjectSchema.ConfigurationUnit))
			});
			ADPagedReader<ExchangeRoleAssignment> adpagedReader = this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId, QueryScope.SubTree, filter, null, 0);
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in adpagedReader)
			{
				base.LogReadObject(exchangeRoleAssignment);
				ValidationError[] array = exchangeRoleAssignment.Validate();
				if (array != null && array.Length > 0)
				{
					this.WriteWarning(Strings.WarningCannotUpgradeInvalidRoleAssignment(exchangeRoleAssignment.Name));
					foreach (ValidationError validationError in array)
					{
						this.WriteWarning(validationError.Description);
					}
				}
				else
				{
					if (exchangeRoleAssignment.ExchangeVersion == ExchangeRoleAssignmentSchema.Exchange2009_R3)
					{
						if (exchangeRoleAssignment.RoleAssigneeType == RoleAssigneeType.User && !this.TryUpdateRoleAssigneeTypeAndScope(exchangeRoleAssignment))
						{
							continue;
						}
						exchangeRoleAssignment.SetExchangeVersion(exchangeRoleAssignment.MaximumSupportedExchangeObjectVersion);
					}
					else if (base.CurrentOrganizationId != null)
					{
						exchangeRoleAssignment.OrganizationId = base.CurrentOrganizationId;
					}
					else
					{
						exchangeRoleAssignment.OrganizationId = base.ExecutingUserOrganizationId;
					}
					this.configurationSession.Save(exchangeRoleAssignment);
					base.LogWriteObject(exchangeRoleAssignment);
				}
			}
		}

		private bool TryUpdateRoleAssigneeTypeAndScope(ExchangeRoleAssignment assignment)
		{
			RoleAssigneeType roleAssigneeType = RoleAssigneeType.User;
			ADRawEntry adrawEntry = this.recipientSession.ReadADRawEntry(assignment.User, InstallCannedRbacRoleAssignments.principalProperties);
			if (adrawEntry == null)
			{
				adrawEntry = this.configurationSession.ReadADRawEntry(assignment.User, InstallCannedRbacRoleAssignments.principalProperties);
				if (adrawEntry == null)
				{
					return false;
				}
			}
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)adrawEntry[ADObjectSchema.ObjectClass];
			foreach (string value in multiValuedProperty)
			{
				if ("group".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					roleAssigneeType = RoleAssigneeType.SecurityGroup;
					break;
				}
				if ("msExchRBACPolicy".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					roleAssigneeType = RoleAssigneeType.RoleAssignmentPolicy;
					break;
				}
				if ("user".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					if (RecipientTypeDetails.MailboxPlan == (RecipientTypeDetails)adrawEntry[ADRecipientSchema.RecipientTypeDetails])
					{
						roleAssigneeType = RoleAssigneeType.MailboxPlan;
						break;
					}
					roleAssigneeType = RoleAssigneeType.User;
					break;
				}
			}
			ConfigWriteScopeType configWriteScopeType = assignment.ConfigWriteScope;
			ScopeType scopeType = assignment.ConfigReadScope;
			if (configWriteScopeType == ConfigWriteScopeType.None)
			{
				ExchangeRole exchangeRole = this.configurationSession.Read<ExchangeRole>(assignment.Role);
				if (exchangeRole != null)
				{
					base.LogReadObject(exchangeRole);
					ValidationError[] array = exchangeRole.Validate();
					if (array.Length > 0)
					{
						this.WriteWarning(Strings.WarningCannotUpgradeRole(exchangeRole.Identity.ToString(), array[0].Description));
						return false;
					}
					scopeType = exchangeRole.ImplicitConfigReadScope;
					configWriteScopeType = (ConfigWriteScopeType)exchangeRole.ImplicitConfigWriteScope;
				}
			}
			if (assignment.RoleAssigneeType != roleAssigneeType || assignment.ConfigWriteScope != configWriteScopeType || assignment.ConfigReadScope != scopeType)
			{
				assignment.RoleAssigneeType = roleAssigneeType;
				assignment.ConfigReadScope = scopeType;
				assignment.ConfigWriteScope = configWriteScopeType;
			}
			return true;
		}

		private void RemoveInvalidRoleAssignments()
		{
			TaskLogger.LogEnter();
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new NotFilter(new ExistsFilter(ExchangeRoleAssignmentSchema.Role)),
				new NotFilter(new ExistsFilter(ExchangeRoleAssignmentSchema.User))
			});
			ADPagedReader<ExchangeRoleAssignment> adpagedReader = this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer), QueryScope.OneLevel, filter, null, 0);
			foreach (ExchangeRoleAssignment roleAssignment in adpagedReader)
			{
				this.RemoveRoleAssignment(roleAssignment);
			}
			if (this.adSplitPermissionMode)
			{
				List<QueryFilter> list = new List<QueryFilter>();
				foreach (RoleType roleType in InstallCannedRbacRoleAssignments.invalidRoleTypesInADSplitPermissionMode)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleSchema.RoleType, roleType));
				}
				ADPagedReader<ExchangeRole> adpagedReader2 = this.configurationSession.FindPaged<ExchangeRole>(base.OrgContainerId.GetDescendantId(ExchangeRole.RdnContainer), QueryScope.SubTree, QueryFilter.OrTogether(list.ToArray()), null, 0);
				List<QueryFilter> list2 = new List<QueryFilter>();
				foreach (ExchangeRole exchangeRole in adpagedReader2)
				{
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, exchangeRole.Id));
				}
				ADPagedReader<ExchangeRoleAssignment> adpagedReader3 = this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer), QueryScope.OneLevel, QueryFilter.AndTogether(new QueryFilter[]
				{
					InstallCannedRbacRoleAssignments.regularRoleAssignmentFilter,
					QueryFilter.OrTogether(list2.ToArray())
				}), null, 0);
				foreach (ExchangeRoleAssignment roleAssignment2 in adpagedReader3)
				{
					this.RemoveRoleAssignment(roleAssignment2);
				}
			}
			TaskLogger.LogExit();
		}

		private void RemoveRoleAssignment(ExchangeRoleAssignment roleAssignment)
		{
			base.LogReadObject(roleAssignment);
			base.WriteVerbose(Strings.WarningRemoveInvalidRoleAssignment(roleAssignment.Name));
			this.configurationSession.Delete(roleAssignment);
			base.LogWriteObject(roleAssignment);
		}

		[Conditional("DEBUG")]
		private void CheckRoleAssignmentDefinition(RoleAssignmentDefinition roleAssignment)
		{
			if (roleAssignment.UseSafeRole)
			{
				if (!Datacenter.IsMicrosoftHostedOnly(false))
				{
					Datacenter.IsDatacenterDedicated(false);
				}
				if (Datacenter.IsMicrosoftHostedOnly(false) || Datacenter.IsDatacenterDedicated(false))
				{
					OrganizationIdParameter organization = this.Organization;
				}
			}
		}

		private void PurgeInvalidAssignmentsFromRoleGroup(RoleGroupRoleMapping rgMapping, ADGroup roleGroup, List<ExchangeRole> topCannedRoles)
		{
			TaskLogger.LogEnter();
			if (!InstallCannedRbacRoleAssignments.MonitoredDCOnlyRoleGroups.Contains(rgMapping.RoleGroup))
			{
				return;
			}
			List<string> list = new List<string>(rgMapping.Assignments.Length * 2);
			RoleAssignmentDefinition[] assignments = rgMapping.Assignments;
			for (int i = 0; i < assignments.Length; i++)
			{
				RoleAssignmentDefinition assignmentDefinition = assignments[i];
				List<ExchangeRole> list2 = topCannedRoles.FindAll((ExchangeRole x) => x.RoleType.Equals(assignmentDefinition.RoleType));
				if (list2 != null)
				{
					foreach (ExchangeRole exchangeRole in list2)
					{
						list.Add(exchangeRole.DistinguishedName);
						list.Add(exchangeRole.Id.GetChildId(RoleDefinition.GetDCSafeNameForRole(exchangeRole.Name)).DistinguishedName);
					}
				}
			}
			ADPagedReader<ExchangeRoleAssignment> adpagedReader = this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.User, roleGroup.Id), null, 0);
			using (IEnumerator<ExchangeRoleAssignment> enumerator2 = adpagedReader.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ExchangeRoleAssignment roleAssignment = enumerator2.Current;
					if (!list.Contains(roleAssignment.Role.DistinguishedName, StringComparer.OrdinalIgnoreCase))
					{
						if (topCannedRoles.Find((ExchangeRole x) => x.Name.Equals(roleAssignment.Role.Name, StringComparison.OrdinalIgnoreCase) && x.RoleType.Equals(RoleType.UnScoped)) == null)
						{
							ExchangeRole exchangeRole2 = this.configurationSession.Read<ExchangeRole>(roleAssignment.Role);
							if (exchangeRole2 != null && !exchangeRole2.RoleType.Equals(RoleType.UnScoped))
							{
								this.RemoveRoleAssignment(roleAssignment);
							}
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		private ADGroup FindCannedRoleGroupByName(string rgName)
		{
			RoleGroupDefinition roleGroupDefinition = RoleGroupDefinitions.RoleGroups.FirstOrDefault((RoleGroupDefinition x) => x.Name.Equals(rgName, StringComparison.OrdinalIgnoreCase));
			Guid guid = Guid.Empty;
			if (roleGroupDefinition != null)
			{
				guid = roleGroupDefinition.RoleGroupGuid;
			}
			ADGroup adgroup = this.ResolveWellKnownGuid(guid);
			if (adgroup == null)
			{
				base.WriteError(new ExRbacRoleGroupNotFoundException(guid, (roleGroupDefinition != null) ? roleGroupDefinition.Name : string.Empty), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(adgroup);
			return adgroup;
		}

		private ADGroup ResolveWellKnownGuid(Guid guidToResolve)
		{
			if (this.organization != null)
			{
				return base.ResolveHostedExchangeGroupGuid<ADGroup>(guidToResolve, this.organization.OrganizationId);
			}
			return base.ResolveExchangeGroupGuid<ADGroup>(guidToResolve);
		}

		private List<string> GetCannedRoleNames()
		{
			RoleDefinition[] array;
			if (InstallCannedRbacRoleAssignments.isFfoEnvironment)
			{
				array = InstallCannedRbacRoles.MergeRoleDefinitions(CannedEOPRoles_Datacenter.Definition, CannedUCCRoles_Datacenter.Definition);
			}
			else if (this.Organization == null)
			{
				if (Datacenter.IsMicrosoftHostedOnly(false))
				{
					array = InstallCannedRbacRoles.MergeRoleDefinitions(CannedRoles_Datacenter.Definition, CannedWebServiceRoles_Datacenter.Definition);
				}
				else if (Datacenter.IsPartnerHostedOnly(false))
				{
					array = InstallCannedRbacRoles.MergeRoleDefinitions(CannedRoles_Hosting.Definition, CannedWebServiceRoles_Hosting.Definition);
				}
				else if (Datacenter.IsDatacenterDedicated(false))
				{
					array = InstallCannedRbacRoles.MergeRoleDefinitions(CannedRoles_Dedicated.Definition, CannedWebServiceRoles_Hosting.Definition);
				}
				else
				{
					array = InstallCannedRbacRoles.MergeRoleDefinitions(CannedRoles_Enterprise.Definition, CannedWebServiceRoles_Enterprise.Definition);
				}
			}
			else if (Datacenter.IsPartnerHostedOnly(false))
			{
				array = InstallCannedRbacRoles.MergeRoleDefinitions(CannedRoles_HostedTenant.Definition, CannedWebServiceRoles_HostedTenant.Definition);
			}
			else
			{
				array = InstallCannedRbacRoles.MergeRoleDefinitions(CannedRoles_Tenant.Definition, CannedWebServiceRoles_Tenant.Definition);
			}
			List<string> list = new List<string>(array.Length);
			foreach (RoleDefinition roleDefinition in array)
			{
				list.Add(roleDefinition.RoleName);
				if (base.ServicePlanSettings != null && Array.BinarySearch<RoleType>(ExchangeRole.EndUserRoleTypes, roleDefinition.RoleType) >= 0)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in base.ServicePlanSettings.MailboxPlans)
					{
						list.Add(string.Format("{0}_{1}", roleDefinition.RoleName, mailboxPlan.Name));
					}
				}
			}
			list.Sort();
			return list;
		}

		private static PropertyDefinition[] principalProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.RecipientTypeDetails,
			ADObjectSchema.ObjectClass
		};

		private static bool isFfoEnvironment = false;

		private bool adSplitPermissionMode;

		private static RoleType[] invalidRoleTypesInADSplitPermissionMode = new RoleType[]
		{
			RoleType.MailRecipientCreation,
			RoleType.ResetPassword,
			RoleType.SecurityGroupCreationAndMembership
		};

		private static string[] MonitoredDCOnlyRoleGroups = new string[]
		{
			"DataCenter Management",
			"Destructive Access",
			"Elevated Permissions",
			"View-Only Local Server Access",
			"Capacity Destructive Access",
			"Capacity Server Admins",
			"Cafe Server Admins",
			"Customer Change Access",
			"Customer Data Access",
			"Access To Customer Data - DC Only",
			"Datacenter Operations - DC Only",
			"Customer Destructive Access",
			"Customer PII Access",
			"Management Admin Access",
			"Management CA Core Admin",
			"Management Change Access",
			"Management Destructive Access",
			"Management Server Admins",
			"Capacity DC Admins",
			"Networking Admin Access",
			"Communications Manager",
			"Mailbox Management",
			"Ffo AntiSpam Admins",
			"Dedicated Support Access",
			"Networking Change Access",
			"AppLocker Exemption",
			"ECS Admin - Server Access",
			"ECS PII Access - Server Access",
			"ECS Admin",
			"ECS PII Access"
		};

		private static ComparisonFilter regularRoleAssignmentFilter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType, RoleAssignmentDelegationType.Regular);
	}
}
