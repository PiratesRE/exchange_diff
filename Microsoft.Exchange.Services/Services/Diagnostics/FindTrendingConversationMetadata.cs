using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum FindTrendingConversationMetadata
	{
		[DisplayName("FTC", "TRC")]
		TotalRowCount,
		[DisplayName("FTC", "QT")]
		AggregatedConversationQueryTime,
		[DisplayName("FTC", "QRC")]
		AggregatedConversationQueryRpcCount,
		[DisplayName("FTC", "QRL")]
		AggregatedConversationQueryRpcLatency,
		[DisplayName("FTC", "QRLS")]
		AggregatedConversationQueryRpcLatencyOnStore,
		[DisplayName("FTC", "QCpu")]
		AggregatedConversationQueryCPUTime,
		[DisplayName("FTC", "QSTS")]
		AggregatedConversationQueryStartTimestamp,
		[DisplayName("FTC", "QETS")]
		AggregatedConversationQueryEndTimestamp,
		[DisplayName("FTC", "RT")]
		AggregatedConversationRankingTime,
		[DisplayName("FTC", "RTC")]
		ReturnedTrendingConversations
	}
}
