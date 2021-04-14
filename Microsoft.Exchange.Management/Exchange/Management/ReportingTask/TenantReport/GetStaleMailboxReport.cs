using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "StaleMailboxReport")]
	[OutputType(new Type[]
	{
		typeof(StaleMailboxReport)
	})]
	public sealed class GetStaleMailboxReport : TenantReportBase<StaleMailboxReport>
	{
	}
}
