using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleUpgradeConfigurationSettings
	{
		public RoleEntry[] AvailableRoleEntries { get; set; }

		public IConfigurationSession ConfigurationSession { get; set; }

		public ADObjectId OrgContainerId { get; set; }

		public OrganizationIdParameter Organization { get; set; }

		public OrganizationId OrganizationId { get; set; }

		public ADObjectId RolesContainerId { get; set; }

		public ServicePlan ServicePlanSettings { get; set; }

		public Task.TaskVerboseLoggingDelegate WriteVerbose { get; set; }

		public Task.TaskErrorLoggingDelegate WriteError { get; set; }

		public Task.TaskWarningLoggingDelegate WriteWarning { get; set; }

		public RoleUpgradeConfigurationSettings.LogWriteObjectDelegate LogWriteObject { get; set; }

		public RoleUpgradeConfigurationSettings.LogReadObjectDelegate LogReadObject { get; set; }

		public RoleUpgradeConfigurationSettings.RemoveRoleAndAssignmentsDelegate RemoveRoleAndAssignments { get; set; }

		public InstallCannedRbacRoles Task { get; set; }

		public delegate void LogReadObjectDelegate(ADRawEntry obj);

		public delegate void LogWriteObjectDelegate(ADObject obj);

		public delegate void RemoveRoleAndAssignmentsDelegate(ADObjectId roleId);
	}
}
