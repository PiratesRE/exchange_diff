using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum FindConversationAndItemMetadata
	{
		[DisplayName("FCI", "CID")]
		CorrelationId,
		[DisplayName("FCI", "MBXT")]
		MailboxTarget,
		[DisplayName("FCI", "QS")]
		QueryString,
		[DisplayName("FCI", "TRC")]
		TotalRowCount,
		[DisplayName("FCI", "VF")]
		ViewFilter,
		[DisplayName("FCI", "VFA")]
		ViewFilterActions,
		[DisplayName("FCI", "VFE")]
		ViewFilterSearchFolderException,
		[DisplayName("FCI", "VFPF")]
		ViewFilterSearchFolderPopulateFailed,
		[DisplayName("FCI", "CST")]
		CreateSearchFolderTime,
		[DisplayName("FCI", "CSRC")]
		CreateSearchFolderRpcCount,
		[DisplayName("FCI", "CSRL")]
		CreateSearchFolderRpcLatency,
		[DisplayName("FCI", "CSRLS")]
		CreateSearchFolderRpcLatencyOnStore,
		[DisplayName("FCI", "CSCpu")]
		CreateSearchFolderCPUTime,
		[DisplayName("FCI", "CSSTS")]
		CreateSearchFolderStartTimestamp,
		[DisplayName("FCI", "CSETS")]
		CreateSearchFolderEndTimestamp,
		[DisplayName("FCI", "PST")]
		PopulateSearchFolderTime,
		[DisplayName("FCI", "PSNQT")]
		PopulateSearchFolderNotificationQueueTime,
		[DisplayName("FCI", "PSRC")]
		PopulateSearchFolderRpcCount,
		[DisplayName("FCI", "PSRL")]
		PopulateSearchFolderRpcLatency,
		[DisplayName("FCI", "PSRLS")]
		PopulateSearchFolderRpcLatencyOnStore,
		[DisplayName("FCI", "PSCpu")]
		PopulateSearchFolderCPUTime,
		[DisplayName("FCI", "PSSTS")]
		PopulateSearchFolderStartTimestamp,
		[DisplayName("FCI", "PSETS")]
		PopulateSearchFolderEndTimestamp,
		[DisplayName("FCI", "PSEVT")]
		PopulateSearchFolderEventType,
		[DisplayName("FCI", "PSEVTD")]
		PopulateSearchFolderEventDelay,
		[DisplayName("FCI", "PSSF")]
		PopulateSearchFolderFailed,
		[DisplayName("FCI", "PSMaxRC")]
		PopulateSearchFolderMaxResultsCount,
		[DisplayName("FCI", "ADT")]
		AggregateDataTime,
		[DisplayName("FCI", "ADRC")]
		AggregateDataRpcCount,
		[DisplayName("FCI", "ADRL")]
		AggregateDataRpcLatency,
		[DisplayName("FCI", "ADRLS")]
		AggregateDataRpcLatencyOnStore,
		[DisplayName("FCI", "ADCpu")]
		AggregateDataCPUTime,
		[DisplayName("FCI", "ADSTS")]
		AggregateDataStartTimestamp,
		[DisplayName("FCI", "ADETS")]
		AggregateDataEndTimestamp,
		[DisplayName("FCI", "OPTS")]
		OptimizedSearch,
		[DisplayName("FCI", "AS")]
		ArchiveState,
		[DisplayName("FCI", "ADST")]
		ArchiveDiscoveryStartTimestamp,
		[DisplayName("FCI", "ADET")]
		ArchiveDiscoveryEndTimestamp,
		[DisplayName("FCI", "ADF")]
		ArchiveDiscoveryFailed,
		[DisplayName("FCI", "ERASST")]
		ExecuteRemoteArchiveSearchStartTimestamp,
		[DisplayName("FCI", "ERASET")]
		ExecuteRemoteArchiveSearchEndTimestamp,
		[DisplayName("FCI", "ERASF")]
		ExecuteRemoteArchiveSearchFailed,
		[DisplayName("FCI", "CVT")]
		CalendarViewTime,
		[DisplayName("FCI", "CSIT")]
		CalendarSingleItemsTotalTime,
		[DisplayName("FCI", "CSIC")]
		CalendarSingleItemsCount,
		[DisplayName("FCI", "CSIQ")]
		CalendarSingleItemsQueryRowsTime,
		[DisplayName("FCI", "CSISTC")]
		CalendarSingleItemsSeekToConditionTime,
		[DisplayName("FCI", "CSIG")]
		CalendarSingleItemsGetRowsTime,
		[DisplayName("FCI", "CRIT")]
		CalendarRecurringItemsTotalTime,
		[DisplayName("FCI", "CRIC")]
		CalendarRecurringItemsCount,
		[DisplayName("FCI", "CRIQ")]
		CalendarRecurringItemsQueryTime,
		[DisplayName("FCI", "CRIG")]
		CalendarRecurringItemsGetRowsTime,
		[DisplayName("FCI", "CRIET")]
		CalendarRecurringItemsExpansionTime,
		[DisplayName("FCI", "CRINI")]
		CalendarRecurringItemsNoInstancesInWindow,
		[DisplayName("FCI", "CRIMS")]
		CalendarRecurringItemsMaxSubject,
		[DisplayName("FCI", "CRIMBT")]
		CalendarRecurringItemsMaxBlobStreamTime,
		[DisplayName("FCI", "CRIMET")]
		CalendarRecurringItemsMaxExpansionTime,
		[DisplayName("FCI", "CRIMP")]
		CalendarRecurringItemsMaxParseTime,
		[DisplayName("FCI", "CRIMAR")]
		CalendarRecurringItemsMaxAddRowsTime,
		[DisplayName("FCI", "CVU")]
		CalendarRecurringItemsViewUsed
	}
}
