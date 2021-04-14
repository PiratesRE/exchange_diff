using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMCallSummaryReport")]
	public interface IUMCallSummaryReportService : IGetListService<UMCallSummaryReportFilter, UMCallSummaryReportRow>
	{
	}
}
