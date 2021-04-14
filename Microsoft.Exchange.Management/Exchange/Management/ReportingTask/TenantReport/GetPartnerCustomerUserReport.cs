using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "PartnerCustomerUserReport")]
	[OutputType(new Type[]
	{
		typeof(PartnerCustomerUserReport)
	})]
	public sealed class GetPartnerCustomerUserReport : TenantReportBase<PartnerCustomerUserReport>
	{
	}
}
