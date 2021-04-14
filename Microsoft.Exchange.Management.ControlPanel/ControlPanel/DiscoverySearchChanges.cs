using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class DiscoverySearchChanges : DataSourceService, IDiscoverySearchChanges, IGetListService<AdminAuditLogSearchFilter, AdminAuditLogResultRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization")]
		public PowerShellResults<AdminAuditLogResultRow> GetList(AdminAuditLogSearchFilter filter, SortOptions sort)
		{
			if (filter == null)
			{
				filter = new AdminAuditLogSearchFilter();
			}
			filter.Cmdlets = "New-MailboxSearch, Start-MailboxSearch, Get-MailboxSearch, Stop-MailboxSearch, Remove-MailboxSearch, Set-MailboxSearch";
			filter.Parameters = "*";
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
					if (!string.Equals(a, adminAuditLogResultRow.SearchObject, StringComparison.InvariantCultureIgnoreCase))
					{
						a = adminAuditLogResultRow.SearchObject;
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
					Identity id = AuditHelper.CreateIdentity(powerShellResults.Output[j].SearchObject, filter.StartDate, filter.EndDate);
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

		internal const string CmdletsAudited = "New-MailboxSearch, Start-MailboxSearch, Get-MailboxSearch, Stop-MailboxSearch, Remove-MailboxSearch, Set-MailboxSearch";

		internal const string ParametersAudited = "*";
	}
}
