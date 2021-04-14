using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.InfoWorker.EventLog;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class AggregatedSearchTaskResult : ISearchTaskResult, ISearchResult
	{
		internal AggregatedSearchTaskResult(MailboxInfoList mailboxInfoList, SortedResultPage resultPage, Dictionary<string, List<IRefinerResult>> refinerResults, ulong totalResultCount, ByteQuantifiedSize totalResultSize, List<Pair<MailboxInfo, Exception>> previewFailures, List<MailboxStatistics> mailboxStatistics, IProtocolLog protocolLog) : this(SearchType.Preview, true, mailboxInfoList, resultPage, refinerResults, mailboxStatistics, protocolLog, totalResultCount, totalResultSize, previewFailures, null, null)
		{
		}

		internal AggregatedSearchTaskResult(MailboxInfoList mailboxInfoList, List<Pair<MailboxInfo, Exception>> previewFailures) : this(SearchType.Preview, false, mailboxInfoList, null, null, null, null, 0UL, ByteQuantifiedSize.Zero, previewFailures, null, null)
		{
		}

		internal AggregatedSearchTaskResult(MailboxInfoList mailboxInfoList, Exception searchError) : this(SearchType.Preview, false, mailboxInfoList, null, null, null, null, 0UL, ByteQuantifiedSize.Zero, null, null, searchError)
		{
		}

		internal AggregatedSearchTaskResult(MailboxInfoList mailboxInfoList, List<IKeywordHit> keywordHitList, ulong totalResultCount, ByteQuantifiedSize totalResultSize) : this(SearchType.Statistics, true, mailboxInfoList, null, null, null, null, totalResultCount, totalResultSize, null, keywordHitList, null)
		{
		}

		internal AggregatedSearchTaskResult(MailboxInfoList mailboxInfoList, List<IKeywordHit> keywordHitList) : this(mailboxInfoList, keywordHitList, 0UL, ByteQuantifiedSize.Zero)
		{
		}

		internal AggregatedSearchTaskResult(MailboxInfoList mailboxInfoList, List<string> keywords, Exception exception) : this(SearchType.Statistics, false, mailboxInfoList, null, null, null, null, 0UL, ByteQuantifiedSize.Zero, null, null, exception)
		{
			Util.ThrowOnNull(keywords, "keywords");
			Util.ThrowOnNull(exception, "exception");
			this.keywordStatisticsResult = new Dictionary<string, IKeywordHit>(keywords.Count, StringComparer.InvariantCultureIgnoreCase);
			foreach (string text in keywords)
			{
				IKeywordHit keywordHit = new KeywordHit(text, this.totalResultCount, this.totalResultSize);
				foreach (MailboxInfo first in this.mailboxInfoList)
				{
					keywordHit.Errors.Add(new Pair<MailboxInfo, Exception>(first, exception));
				}
				this.keywordStatisticsResult.Add(text, keywordHit);
			}
		}

		private AggregatedSearchTaskResult(SearchType searchType, bool isSuccess, MailboxInfoList mailboxInfoList, SortedResultPage previewResultPage, Dictionary<string, List<IRefinerResult>> refinerResult, List<MailboxStatistics> mailboxStatistics, IProtocolLog protocolLog, ulong totalResultCount, ByteQuantifiedSize totalResultSize, List<Pair<MailboxInfo, Exception>> previewFailures, List<IKeywordHit> keywordStatsResults, Exception error)
		{
			Util.ThrowOnNull(mailboxInfoList, "mailboxInfoList");
			if (mailboxInfoList.Count == 0)
			{
				throw new ArgumentException("Invalid or empty mailboxInfoList");
			}
			this.resultType = searchType;
			this.mailboxInfoList = mailboxInfoList;
			this.success = isSuccess;
			this.exception = error;
			this.totalResultCount = totalResultCount;
			this.totalResultSize = totalResultSize;
			this.protocolLog = protocolLog;
			if (searchType == SearchType.Preview)
			{
				if (this.success)
				{
					if (totalResultCount > 0UL)
					{
						Util.ThrowOnNull(previewResultPage, "resultPage");
					}
					if (previewResultPage != null && previewResultPage.ResultCount > 0)
					{
						if (totalResultCount < (ulong)((long)previewResultPage.ResultCount))
						{
							Factory.Current.LocalTaskTracer.TraceError<string, Guid>((long)this.GetHashCode(), "The total result count was less than the current page result count for the mailbox:{0} on database:{1}", this.mailboxInfoList[0].MailboxGuid.ToString(), this.mailboxInfoList[0].MdbGuid);
							throw new ArgumentException("The totalResultCount must be greater than or equal to the current page result count");
						}
						if (totalResultSize == ByteQuantifiedSize.Zero)
						{
							Factory.Current.LocalTaskTracer.TraceError<string, string>((long)this.GetHashCode(), "There are results from FAST but the size information was not returned from FAST for the mailbox:{0} on database:{1}", this.mailboxInfoList[0].MailboxGuid.ToString(), this.mailboxInfoList[0].MdbGuid.ToString());
							Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoveryFailedToFetchSizeInformation, null, new object[]
							{
								this.mailboxInfoList[0].MailboxGuid.ToString(),
								this.mailboxInfoList[0].MdbGuid.ToString()
							});
						}
					}
					this.previewSearchResultPage = previewResultPage;
					this.refinersResults = refinerResult;
					Util.ThrowOnNull(mailboxStatistics, "mailboxStatistics");
					if (this.previewSearchResultPage != null && this.previewSearchResultPage.ResultCount > 0 && mailboxStatistics.Count == 0)
					{
						throw new ArgumentException("The MailboxStats cannot be an empty list when the results are present.");
					}
					this.mailboxStatistics = mailboxStatistics;
				}
				this.previewErrors = new List<Pair<MailboxInfo, Exception>>(this.mailboxInfoList.Count);
				if (this.exception != null)
				{
					foreach (MailboxInfo first in this.mailboxInfoList)
					{
						this.previewErrors.Add(new Pair<MailboxInfo, Exception>(first, this.exception));
					}
				}
				if (previewFailures != null)
				{
					this.previewErrors.AddRange(previewFailures);
					return;
				}
			}
			else if (searchType == SearchType.Statistics && isSuccess)
			{
				Util.ThrowOnNull(keywordStatsResults, null);
				this.keywordStatisticsResult = new Dictionary<string, IKeywordHit>(keywordStatsResults.Count, StringComparer.InvariantCultureIgnoreCase);
				foreach (IKeywordHit keywordHit in keywordStatsResults)
				{
					IKeywordHit keywordHit2 = null;
					if (!this.keywordStatisticsResult.TryGetValue(keywordHit.Phrase, out keywordHit2))
					{
						this.keywordStatisticsResult.Add(keywordHit.Phrase, keywordHit);
					}
				}
			}
		}

		public SearchType ResultType
		{
			get
			{
				return this.resultType;
			}
		}

		public SortedResultPage PreviewResult
		{
			get
			{
				return this.previewSearchResultPage;
			}
		}

		public List<Pair<MailboxInfo, Exception>> PreviewErrors
		{
			get
			{
				return this.previewErrors;
			}
		}

		public bool Success
		{
			get
			{
				return this.success;
			}
		}

		public IProtocolLog ProtocolLog
		{
			get
			{
				return this.protocolLog;
			}
		}

		public Dictionary<string, List<IRefinerResult>> RefinersResult
		{
			get
			{
				return this.refinersResults;
			}
		}

		public IDictionary<string, IKeywordHit> KeywordStatistics
		{
			get
			{
				return this.keywordStatisticsResult;
			}
		}

		public ulong TotalResultCount
		{
			get
			{
				return this.totalResultCount;
			}
		}

		public ByteQuantifiedSize TotalResultSize
		{
			get
			{
				return this.totalResultSize;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public List<MailboxStatistics> MailboxStats
		{
			get
			{
				return this.mailboxStatistics;
			}
		}

		internal MailboxInfoList MailboxInfoList
		{
			get
			{
				return this.mailboxInfoList;
			}
		}

		public void MergeSearchResult(ISearchResult result)
		{
			throw new NotImplementedException();
		}

		private readonly SortedResultPage previewSearchResultPage;

		private readonly MailboxInfoList mailboxInfoList;

		private readonly Exception exception;

		private readonly bool success;

		private readonly SearchType resultType;

		private readonly Dictionary<string, List<IRefinerResult>> refinersResults;

		private readonly ulong totalResultCount;

		private readonly ByteQuantifiedSize totalResultSize;

		private readonly List<MailboxStatistics> mailboxStatistics;

		private readonly List<Pair<MailboxInfo, Exception>> previewErrors;

		private readonly Dictionary<string, IKeywordHit> keywordStatisticsResult;

		private readonly IProtocolLog protocolLog;
	}
}
