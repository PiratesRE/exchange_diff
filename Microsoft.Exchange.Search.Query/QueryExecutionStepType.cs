using System;

namespace Microsoft.Exchange.Search.Query
{
	public enum QueryExecutionStepType
	{
		InstantSearchRequest,
		AnalyzeQuery,
		CreateSearchFolder,
		GetSearchFolderView,
		QuerySearchFolder,
		GetHitHighlighiting,
		HitHighlightingCallback,
		GetSuggestions,
		SuggestionsCallback,
		GetSuggestionsPrimer,
		QueryResultsCallback,
		RefinersCallback,
		GetRefiners,
		GetFastResults,
		ConvertFastResultsToPropertyBags,
		TopNInitialization
	}
}
