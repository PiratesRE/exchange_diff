using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ManagementRoles : DataSourceService, IManagementRoles, IGetListService<ManagementRoleFilter, ManagementRoleRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ManagementRole@R:Organization")]
		public PowerShellResults<ManagementRoleRow> GetList(ManagementRoleFilter filter, SortOptions sort)
		{
			PowerShellResults<ManagementRoleRow> list = base.GetList<ManagementRoleRow, ManagementRoleFilter>("Get-ManagementRole", filter, sort, "Name");
			list.Output = Array.FindAll<ManagementRoleRow>(list.Output, (ManagementRoleRow x) => !x.IsEndUserRole);
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ManagementRole?Identity@R:Organization")]
		public PowerShellResults<ManagementRoleObject> GetObject(Identity identity)
		{
			return base.GetObject<ManagementRoleObject>("Get-ManagementRole", identity);
		}

		internal const string ReadScope = "@R:Organization";

		internal const string GetListRole = "Get-ManagementRole@R:Organization";

		private const string GetObjectRole = "Get-ManagementRole?Identity@R:Organization";
	}
}
