using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class QueryResultEnumerator<T> : QueryResultEnumeratorBase, IEnumerable<T>, IEnumerable
	{
		private QueryResultEnumerator(QueryResult queryResult) : base(queryResult)
		{
		}

		private QueryResultEnumerator(QueryResult queryResult, int fetchCount) : base(queryResult, fetchCount)
		{
		}

		public static QueryResultEnumerator<T> From(QueryResult queryResult)
		{
			return new QueryResultEnumerator<T>(queryResult);
		}

		public static QueryResultEnumerator<T> From(QueryResult queryResult, int fetchCount)
		{
			return new QueryResultEnumerator<T>(queryResult, fetchCount);
		}

		public IEnumerator<T> GetEnumerator()
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
					if (rowResults[i][0] != null && !(rowResults[i][0] is PropertyError))
					{
						yield return (T)((object)rowResults[i][0]);
					}
					else
					{
						yield return default(T);
					}
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
