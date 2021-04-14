using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class RoleGroupChanges : DataSourceService, IRoleGroupChanges, IGetListService<AdminAuditLogSearchFilter, AdminAuditLogResultRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization")]
		public PowerShellResults<AdminAuditLogResultRow> GetList(AdminAuditLogSearchFilter filter, SortOptions sort)
		{
			if (filter == null)
			{
				filter = new AdminAuditLogSearchFilter();
			}
			filter.Cmdlets = "Add-RoleGroupMember,Remove-RoleGroupMember,Update-RoleGroupMember,New-RoleGroup,Remove-RoleGroup";
			PowerShellResults<AdminAuditLogResultRow> list = base.GetList<AdminAuditLogResultRow, AdminAuditLogSearchFilter>("Search-AdminAuditLog", filter, sort);
			PowerShellResults<AdminAuditLogResultRow> powerShellResults = new PowerShellResults<AdminAuditLogResultRow>();
			if (list.Succeeded)
			{
				Dictionary<string, AdminAuditLogResultRow> dictionary = new Dictionary<string, AdminAuditLogResultRow>();
				foreach (AdminAuditLogResultRow adminAuditLogResultRow in list.Output)
				{
					if (!dictionary.ContainsKey(adminAuditLogResultRow.ObjectModified.ToLower()))
					{
						dictionary[adminAuditLogResultRow.ObjectModified.ToLower()] = adminAuditLogResultRow;
					}
				}
				using (Dictionary<string, AdminAuditLogResultRow>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AdminAuditLogResultRow adminAuditLogResultRow2 = enumerator.Current;
						Identity id = AuditHelper.CreateIdentity(adminAuditLogResultRow2.ObjectModified, filter.StartDate, filter.EndDate);
						powerShellResults.MergeOutput(new AdminAuditLogResultRow[]
						{
							new AdminAuditLogResultRow(id, adminAuditLogResultRow2.AuditReportSearchBaseResult)
						});
					}
					return powerShellResults;
				}
			}
			powerShellResults.MergeErrors<AdminAuditLogResultRow>(list);
			return powerShellResults;
		}

		internal const string NoStart = "NoStart";

		internal const string NoEnd = "NoEnd";

		internal const string GetCmdlet = "Search-AdminAuditLog";

		internal const string ReadScope = "@R:Organization";

		internal const string AddRoleGroupMember = "Add-RoleGroupMember";

		internal const string RemoveRoleGroupMember = "Remove-RoleGroupMember";

		internal const string UpdateRoleGroupMember = "Update-RoleGroupMember";

		internal const string NewRoleGroup = "New-RoleGroup";

		internal const string RemoveRoleGroup = "Remove-RoleGroup";

		internal const string CmdletsAudited = "Add-RoleGroupMember,Remove-RoleGroupMember,Update-RoleGroupMember,New-RoleGroup,Remove-RoleGroup";

		private const string GetListRole = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization";
	}
}
