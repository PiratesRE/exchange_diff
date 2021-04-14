using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum SessionDataMetadata
	{
		[DisplayName("SD", "CV")]
		BootWithConversationView,
		[DisplayName("SD", "SDCU")]
		SessionDataCacheUsed,
		[DisplayName("SD", "SDCTO")]
		SessionDataCacheWaitTimeOut,
		[DisplayName("SD", "SDH.B")]
		SessionDataHandlerBegin,
		[DisplayName("SD", "SDH.E")]
		SessionDataHandlerEnd,
		[DisplayName("SD", "SDP.B")]
		SessionDataProcessingBegin,
		[DisplayName("SD", "SDP.E")]
		SessionDataProcessingEnd,
		[DisplayName("SD", "GUCT.B")]
		GetOwaUserContextBegin,
		[DisplayName("SD", "GUCT.E")]
		GetOwaUserContextEnd,
		[DisplayName("SD", "PLMbxS.E")]
		TryPreLoadMailboxSessionEnd,
		[DisplayName("SD", "CAC.B")]
		CreateAggregatedConfigurationBegin,
		[DisplayName("SD", "CAC.E")]
		CreateAggregatedConfigurationEnd,
		[DisplayName("SD", "GUC.B")]
		GetOwaUserConfigurationBegin,
		[DisplayName("SD", "GUC.E")]
		GetOwaUserConfigurationEnd,
		[DisplayName("SD", "FF.B")]
		FindFoldersBegin,
		[DisplayName("SD", "FF.E")]
		FindFoldersEnd,
		[DisplayName("SD", "FCI.B")]
		FindConversationOrItemBegin,
		[DisplayName("SD", "FCI.E")]
		FindConversationOrItemEnd,
		[DisplayName("SD", "GCI.B")]
		GetConversationItemsOrItemBegin,
		[DisplayName("SD", "GCI.E")]
		GetConversationItemsOrItemEnd,
		[DisplayName("SD", "AGR.C")]
		AggregationContextReadCount,
		[DisplayName("SD", "AGRQ.C")]
		AggregationContextRequestCount,
		[DisplayName("SD", "SDC.B1")]
		SessionDataCacheFirstTimeRetriveveBegin,
		[DisplayName("SD", "SDC.E1")]
		SessionDataCacheFirstTimeRetriveveEnd,
		[DisplayName("SD", "SDC.B2")]
		SessionDataCacheSecondTimeRetriveveBegin,
		[DisplayName("SD", "SDC.E2")]
		SessionDataCacheSecondTimeRetriveveEnd
	}
}
