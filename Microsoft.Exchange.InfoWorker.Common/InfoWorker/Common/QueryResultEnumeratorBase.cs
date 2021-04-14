using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class QueryResultEnumeratorBase
	{
		protected QueryResultEnumeratorBase(QueryResult queryResult) : this(queryResult, QueryResultEnumeratorBase.DefaultFetchCount)
		{
		}

		protected QueryResultEnumeratorBase(QueryResult queryResult, int fetchCount)
		{
			if (queryResult == null)
			{
				throw new ArgumentNullException("queryResult");
			}
			if (fetchCount <= 0)
			{
				throw new ArgumentException("fetchCount");
			}
			this.queryResult = queryResult;
			this.fetchCount = fetchCount;
		}

		internal QueryResult QueryResult
		{
			get
			{
				return this.queryResult;
			}
		}

		internal int FetchCount
		{
			get
			{
				return this.fetchCount;
			}
		}

		public static readonly int DefaultFetchCount = 512;

		private QueryResult queryResult;

		private int fetchCount;
	}
}
