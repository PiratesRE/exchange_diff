using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class QueryList : IEnumerable<BaseQuery>, IEnumerable
	{
		public QueryList() : this(new List<BaseQuery>())
		{
		}

		public QueryList(int count) : this(new List<BaseQuery>(count))
		{
		}

		public QueryList(QueryList existing) : this(new List<BaseQuery>(existing))
		{
		}

		public QueryList(params BaseQuery[] baseQueries) : this(new List<BaseQuery>(baseQueries))
		{
		}

		IEnumerator<BaseQuery> IEnumerable<BaseQuery>.GetEnumerator()
		{
			return this.queryList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.queryList.GetEnumerator();
		}

		public void Add(BaseQuery baseQuery)
		{
			this.queryList.Add(baseQuery);
		}

		public int Count
		{
			get
			{
				return this.queryList.Count;
			}
		}

		public BaseQuery[] ToArray()
		{
			return this.queryList.ToArray();
		}

		public BaseQuery this[int i]
		{
			get
			{
				return this.queryList[i];
			}
		}

		public void SetResultInAllQueries(BaseQueryResult result)
		{
			foreach (BaseQuery baseQuery in this.queryList)
			{
				if (baseQuery.SetResultOnFirstCall(result))
				{
					QueryList.RequestRoutingTracer.TraceError<object, EmailAddress, BaseQueryResult>((long)this.GetHashCode(), "{0}: the following result was set for query {1}: {2}", TraceContext.Get(), baseQuery.Email, result);
				}
			}
		}

		public void LogLatency(string key, long value)
		{
			foreach (BaseQuery baseQuery in this.queryList)
			{
				baseQuery.LogLatency(key, value);
			}
		}

		public BaseQuery[] FindByEmailAddress(string email)
		{
			List<BaseQuery> list = this.queryList.FindAll((BaseQuery query) => StringComparer.OrdinalIgnoreCase.Equals(query.Email.Address, email));
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			throw new ArgumentException(string.Format("The recipient with email address {0} was not found in the request", email), "email");
		}

		private QueryList(List<BaseQuery> queryList)
		{
			this.queryList = queryList;
		}

		private List<BaseQuery> queryList;

		private static readonly Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;
	}
}
