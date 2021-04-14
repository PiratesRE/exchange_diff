using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(ScorecardMetricsReport)
	})]
	[Cmdlet("Get", "ScorecardMetricsReport")]
	public sealed class GetScorecardMetricsReport : TenantReportBase<ScorecardMetricsReport>
	{
	}
}
