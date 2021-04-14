using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MultiMailboxSearch
{
	internal interface IMultiMailboxSearch
	{
		List<string> RefinersList { get; set; }

		List<string> ExtraFieldsList { get; set; }

		ErrorCode Search(Context context, MultiMailboxSearchCriteria criteria, out IList<FullTextIndexRow> results, out KeywordStatsResultRow statsResult, out Dictionary<string, List<RefinersResultRow>> refinersOutput);

		ErrorCode GetKeywordStatistics(Context context, MultiMailboxSearchCriteria[] criterias, out IList<KeywordStatsResultRow> results);
	}
}
