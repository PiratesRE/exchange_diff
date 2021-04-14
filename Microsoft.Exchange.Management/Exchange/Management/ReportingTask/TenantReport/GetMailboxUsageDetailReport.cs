using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "MailboxUsageDetailReport")]
	[OutputType(new Type[]
	{
		typeof(MailboxUsageDetailReport)
	})]
	public sealed class GetMailboxUsageDetailReport : ScaledTenantReportBase<MailboxUsageDetailReport>
	{
	}
}
