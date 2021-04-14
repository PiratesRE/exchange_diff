using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "RoleAssignmentPolicy")]
	public sealed class GetRoleAssignmentPolicy : GetMailboxPolicyBase<RoleAssignmentPolicy>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Static;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override void WriteResult(IConfigurable dataObject)
		{
			RoleAssignmentPolicy roleAssignmentPolicy = (RoleAssignmentPolicy)dataObject;
			Result<ExchangeRoleAssignment>[] roleAssignmentResults;
			if (base.SharedConfiguration != null)
			{
				SharedConfiguration sharedConfiguration = base.SharedConfiguration;
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sharedConfiguration.GetSharedConfigurationSessionSettings(), 553, "WriteResult", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxPolicies\\RoleAssignmentPolicyTasks.cs");
				roleAssignmentResults = tenantOrTopologyConfigurationSession.FindRoleAssignmentsByUserIds(new ADObjectId[]
				{
					sharedConfiguration.GetSharedRoleAssignmentPolicy()
				}, false);
				roleAssignmentPolicy.SharedConfiguration = sharedConfiguration.SharedConfigurationCU.Id;
			}
			else
			{
				roleAssignmentResults = this.ConfigurationSession.FindRoleAssignmentsByUserIds(new ADObjectId[]
				{
					roleAssignmentPolicy.Id
				}, false);
			}
			roleAssignmentPolicy.PopulateRoles(roleAssignmentResults);
			base.WriteResult(roleAssignmentPolicy);
		}
	}
}
