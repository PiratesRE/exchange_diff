using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class QueryResultEnumerator : QueryResultEnumeratorBase, IEnumerable<object[]>, IEnumerable
	{
		private QueryResultEnumerator(QueryResult queryResult) : base(queryResult)
		{
		}

		private QueryResultEnumerator(QueryResult queryResult, int fetchCount) : base(queryResult, fetchCount)
		{
		}

		public static QueryResultEnumerator From(QueryResult queryResult)
		{
			return new QueryResultEnumerator(queryResult);
		}

		public static QueryResultEnumerator From(QueryResult queryResult, int fetchCount)
		{
			return new QueryResultEnumerator(queryResult, fetchCount);
		}

		public IEnumerator<object[]> GetEnumerator()
		{
			for (;;)
			{
				object[][] rowResults = base.QueryResult.GetRows(base.FetchCount);
				if (rowResults == null || rowResults.Length <= 0)
				{
					break;
				}
				for (int i = 0; i < rowResults.Length; i++)
				{
					yield return rowResults[i];
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
