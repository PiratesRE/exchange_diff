using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class UMCallSummaryReportService : DataSourceService, IUMCallSummaryReportService, IGetListService<UMCallSummaryReportFilter, UMCallSummaryReportRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallSummaryReport?GroupBy@R:Organization")]
		public PowerShellResults<UMCallSummaryReportRow> GetList(UMCallSummaryReportFilter filter, SortOptions sort)
		{
			bool isExportCallDataEnabled = false;
			if (filter != null && string.Equals(filter.GroupBy, GroupBy.Day.ToString()))
			{
				isExportCallDataEnabled = true;
			}
			PowerShellResults<UMCallSummaryReportRow> list = base.GetList<UMCallSummaryReportRow, UMCallSummaryReportFilter>("Get-UMCallSummaryReport", filter, sort);
			if (list.Succeeded)
			{
				foreach (UMCallSummaryReportRow umcallSummaryReportRow in list.Output)
				{
					umcallSummaryReportRow.IsExportCallDataEnabled = isExportCallDataEnabled;
					umcallSummaryReportRow.UMDialPlanID = filter.UMDialPlan;
					umcallSummaryReportRow.UMIPGatewayID = filter.UMIPGateway;
				}
			}
			return list;
		}

		private const string Noun = "UMCallSummaryReport";

		internal const string GetCmdlet = "Get-UMCallSummaryReport";

		internal const string ReadScope = "@R:Organization";

		private const string GetListRole = "Get-UMCallSummaryReport?GroupBy@R:Organization";
	}
}
