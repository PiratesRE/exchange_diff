using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "MailboxUsageReport")]
	[OutputType(new Type[]
	{
		typeof(MailboxUsageReport)
	})]
	public sealed class GetMailboxUsageReport : TenantReportBase<MailboxUsageReport>
	{
	}
}
