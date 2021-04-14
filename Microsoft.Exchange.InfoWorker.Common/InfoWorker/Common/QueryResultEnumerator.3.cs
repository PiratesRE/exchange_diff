using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class QueryResultEnumerator<T, U> : QueryResultEnumeratorBase, IEnumerable<Pair<T, U>>, IEnumerable
	{
		private QueryResultEnumerator(QueryResult queryResult) : base(queryResult)
		{
		}

		private QueryResultEnumerator(QueryResult queryResult, int fetchCount) : base(queryResult, fetchCount)
		{
		}

		public static QueryResultEnumerator<T, U> From(QueryResult queryResult)
		{
			return new QueryResultEnumerator<T, U>(queryResult);
		}

		public static QueryResultEnumerator<T, U> From(QueryResult queryResult, int fetchCount)
		{
			return new QueryResultEnumerator<T, U>(queryResult, fetchCount);
		}

		public IEnumerator<Pair<T, U>> GetEnumerator()
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
					T first = default(T);
					U second = default(U);
					if (rowResults[i][0] != null && !(rowResults[i][0] is PropertyError))
					{
						first = (T)((object)rowResults[i][0]);
					}
					if (rowResults[i][1] != null && !(rowResults[i][1] is PropertyError))
					{
						second = (U)((object)rowResults[i][1]);
					}
					yield return new Pair<T, U>(first, second);
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
