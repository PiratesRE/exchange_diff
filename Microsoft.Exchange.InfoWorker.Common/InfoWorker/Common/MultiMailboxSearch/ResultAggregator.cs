using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MultiMailboxSearch;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class ResultAggregator : ISearchResult
	{
		public ResultAggregator() : this(0)
		{
		}

		public ResultAggregator(int refinerResultTrim) : this(null, new Dictionary<string, List<IRefinerResult>>(0), 0UL, ByteQuantifiedSize.Zero, new List<Pair<MailboxInfo, Exception>>(0), new Dictionary<string, IKeywordHit>(0), new List<MailboxStatistics>(4))
		{
			this.refinerResultsTrimCount = refinerResultTrim;
		}

		public ResultAggregator(SortedResultPage resultPage, Dictionary<string, List<IRefinerResult>> refinerResults, ulong totalResultCount, ByteQuantifiedSize totalResultSize, List<Pair<MailboxInfo, Exception>> previewErrors) : this(resultPage, refinerResults, totalResultCount, totalResultSize, previewErrors, new Dictionary<string, IKeywordHit>(0), new List<MailboxStatistics>(4))
		{
			if (resultPage != null)
			{
				int resultCount = resultPage.ResultCount;
			}
			if (resultPage != null)
			{
				int resultCount2 = resultPage.ResultCount;
			}
		}

		public ResultAggregator(Dictionary<string, IKeywordHit> keywordStatistics) : this(null, new Dictionary<string, List<IRefinerResult>>(0), 0UL, ByteQuantifiedSize.Zero, new List<Pair<MailboxInfo, Exception>>(0), keywordStatistics, new List<MailboxStatistics>(4))
		{
			Util.ThrowOnNull(keywordStatistics, "keywordStatistics");
		}

		public ResultAggregator(SortedResultPage resultPage, Dictionary<string, List<IRefinerResult>> refinerResults, ulong totalResultCount, ByteQuantifiedSize totalResultSize, List<Pair<MailboxInfo, Exception>> previewErrors, Dictionary<string, IKeywordHit> keywordStatistics, List<MailboxStatistics> mailboxStatistics)
		{
			this.keywordStatistics = keywordStatistics;
			this.previewResult = resultPage;
			this.refinerResults = refinerResults;
			this.previewErrors = previewErrors;
			this.totalResultCount = totalResultCount;
			this.totalResultSize = totalResultSize;
			this.mailboxStatistics = mailboxStatistics;
		}

		public SortedResultPage PreviewResult
		{
			get
			{
				return this.previewResult;
			}
		}

		public IDictionary<string, IKeywordHit> KeywordStatistics
		{
			get
			{
				return this.keywordStatistics;
			}
		}

		public List<Pair<MailboxInfo, Exception>> PreviewErrors
		{
			get
			{
				return this.previewErrors;
			}
		}

		public Dictionary<string, List<IRefinerResult>> RefinersResult
		{
			get
			{
				return this.refinerResults;
			}
		}

		public IProtocolLog ProtocolLog
		{
			get
			{
				return this.protocolLog;
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

		public List<MailboxStatistics> MailboxStats
		{
			get
			{
				return this.mailboxStatistics;
			}
		}

		public void MergeSearchResult(ISearchResult aggregator)
		{
			if (aggregator == null || (aggregator != null && aggregator.PreviewResult == null && aggregator.PreviewErrors == null && aggregator.RefinersResult == null && aggregator.MailboxStats == null && aggregator.KeywordStatistics == null))
			{
				return;
			}
			lock (this.locker)
			{
				if (this.previewResult == null)
				{
					this.previewResult = aggregator.PreviewResult;
				}
				else
				{
					this.previewResult.Merge(aggregator.PreviewResult);
				}
				this.MergeRefiners(aggregator.RefinersResult);
				this.MergeStatistics(aggregator.KeywordStatistics);
				this.totalResultCount += aggregator.TotalResultCount;
				this.totalResultSize += aggregator.TotalResultSize;
				if (aggregator.MailboxStats != null)
				{
					this.MergeMailboxStatistics(aggregator.MailboxStats);
				}
				if (aggregator.PreviewErrors != null)
				{
					this.previewErrors.AddRange(aggregator.PreviewErrors);
				}
				if (aggregator.ProtocolLog != null)
				{
					this.protocolLog.Merge(aggregator.ProtocolLog);
				}
			}
		}

		protected void MergeMailboxStatistics(List<MailboxStatistics> other)
		{
			if (other == null || other.Count == 0)
			{
				return;
			}
			Dictionary<string, MailboxStatistics> dictionary = this.MailboxStats.ToDictionary((MailboxStatistics x) => x.MailboxInfo.GetUniqueKey());
			foreach (MailboxStatistics mailboxStatistics in other)
			{
				string uniqueKey = mailboxStatistics.MailboxInfo.GetUniqueKey();
				if (!dictionary.ContainsKey(uniqueKey))
				{
					dictionary.Add(uniqueKey, mailboxStatistics);
				}
				else
				{
					dictionary[uniqueKey].Merge(mailboxStatistics);
				}
			}
			this.mailboxStatistics = dictionary.Values.ToList<MailboxStatistics>();
			this.mailboxStatistics.Sort(new MailboxStatsComparer(false));
		}

		protected virtual void MergeRefiners(Dictionary<string, List<IRefinerResult>> refinerToBeMerged)
		{
			if (refinerToBeMerged == null)
			{
				return;
			}
			foreach (string key in refinerToBeMerged.Keys)
			{
				List<IRefinerResult> source;
				if (!this.refinerResults.TryGetValue(key, out source))
				{
					this.refinerResults.Add(key, refinerToBeMerged[key]);
				}
				else
				{
					List<IRefinerResult> list = refinerToBeMerged[key];
					if (list != null && list.Count > 0)
					{
						Dictionary<string, IRefinerResult> dictionary = source.ToDictionary((IRefinerResult x) => x.Value);
						foreach (IRefinerResult refinerResult in list)
						{
							IRefinerResult refinerResult2;
							if (!dictionary.TryGetValue(refinerResult.Value, out refinerResult2))
							{
								dictionary.Add(refinerResult.Value, refinerResult);
							}
							else
							{
								refinerResult2.Merge(refinerResult);
							}
						}
						IEnumerable<IRefinerResult> source2 = from x in dictionary.Values
						orderby x.Count descending
						select x;
						int count = Math.Min(this.refinerResultsTrimCount, dictionary.Values.Count);
						List<IRefinerResult> value = source2.Take(count).ToList<IRefinerResult>();
						this.refinerResults[key] = value;
					}
				}
			}
		}

		private void MergeStatistics(IKeywordHit hit)
		{
			if (hit == null)
			{
				return;
			}
			IKeywordHit keywordHit = null;
			if (!this.keywordStatistics.TryGetValue(hit.Phrase, out keywordHit))
			{
				this.keywordStatistics.Add(hit.Phrase, hit);
				return;
			}
			keywordHit.Merge(hit);
		}

		private void MergeStatistics(IDictionary<string, IKeywordHit> hits)
		{
			if (hits == null)
			{
				return;
			}
			foreach (KeyValuePair<string, IKeywordHit> keyValuePair in hits)
			{
				this.MergeStatistics(keyValuePair.Value);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.LocalSearchTracer;

		private SortedResultPage previewResult;

		private readonly List<Pair<MailboxInfo, Exception>> previewErrors;

		private readonly Dictionary<string, IKeywordHit> keywordStatistics;

		private readonly Dictionary<string, List<IRefinerResult>> refinerResults;

		private readonly IProtocolLog protocolLog = new ProtocolLog();

		private readonly object locker = new object();

		private ulong totalResultCount;

		private ByteQuantifiedSize totalResultSize;

		private List<MailboxStatistics> mailboxStatistics;

		private readonly int refinerResultsTrimCount;
	}
}
