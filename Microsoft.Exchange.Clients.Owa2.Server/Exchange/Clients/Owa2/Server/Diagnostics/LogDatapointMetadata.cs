using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum LogDatapointMetadata
	{
		[DisplayName("LD", "CDE")]
		CreateDatapointEventsElapsed,
		[DisplayName("LD", "DE")]
		DatapointsToLoggerElapsed,
		[DisplayName("LD", "TDSz")]
		TotalDatapointSize,
		[DisplayName("LD", "ADC")]
		AnalyticsDatapointCount,
		[DisplayName("LD", "ADSz")]
		AnalyticsDatapointSize,
		[DisplayName("LD", "IDC")]
		InferenceDatapointCount,
		[DisplayName("LD", "IDSz")]
		InferenceDatapointSize,
		[DisplayName("LD", "DDC")]
		DiagnosticsDatapointCount,
		[DisplayName("LD", "DDSz")]
		DiagnosticsDatapointSize,
		[DisplayName("LD", "CIAE")]
		CreateInferenceActivitiesElapsed,
		[DisplayName("LD", "IAE")]
		InferenceActivitiesToMailboxElapsed,
		[DisplayName("LD", "IAC")]
		InferenceActivitiesToMailboxCount,
		[DisplayName("LD", "WRE")]
		WatsonReportingElapsed,
		[DisplayName("LD", "WDC")]
		WatsonDatapointCount,
		[DisplayName("LD", "WDSz")]
		WatsonDatapointSize,
		[DisplayName("LD", "WDS")]
		WatsonDatapointSkipped,
		[DisplayName("LD", "WDF")]
		WatsonDatapointFailed
	}
}
