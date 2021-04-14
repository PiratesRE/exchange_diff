using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class BaseRequestDispatcher : AsyncTask, IDisposable
	{
		public int CrossForestQueryCount
		{
			get
			{
				return this.crossForestQueryCount;
			}
		}

		public int FederatedCrossForestQueryCount
		{
			get
			{
				return this.federatedCrossForestQueryCount;
			}
		}

		public BaseRequestDispatcher()
		{
			this.queryListDictionary = new Dictionary<string, QueryList>();
			this.requests = new List<AsyncRequest>();
		}

		public void Add(string key, BaseQuery query, RequestType requestType, BaseRequestDispatcher.CreateRequestDelegate createRequestDelegate)
		{
			BaseRequestDispatcher.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Adding a proxy web request of type {1} for mailbox {2} to request key {3}", new object[]
			{
				TraceContext.Get(),
				requestType,
				query.Email,
				key
			});
			query.Type = new RequestType?(requestType);
			QueryList queryList;
			if (!this.queryListDictionary.TryGetValue(key, out queryList))
			{
				BaseRequestDispatcher.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: key {1} was not found. Creating new request for it", TraceContext.Get(), key);
				queryList = new QueryList();
				queryList.Add(query);
				this.requests.Add(createRequestDelegate(queryList));
				this.queryListDictionary.Add(key, queryList);
			}
			else
			{
				BaseRequestDispatcher.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: key {1} was found.", TraceContext.Get(), key);
				queryList.Add(query);
			}
			switch (requestType)
			{
			case RequestType.Local:
				PerformanceCounters.IntraSiteCalendarQueriesPerSecond.Increment();
				this.intraSiteQueryCount++;
				return;
			case RequestType.IntraSite:
				PerformanceCounters.IntraSiteProxyFreeBusyQueriesPerSecond.Increment();
				this.intraSiteProxyQueryCount++;
				return;
			case RequestType.CrossSite:
				PerformanceCounters.CrossSiteCalendarQueriesPerSecond.Increment();
				this.crossSiteQueryCount++;
				return;
			case RequestType.CrossForest:
				PerformanceCounters.CrossForestCalendarQueriesPerSecond.Increment();
				this.crossForestQueryCount++;
				return;
			case RequestType.FederatedCrossForest:
				PerformanceCounters.FederatedFreeBusyQueriesPerSecond.Increment();
				this.federatedCrossForestQueryCount++;
				return;
			case RequestType.PublicFolder:
				PerformanceCounters.PublicFolderQueriesPerSecond.Increment();
				this.publicFolderQueryCount++;
				return;
			default:
				return;
			}
		}

		protected ICollection<AsyncRequest> Requests
		{
			get
			{
				return this.requests;
			}
		}

		public override void Abort()
		{
			base.Abort();
			if (this.parallel != null)
			{
				this.parallel.Abort();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			this.queryListDictionary = null;
			if (this.requests.Count == 0)
			{
				BaseRequestDispatcher.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: No requests to dispatch.", new object[]
				{
					TraceContext.Get()
				});
				base.Complete();
				return;
			}
			BaseRequestDispatcher.RequestRoutingTracer.TraceDebug<object, int>((long)this.GetHashCode(), "{0}: dispatching {1} requests.", TraceContext.Get(), this.requests.Count);
			this.parallel = new AsyncTaskParallel(this.requests.ToArray());
			this.parallel.BeginInvoke(new TaskCompleteCallback(this.Complete));
		}

		private void Complete(AsyncTask task)
		{
			base.Complete();
		}

		public void Dispose()
		{
			foreach (AsyncRequest asyncRequest in this.requests)
			{
				IDisposable disposable = asyncRequest as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void LogStatistics(RequestLogger requestLogger)
		{
			if (this.intraSiteQueryCount > 0)
			{
				requestLogger.AppendToLog<int>("local", this.intraSiteQueryCount);
			}
			if (this.intraSiteProxyQueryCount > 0)
			{
				requestLogger.AppendToLog<int>("intrasiteproxy", this.intraSiteProxyQueryCount);
			}
			if (this.crossSiteQueryCount > 0)
			{
				requestLogger.AppendToLog<int>("x-site", this.crossSiteQueryCount);
			}
			if (this.crossForestQueryCount > 0)
			{
				requestLogger.AppendToLog<int>("x-forest", this.crossForestQueryCount);
			}
			if (this.federatedCrossForestQueryCount > 0)
			{
				requestLogger.AppendToLog<int>("federatedxforest", this.federatedCrossForestQueryCount);
			}
			if (this.publicFolderQueryCount > 0)
			{
				requestLogger.AppendToLog<int>("PF", this.publicFolderQueryCount);
			}
		}

		private Dictionary<string, QueryList> queryListDictionary;

		private List<AsyncRequest> requests;

		private AsyncTaskParallel parallel;

		private int intraSiteQueryCount;

		private int intraSiteProxyQueryCount;

		private int crossSiteQueryCount;

		private int crossForestQueryCount;

		private int publicFolderQueryCount;

		private int federatedCrossForestQueryCount;

		private static readonly Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;

		public delegate AsyncRequest CreateRequestDelegate(QueryList queryList);
	}
}
