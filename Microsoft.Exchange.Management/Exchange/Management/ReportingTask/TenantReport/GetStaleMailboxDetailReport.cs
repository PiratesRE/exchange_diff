using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(StaleMailboxDetailReport)
	})]
	[Cmdlet("Get", "StaleMailboxDetailReport")]
	public sealed class GetStaleMailboxDetailReport : ScaledTenantReportBase<StaleMailboxDetailReport>
	{
	}
}
