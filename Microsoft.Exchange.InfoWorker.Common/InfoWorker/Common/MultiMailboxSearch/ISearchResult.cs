using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface ISearchResult
	{
		SortedResultPage PreviewResult { get; }

		IDictionary<string, IKeywordHit> KeywordStatistics { get; }

		Dictionary<string, List<IRefinerResult>> RefinersResult { get; }

		List<Pair<MailboxInfo, Exception>> PreviewErrors { get; }

		ByteQuantifiedSize TotalResultSize { get; }

		ulong TotalResultCount { get; }

		List<MailboxStatistics> MailboxStats { get; }

		IProtocolLog ProtocolLog { get; }

		void MergeSearchResult(ISearchResult aggregator);
	}
}
