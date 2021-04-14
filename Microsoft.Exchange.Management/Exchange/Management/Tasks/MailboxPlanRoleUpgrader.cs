using System;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class MailboxPlanRoleUpgrader : NonDeprecatedRoleUpgrader
	{
		protected MailboxPlanRoleUpgrader(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping) : base(settings, roleNameMapping)
		{
		}

		public override void UpdateRole(RoleDefinition definition)
		{
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.settings.ServicePlanSettings.MailboxPlans)
			{
				string suffix = "_" + mailboxPlan.Name;
				base.CreateOrUpdateRole(this.roleNameMapping, definition, mailboxPlan.GetEnabledPermissionFeatures(), suffix, mailboxPlan.MailboxPlanIndex);
			}
		}

		public new static bool CanUpgrade(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping, RoleDefinition roleDefinition, out RoleUpgrader roleUpgrader)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20007L, "-->MailboxPlanRoleUpgrader.CanUpgrade: roleName = {0}", roleDefinition.RoleName);
			bool flag = false;
			roleUpgrader = null;
			if (settings.Organization != null && roleDefinition.IsEndUserRole && (roleNameMapping == null || (!roleNameMapping.IsDeprecatedRole && !roleNameMapping.IsSplitting)) && settings.ServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
			{
				flag = true;
			}
			if (flag)
			{
				roleUpgrader = new MailboxPlanRoleUpgrader(settings, roleNameMapping);
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(20007L, "<--MailboxPlanRoleUpgrader.CanUpgrade: canUpgrade = {0}", flag);
			return flag;
		}
	}
}
