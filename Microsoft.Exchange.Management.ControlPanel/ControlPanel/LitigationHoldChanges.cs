using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class LitigationHoldChanges : DataSourceService, ILitigationHoldChanges, IGetListService<AdminAuditLogSearchFilter, AdminAuditLogResultRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization")]
		public PowerShellResults<AdminAuditLogResultRow> GetList(AdminAuditLogSearchFilter filter, SortOptions sort)
		{
			if (filter == null)
			{
				filter = new AdminAuditLogSearchFilter();
			}
			filter.Cmdlets = "Set-Mailbox";
			filter.Parameters = "LitigationHoldEnabled";
			if (filter != null && filter.ObjectIds != null && filter.ObjectIds.Length > 0)
			{
				SmtpAddress[] addresses = filter.ObjectIds.ToSmtpAddressArray();
				string[] identitiesFromSmtpAddresses = AuditHelper.GetIdentitiesFromSmtpAddresses(addresses);
				if (identitiesFromSmtpAddresses.Length != 0)
				{
					filter.ObjectIds = string.Join(",", identitiesFromSmtpAddresses);
				}
			}
			PowerShellResults<AdminAuditLogResultRow> list = base.GetList<AdminAuditLogResultRow, AdminAuditLogSearchFilter>("Search-AdminAuditLog", filter, sort);
			PowerShellResults<AdminAuditLogResultRow> powerShellResults = new PowerShellResults<AdminAuditLogResultRow>();
			if (list.Succeeded)
			{
				string a = null;
				foreach (AdminAuditLogResultRow adminAuditLogResultRow in list.Output)
				{
					if (!string.Equals(a, adminAuditLogResultRow.ObjectModified, StringComparison.InvariantCultureIgnoreCase))
					{
						a = adminAuditLogResultRow.ObjectModified;
						powerShellResults.MergeOutput(new AdminAuditLogResultRow[]
						{
							adminAuditLogResultRow
						});
					}
				}
				int num = powerShellResults.Output.Length;
				AdminAuditLogResultRow[] array = new AdminAuditLogResultRow[num];
				for (int j = 0; j < num; j++)
				{
					Identity id = AuditHelper.CreateIdentity(powerShellResults.Output[j].ObjectModified, filter.StartDate, filter.EndDate);
					array[j] = new AdminAuditLogResultRow(id, powerShellResults.Output[j].AuditReportSearchBaseResult);
				}
				powerShellResults.Output = array;
				return powerShellResults;
			}
			return list;
		}

		internal const string GetCmdlet = "Search-AdminAuditLog";

		internal const string ReadScope = "@R:Organization";

		internal const string GetListRole = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization";

		internal const string CmdletsAudited = "Set-Mailbox";

		internal const string ParametersAudited = "LitigationHoldEnabled";
	}
}
