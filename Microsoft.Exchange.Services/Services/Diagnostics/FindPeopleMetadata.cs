using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum FindPeopleMetadata
	{
		[DisplayName("FP", "QS")]
		QueryString,
		[DisplayName("FP", "PC")]
		PersonalCount,
		[DisplayName("FP", "NPV")]
		TotalNumberOfPeopleInView,
		[DisplayName("FP", "GC")]
		GalCount,
		[DisplayName("FP", "PT")]
		PersonalSearchTime,
		[DisplayName("FP", "GT")]
		GalSearchTime,
		[DisplayName("FP", "PM")]
		PersonalSearchMode,
		[DisplayName("FP", "CST")]
		CreateSearchFolderTime,
		[DisplayName("FP", "CSRC")]
		CreateSearchFolderRpcCount,
		[DisplayName("FP", "CSRL")]
		CreateSearchFolderRpcLatency,
		[DisplayName("FP", "CSRLS")]
		CreateSearchFolderRpcLatencyOnStore,
		[DisplayName("FP", "CSCpu")]
		CreateSearchFolderCPUTime,
		[DisplayName("FP", "CSSTS")]
		CreateSearchFolderStartTimestamp,
		[DisplayName("FP", "CSETS")]
		CreateSearchFolderEndTimestamp,
		[DisplayName("FP", "PFT")]
		PublicFolderSearchTime,
		[DisplayName("FP", "PFRC")]
		PublicFolderSearchRpcCount,
		[DisplayName("FP", "PFRL")]
		PublicFolderSearchRpcLatency,
		[DisplayName("FP", "PFRLS")]
		PublicFolderSearchRpcLatencyOnStore,
		[DisplayName("FP", "PFCpu")]
		PublicFolderSearchCPUTime,
		[DisplayName("FP", "PFSTS")]
		PublicFolderSearchStartTimestamp,
		[DisplayName("FP", "PFETS")]
		PublicFolderSearchEndTimestamp,
		[DisplayName("FP", "PST")]
		PopulateSearchFolderTime,
		[DisplayName("FP", "PSRC")]
		PopulateSearchFolderRpcCount,
		[DisplayName("FP", "PSRL")]
		PopulateSearchFolderRpcLatency,
		[DisplayName("FP", "PSRLS")]
		PopulateSearchFolderRpcLatencyOnStore,
		[DisplayName("FP", "PSCpu")]
		PopulateSearchFolderCPUTime,
		[DisplayName("FP", "PSSTS")]
		PopulateSearchFolderStartTimestamp,
		[DisplayName("FP", "PSETS")]
		PopulateSearchFolderEndTimestamp,
		[DisplayName("FP", "PSNQT")]
		PopulateSearchFolderNotificationQueueTime,
		[DisplayName("FP", "ADT")]
		AggregateDataTime,
		[DisplayName("FP", "ADRC")]
		AggregateDataRpcCount,
		[DisplayName("FP", "ADRL")]
		AggregateDataRpcLatency,
		[DisplayName("FP", "ADRLS")]
		AggregateDataRpcLatencyOnStore,
		[DisplayName("FP", "ADCpu")]
		AggregateDataCPUTime,
		[DisplayName("FP", "ADSTS")]
		AggregateDataStartTimestamp,
		[DisplayName("FP", "ADETS")]
		AggregateDataEndTimestamp,
		[DisplayName("FP", "PSS")]
		PersonalSearchSuccessful,
		[DisplayName("FP", "FID")]
		FolderId,
		[DisplayName("FP", "CID")]
		CorrelationId,
		[DisplayName("FP", "GDC")]
		GalDataConversion,
		[DisplayName("FP", "PDC")]
		PersonalDataConversion,
		[DisplayName("FP", "CES")]
		CommandExecutionStart,
		[DisplayName("FP", "CEE")]
		CommandExecutionEnd,
		[DisplayName("FP", "BST")]
		BrowseSendersTime
	}
}
