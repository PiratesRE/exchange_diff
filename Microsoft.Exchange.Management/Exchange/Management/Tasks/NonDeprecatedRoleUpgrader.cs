using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class NonDeprecatedRoleUpgrader : RoleUpgrader
	{
		protected NonDeprecatedRoleUpgrader(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping) : base(settings, roleNameMapping)
		{
		}

		public override void UpdateRole(RoleDefinition definition)
		{
			List<string> enabledPermissionFeatures = null;
			if (this.settings.ServicePlanSettings != null)
			{
				if (definition.IsEndUserRole && !this.settings.ServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
				{
					enabledPermissionFeatures = this.settings.ServicePlanSettings.GetAggregatedMailboxPlanPermissions();
				}
				else if (!definition.IsEndUserRole)
				{
					enabledPermissionFeatures = this.settings.ServicePlanSettings.Organization.GetEnabledPermissionFeatures();
				}
			}
			base.CreateOrUpdateRole(this.roleNameMapping, definition, enabledPermissionFeatures);
		}

		public override void UpdateRoles(List<RoleDefinition> rolesDefinitions)
		{
			throw new NotImplementedException("NonDeprecatedRoleUpgrader feature not implemented");
		}

		public static bool CanUpgrade(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping, RoleDefinition roleDefinition, out RoleUpgrader roleUpgrader)
		{
			ExTraceGlobals.AccessCheckTracer.TraceFunction<string>(20006L, "-->NonDeprecatedRoleUpgrader.CanUpgrade: roleName = {0}", roleDefinition.RoleName);
			bool flag = false;
			roleUpgrader = null;
			if (settings.Organization == null)
			{
				flag = true;
			}
			else if (!roleDefinition.IsEndUserRole || !settings.ServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
			{
				flag = true;
			}
			flag = (flag && (roleNameMapping == null || (!roleNameMapping.IsDeprecatedRole && !roleNameMapping.IsSplitting)));
			if (flag)
			{
				roleUpgrader = new NonDeprecatedRoleUpgrader(settings, roleNameMapping);
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(20006L, "<--NonDeprecatedRoleUpgrader.CanUpgrade: canUpgrade = {0}", flag);
			return flag;
		}
	}
}
