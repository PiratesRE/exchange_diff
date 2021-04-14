using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(PartnerClientExpiringSubscriptionReport)
	})]
	[Cmdlet("Get", "PartnerClientExpiringSubscriptionReport")]
	public sealed class GetPartnerClientExpiringSubscriptionReport : TenantReportBase<PartnerClientExpiringSubscriptionReport>
	{
	}
}
