using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.FullTextIndex;

namespace Microsoft.Exchange.Server.Storage.MultiMailboxSearch
{
	internal interface IMultiMailboxSearchFullTextIndexQuery
	{
		List<string> RefinersList { get; set; }

		List<string> ExtraFieldsList { get; set; }

		Guid QueryCorrelationId { get; set; }

		KeywordStatsResultRow ExecuteFullTextKeywordHitsQuery(Guid databaseGuid, Guid mailboxGuid, string query);

		IList<FullTextIndexRow> ExecuteFullTextIndexQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, int pageSize, string sortSpec, out KeywordStatsResultRow keywordStatsResult, out Dictionary<string, List<RefinersResultRow>> refinersOutput);

		void Abort();
	}
}
