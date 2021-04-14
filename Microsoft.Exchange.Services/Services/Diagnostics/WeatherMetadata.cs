using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum WeatherMetadata
	{
		[DisplayName("WEA", "QRY")]
		QueryString,
		[DisplayName("WEA", "CNT")]
		LocationsCount,
		[DisplayName("WEA", "LOC")]
		Culture,
		[DisplayName("WEA", "SLA")]
		SearchLocationsLatency,
		[DisplayName("WEA", "FLA")]
		ForecastLatency,
		[DisplayName("WEA", "WESC")]
		WebExceptionStatusCode,
		[DisplayName("WEA", "FAIL")]
		Failed
	}
}
