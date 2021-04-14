using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditQueryContext<TFilter> : IDisposable
	{
		IAsyncResult BeginAuditLogQuery(TFilter queryFilter, int maximumResultsCount);

		IEnumerable<T> EndAuditLogQuery<T>(IAsyncResult asyncResult, IAuditQueryStrategy<T> queryStrategy);
	}
}
