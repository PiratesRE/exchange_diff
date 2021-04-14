using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ExternalAccess : DataSourceService, IExternalAccess, IGetListService<ExternalAccessFilter, AdminAuditLogResultRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-AdminAuditLog?ResultSize&StartDate&EndDate&ExternalAccess@R:Organization")]
		public PowerShellResults<AdminAuditLogResultRow> GetList(ExternalAccessFilter filter, SortOptions sort)
		{
			filter.ExternalAccess = new bool?(true);
			PowerShellResults<AdminAuditLogResultRow> list = base.GetList<AdminAuditLogResultRow, ExternalAccessFilter>("Search-AdminAuditLog", filter, sort);
			if (list.Succeeded)
			{
				PowerShellResults<AdminAuditLogResultRow> powerShellResults = new PowerShellResults<AdminAuditLogResultRow>();
				int num = list.Output.Length;
				AdminAuditLogResultRow[] array = new AdminAuditLogResultRow[num];
				for (int i = 0; i < num; i++)
				{
					string text = string.Format("{0};{1};{2};{3}", new object[]
					{
						list.Output[i].AuditReportSearchBaseResult.ObjectModified,
						list.Output[i].AuditReportSearchBaseResult.CmdletName,
						filter.StartDate,
						filter.EndDate
					});
					Identity id = new Identity(text, text);
					array[i] = new AdminAuditLogResultRow(id, list.Output[i].AuditReportSearchBaseResult);
				}
				powerShellResults.Output = array;
				return powerShellResults;
			}
			return list;
		}

		internal const string NoStart = "NoStart";

		internal const string NoEnd = "NoEnd";

		internal const string GetCmdlet = "Search-AdminAuditLog";

		private const string GetListRole = "Search-AdminAuditLog?ResultSize&StartDate&EndDate&ExternalAccess@R:Organization";
	}
}
