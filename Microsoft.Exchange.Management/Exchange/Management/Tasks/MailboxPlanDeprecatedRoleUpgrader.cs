using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class MailboxPlanDeprecatedRoleUpgrader : DeprecatedRoleUpgrader
	{
		protected MailboxPlanDeprecatedRoleUpgrader(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping) : base(settings, roleNameMapping)
		{
		}

		public override void UpdateRoles(List<RoleDefinition> rolesDefinitions)
		{
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.settings.ServicePlanSettings.MailboxPlans)
			{
				string suffix = "_" + mailboxPlan.Name;
				this.CreateOrUpdateRoles(this.roleNameMapping, rolesDefinitions, mailboxPlan.GetEnabledPermissionFeatures(), suffix, mailboxPlan.MailboxPlanIndex);
			}
		}

		public new static bool CanUpgrade(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping, RoleDefinition roleDefinition, out RoleUpgrader roleUpgrader)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20009L, "-->MailboxPlanDeprecatedRoleUpgrader.CanUpgrade: roleName = {0}", roleDefinition.RoleName);
			bool flag = false;
			roleUpgrader = null;
			if (settings.Organization != null && roleDefinition.IsEndUserRole && roleNameMapping != null && roleNameMapping.IsDeprecatedRole && settings.ServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
			{
				flag = true;
			}
			if (flag)
			{
				roleUpgrader = new MailboxPlanDeprecatedRoleUpgrader(settings, roleNameMapping);
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(20008L, "<--MailboxPlanDeprecatedRoleUpgrader.CanUpgrade: canUpgrade = {0}", flag);
			return flag;
		}
	}
}
