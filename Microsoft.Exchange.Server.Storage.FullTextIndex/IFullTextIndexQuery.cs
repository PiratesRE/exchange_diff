using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public interface IFullTextIndexQuery
	{
		List<FullTextIndexRow> ExecuteFullTextIndexQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, CultureInfo culture, Guid correlationId, PagingImsFlowExecutor.QueryLoggingContext loggingContext);

		List<FullTextIndexRow> ExecutePagedFullTextIndexQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, CultureInfo culture, Guid correlationId, bool needConversationId, PagingImsFlowExecutor.QueryLoggingContext loggingContext, PagedQueryResults pagedQueryResults);

		IEnumerable<FullTextDiagnosticRow> ExecuteDiagnosticQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, CultureInfo culture, Guid correlationId, string sortOrder, ICollection<string> additionalColumns, PagingImsFlowExecutor.QueryLoggingContext loggingContext);

		int GetPageSize();
	}
}
