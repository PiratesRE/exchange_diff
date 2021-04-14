using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class AggregatedMailboxSearchTask : DisposeTrackableBase, ISearchMailboxTask
	{
		public AggregatedMailboxSearchTask(Guid databaseGuid, MailboxInfoList mailboxesToSearch, SearchCriteria criteria, PagingInfo pagingInfo, List<string> keywordList, CallerInfo executingUser) : this(databaseGuid, mailboxesToSearch, SearchType.Statistics, criteria, pagingInfo, executingUser)
		{
			Util.ThrowOnNull(keywordList, "keywordList");
			if (keywordList.Count == 0)
			{
				throw new ArgumentException("AggregatedMailboxSearchTask: The keyword list for Stats search cannot be an empty list.");
			}
			this.keywordList = keywordList;
		}

		public AggregatedMailboxSearchTask(Guid databaseGuid, MailboxInfoList mailboxesToSearch, SearchType type, SearchCriteria criteria, PagingInfo pagingInfo, CallerInfo executingUser)
		{
			Util.ThrowOnNull(databaseGuid, "databaseGuid");
			if (databaseGuid.Equals(Guid.Empty))
			{
				throw new ArgumentNullException("databaseGuid");
			}
			Util.ThrowOnNull(mailboxesToSearch, "mailboxesToSearch");
			Util.ThrowOnNull(criteria, "criteria");
			Util.ThrowOnNull(pagingInfo, "pagingInfo");
			Util.ThrowOnNull(executingUser, "executingUser");
			if ((type & (SearchType)(-4)) != (SearchType)0 || (type & SearchType.ExpandSources) == SearchType.ExpandSources)
			{
				throw new ArgumentException("AggregatedMailboxSearchTask: the task can either be a preview task or a statistics task");
			}
			if ((type & SearchType.ExpandSources) == SearchType.ExpandSources)
			{
				throw new ArgumentException("AggregatedMailboxSearchTask: the task can either be a preview task or a statistics task");
			}
			this.mailboxesToSearch = mailboxesToSearch;
			this.mailboxDatabaseId = databaseGuid;
			this.type = type;
			this.criteria = criteria;
			this.pagingInfo = pagingInfo;
			this.executingUser = executingUser;
		}

		internal PagingInfo PagingInfo
		{
			get
			{
				return this.pagingInfo;
			}
		}

		internal CallerInfo ExecutingUserIdentity
		{
			get
			{
				return this.executingUser;
			}
		}

		internal SearchCriteria SearchCriteria
		{
			get
			{
				return this.criteria;
			}
		}

		internal MailboxInfoList MailboxInfoList
		{
			get
			{
				return this.mailboxesToSearch;
			}
		}

		public MailboxInfo Mailbox
		{
			get
			{
				return null;
			}
		}

		public SearchType Type
		{
			get
			{
				return this.type;
			}
		}

		internal virtual MultiMailboxSearchClient RpcClient
		{
			get
			{
				return this.rpcSearchClient;
			}
		}

		internal Guid MailboxDatabaseGuid
		{
			get
			{
				return this.mailboxDatabaseId;
			}
		}

		public Dictionary<string, Dictionary<string, string>> SearchStatistics { get; private set; }

		public long FastTime { get; set; }

		public long StoreTime { get; set; }

		public long RestrictionTime { get; set; }

		public long TotalItems { get; set; }

		public long TotalSize { get; set; }

		public long ReturnedFastItems { get; set; }

		public long ReturnedStoreItems { get; set; }

		public long ReturnedStoreSize { get; set; }

		public void Execute(SearchCompletedCallback searchCallback)
		{
			if (searchCallback == null)
			{
				throw new ArgumentNullException("searchCallback");
			}
			this.InvokeMethodAndHandleExceptions(delegate
			{
				this.attempts++;
				this.callback = searchCallback;
				AggregatedSearchTaskResult result = null;
				this.InitRpcSearchClient();
				Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Executing {1} search for the queryFilter:{2} on {3} mailboxes in database:{4}", new object[]
				{
					this.ExecutingUserIdentity.QueryCorrelationId,
					(this.RpcClient.Criteria.SearchType == SearchType.Preview) ? "preview" : "keyword stats",
					this.RpcClient.Criteria.QueryString,
					this.MailboxInfoList.Count,
					this.RpcClient.MailboxDatabaseGuid
				});
				if ((this.type & SearchType.Preview) == SearchType.Preview)
				{
					result = this.RpcClient.Search(Factory.Current.GetMaxRefinerResults(this.SearchCriteria.RecipientSession));
				}
				else if ((this.type & SearchType.Statistics) == SearchType.Statistics)
				{
					List<IKeywordHit> keywordHits = this.RpcClient.GetKeywordHits(this.keywordList);
					IKeywordHit keywordHit;
					if (keywordHits == null)
					{
						keywordHit = null;
					}
					else
					{
						keywordHit = keywordHits.Find((IKeywordHit x) => x.Phrase.Equals(this.SearchCriteria.QueryString, StringComparison.OrdinalIgnoreCase));
					}
					IKeywordHit keywordHit2 = keywordHit;
					result = new AggregatedSearchTaskResult(this.MailboxInfoList, keywordHits, (keywordHit2 != null) ? keywordHit2.Count : 0UL, (keywordHit2 != null) ? keywordHit2.Size : ByteQuantifiedSize.Zero);
				}
				this.UpdateSearchStatistics();
				this.callback(this, result);
			});
		}

		public bool ShouldRetry(ISearchTaskResult taskResult)
		{
			AggregatedSearchTaskResult aggregatedSearchTaskResult = taskResult as AggregatedSearchTaskResult;
			if (aggregatedSearchTaskResult == null)
			{
				throw new ArgumentNullException("result");
			}
			if (this.type != aggregatedSearchTaskResult.ResultType)
			{
				throw new ArgumentException("result types don't match");
			}
			if (aggregatedSearchTaskResult.Success)
			{
				return false;
			}
			Exception exception = aggregatedSearchTaskResult.Exception;
			return this.attempts <= 3 && exception != null && (exception is DiscoverySearchMaxSearchesExceeded || exception is DiscoveryKeywordStatsSearchTimedOut || exception is DiscoverySearchTimedOut || exception is StorageTransientException || exception is ADTransientException || exception is SearchTransientException);
		}

		public ISearchTaskResult GetErrorResult(Exception ex)
		{
			if ((this.type & SearchType.Statistics) == SearchType.Statistics)
			{
				List<IKeywordHit> list = new List<IKeywordHit>(1);
				List<string> list2 = new List<string>(1);
				list2.Add(this.criteria.QueryString);
				if (this.criteria.SubFilters != null)
				{
					list2.AddRange(this.criteria.SubFilters.Keys);
				}
				foreach (string phrase in list2)
				{
					IKeywordHit keywordHit = null;
					foreach (MailboxInfo mailbox in this.MailboxInfoList)
					{
						if (keywordHit == null)
						{
							keywordHit = new KeywordHit(phrase, mailbox, ex);
						}
						else
						{
							keywordHit.Merge(new KeywordHit(phrase, mailbox, ex));
						}
					}
					list.Add(keywordHit);
				}
				return new AggregatedSearchTaskResult(this.MailboxInfoList, list);
			}
			return new AggregatedSearchTaskResult(this.MailboxInfoList, ex);
		}

		public void Abort()
		{
			if (this.rpcSearchClient != null)
			{
				this.rpcSearchClient.Abort();
			}
		}

		internal virtual void InitRpcSearchClient()
		{
			if (this.rpcSearchClient == null)
			{
				this.rpcSearchClient = Factory.Current.CreateSearchRpcClient(this.mailboxDatabaseId, this.MailboxInfoList.ToArray(), this.criteria, this.executingUser, this.pagingInfo);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.rpcSearchClient != null)
			{
				this.rpcSearchClient.Dispose();
				this.rpcSearchClient = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AggregatedMailboxSearchTask>(this);
		}

		protected void InvokeMethodAndHandleExceptions(Util.MethodDelegate method)
		{
			Util.HandleExceptions(delegate
			{
				Exception ex = null;
				try
				{
					method();
				}
				catch (DiscoverySearchPermanentException ex2)
				{
					Factory.Current.LocalTaskTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Aggregated Search task failed. DiscoverySearchPermanentException: {1}", this.ExecutingUserIdentity.QueryCorrelationId, ex2.ToString());
					ex = ex2;
				}
				catch (MultiMailboxSearchException ex3)
				{
					Factory.Current.LocalTaskTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Aggregated Search task failed. MultiMailboxSearchException: {1}", this.ExecutingUserIdentity.QueryCorrelationId, ex3.ToString());
					ex = ex3;
				}
				catch (StoragePermanentException ex4)
				{
					Factory.Current.LocalTaskTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Aggregated Search task failed. StorageTransientException: {1}", this.ExecutingUserIdentity.QueryCorrelationId, ex4.ToString());
					ex = ex4;
				}
				catch (StorageTransientException ex5)
				{
					Factory.Current.LocalTaskTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Aggregated Search task failed. StoragePermanentException: {1}", this.ExecutingUserIdentity.QueryCorrelationId, ex5.ToString());
					ex = ex5;
				}
				catch (ADTransientException ex6)
				{
					Factory.Current.LocalTaskTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Aggregated Search task failed. ADTransientException: {1}", this.ExecutingUserIdentity.QueryCorrelationId, ex6.ToString());
					ex = ex6;
				}
				finally
				{
					if (ex != null)
					{
						AggregatedSearchTaskResult result = (AggregatedSearchTaskResult)this.GetErrorResult(ex);
						this.callback(this, result);
					}
				}
			}, delegate(GrayException ex)
			{
				AggregatedSearchTaskResult result = (AggregatedSearchTaskResult)this.GetErrorResult((ex.InnerException != null) ? ex.InnerException : ex);
				this.callback(this, result);
			});
		}

		private void UpdateSearchStatistics()
		{
			MultiMailboxSearchClient rpcClient = this.RpcClient;
			if (rpcClient != null)
			{
				this.SearchStatistics = rpcClient.SearchStatistics;
				this.FastTime = rpcClient.FastTime;
				this.StoreTime = rpcClient.StoreTime;
				this.RestrictionTime = rpcClient.RestrictionTime;
				this.TotalItems = rpcClient.TotalItems;
				this.TotalSize = rpcClient.TotalSize;
				this.ReturnedFastItems = rpcClient.ReturnedFastItems;
				this.ReturnedStoreItems = rpcClient.ReturnedStoreItems;
				this.ReturnedStoreSize = rpcClient.ReturnedStoreSize;
			}
		}

		private const int MaxRetryCount = 3;

		private readonly MailboxInfoList mailboxesToSearch;

		private readonly SearchType type;

		private readonly SearchCriteria criteria;

		private readonly PagingInfo pagingInfo;

		private readonly CallerInfo executingUser;

		private readonly List<string> keywordList;

		private int attempts;

		private readonly Guid mailboxDatabaseId;

		private MultiMailboxSearchClient rpcSearchClient;

		private SearchCompletedCallback callback;
	}
}
