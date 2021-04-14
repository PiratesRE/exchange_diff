using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class QueryResultEnumerator<T, U, V, W> : QueryResultEnumeratorBase, IEnumerable<Quad<T, U, V, W>>, IEnumerable
	{
		private QueryResultEnumerator(QueryResult queryResult) : base(queryResult)
		{
		}

		private QueryResultEnumerator(QueryResult queryResult, int fetchCount) : base(queryResult, fetchCount)
		{
		}

		public static QueryResultEnumerator<T, U, V, W> From(QueryResult queryResult)
		{
			return new QueryResultEnumerator<T, U, V, W>(queryResult);
		}

		public static QueryResultEnumerator<T, U, V, W> From(QueryResult queryResult, int fetchCount)
		{
			return new QueryResultEnumerator<T, U, V, W>(queryResult, fetchCount);
		}

		public IEnumerator<Quad<T, U, V, W>> GetEnumerator()
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
					V third = default(V);
					W fourth = default(W);
					if (rowResults[i][0] != null && !(rowResults[i][0] is PropertyError))
					{
						first = (T)((object)rowResults[i][0]);
					}
					if (rowResults[i][1] != null && !(rowResults[i][1] is PropertyError))
					{
						second = (U)((object)rowResults[i][1]);
					}
					if (rowResults[i][2] != null && !(rowResults[i][2] is PropertyError))
					{
						third = (V)((object)rowResults[i][2]);
					}
					if (rowResults[i][3] != null && !(rowResults[i][3] is PropertyError))
					{
						fourth = (W)((object)rowResults[i][3]);
					}
					yield return new Quad<T, U, V, W>(first, second, third, fourth);
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
