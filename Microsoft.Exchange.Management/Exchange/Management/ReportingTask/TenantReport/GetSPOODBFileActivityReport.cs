using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "SPOOneDriveForBusinessFileActivityReport")]
	[OutputType(new Type[]
	{
		typeof(SPOODBFileActivityReport)
	})]
	public sealed class GetSPOODBFileActivityReport : ScaledTenantReportBase<SPOODBFileActivityReport>
	{
	}
}
