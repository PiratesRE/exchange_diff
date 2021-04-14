using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface ISearchResults
	{
		IEnumerable<IDictionary<string, object>> SearchResultItems { get; }

		IEnumerable<IDictionary<string, object>> UnsearchableItems { get; }

		int SearchResultItemsCount { get; }

		int ProcessedItemCount { get; }

		int UnsearchableItemsCount { get; }

		int ProcessedUnsearchableItemCount { get; }

		long TotalSize { get; }

		string ItemIdKey { get; }

		long DuplicateItemCount { get; }

		long UnsearchableDuplicateItemCount { get; }

		long ErrorItemCount { get; }

		void IncrementErrorItemCount(string sourceId);

		ISourceStatus GetSourceStatusBySourceId(string sourceId);
	}
}
