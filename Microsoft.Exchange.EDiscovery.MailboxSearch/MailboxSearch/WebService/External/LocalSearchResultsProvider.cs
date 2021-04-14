using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.External
{
	internal class LocalSearchResultsProvider : ISearchResultProvider
	{
		public long FastTime { get; set; }

		public long StoreTime { get; set; }

		public long RestrictionTime { get; set; }

		public long TotalItems { get; set; }

		public long TotalSize { get; set; }

		public long ReturnedFastItems { get; set; }

		public long ReturnedStoreItems { get; set; }

		public long ReturnedStoreSize { get; set; }

		public SearchMailboxesResults Search(ISearchPolicy policy, SearchMailboxesInputs input)
		{
			Guid databaseGuid = Guid.Empty;
			List<SearchSource> list = new List<SearchSource>(input.Sources);
			ResultAggregator aggregator = new ResultAggregator();
			IEnumerable<List<string>> enumerable = null;
			Recorder.Record record = policy.Recorder.Start("SearchResultProvider", TraceType.InfoTrace, true);
			Recorder.Trace(5L, TraceType.InfoTrace, new object[]
			{
				"LocalSearchResultsProvider.Search Input:",
				input,
				"Type:",
				input.SearchType
			});
			try
			{
				if (input.SearchType == SearchType.Statistics)
				{
					enumerable = this.GenerateKeywordStatsQueryBatches(policy, aggregator, input.Criteria);
				}
				SearchCompletedCallback searchCallback = delegate(ISearchMailboxTask task, ISearchTaskResult result)
				{
					aggregator.MergeSearchResult(result);
				};
				record.Attributes["MBXCNT"] = list.Count;
				while (list.Count > 0)
				{
					Recorder.Trace(5L, TraceType.InfoTrace, "LocalSearchResultsProvider.Search UnsearchedSources:", list.Count);
					HashSet<Guid> hashSet = new HashSet<Guid>();
					MailboxInfoList mailboxInfoList = new MailboxInfoList();
					int i = 0;
					while (i < list.Count)
					{
						SearchSource searchSource = list[i];
						Guid item = searchSource.MailboxInfo.IsArchive ? searchSource.MailboxInfo.ArchiveGuid : searchSource.MailboxInfo.MailboxGuid;
						if (!hashSet.Contains(item))
						{
							mailboxInfoList.Add(searchSource.MailboxInfo);
							list.RemoveAt(i);
							hashSet.Add(item);
							databaseGuid = (searchSource.MailboxInfo.IsArchive ? searchSource.MailboxInfo.ArchiveDatabase : searchSource.MailboxInfo.MdbGuid);
						}
						else
						{
							i++;
						}
					}
					Recorder.Trace(5L, TraceType.InfoTrace, "LocalSearchResultsProvider.Search NonDuplicateSourcesToSearch:", mailboxInfoList.Count);
					AggregatedMailboxSearchTask aggregatedMailboxSearchTask;
					if (input.SearchType == SearchType.Statistics)
					{
						Recorder.Trace(5L, TraceType.InfoTrace, "LocalSearchResultsProvider.Search Statistics:", enumerable);
						using (IEnumerator<List<string>> enumerator = enumerable.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								List<string> keywordList = enumerator.Current;
								AggregatedMailboxSearchTask aggregatedMailboxSearchTask2;
								aggregatedMailboxSearchTask = (aggregatedMailboxSearchTask2 = new AggregatedMailboxSearchTask(databaseGuid, mailboxInfoList, input.Criteria, input.PagingInfo, keywordList, input.CallerInfo));
								try
								{
									aggregatedMailboxSearchTask.Execute(searchCallback);
									this.UpdateSearchStatistics(policy, record, aggregatedMailboxSearchTask);
								}
								finally
								{
									if (aggregatedMailboxSearchTask2 != null)
									{
										((IDisposable)aggregatedMailboxSearchTask2).Dispose();
									}
								}
							}
							continue;
						}
					}
					Recorder.Trace(5L, TraceType.InfoTrace, "LocalSearchResultsProvider.Search Regular");
					AggregatedMailboxSearchTask aggregatedMailboxSearchTask3;
					aggregatedMailboxSearchTask = (aggregatedMailboxSearchTask3 = new AggregatedMailboxSearchTask(databaseGuid, mailboxInfoList, input.SearchType, input.Criteria, input.PagingInfo, input.CallerInfo));
					try
					{
						aggregatedMailboxSearchTask.Execute(searchCallback);
						this.UpdateSearchStatistics(policy, record, aggregatedMailboxSearchTask);
					}
					finally
					{
						if (aggregatedMailboxSearchTask3 != null)
						{
							((IDisposable)aggregatedMailboxSearchTask3).Dispose();
						}
					}
				}
			}
			finally
			{
				policy.Recorder.End(record);
			}
			return new SearchMailboxesResults(input.Sources)
			{
				SearchResult = aggregator
			};
		}

		private IEnumerable<List<string>> GenerateKeywordStatsQueryBatches(ISearchPolicy policy, ResultAggregator aggregator, SearchCriteria criteria)
		{
			List<string> list = new List<string>
			{
				criteria.QueryString
			};
			aggregator.KeywordStatistics.Add(criteria.QueryString, new KeywordHit(criteria.QueryString, 0UL, ByteQuantifiedSize.Zero));
			if (criteria.SubFilters != null)
			{
				foreach (string text in criteria.SubFilters.Keys)
				{
					aggregator.KeywordStatistics.Add(text, new KeywordHit(text, 0UL, ByteQuantifiedSize.Zero));
				}
				list.AddRange(criteria.SubFilters.Keys);
			}
			return Util.PartitionInSetsOf<string>(list, 5);
		}

		private void UpdateSearchStatistics(ISearchPolicy policy, Recorder.Record currentRecord, AggregatedMailboxSearchTask task)
		{
			currentRecord.Attributes["FAST"] = (this.FastTime += task.FastTime);
			currentRecord.Attributes["STORE"] = (this.StoreTime += task.StoreTime);
			currentRecord.Attributes["REST"] = (this.RestrictionTime += task.RestrictionTime);
			currentRecord.Attributes["TOTALSIZE"] = (this.TotalSize += task.TotalSize);
			currentRecord.Attributes["TOTALCNT"] = (this.TotalItems += task.TotalItems);
			currentRecord.Attributes["RTNSIZE"] = (this.ReturnedStoreSize += task.ReturnedStoreSize);
			currentRecord.Attributes["RTNSTORE"] = (this.ReturnedStoreItems += task.ReturnedStoreItems);
			currentRecord.Attributes["RTNFAST"] = (this.ReturnedFastItems += task.ReturnedFastItems);
			if (task.SearchStatistics != null && task.SearchStatistics.Count > 0)
			{
				string description = string.Format("{0}Mailboxes", currentRecord.Description);
				Recorder.Record record = policy.Recorder.Start(description, TraceType.InfoTrace, false);
				foreach (string text in task.SearchStatistics.Keys)
				{
					Dictionary<string, string> dictionary = task.SearchStatistics[text];
					foreach (string text2 in dictionary.Keys)
					{
						record.Attributes[string.Format("{0}-{1}", text, text2)] = dictionary[text2];
					}
				}
				policy.Recorder.End(record);
			}
		}
	}
}
