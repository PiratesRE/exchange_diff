using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum PerformInstantSearchMetaData
	{
		[DisplayName("IS", "AI")]
		ApplicationID,
		[DisplayName("IS", "SID")]
		SearchSessionID,
		[DisplayName("IS", "RID")]
		SearchRequestID,
		[DisplayName("IS", "WSR")]
		WaitOnSearchResults,
		[DisplayName("IS", "QO")]
		QueryOptions,
		[DisplayName("IS", "SC")]
		RequestedSuggestionsCount,
		[DisplayName("IS", "SS")]
		SuggestionSources,
		[DisplayName("IS", "RT")]
		RequestedResultTypes,
		[DisplayName("IS", "FS")]
		FolderScope,
		[DisplayName("IS", "QF")]
		QueryFilter,
		[DisplayName("IS", "RFC")]
		AppliedRefinementFiltersCount,
		[DisplayName("IS", "REC")]
		RequestedRefinersCount,
		[DisplayName("IS", "RC")]
		RequestedResultsCount,
		[DisplayName("IS", "IE")]
		InternalExecuteTime,
		[DisplayName("IS", "PSD")]
		PreSearchDuration,
		[DisplayName("IS", "TSD")]
		TotalSearchDuration
	}
}
