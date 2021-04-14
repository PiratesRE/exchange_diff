using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface ISourceStatus
	{
		int ProcessedItemCount { get; }

		int ItemCount { get; }

		long TotalSize { get; }

		int ProcessedUnsearchableItemCount { get; }

		int UnsearchableItemCount { get; }

		long DuplicateItemCount { get; }

		long UnsearchableDuplicateItemCount { get; }

		long ErrorItemCount { get; }

		bool IsSearchCompleted(bool includeSearchableItems, bool includeUnsearchableItems);

		bool IsExportCompleted(bool includeSearchableItems, bool includeUnsearchableItems);
	}
}
