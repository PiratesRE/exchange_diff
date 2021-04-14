using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal static class QueryResultExtensions
	{
		public static QueryResultEnumerator Enumerator(this QueryResult queryResult)
		{
			return QueryResultEnumerator.From(queryResult);
		}

		public static QueryResultEnumerator Enumerator(this QueryResult queryResult, int fetchCount)
		{
			return QueryResultEnumerator.From(queryResult, fetchCount);
		}

		public static QueryResultEnumerator<T> Enumerator<T>(this QueryResult queryResult)
		{
			return QueryResultEnumerator<T>.From(queryResult);
		}

		public static QueryResultEnumerator<T> Enumerator<T>(this QueryResult queryResult, int fetchCount)
		{
			return QueryResultEnumerator<T>.From(queryResult, fetchCount);
		}

		public static QueryResultEnumerator<T, U> Enumerator<T, U>(this QueryResult queryResult)
		{
			return QueryResultEnumerator<T, U>.From(queryResult);
		}

		public static QueryResultEnumerator<T, U> Enumerator<T, U>(this QueryResult queryResult, int fetchCount)
		{
			return QueryResultEnumerator<T, U>.From(queryResult, fetchCount);
		}

		public static QueryResultEnumerator<T, U, V> Enumerator<T, U, V>(this QueryResult queryResult)
		{
			return QueryResultEnumerator<T, U, V>.From(queryResult);
		}

		public static QueryResultEnumerator<T, U, V> Enumerator<T, U, V>(this QueryResult queryResult, int fetchCount)
		{
			return QueryResultEnumerator<T, U, V>.From(queryResult, fetchCount);
		}

		public static QueryResultEnumerator<T, U, V, W> Enumerator<T, U, V, W>(this QueryResult queryResult)
		{
			return QueryResultEnumerator<T, U, V, W>.From(queryResult);
		}

		public static QueryResultEnumerator<T, U, V, W> Enumerator<T, U, V, W>(this QueryResult queryResult, int fetchCount)
		{
			return QueryResultEnumerator<T, U, V, W>.From(queryResult, fetchCount);
		}
	}
}
