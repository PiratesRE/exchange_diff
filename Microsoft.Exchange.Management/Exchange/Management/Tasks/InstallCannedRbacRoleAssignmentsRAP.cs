using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "CannedRbacRoleAssignmentsRAP", DefaultParameterSetName = "Default")]
	public sealed class InstallCannedRbacRoleAssignmentsRAP : InstallCannedRbacObjectsTaskBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "Organization")]
		[ValidateNotNullOrEmpty]
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

		private bool IsAutoGroupRelatedRole(ExchangeRole role)
		{
			return this.Organization != null && InstallCannedRbacRoleAssignmentsRAP.tenantAutogroupRoles.Contains(role.RoleType);
		}

		protected override void InternalProcessRecord()
		{
			this.configurationSession.SessionSettings.IsSharedConfigChecked = true;
			base.InternalProcessRecord();
			List<string> enabledFeatures = (base.ServicePlanSettings == null) ? null : base.ServicePlanSettings.GetAggregatedMailboxPlanRoleAssignmentFeatures();
			List<string> enabledFeatures2 = (this.PreviousServicePlanSettings == null) ? null : this.PreviousServicePlanSettings.GetAggregatedMailboxPlanRoleAssignmentFeatures();
			RbacContainer rbacContainer = this.configurationSession.GetRbacContainer();
			ExchangeBuild currentRBACConfigVersion = base.GetCurrentRBACConfigVersion(rbacContainer);
			List<RoleToRAPAssignmentDefinition> list = new List<RoleToRAPAssignmentDefinition>();
			foreach (RoleToRAPAssignmentDefinition roleToRAPAssignmentDefinition in this.GetRoleAssignmentDefinitions().Assignments)
			{
				if (roleToRAPAssignmentDefinition.SatisfyCondition(enabledFeatures))
				{
					switch (base.InvocationMode)
					{
					case InvocationMode.Install:
						list.Add(roleToRAPAssignmentDefinition);
						break;
					case InvocationMode.BuildToBuildUpgrade:
						if (roleToRAPAssignmentDefinition.IntroducedInBuild > currentRBACConfigVersion)
						{
							list.Add(roleToRAPAssignmentDefinition);
						}
						break;
					case InvocationMode.ServicePlanUpdate:
						if (!roleToRAPAssignmentDefinition.SatisfyCondition(enabledFeatures2) || roleToRAPAssignmentDefinition.IntroducedInBuild > currentRBACConfigVersion)
						{
							list.Add(roleToRAPAssignmentDefinition);
						}
						break;
					}
				}
			}
			List<ExchangeRole> list2 = new List<ExchangeRole>();
			List<ExchangeRole> list3 = new List<ExchangeRole>();
			List<ExchangeRole> list4 = new List<ExchangeRole>();
			foreach (ExchangeRole exchangeRole in this.configurationSession.FindPaged<ExchangeRole>(this.rolesContainerId, QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleSchema.IsEndUserRole, true), null, 0))
			{
				if (exchangeRole.IsEndUserRole)
				{
					if (this.IsRoleInFilteredList(exchangeRole, list))
					{
						if (this.IsAutoGroupRelatedRole(exchangeRole))
						{
							list3.Add(exchangeRole);
						}
						else
						{
							list2.Add(exchangeRole);
						}
					}
					if (this.IsAutoGroupRelatedRole(exchangeRole))
					{
						list4.Add(exchangeRole);
					}
				}
			}
			if (this.Organization == null)
			{
				using (IEnumerator<RoleAssignmentPolicy> enumerator2 = this.FindAllRoleAssignmentPolicies().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						RoleAssignmentPolicy policy = enumerator2.Current;
						this.CreateRAPRoleAssignments(policy, list2);
					}
					goto IL_455;
				}
			}
			if (!base.ServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
			{
				foreach (RoleAssignmentPolicy policy2 in this.FindAllRoleAssignmentPolicies())
				{
					this.CreateRAPRoleAssignments(policy2, list2);
				}
				RoleAssignmentPolicy policy3 = this.FindDefaultRoleAssignmentPolicy();
				bool currentPlanAutoGroupEnabled = false;
				base.ServicePlanSettings.MailboxPlans.ForEach(delegate(ServicePlan.MailboxPlan x)
				{
					currentPlanAutoGroupEnabled |= x.AutoGroupPermissions;
				});
				bool previousPlanAutoGroupEnabled = false;
				bool flag = false;
				bool flag2 = false;
				if (this.PreviousServicePlanSettings != null)
				{
					this.PreviousServicePlanSettings.MailboxPlans.ForEach(delegate(ServicePlan.MailboxPlan x)
					{
						previousPlanAutoGroupEnabled |= x.AutoGroupPermissions;
					});
					if (previousPlanAutoGroupEnabled && !this.PreviousServicePlanSettings.Organization.ShareableConfigurationEnabled)
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = true;
				}
				if (currentPlanAutoGroupEnabled && !flag)
				{
					this.CreateRAPRoleAssignments(policy3, list3);
				}
				else if (!currentPlanAutoGroupEnabled && !flag2)
				{
					this.RemoveRAPRoleAssignmentsIfNeeded(policy3, list4, null);
				}
			}
			else
			{
				foreach (ServicePlan.MailboxPlan mailboxPlan in base.ServicePlanSettings.MailboxPlans)
				{
					ADUser aduser = this.FindMailboxPlanByName(mailboxPlan.Name);
					if (aduser.RoleAssignmentPolicy == null)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorRBACPolicyLinkNotFound(aduser.Name)), ErrorCategory.InvalidArgument, null);
					}
					RoleAssignmentPolicy roleAssignmentPolicy = this.configurationSession.Read<RoleAssignmentPolicy>(aduser.RoleAssignmentPolicy);
					if (roleAssignmentPolicy == null)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorRBACPolicyNotFound(aduser.RoleAssignmentPolicy.ToString())), ErrorCategory.InvalidArgument, null);
					}
					ServicePlan.MailboxPlan mailboxPlan2 = null;
					if (this.PreviousServicePlanSettings != null)
					{
						mailboxPlan2 = this.PreviousServicePlanSettings.GetMailboxPlanByName(mailboxPlan.Name);
					}
					if (this.PreviousServicePlanSettings == null || mailboxPlan2 == null)
					{
						this.CreateRAPRoleAssignments(roleAssignmentPolicy, list2, aduser.MailboxPlanIndex);
					}
					bool flag3 = false;
					bool flag4 = false;
					if (mailboxPlan2 != null)
					{
						if (mailboxPlan2.AutoGroupPermissions)
						{
							flag3 = true;
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag4 = true;
					}
					if (mailboxPlan.AutoGroupPermissions && !flag3)
					{
						this.CreateRAPRoleAssignments(roleAssignmentPolicy, list3, aduser.MailboxPlanIndex);
					}
					else if (!mailboxPlan.AutoGroupPermissions && !flag4)
					{
						this.RemoveRAPRoleAssignmentsIfNeeded(roleAssignmentPolicy, list4, aduser.MailboxPlanIndex);
					}
				}
			}
			IL_455:
			this.StampCurrentVersionOnRBACContainer(rbacContainer);
		}

		private RoleToRAPMapping GetRoleAssignmentDefinitions()
		{
			if (this.Organization == null)
			{
				return Enterprise_RoleToRAPMapping.Definition;
			}
			return Tenant_RoleToRAPMapping.Definition;
		}

		private void RemoveRAPRoleAssignmentsIfNeeded(RoleAssignmentPolicy policy, IList<ExchangeRole> roles, string mailboxPlanIndex)
		{
			List<ExchangeRole> list = new List<ExchangeRole>(2);
			foreach (ExchangeRole exchangeRole in roles)
			{
				if (string.IsNullOrEmpty(mailboxPlanIndex))
				{
					list.Add(exchangeRole);
				}
				else if (!string.IsNullOrEmpty(mailboxPlanIndex) && string.Equals(exchangeRole.MailboxPlanIndex, mailboxPlanIndex, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(exchangeRole);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.User, policy.Id);
			foreach (ExchangeRoleAssignment exchangeRoleAssignment in this.configurationSession.FindPaged<ExchangeRoleAssignment>(base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer), QueryScope.OneLevel, filter, null, 0))
			{
				if (InstallCannedRbacRoleAssignmentsRAP.IsRoleInListOrDerived(exchangeRoleAssignment.Role, list))
				{
					this.configurationSession.Delete(exchangeRoleAssignment);
					base.LogWriteObject(exchangeRoleAssignment);
				}
			}
		}

		private static bool IsRoleInListOrDerived(ADObjectId roleId, IList<ExchangeRole> roles)
		{
			foreach (ExchangeRole exchangeRole in roles)
			{
				if (roleId.IsDescendantOf(exchangeRole.Id))
				{
					return true;
				}
			}
			return false;
		}

		private void CreateRAPRoleAssignments(RoleAssignmentPolicy policy, IList<ExchangeRole> roles)
		{
			this.CreateRAPRoleAssignments(policy, roles, null);
		}

		private void CreateRAPRoleAssignments(RoleAssignmentPolicy policy, IList<ExchangeRole> roles, string mailboxPlanIndex)
		{
			foreach (ExchangeRole exchangeRole in roles)
			{
				if (string.IsNullOrEmpty(mailboxPlanIndex) || mailboxPlanIndex.Equals(exchangeRole.MailboxPlanIndex, StringComparison.OrdinalIgnoreCase))
				{
					RoleHelper.CreateRoleAssignment(exchangeRole, policy.Id, policy.OrganizationId, RoleAssigneeType.RoleAssignmentPolicy, policy.OriginatingServer, RoleAssignmentDelegationType.Regular, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, this.configurationSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
		}

		private RoleAssignmentPolicy FindDefaultRoleAssignmentPolicy()
		{
			return RecipientTaskHelper.FindDefaultRoleAssignmentPolicy(this.configurationSession, new Task.ErrorLoggerDelegate(base.WriteError), Strings.ErrorDefaultRoleAssignmentPolicyNotUnique, Strings.ErrorDefaultRoleAssignmentPolicyNotFound);
		}

		private ADPagedReader<RoleAssignmentPolicy> FindAllRoleAssignmentPolicies()
		{
			return this.configurationSession.FindPaged<RoleAssignmentPolicy>(base.OrgContainerId, QueryScope.SubTree, null, null, 0);
		}

		private void StampCurrentVersionOnRBACContainer(RbacContainer rbacContainer)
		{
			rbacContainer.StampExchangeObjectVersion(OrganizationTaskHelper.ManagementDllVersion);
			this.configurationSession.Save(rbacContainer);
			base.LogWriteObject(rbacContainer);
		}

		private bool IsRoleInFilteredList(ExchangeRole role, List<RoleToRAPAssignmentDefinition> filteredDefinitions)
		{
			foreach (RoleToRAPAssignmentDefinition roleToRAPAssignmentDefinition in filteredDefinitions)
			{
				if (role.RoleType == roleToRAPAssignmentDefinition.Type)
				{
					return true;
				}
			}
			return false;
		}

		private ADUser FindMailboxPlanByName(string name)
		{
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MailboxPlanRelease, MailboxPlanRelease.CurrentRelease),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MailboxPlanRelease, MailboxPlanRelease.AllReleases)
			});
			QueryFilter queryFilter2 = new TextFilter(ADObjectSchema.Name, name + "-", MatchOptions.Prefix, MatchFlags.IgnoreCase);
			QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.MailboxPlan);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter3,
				queryFilter,
				queryFilter2
			});
			ADUser[] array = this.recipientSession.FindADUser(null, QueryScope.SubTree, filter, null, 2);
			ADUser result = null;
			if (array.Length < 1)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorAssigneeUserNotFound(name)), ErrorCategory.InvalidArgument, null);
			}
			else if (array.Length > 1)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorAssigneeUserNotUnique(name)), ErrorCategory.InvalidArgument, null);
			}
			else
			{
				result = array[0];
			}
			return result;
		}

		private static readonly RoleType[] tenantAutogroupRoles = new RoleType[]
		{
			RoleType.MyDistributionGroups,
			RoleType.MyDistributionGroupMembership
		};
	}
}
