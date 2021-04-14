using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Tasks
{
	internal static class RoleUpgraderFactory
	{
		public static RoleUpgrader GetRoleUpgrader(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping, RoleDefinition roleDefinition)
		{
			RoleUpgrader result = null;
			foreach (RoleUpgraderFactory.CanUpgradeRoleDelegate canUpgradeRoleDelegate in RoleUpgraderFactory.knownUpgrades)
			{
				if (canUpgradeRoleDelegate(settings, roleNameMapping, roleDefinition, out result))
				{
					break;
				}
			}
			return result;
		}

		private static List<RoleUpgraderFactory.CanUpgradeRoleDelegate> knownUpgrades = new List<RoleUpgraderFactory.CanUpgradeRoleDelegate>(4)
		{
			new RoleUpgraderFactory.CanUpgradeRoleDelegate(NonDeprecatedRoleUpgrader.CanUpgrade),
			new RoleUpgraderFactory.CanUpgradeRoleDelegate(MailboxPlanRoleUpgrader.CanUpgrade),
			new RoleUpgraderFactory.CanUpgradeRoleDelegate(DeprecatedRoleUpgrader.CanUpgrade),
			new RoleUpgraderFactory.CanUpgradeRoleDelegate(MailboxPlanDeprecatedRoleUpgrader.CanUpgrade),
			new RoleUpgraderFactory.CanUpgradeRoleDelegate(SplitRoleUpgrader.CanUpgrade)
		};

		private delegate bool CanUpgradeRoleDelegate(RoleUpgradeConfigurationSettings settings, RoleNameMapping roleNameMapping, RoleDefinition roleDefinition, out RoleUpgrader roleUpgrader);
	}
}
