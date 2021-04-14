using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(SPOODBUserStatisticsReport)
	})]
	[Cmdlet("Get", "SPOOneDriveForBusinessUserStatisticsReport")]
	public sealed class GetSPOODBUserStatisticsReport : ScaledTenantReportBase<SPOODBUserStatisticsReport>
	{
	}
}
