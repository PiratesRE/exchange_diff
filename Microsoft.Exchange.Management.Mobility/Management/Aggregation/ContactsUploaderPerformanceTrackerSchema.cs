using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.Aggregation
{
	internal enum ContactsUploaderPerformanceTrackerSchema
	{
		[DisplayName("CU", "Read")]
		ContactsRead,
		[DisplayName("CU", "Exported")]
		ContactsExported,
		[DisplayName("CU", "Received")]
		ContactsReceived,
		[DisplayName("CU", "RpcC")]
		RpcCount,
		[DisplayName("CU", "RpcT")]
		RpcLatency,
		[DisplayName("CU", "Time")]
		CpuTime,
		[DisplayName("CU", "Size")]
		DataSize,
		[DisplayName("CU", "Result")]
		Result
	}
}
