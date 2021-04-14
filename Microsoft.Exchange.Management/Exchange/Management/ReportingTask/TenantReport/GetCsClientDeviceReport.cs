using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "CsClientDeviceReport")]
	[OutputType(new Type[]
	{
		typeof(CsClientDeviceReport)
	})]
	public sealed class GetCsClientDeviceReport : TenantReportBase<CsClientDeviceReport>
	{
	}
}
