using System;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class EndUserRoles : DataSourceService, IEndUserRoles, IGetListService<ManagementRoleFilter, EndUserRoleRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ManagementRole@R:Organization")]
		public PowerShellResults<EndUserRoleRow> GetList(ManagementRoleFilter filter, SortOptions sort)
		{
			PowerShellResults<EndUserRoleRow> list = base.GetList<EndUserRoleRow, ManagementRoleFilter>("Get-ManagementRole", filter, sort);
			if (!list.Succeeded)
			{
				return list;
			}
			list.Output = Array.FindAll<EndUserRoleRow>(list.Output, (EndUserRoleRow x) => x.IsEndUserRole);
			if (Util.IsDataCenter && filter != null && filter.Policy != null)
			{
				PowerShellResults<RoleAssignmentPolicy> result = EndUserRoles.policyService.GetObject(filter.Policy);
				if (result.HasValue)
				{
					bool flag = (from role in list.Output
					where !string.IsNullOrEmpty(role.MailboxPlanIndex)
					select role).Count<EndUserRoleRow>() > 0;
					if (flag)
					{
						list.Output = (from role in list.Output
						where string.IsNullOrEmpty(role.MailboxPlanIndex) || role.MailboxPlanIndex == result.Value.MailboxPlanIndex
						select role).ToArray<EndUserRoleRow>();
					}
				}
				else
				{
					list.Output = new EndUserRoleRow[0];
				}
			}
			return list;
		}

		internal const string ReadScope = "@R:Organization";

		internal const string GetListRole = "Get-ManagementRole@R:Organization";

		private static RoleAssignmentPolicies policyService = new RoleAssignmentPolicies();
	}
}
