using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.External
{
	internal class FastLocalSearchResultsProvider : ISearchResultProvider
	{
		private static SearchConfig SearchConfig
		{
			get
			{
				return SearchConfig.Instance;
			}
		}

		private static Hookable<PagingImsFlowExecutor> ServiceProxyWrapper
		{
			get
			{
				if (FastLocalSearchResultsProvider.serviceProxyWrapperInstance == null)
				{
					string hostName = FastLocalSearchResultsProvider.SearchConfig.HostName;
					int queryServicePort = FastLocalSearchResultsProvider.SearchConfig.QueryServicePort;
					int fastMmsImsChannelPoolSize = FastLocalSearchResultsProvider.SearchConfig.FastMmsImsChannelPoolSize;
					TimeSpan fastImsMmsSendTimeout = FastLocalSearchResultsProvider.SearchConfig.FastImsMmsSendTimeout;
					TimeSpan fastImsMmsReceiveTimeout = FastLocalSearchResultsProvider.SearchConfig.FastImsMmsReceiveTimeout;
					int fastMmsImsRetryCount = FastLocalSearchResultsProvider.SearchConfig.FastMmsImsRetryCount;
					long num = (long)FastLocalSearchResultsProvider.SearchConfig.FastIMSMaxReceivedMessageSize;
					int fastIMSMaxStringContentLength = FastLocalSearchResultsProvider.SearchConfig.FastIMSMaxStringContentLength;
					TimeSpan fastProxyCacheTimeout = FastLocalSearchResultsProvider.SearchConfig.FastProxyCacheTimeout;
					Hookable<PagingImsFlowExecutor> value = Hookable<PagingImsFlowExecutor>.Create(true, new PagingImsFlowExecutor(hostName, queryServicePort, fastMmsImsChannelPoolSize, fastImsMmsSendTimeout, fastImsMmsSendTimeout, fastImsMmsReceiveTimeout, fastProxyCacheTimeout, num, fastIMSMaxStringContentLength, fastMmsImsRetryCount));
					Interlocked.CompareExchange<Hookable<PagingImsFlowExecutor>>(ref FastLocalSearchResultsProvider.serviceProxyWrapperInstance, value, null);
				}
				return FastLocalSearchResultsProvider.serviceProxyWrapperInstance;
			}
		}

		private static PagingImsFlowExecutor FlowExecutor
		{
			get
			{
				return FastLocalSearchResultsProvider.ServiceProxyWrapper.Value;
			}
		}

		public SearchMailboxesResults Search(ISearchPolicy policy, SearchMailboxesInputs input)
		{
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			long num5 = 0L;
			long num6 = 0L;
			ulong num7 = 0UL;
			SortedResultPage resultPage = null;
			MultiMailboxSearchClient multiMailboxSearchClient = null;
			ResultAggregator resultAggregator = new ResultAggregator();
			ByteQuantifiedSize byteQuantifiedSize = new ByteQuantifiedSize(0UL);
			List<MailboxStatistics> list = new List<MailboxStatistics>();
			Dictionary<Guid, List<KeyValuePair<int, long>>> dictionary = new Dictionary<Guid, List<KeyValuePair<int, long>>>();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Recorder.Record record = policy.Recorder.Start("SearchResultProvider", TraceType.InfoTrace, true);
			Recorder.Trace(5L, TraceType.InfoTrace, new object[]
			{
				"FastLocalSearchResultsProvider.Search Input:",
				input,
				"Type:",
				input.SearchType
			});
			SearchMailboxesResults result;
			try
			{
				Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search Initializing");
				num3 = stopwatch.ElapsedMilliseconds;
				string str = (input.PagingInfo != null && input.PagingInfo.SortBy != null && input.PagingInfo.SortBy.SortOrder == SortOrder.Ascending) ? "+" : "-";
				string str2 = (input.PagingInfo != null && input.PagingInfo.SortValue != null && input.PagingInfo.SortValue.SortColumn == ItemSchema.DocumentId) ? "[docid]" : FastIndexSystemSchema.Received.Name;
				AdditionalParameters additionalParameters = new AdditionalParameters
				{
					Sort = str + str2,
					Refiners = new string[]
					{
						FastIndexSystemSchema.Size.Name
					}
				};
				long referenceDocId = 0L;
				PaginationQueryFilter pagingFilter = null;
				QueryFilter queryFilter = input.Criteria.Query;
				if (input.PagingInfo != null && input.PagingInfo.SortValue != null)
				{
					referenceDocId = input.PagingInfo.SortValue.SecondarySortValue;
					if (input.PagingInfo.SortValue.SortColumnValue != null && input.PagingInfo.SortValue.SortColumn != ItemSchema.DocumentId)
					{
						pagingFilter = new PaginationQueryFilter(input.PagingInfo);
					}
				}
				SearchSource searchSource = input.Sources.FirstOrDefault<SearchSource>();
				if (searchSource != null)
				{
					Guid guid = searchSource.MailboxInfo.IsArchive ? searchSource.MailboxInfo.ArchiveDatabase : searchSource.MailboxInfo.MdbGuid;
					string displayName = FlowDescriptor.GetImsFlowDescriptor(FastLocalSearchResultsProvider.SearchConfig, FastIndexVersion.GetIndexSystemName(guid)).DisplayName;
					num4 += stopwatch.ElapsedMilliseconds - num3;
					num3 = stopwatch.ElapsedMilliseconds;
					Recorder.Trace(5L, TraceType.InfoTrace, new object[]
					{
						"FastLocalSearchResultsProvider.Search Initialized DB:",
						guid,
						"Flow:",
						displayName
					});
					List<SearchSource> list2 = new List<SearchSource>(input.Sources);
					while (list2.Count > 0)
					{
						HashSet<Guid> hashSet = new HashSet<Guid>();
						List<SearchSource> list3 = new List<SearchSource>();
						int i = 0;
						while (i < list2.Count)
						{
							SearchSource searchSource2 = list2[i];
							Guid item = searchSource2.MailboxInfo.IsArchive ? searchSource2.MailboxInfo.ArchiveGuid : searchSource2.MailboxInfo.MailboxGuid;
							if (!hashSet.Contains(item))
							{
								list3.Add(searchSource2);
								list2.RemoveAt(i);
								hashSet.Add(item);
							}
							else
							{
								i++;
							}
						}
						multiMailboxSearchClient = new MultiMailboxSearchClient(guid, (from s in list3
						select s.MailboxInfo).ToArray<MailboxInfo>(), input.Criteria, input.CallerInfo, input.PagingInfo);
						foreach (SearchSource searchSource3 in list3)
						{
							Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search Searching Source", searchSource3);
							ulong num8 = 0UL;
							bool flag = false;
							MailboxStatistics mailboxStatistics = null;
							ByteQuantifiedSize byteQuantifiedSize2 = new ByteQuantifiedSize(0UL);
							List<KeyValuePair<int, long>> list4 = new List<KeyValuePair<int, long>>();
							Guid guid2 = searchSource3.MailboxInfo.IsArchive ? searchSource3.MailboxInfo.ArchiveGuid : searchSource3.MailboxInfo.MailboxGuid;
							queryFilter = this.ApplyFolderFilter(queryFilter, searchSource3.MailboxInfo, multiMailboxSearchClient);
							string text = FqlQueryBuilder.ToFqlString(queryFilter, input.Criteria.QueryCulture);
							text = this.ApplyPagingFilter(text, referenceDocId, pagingFilter, input.PagingInfo, input.Criteria.QueryCulture);
							Recorder.Trace(5L, TraceType.InfoTrace, new object[]
							{
								"FastLocalSearchResultsProvider.Search Searching Source Guid:",
								guid2,
								"Filter:",
								queryFilter,
								"Query:",
								text
							});
							num6 += stopwatch.ElapsedMilliseconds - num3;
							num3 = stopwatch.ElapsedMilliseconds;
							IEnumerable<KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]>> enumerable = FastLocalSearchResultsProvider.FlowExecutor.Execute(displayName, guid2, policy.CallerInfo.QueryCorrelationId, text, 0L, input.Criteria.QueryCulture, additionalParameters, Math.Min(FastLocalSearchResultsProvider.SearchConfig.FastQueryResultTrimHits, input.PagingInfo.PageSize), null);
							foreach (KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]> keyValuePair in enumerable)
							{
								Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search found pages");
								PagingImsFlowExecutor.QueryExecutionContext key = keyValuePair.Key;
								ISearchResultItem[] value = keyValuePair.Value;
								if (!flag)
								{
									Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search found statistics");
									num8 += (ulong)FastLocalSearchResultsProvider.FlowExecutor.ReadHitCount(key);
									IEnumerable<RefinerResult> source = FastLocalSearchResultsProvider.FlowExecutor.ReadRefiners(key);
									RefinerResult refinerResult = source.FirstOrDefault((RefinerResult t) => t.Name == FastIndexSystemSchema.Size.Name);
									if (refinerResult != null)
									{
										byteQuantifiedSize2 += new ByteQuantifiedSize((ulong)refinerResult.Sum);
									}
									mailboxStatistics = new MailboxStatistics(searchSource3.MailboxInfo, num8, byteQuantifiedSize2);
									flag = true;
								}
								foreach (ISearchResultItem searchResultItem in value)
								{
									Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search found items");
									IFieldHolder fieldHolder = searchResultItem.Fields.FirstOrDefault((IFieldHolder f) => string.Equals(f.Name, "DocId", StringComparison.InvariantCultureIgnoreCase));
									if (fieldHolder != null)
									{
										long num9 = (long)fieldHolder.Value;
										int documentId = IndexId.GetDocumentId(num9);
										IndexId.GetMailboxNumber(num9);
										list4.Add(new KeyValuePair<int, long>(documentId, num9));
									}
								}
								if (list4.Count >= input.PagingInfo.PageSize)
								{
									Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search completed FAST");
									break;
								}
							}
							num7 += num8;
							byteQuantifiedSize += byteQuantifiedSize2;
							if (list4.Count > 0)
							{
								dictionary[guid2] = list4;
							}
							if (mailboxStatistics != null)
							{
								list.Add(mailboxStatistics);
							}
							num += stopwatch.ElapsedMilliseconds - num3;
							num3 = stopwatch.ElapsedMilliseconds;
						}
						if (dictionary.Count > 0)
						{
							Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search loading preview items");
							resultPage = multiMailboxSearchClient.FetchPreviewProperties(dictionary);
							num2 += stopwatch.ElapsedMilliseconds - num3;
							num3 = stopwatch.ElapsedMilliseconds;
						}
						Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search aggregating");
						ISearchResult aggregator = new AggregatedSearchTaskResult(new MailboxInfoList(multiMailboxSearchClient.Mailboxes), resultPage, null, num7, byteQuantifiedSize, null, list, null);
						resultAggregator.MergeSearchResult(aggregator);
						num5 += stopwatch.ElapsedMilliseconds - num3;
						num3 = stopwatch.ElapsedMilliseconds;
					}
				}
				result = new SearchMailboxesResults(input.Sources)
				{
					SearchResult = resultAggregator
				};
			}
			finally
			{
				record.Attributes["FAST"] = num;
				record.Attributes["STORE"] = num2;
				record.Attributes["REST"] = num6;
				record.Attributes["INIT"] = num4;
				record.Attributes["AGGR"] = num5;
				record.Attributes["TOTALSIZE"] = byteQuantifiedSize;
				record.Attributes["TOTALCNT"] = num7;
				policy.Recorder.End(record);
			}
			return result;
		}

		private QueryFilter ApplyFolderFilter(QueryFilter filter, MailboxInfo mailboxInfo, MultiMailboxSearchClient client)
		{
			if (!string.IsNullOrEmpty(mailboxInfo.Folder))
			{
				Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search folder inclusion Folder:", mailboxInfo.Folder);
				VersionedId versionedId = VersionedId.Deserialize(mailboxInfo.Folder);
				filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					filter,
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ParentEntryId, versionedId.ObjectId)
				});
			}
			else
			{
				List<StoreId> excludedFolderListForMailbox = client.GetExcludedFolderListForMailbox(mailboxInfo);
				if (excludedFolderListForMailbox != null && excludedFolderListForMailbox.Count > 0)
				{
					Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search folder exclusion List:", excludedFolderListForMailbox);
					List<QueryFilter> list = new List<QueryFilter>();
					list.Add(filter);
					list.AddRange(from s in excludedFolderListForMailbox
					select new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.ParentEntryId, s));
					filter = QueryFilter.AndTogether(list.ToArray());
				}
			}
			return filter;
		}

		private string ApplyPagingFilter(string query, long referenceDocId, PaginationQueryFilter pagingFilter, PagingInfo pagingInfo, CultureInfo queryCulture)
		{
			Recorder.Trace(5L, TraceType.InfoTrace, "FastLocalSearchResultsProvider.Search paging filter", pagingFilter);
			string text = string.Empty;
			string text2 = string.Empty;
			if (pagingInfo.AscendingSort)
			{
				text = ((referenceDocId == 0L) ? "min" : referenceDocId.ToString());
				text2 = "max";
			}
			else
			{
				text = "min";
				text2 = ((referenceDocId == 0L) ? "max" : referenceDocId.ToString());
			}
			if (pagingFilter != null && pagingFilter.ComparisionQueryFilter != null && pagingFilter.EqualsQueryFilter != null)
			{
				query = string.Format("and({0},or(and(documentid:range({1},{2},from=gt,to=lt),{3}),{4}))", new string[]
				{
					query,
					text,
					text2,
					FqlQueryBuilder.ToFqlString(pagingFilter.EqualsQueryFilter, queryCulture),
					FqlQueryBuilder.ToFqlString(pagingFilter.ComparisionQueryFilter, queryCulture)
				});
			}
			else
			{
				query = string.Format("and({0},documentid:range({1},{2},from=gt,to=lt))", new string[]
				{
					query,
					text,
					text2
				});
			}
			return query;
		}

		private static Hookable<PagingImsFlowExecutor> serviceProxyWrapperInstance;
	}
}
