using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface ISearchRpcClient
	{
		AggregatedSearchTaskResult Search(int refinerResultTrimCount);

		List<IKeywordHit> GetKeywordHits(List<string> keywordList);

		void Abort();
	}
}
