using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class MultiMailboxSearch : DisposeTrackableBase
	{
		public MultiMailboxSearch(SearchCriteria criteria, List<MailboxInfo> users, PagingInfo pagingInfo, CallerInfo callerInfo, OrganizationId orgId)
		{
			Util.ThrowOnNull(criteria, "criteria");
			Util.ThrowOnNull(users, "users");
			Util.ThrowOnNull(pagingInfo, "pagingInfo");
			Util.ThrowOnNull(callerInfo, "callerInfo");
			this.mailboxes = users;
			this.criteria = criteria;
			this.pagingInfo = pagingInfo;
			this.callerInfo = callerInfo;
			this.orgId = orgId;
			this.resultAggregator = new ResultAggregator(Factory.Current.GetMaxRefinerResults(this.criteria.RecipientSession));
		}

		public IAsyncResult BeginSearch(AsyncCallback callback, object state)
		{
			if (MultiMailboxSearch.SearchState.Running == this.searchState)
			{
				return this.asyncResult;
			}
			if (this.asyncResult != null)
			{
				this.asyncResult.Dispose();
				this.asyncResult = null;
			}
			this.asyncResult = new AsyncResult(callback, state);
			this.searchState = MultiMailboxSearch.SearchState.Running;
			this.IncrementPerfCounters();
			IEwsEndpointDiscovery ewsEndpointDiscovery = Factory.Current.GetEwsEndpointDiscovery(this.mailboxes, this.orgId, this.callerInfo);
			long num = 0L;
			long num2 = 0L;
			Dictionary<GroupId, List<MailboxInfo>> mailboxGroups = ewsEndpointDiscovery.FindEwsEndpoints(out num, out num2);
			this.resultAggregator.ProtocolLog.Add("LocalMailboxMappingTime", num);
			this.resultAggregator.ProtocolLog.Add("AutodiscoverTime", num2);
			this.StartGroupSearches(mailboxGroups);
			return this.asyncResult;
		}

		public ISearchResult EndSearch(IAsyncResult result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			if (!this.asyncResult.Equals(result))
			{
				throw new ArgumentException("result doesn't match with the search's result");
			}
			this.asyncResult.AsyncWaitHandle.WaitOne();
			this.searchState = MultiMailboxSearch.SearchState.Completed;
			this.UpdatePerfCountersBasedOnResults();
			return this.resultAggregator;
		}

		public void AbortSearch()
		{
			if (this.searchState != MultiMailboxSearch.SearchState.Running)
			{
				return;
			}
			foreach (MailboxSearchGroup mailboxSearchGroup in this.groups)
			{
				mailboxSearchGroup.Abort();
			}
			this.searchState = MultiMailboxSearch.SearchState.Stopped;
		}

		private static void IncrementGroupPerfCounters(int localSearchCount, int remoteSearchCount, int searchGroupCount)
		{
			PerformanceCounters.AverageSearchGroupCreated.IncrementBy((long)searchGroupCount);
			PerformanceCounters.AverageSearchGroupCreatedBase.IncrementBy((long)searchGroupCount);
			if (localSearchCount > 0)
			{
				PerformanceCounters.TotalLocalSearches.IncrementBy((long)localSearchCount);
				PerformanceCounters.AverageLocalSearchGroupCreated.IncrementBy((long)localSearchCount);
				PerformanceCounters.AverageLocalSearchGroupCreatedBase.IncrementBy((long)localSearchCount);
			}
			if (remoteSearchCount > 0)
			{
				PerformanceCounters.TotalFanOutSearches.IncrementBy((long)remoteSearchCount);
				PerformanceCounters.AverageFanOutSearchGroupCreated.IncrementBy((long)remoteSearchCount);
				PerformanceCounters.AverageFanOutSearchGroupCreatedBase.IncrementBy((long)remoteSearchCount);
			}
		}

		private void StartGroupSearches(Dictionary<GroupId, List<MailboxInfo>> mailboxGroups)
		{
			int num = 0;
			int num2 = 0;
			this.groups = new List<MailboxSearchGroup>(mailboxGroups.Count);
			foreach (KeyValuePair<GroupId, List<MailboxInfo>> keyValuePair in mailboxGroups)
			{
				if (keyValuePair.Key.GroupType == GroupType.Local)
				{
					num++;
				}
				else if (keyValuePair.Key.GroupType == GroupType.CrossServer)
				{
					num2++;
				}
				if (keyValuePair.Key.GroupType != GroupType.SkippedError)
				{
					if (keyValuePair.Key.GroupType != GroupType.Local && Util.IsNestedFanoutCall(this.callerInfo))
					{
						this.AddNestedFanoutMailboxesToPreviewErrors(keyValuePair.Value);
					}
					else
					{
						this.groups.Add(Factory.Current.CreateMailboxSearchGroup(keyValuePair.Key, keyValuePair.Value, this.criteria, this.pagingInfo, this.callerInfo));
					}
				}
				else
				{
					this.AddSkippedMailboxesToPreviewErrors(keyValuePair.Value, keyValuePair.Key.Error);
				}
			}
			this.resultAggregator.ProtocolLog.Add("NumberOfRemoteSearch", num2);
			MultiMailboxSearch.IncrementGroupPerfCounters(num, num2, mailboxGroups.Count);
			try
			{
				foreach (MailboxSearchGroup mailboxSearchGroup in this.groups)
				{
					mailboxSearchGroup.BeginExecuteSearch(new AsyncCallback(this.MailboxSearchGroupCompleted), mailboxSearchGroup);
					this.scheduledGroups++;
				}
			}
			catch (MultiMailboxSearchException ex)
			{
				Factory.Current.GeneralTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Hit an unexpected exception during search execution.", this.callerInfo.QueryCorrelationId, ex.ToString());
			}
			finally
			{
				if (this.groups.Count == 0)
				{
					this.asyncResult.ReportCompletion();
				}
			}
		}

		private void MailboxSearchGroupCompleted(IAsyncResult result)
		{
			MailboxSearchGroup mailboxSearchGroup = (MailboxSearchGroup)result.AsyncState;
			ISearchResult aggregator = mailboxSearchGroup.EndExecuteSearch(result);
			this.resultAggregator.MergeSearchResult(aggregator);
			lock (this.locker)
			{
				this.completedGroups++;
				if (this.completedGroups == this.scheduledGroups && this.asyncResult != null)
				{
					this.asyncResult.ReportCompletion();
				}
				mailboxSearchGroup.Dispose();
			}
		}

		private void AddSkippedMailboxesToPreviewErrors(List<MailboxInfo> mailboxes, Exception exception)
		{
			for (int i = 0; i < mailboxes.Count; i++)
			{
				this.resultAggregator.PreviewErrors.Add(new Pair<MailboxInfo, Exception>(mailboxes[i], exception));
			}
		}

		private void AddNestedFanoutMailboxesToPreviewErrors(List<MailboxInfo> mailboxes)
		{
			for (int i = 0; i < mailboxes.Count; i++)
			{
				this.resultAggregator.PreviewErrors.Add(new Pair<MailboxInfo, Exception>(mailboxes[i], new NestedFanoutException(mailboxes[i].DisplayName)));
			}
		}

		private void IncrementPerfCounters()
		{
			PerformanceCounters.TotalSearches.Increment();
			PerformanceCounters.TotalSearchesInProgress.Increment();
			PerformanceCounters.TotalRequestsPerSecond.Increment();
			switch (this.criteria.SearchType)
			{
			case SearchType.Statistics:
				PerformanceCounters.StatisticsRequestsPerSecond.Increment();
				break;
			case SearchType.Preview:
				PerformanceCounters.PreviewRequestsPerSecond.Increment();
				break;
			case SearchType.ExpandSources:
				PerformanceCounters.PreviewAndStatisticsRequestsPerSecond.Increment();
				break;
			}
			PerformanceCounters.AverageMailboxCountPerQuery.IncrementBy((long)this.mailboxes.Count);
			this.GetMailboxBucketPerfCounter(this.mailboxes.Count).Increment();
			long incrementValue = (long)((this.criteria.SubFilters == null) ? 0 : this.criteria.SubFilters.Count);
			PerformanceCounters.AverageKeywordsCountPerQuery.IncrementBy(incrementValue);
			this.GetKeywordsBucketPerfCounter(this.criteria.Query.Keywords().Count<string>()).Increment();
			this.searchTimer.Restart();
		}

		private ExPerformanceCounter GetKeywordsBucketPerfCounter(int keywordCount)
		{
			ExPerformanceCounter result = PerformanceCounters.TotalSearchesGreaterThan300Keywords;
			if (keywordCount == MultiMailboxSearch.KeywordsBuckets[0])
			{
				result = PerformanceCounters.TotalSearchesWithNoKeywords;
			}
			else if (keywordCount > MultiMailboxSearch.KeywordsBuckets[0] && keywordCount < MultiMailboxSearch.KeywordsBuckets[1])
			{
				result = PerformanceCounters.TotalSearchesBetween1To10Keywords;
			}
			else if (keywordCount >= MultiMailboxSearch.KeywordsBuckets[1] && keywordCount < MultiMailboxSearch.KeywordsBuckets[2])
			{
				result = PerformanceCounters.TotalSearchesBetween10To20Keywords;
			}
			else if (keywordCount >= MultiMailboxSearch.KeywordsBuckets[2] && keywordCount < MultiMailboxSearch.KeywordsBuckets[3])
			{
				result = PerformanceCounters.TotalSearchesBetween20To50Keywords;
			}
			else if (keywordCount >= MultiMailboxSearch.KeywordsBuckets[3] && keywordCount < MultiMailboxSearch.KeywordsBuckets[4])
			{
				result = PerformanceCounters.TotalSearchesBetween50To100Keywords;
			}
			else if (keywordCount >= MultiMailboxSearch.KeywordsBuckets[4] && keywordCount < MultiMailboxSearch.KeywordsBuckets[5])
			{
				result = PerformanceCounters.TotalSearchesBetween100To300Keywords;
			}
			return result;
		}

		private ExPerformanceCounter GetMailboxBucketPerfCounter(int mailboxCount)
		{
			ExPerformanceCounter result = PerformanceCounters.TotalSearchesGreaterThan700Mailboxes;
			if (mailboxCount < MultiMailboxSearch.MailboxBuckets[0])
			{
				result = PerformanceCounters.TotalSearchesBelow5Mailboxes;
			}
			else if (mailboxCount >= MultiMailboxSearch.MailboxBuckets[0] && mailboxCount < MultiMailboxSearch.MailboxBuckets[1])
			{
				result = PerformanceCounters.TotalSearchesBetween5To10Mailboxes;
			}
			else if (mailboxCount >= MultiMailboxSearch.MailboxBuckets[1] && mailboxCount < MultiMailboxSearch.MailboxBuckets[2])
			{
				result = PerformanceCounters.TotalSearchesBetween10To50Mailboxes;
			}
			else if (mailboxCount >= MultiMailboxSearch.MailboxBuckets[2] && mailboxCount < MultiMailboxSearch.MailboxBuckets[3])
			{
				result = PerformanceCounters.TotalSearchesBetween50To100Mailboxes;
			}
			else if (mailboxCount >= MultiMailboxSearch.MailboxBuckets[3] && mailboxCount < MultiMailboxSearch.MailboxBuckets[4])
			{
				result = PerformanceCounters.TotalSearchesBetween100To400Mailboxes;
			}
			else if (mailboxCount >= MultiMailboxSearch.MailboxBuckets[4] && mailboxCount < MultiMailboxSearch.MailboxBuckets[5])
			{
				result = PerformanceCounters.TotalSearchesBetween400To700Mailboxes;
			}
			return result;
		}

		private void UpdatePerfCountersBasedOnResults()
		{
			this.searchTimer.Stop();
			PerformanceCounters.TotalSearchesInProgress.Decrement();
			MultiMailboxSearch.GetSearchLatencyPerfCounter(this.criteria.SearchType, (int)this.searchTimer.ElapsedMilliseconds).Increment();
			switch (this.criteria.SearchType)
			{
			case SearchType.Statistics:
				PerformanceCounters.AverageStatisticsRequestProcessingTime.IncrementBy(this.searchTimer.ElapsedTicks);
				PerformanceCounters.AverageStatisticsRequestProcessingTimeBase.Increment();
				return;
			case SearchType.Preview:
				PerformanceCounters.AveragePreviewRequestProcessingTime.IncrementBy(this.searchTimer.ElapsedTicks);
				PerformanceCounters.AveragePreviewRequestProcessingTimeBase.Increment();
				PerformanceCounters.AverageFailedMailboxesInPreview.IncrementBy((long)this.resultAggregator.PreviewErrors.Count);
				PerformanceCounters.AverageFailedMailboxesInPreviewBase.Increment();
				return;
			case SearchType.ExpandSources:
				PerformanceCounters.AveragePreviewAndStatisticsRequestProcessingTime.IncrementBy(this.searchTimer.ElapsedTicks);
				PerformanceCounters.AveragePreviewAndStatisticsRequestProcessingTimeBase.Increment();
				PerformanceCounters.AverageFailedMailboxesInPreview.IncrementBy((long)this.resultAggregator.PreviewErrors.Count);
				PerformanceCounters.AverageFailedMailboxesInPreviewBase.Increment();
				return;
			default:
				return;
			}
		}

		internal static ExPerformanceCounter GetSearchLatencyPerfCounter(SearchType searchType, int searchTimeInMilliSeconds)
		{
			ExPerformanceCounter result = (searchType == SearchType.Preview) ? PerformanceCounters.TotalPreviewSearchesGreaterThan120Seconds : PerformanceCounters.TotalStatsSearchesGreaterThan120Seconds;
			if (searchTimeInMilliSeconds < MultiMailboxSearch.SearchLatencyBuckets[0])
			{
				result = ((searchType == SearchType.Preview) ? PerformanceCounters.TotalPreviewSearchesBelow500msec : PerformanceCounters.TotalStatsSearchesBelow500msec);
			}
			else if (searchTimeInMilliSeconds >= MultiMailboxSearch.SearchLatencyBuckets[0] && searchTimeInMilliSeconds < MultiMailboxSearch.SearchLatencyBuckets[1])
			{
				result = ((searchType == SearchType.Preview) ? PerformanceCounters.TotalPreviewSearchesBetween500msecTo2sec : PerformanceCounters.TotalStatsSearchesBetween500msecTo2sec);
			}
			else if (searchTimeInMilliSeconds >= MultiMailboxSearch.SearchLatencyBuckets[1] && searchTimeInMilliSeconds < MultiMailboxSearch.SearchLatencyBuckets[2])
			{
				result = ((searchType == SearchType.Preview) ? PerformanceCounters.TotalPreviewSearchesBetween2To10sec : PerformanceCounters.TotalStatsSearchesBetween2To10sec);
			}
			else if (searchTimeInMilliSeconds >= MultiMailboxSearch.SearchLatencyBuckets[2] && searchTimeInMilliSeconds < MultiMailboxSearch.SearchLatencyBuckets[3])
			{
				result = ((searchType == SearchType.Preview) ? PerformanceCounters.TotalPreviewSearchesBetween10SecTo60Sec : PerformanceCounters.TotalStatsSearchesBetween10SecTo60Sec);
			}
			else if (searchTimeInMilliSeconds >= MultiMailboxSearch.SearchLatencyBuckets[3] && searchTimeInMilliSeconds < MultiMailboxSearch.SearchLatencyBuckets[4])
			{
				result = ((searchType == SearchType.Preview) ? PerformanceCounters.TotalPreviewSearchesBetween60SecTo120Sec : PerformanceCounters.TotalStatsSearchesBetween60SecTo120Sec);
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.searchState == MultiMailboxSearch.SearchState.Running)
				{
					if (this.groups != null)
					{
						foreach (MailboxSearchGroup mailboxSearchGroup in this.groups)
						{
							mailboxSearchGroup.Dispose();
						}
					}
					this.searchState = MultiMailboxSearch.SearchState.Stopped;
				}
				if (this.asyncResult != null)
				{
					this.asyncResult.Dispose();
					this.asyncResult = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MultiMailboxSearch>(this);
		}

		private readonly Stopwatch searchTimer = new Stopwatch();

		private readonly List<MailboxInfo> mailboxes;

		private readonly PagingInfo pagingInfo;

		private readonly SearchCriteria criteria;

		private readonly OrganizationId orgId;

		private List<MailboxSearchGroup> groups;

		private ISearchResult resultAggregator;

		private int scheduledGroups;

		private int completedGroups;

		private CallerInfo callerInfo;

		private object locker = new object();

		private AsyncResult asyncResult;

		private MultiMailboxSearch.SearchState searchState;

		internal static readonly int[] SearchLatencyBuckets = new int[]
		{
			500,
			TimeSpan.FromSeconds(2.0).Milliseconds,
			TimeSpan.FromSeconds(10.0).Milliseconds,
			TimeSpan.FromMinutes(1.0).Milliseconds,
			TimeSpan.FromMinutes(2.0).Milliseconds
		};

		private static readonly int[] KeywordsBuckets = new int[]
		{
			0,
			10,
			20,
			50,
			100,
			300
		};

		private static readonly int[] MailboxBuckets = new int[]
		{
			5,
			10,
			50,
			100,
			400,
			700
		};

		private enum SearchState
		{
			NotStarted,
			Running,
			Stopped,
			Completed
		}
	}
}
