using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public enum AvailabilityServiceMetadata
	{
		[DisplayName("AS", "ExtC")]
		ExtC,
		[DisplayName("AS", "IntC")]
		IntC,
		[DisplayName("AS", "PASQ1")]
		PASQ1,
		[DisplayName("AS", "PASQ2")]
		PASQ2,
		[DisplayName("AS", "PASQT")]
		PASQT,
		[DisplayName("AS", "TimeInAS")]
		TimeInAs,
		[DisplayName("AS", "PEL")]
		PreexecutionLatency,
		[DisplayName("AS", "ReqStats")]
		RequestStatistics
	}
}
