using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "LicenseVsUsageSummaryReport")]
	[OutputType(new Type[]
	{
		typeof(LicenseVsUsageSummaryReport)
	})]
	public sealed class GetLicenseVsUsageSummaryReport : TenantReportBase<LicenseVsUsageSummaryReport>
	{
	}
}
