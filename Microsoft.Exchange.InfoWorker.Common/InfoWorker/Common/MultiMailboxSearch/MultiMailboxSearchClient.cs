using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.Rpc.MultiMailboxSearch;
using Microsoft.Mapi;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class MultiMailboxSearchClient : DisposeTrackableBase, ISearchRpcClient, IDisposeTrackable, IDisposable
	{
		internal MultiMailboxSearchClient(Guid mailboxDatabaseGuid, MailboxInfo[] mailboxes, SearchCriteria criteria, CallerInfo executingUserIdentity, PagingInfo pagingInfo)
		{
			Util.ThrowOnNull(mailboxDatabaseGuid, "mailboxDatabaseGuid");
			if (mailboxDatabaseGuid.Equals(Guid.Empty))
			{
				throw new ArgumentNullException("mailboxDatabaseGuid");
			}
			Util.ThrowOnNull(mailboxes, "mailboxes");
			Util.ThrowOnNull(criteria, "criteria");
			Util.ThrowOnNull(pagingInfo, "pagingInfo");
			Util.ThrowOnNull(executingUserIdentity, "executingUserIdentity");
			this.searchCriteria = criteria;
			this.pagingInfo = pagingInfo;
			this.executingUserIdentity = executingUserIdentity;
			this.mailboxes = mailboxes;
			this.databaseGuid = mailboxDatabaseGuid;
			this.previewSearchFailures = new List<Pair<MailboxInfo, Exception>>(this.mailboxes.Length);
			this.Init();
			this.queryCorrelationId = executingUserIdentity.QueryCorrelationId;
			TimeSpan defaultSearchTimeout = Factory.Current.GetDefaultSearchTimeout(this.searchCriteria.RecipientSession);
			this.sessionCache = new MailboxSessionCache(this.mailboxes.Length, Util.GetGenericIdentity(this.executingUserIdentity), this.queryCorrelationId, defaultSearchTimeout, new MailboxSessionCache.CreateSessionHandler(this.CreateSessionForCache));
			this.aborted = 0L;
		}

		internal Guid MailboxDatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		internal MailboxInfo[] Mailboxes
		{
			get
			{
				return this.mailboxes;
			}
		}

		internal PagingInfo PagingInfo
		{
			get
			{
				return this.pagingInfo;
			}
		}

		internal SearchCriteria Criteria
		{
			get
			{
				return this.searchCriteria;
			}
		}

		internal CallerInfo ExecutingUserIdentity
		{
			get
			{
				return this.executingUserIdentity;
			}
		}

		internal MailboxSessionCache MailboxSessionCache
		{
			get
			{
				return this.sessionCache;
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

		internal static string PatchSenderRefinerEntryName(string entryName)
		{
			if (string.IsNullOrEmpty(entryName))
			{
				return entryName;
			}
			string text = entryName;
			int num = entryName.IndexOf('[');
			if (num >= 0)
			{
				int num2 = entryName.IndexOf(']');
				num2 = ((num2 - 1 < 0) ? entryName.Length : (num2 - 1));
				text = entryName.Substring(num + 1, num2);
			}
			text = text.Trim();
			string[] array = text.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				text = array[0];
			}
			return text;
		}

		internal static Dictionary<string, List<IRefinerResult>> GetRefinerResults(MultiMailboxSearchResponse searchResponse)
		{
			Dictionary<string, List<IRefinerResult>> dictionary = null;
			if (searchResponse.RefinerOutput != null && searchResponse.RefinerOutput.Count > 0)
			{
				dictionary = new Dictionary<string, List<IRefinerResult>>(searchResponse.RefinerOutput.Count);
				foreach (string text in searchResponse.RefinerOutput.Keys)
				{
					List<MultiMailboxSearchRefinersResult> list = searchResponse.RefinerOutput[text];
					List<IRefinerResult> list2 = new List<IRefinerResult>(list.Count);
					foreach (MultiMailboxSearchRefinersResult multiMailboxSearchRefinersResult in list)
					{
						string text2 = multiMailboxSearchRefinersResult.Name;
						if (text.Equals("from", StringComparison.OrdinalIgnoreCase))
						{
							text2 = MultiMailboxSearchClient.PatchSenderRefinerEntryName(text2);
						}
						list2.Add(new RefinerResult(text2, multiMailboxSearchRefinersResult.Value));
					}
					dictionary.Add(text, list2);
				}
			}
			return dictionary;
		}

		internal List<StoreId> GetExcludedFolderListForMailbox(MailboxInfo mailboxInfo)
		{
			StoreSession storeSession = this.OpenMailboxSession(mailboxInfo.ExchangePrincipal);
			if (!storeSession.IsPublicFolderSession)
			{
				return this.GetExcludedFolderRestrictionListForMailbox(storeSession);
			}
			return null;
		}

		internal List<StoreId> GetExcludedFolderRestrictionListForMailbox(StoreSession mailboxSession)
		{
			List<StoreId> list = new List<StoreId>(4);
			StoreId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsRoot);
			StoreId defaultFolderId2 = mailboxSession.GetDefaultFolderId(DefaultFolderType.CalendarLogging);
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.DisplayName, "Audits"),
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.DisplayName, "AdminAuditLogs")
			});
			if (defaultFolderId != null)
			{
				list.Add(defaultFolderId);
				using (Folder folder = Folder.Bind(mailboxSession, defaultFolderId))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, queryFilter, null, new PropertyDefinition[]
					{
						FolderSchema.Id
					}))
					{
						list.AddRange(queryResult.Enumerator<StoreId>());
					}
				}
			}
			if (defaultFolderId2 != null)
			{
				list.Add(defaultFolderId2);
			}
			foreach (DefaultFolderType defaultFolderType in this.searchCriteria.ExcludedFolders)
			{
				try
				{
					StoreId defaultFolderId3 = mailboxSession.GetDefaultFolderId(defaultFolderType);
					if (defaultFolderId3 != null)
					{
						list.Add(defaultFolderId3);
					}
				}
				catch (Exception ex)
				{
					if (!(ex is AccessDeniedException) && !(ex is InvalidOperationException) && !(ex is EnumOutOfRangeException) && !(ex is EnumArgumentException))
					{
						throw;
					}
				}
			}
			return list;
		}

		public virtual AggregatedSearchTaskResult Search(int refinerResultTrimCount)
		{
			Stopwatch.StartNew();
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			ResultAggregator resultAggregator = new ResultAggregator(refinerResultTrimCount);
			using (ExRpcAdmin exRpcAdmin = this.InitRpcAdminConnection())
			{
				PagingInfo rpcPagingInfo = null;
				byte[] sortByRestriction = null;
				List<KeyValuePair<string, byte[]>> queryFilterList = this.ConvertQueryFilterToRestriction(new string[]
				{
					this.searchCriteria.QueryString
				}, exRpcAdmin, false, out rpcPagingInfo, out sortByRestriction);
				Sorting rpcSortOrder = this.CreateRPCSortOrder();
				for (int i = 0; i < this.Mailboxes.Length; i++)
				{
					this.CheckIfAborted();
					Stopwatch stopwatch = Stopwatch.StartNew();
					long num4 = 0L;
					long num5 = 0L;
					long num6 = 0L;
					this.ExecutePreviewSearch(refinerResultTrimCount, exRpcAdmin, rpcPagingInfo, sortByRestriction, queryFilterList, rpcSortOrder, i, ref resultAggregator, ref num6, ref num4, ref num5);
					stopwatch.Stop();
					num2 += num4;
					num3 += num5;
					num += num6;
					Factory.Current.LocalTaskTracer.TracePerformance(this.GetHashCode(), 0L, "Correlation Id:{0}. eDiscovery MultiMailbox Preview Search in {1} database for {2} mailbox, took {3} ms. Fast time was {4}ms and store time was {5}ms.", new object[]
					{
						this.queryCorrelationId,
						this.databaseServerFqdn,
						this.Mailboxes[i].DisplayName,
						stopwatch.ElapsedMilliseconds,
						num4,
						num5
					});
				}
			}
			this.GetRpcSearchLatencyPerfCounter(SearchType.Preview, (int)num).Increment();
			resultAggregator.ProtocolLog.Add("LongestLocalSearchTaskTime", num);
			resultAggregator.ProtocolLog.Add("LongestLocalSearchTaskFastTime", num2);
			resultAggregator.ProtocolLog.Add("LongestLocalSearchTaskStoreTime", num3);
			Factory.Current.LocalTaskTracer.TracePerformance(this.GetHashCode(), 0L, "Correlation Id:{0}. eDiscovery MultiMailbox Preview Search in {1} database for {2} mailboxes, took {3} ms. Fast time was {4}ms and store time was {5}ms.", new object[]
			{
				this.queryCorrelationId,
				this.databaseServerFqdn,
				this.mailboxes.Length,
				num,
				num2,
				num3
			});
			AggregatedSearchTaskResult aggregatedSearchTaskResult = null;
			if (resultAggregator.PreviewResult != null || resultAggregator.MailboxStats != null)
			{
				aggregatedSearchTaskResult = new AggregatedSearchTaskResult(new MailboxInfoList(this.Mailboxes), resultAggregator.PreviewResult, resultAggregator.RefinersResult, resultAggregator.TotalResultCount, resultAggregator.TotalResultSize, resultAggregator.PreviewErrors, resultAggregator.MailboxStats, resultAggregator.ProtocolLog);
			}
			else if (resultAggregator.PreviewErrors != null && resultAggregator.PreviewErrors.Count > 0)
			{
				aggregatedSearchTaskResult = new AggregatedSearchTaskResult(new MailboxInfoList(this.Mailboxes), resultAggregator.PreviewErrors);
			}
			Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. eDiscovery preview search for query:{1}, on Mailboxes:{2} in Database:{3} completed in {4} ms, yielding totalresults:{5} with size:{6}", new object[]
			{
				this.queryCorrelationId,
				this.Criteria.QueryString,
				(aggregatedSearchTaskResult != null) ? aggregatedSearchTaskResult.MailboxInfoList.Count : this.Mailboxes.Length,
				this.databaseGuid,
				num,
				resultAggregator.TotalResultCount,
				resultAggregator.TotalResultSize
			});
			return aggregatedSearchTaskResult;
		}

		public virtual List<IKeywordHit> GetKeywordHits(List<string> keywordList)
		{
			HashSet<string> source = new HashSet<string>(keywordList);
			string[] array = source.ToArray<string>();
			List<IKeywordHit> list = new List<IKeywordHit>(array.Length * this.Mailboxes.Length);
			ResultAggregator resultAggregator = new ResultAggregator();
			using (ExRpcAdmin exRpcAdmin = this.InitRpcAdminConnection())
			{
				List<KeyValuePair<string, byte[]>> queryFilterRestrictions = this.ConvertQueryFilterToRestriction(array, exRpcAdmin);
				Stopwatch.StartNew();
				long num = 0L;
				for (int i = 0; i < this.Mailboxes.Length; i++)
				{
					this.CheckIfAborted();
					long num2 = 0L;
					this.ExecuteKeywordHits(array, exRpcAdmin, queryFilterRestrictions, i, ref resultAggregator, ref num2);
					num += num2;
				}
				if (resultAggregator.KeywordStatistics != null && resultAggregator.KeywordStatistics.Count > 0)
				{
					this.GetRpcSearchLatencyPerfCounter(SearchType.Statistics, (int)num).Increment();
					list.AddRange(resultAggregator.KeywordStatistics.Values);
				}
				Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. eDiscovery MultiMailbox Keyword Stats in {1} database for {2} mailboxes, took {3} ms.", new object[]
				{
					this.queryCorrelationId,
					this.databaseServerFqdn,
					this.mailboxes.Length,
					num
				});
			}
			return list;
		}

		public void Abort()
		{
			Interlocked.Exchange(ref this.aborted, 1L);
		}

		internal AggregatedSearchTaskResult ProcessRpcSearchResponse(MultiMailboxSearchResponse searchResponse, MailboxInfo mailbox)
		{
			bool flag = Factory.Current.LocalTaskTracer.IsTraceEnabled(TraceType.PerformanceTrace);
			Stopwatch stopwatch = flag ? Stopwatch.StartNew() : null;
			AggregatedSearchTaskResult result;
			if (searchResponse.Results.Length > 0)
			{
				Dictionary<string, List<IRefinerResult>> refinerResults = MultiMailboxSearchClient.GetRefinerResults(searchResponse);
				Dictionary<Guid, List<KeyValuePair<int, long>>> groupedResultMap = this.GroupMessagesByMailbox((MultiMailboxSearchResult[])searchResponse.Results);
				if (flag)
				{
					stopwatch.Restart();
				}
				SortedResultPage sortedResultPage = this.FetchPreviewProperties(groupedResultMap);
				if (flag)
				{
					stopwatch.Stop();
					Factory.Current.LocalTaskTracer.TracePerformance((long)this.GetHashCode(), "Correlation Id:{0}. Fetching preview properties for mailbox {1} took {2}ms for {3} items", new object[]
					{
						this.queryCorrelationId,
						mailbox.ExchangeGuid,
						stopwatch.ElapsedMilliseconds,
						(sortedResultPage != null) ? sortedResultPage.ResultCount : 0
					});
				}
				if (searchResponse.TotalResultCount > 0L && sortedResultPage == null)
				{
					return this.CreateEmptyResult(mailbox);
				}
				MailboxStatistics item = new MailboxStatistics(mailbox, (ulong)searchResponse.TotalResultCount, ByteQuantifiedSize.FromBytes((ulong)searchResponse.TotalResultSize));
				result = new AggregatedSearchTaskResult(new MailboxInfoList(new MailboxInfo[]
				{
					mailbox
				}), sortedResultPage, refinerResults, (ulong)searchResponse.TotalResultCount, ByteQuantifiedSize.FromBytes((ulong)searchResponse.TotalResultSize), this.previewSearchFailures, new List<MailboxStatistics>
				{
					item
				}, null);
			}
			else
			{
				result = this.CreateEmptyResult(mailbox);
			}
			return result;
		}

		private AggregatedSearchTaskResult CreateEmptyResult(MailboxInfo mailbox)
		{
			MailboxStatistics item = new MailboxStatistics(mailbox, 0UL, ByteQuantifiedSize.FromBytes(0UL));
			return new AggregatedSearchTaskResult(new MailboxInfoList(new MailboxInfo[]
			{
				mailbox
			}), null, null, 0UL, ByteQuantifiedSize.FromBytes(0UL), null, new List<MailboxStatistics>
			{
				item
			}, null);
		}

		internal virtual void Init()
		{
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(this.databaseGuid);
			this.databaseServerFqdn = serverForDatabase.ServerFqdn;
		}

		internal List<IKeywordHit> ProcessKeywordHitRpcResponse(MultiMailboxKeywordStatsResult[] results)
		{
			List<IKeywordHit> list = new List<IKeywordHit>((results != null) ? results.Length : 0);
			if (results != null)
			{
				list.AddRange(from row in results
				select new KeywordHit(row.Keyword, (ulong)row.Count, new ByteQuantifiedSize((ulong)row.Size)));
			}
			return list;
		}

		internal SortedResultPage FetchPreviewProperties(Dictionary<Guid, List<KeyValuePair<int, long>>> groupedResultMap)
		{
			SortedResultPage sortedResultPage = null;
			foreach (Guid guid in groupedResultMap.Keys)
			{
				this.CheckIfAborted();
				if (sortedResultPage == null)
				{
					sortedResultPage = this.FetchPreviewPropertiesFromMailbox(guid, groupedResultMap[guid]);
				}
				else
				{
					sortedResultPage.Merge(this.FetchPreviewPropertiesFromMailbox(guid, groupedResultMap[guid]));
				}
			}
			return sortedResultPage;
		}

		internal SortedResultPage FetchPreviewPropertiesFromMailbox(Guid mailboxId, List<KeyValuePair<int, long>> messageIdPairs)
		{
			SortedResultPage resultPage = null;
			if (messageIdPairs.Count == 0)
			{
				return resultPage;
			}
			messageIdPairs = (from x in messageIdPairs
			orderby x.Key
			select x).Distinct<KeyValuePair<int, long>>().ToList<KeyValuePair<int, long>>();
			MailboxInfo mailboxInfo = (from x in this.mailboxes
			where x.MailboxGuid.Equals(mailboxId)
			select x).FirstOrDefault<MailboxInfo>();
			if (mailboxInfo == null)
			{
				Factory.Current.LocalTaskTracer.TraceDebug<Guid, Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Could not find mailbox {1} in {2} database in the MailboxInfoList", this.queryCorrelationId, mailboxId, this.databaseServerFqdn);
				return resultPage;
			}
			this.CheckIfAborted();
			Util.HandleGenericMailboxFailureForMailboxOperation(delegate
			{
				resultPage = this.CreatePreviewItemsFromStore(mailboxId, messageIdPairs, mailboxInfo);
			}, delegate(LocalizedException ex)
			{
				Factory.Current.LocalTaskTracer.TraceError((long)this.GetHashCode(), "Correlation Id:{0}. Fetching Preview properties for {1} items in mailbox {2} in database {3} failed with the error {4}", new object[]
				{
					this.queryCorrelationId,
					messageIdPairs.Count,
					mailboxId,
					this.databaseServerFqdn,
					ex.ToString()
				});
				this.previewSearchFailures.Add(new Pair<MailboxInfo, Exception>(mailboxInfo, ex));
				if (ex != null)
				{
					Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoverySearchFailure, null, new object[]
					{
						"preview",
						this.queryCorrelationId.ToString(),
						mailboxId.ToString(),
						this.databaseGuid.ToString(),
						this.databaseServerFqdn,
						ex
					});
				}
			});
			return resultPage;
		}

		internal Dictionary<Guid, List<KeyValuePair<int, long>>> GroupMessagesByMailbox(MultiMailboxSearchResult[] results)
		{
			Dictionary<Guid, List<KeyValuePair<int, long>>> dictionary = new Dictionary<Guid, List<KeyValuePair<int, long>>>(1);
			for (int i = 0; i < results.Length; i++)
			{
				Guid mailboxGuid = results[i].MailboxGuid;
				List<KeyValuePair<int, long>> list;
				if (!dictionary.TryGetValue(mailboxGuid, out list))
				{
					list = new List<KeyValuePair<int, long>>(10);
					dictionary.Add(mailboxGuid, list);
				}
				KeyValuePair<int, long> item = new KeyValuePair<int, long>(results[i].DocumentId, results[i].ReferenceId);
				list.Add(item);
			}
			return dictionary;
		}

		internal PagingInfo CreateRpcPagingInfo(StoreSession mailboxSession, ExRpcAdmin rpcAdmin)
		{
			this.CheckIfAborted();
			long num = 0L;
			PagingReferenceItem pagingReferenceItem = null;
			if (this.pagingInfo.SortValue != null)
			{
				num = this.pagingInfo.SortValue.SecondarySortValue;
			}
			if (mailboxSession != null && this.PagingInfo.SortValue != null && this.PagingInfo.SortValue.SortColumnValue != null)
			{
				PaginationQueryFilter paginationQueryFilter = new PaginationQueryFilter(this.PagingInfo);
				if (paginationQueryFilter.EqualsQueryFilter == null || paginationQueryFilter.ComparisionQueryFilter == null)
				{
					if (Factory.Current.LocalTaskTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Factory.Current.LocalTaskTracer.TraceError<Guid, string, string>(this.GetHashCode(), 0L, "Correlation Id:{0}. ReferenceItem is not null and it is a pagination navigation request, but the paginationQueryFilter for the query {1} on the server {2} is null or the sort equals or sort comparision is null.", this.queryCorrelationId, this.searchCriteria.QueryString, Environment.MachineName);
					}
					throw new DiscoverySearchInvalidPagination();
				}
				if (Factory.Current.LocalTaskTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Creating pagination reference item(EqualsRestriction:{1}, Restriction: {2}) for the query:{3}", new object[]
					{
						this.queryCorrelationId,
						paginationQueryFilter.EqualsQueryFilter.ToString(),
						paginationQueryFilter.ComparisionQueryFilter.ToString(),
						this.searchCriteria.QueryString
					});
				}
				byte[] equalsRestriction = this.CreateRestriction(mailboxSession, rpcAdmin, paginationQueryFilter.EqualsQueryFilter);
				byte[] comparisionFilterRestriction = this.CreateRestriction(mailboxSession, rpcAdmin, paginationQueryFilter.ComparisionQueryFilter);
				pagingReferenceItem = new PagingReferenceItem(equalsRestriction, comparisionFilterRestriction);
			}
			return new PagingInfo(Util.GetSearchResultPageSize(this.pagingInfo, this.searchCriteria.RecipientSession), (num > 0L) ? ((this.pagingInfo.Direction == PageDirection.Next) ? PagingDirection.Next : PagingDirection.Previous) : PagingDirection.None, num, pagingReferenceItem);
		}

		internal MultiMailboxSearchMailboxInfo[] CreateRpcMailInfo(int idx, ExRpcAdmin rpcAdmin)
		{
			byte[] folderRestrictionForMailbox = this.GetFolderRestrictionForMailbox(idx, rpcAdmin);
			return new MultiMailboxSearchMailboxInfo[]
			{
				new MultiMailboxSearchMailboxInfo(this.mailboxes[idx].MailboxGuid, folderRestrictionForMailbox)
			};
		}

		internal List<KeyValuePair<string, byte[]>> ConvertQueryFilterToRestriction(string[] queries, ExRpcAdmin rpcAdmin)
		{
			PagingInfo pagingInfo = null;
			byte[] array = null;
			return this.ConvertQueryFilterToRestriction(queries, rpcAdmin, queries.Length > 1, out pagingInfo, out array);
		}

		internal List<KeyValuePair<string, byte[]>> ConvertQueryFilterToRestriction(string[] queries, ExRpcAdmin rpcAdmin, bool isSubFilters, out PagingInfo rpcPagingInfo, out byte[] sortByRestriction)
		{
			rpcPagingInfo = null;
			sortByRestriction = null;
			if (queries == null || queries.Length == 0)
			{
				return new List<KeyValuePair<string, byte[]>>(0);
			}
			int num = 3;
			Exception exception = null;
			int maxAllowedKeywords = Factory.Current.GetMaxAllowedKeywords(this.searchCriteria.RecipientSession);
			int capacity = this.searchCriteria.IsStatisticsSearch ? Math.Min(maxAllowedKeywords, queries.Length) : 1;
			List<KeyValuePair<string, byte[]>> filterToRestrictionList = new List<KeyValuePair<string, byte[]>>(capacity);
			bool flag = false;
			int idx = 0;
			PagingInfo localRpcPagingInfo = null;
			byte[] localSortByRestriction = null;
			while (num > 0 && !flag && idx < this.mailboxes.Length)
			{
				this.CheckIfAborted();
				flag = Util.HandleGenericMailboxFailureForMailboxOperation(delegate
				{
					StoreSession storeSession = this.OpenMailboxSession(this.mailboxes[idx].ExchangePrincipal);
					if (this.Criteria.IsPreviewSearch)
					{
						Factory.Current.LocalTaskTracer.TraceDebug<Guid, Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Using mailbox {1} on database {2} for creating the Pagination info.", this.queryCorrelationId, this.mailboxes[idx].MailboxGuid, this.databaseServerFqdn);
						localRpcPagingInfo = this.CreateRpcPagingInfo(storeSession, rpcAdmin);
						Factory.Current.LocalTaskTracer.TraceDebug((long)this.GetHashCode(), "Correlation Id:{0}. Using mailbox {1} on database {2} for creating the SortBy criteria restriction for property {3}.", new object[]
						{
							this.queryCorrelationId,
							this.mailboxes[idx].MailboxGuid,
							this.databaseServerFqdn,
							this.PagingInfo.SortColumn.Name
						});
						QueryFilter queryFilter = new ExistsFilter(this.PagingInfo.SortColumn);
						localSortByRestriction = this.CreateRestriction(storeSession, rpcAdmin, queryFilter);
					}
					ExAssert.RetailAssert(queries != null, "Empty Keywords list");
					foreach (string text in queries)
					{
						QueryFilter queryFilter2 = text.Equals(this.Criteria.QueryString, StringComparison.OrdinalIgnoreCase) ? this.Criteria.Query : this.Criteria.SubFilters[text];
						filterToRestrictionList.Add(new KeyValuePair<string, byte[]>(text, this.CreateRestriction(storeSession, rpcAdmin, queryFilter2)));
					}
					Factory.Current.LocalTaskTracer.TraceDebug<Guid, Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Using mailbox {1} on database {2} for creating the SRestriction", this.queryCorrelationId, this.mailboxes[idx].MailboxGuid, this.databaseServerFqdn);
				}, delegate(LocalizedException ex)
				{
					exception = ex;
					queries = queries.Except(from x in filterToRestrictionList
					select x.Key).ToArray<string>();
					Factory.Current.LocalTaskTracer.TraceDebug((long)this.GetHashCode(), "Correlation Id:{0}. Serializing and creating SRestriction using mailbox {1} on database {2} failed with the error {3}", new object[]
					{
						this.queryCorrelationId,
						this.mailboxes[idx].MailboxGuid,
						this.databaseServerFqdn,
						ex.ToString()
					});
				});
				num--;
				idx++;
			}
			if (!flag)
			{
				Factory.Current.LocalTaskTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Failed to Convert QueryFilter to Restriction on database {1}", this.queryCorrelationId, this.databaseServerFqdn);
				throw new FailedToConvertSearchCriteriaToRestrictionException(this.searchCriteria.QueryString, this.databaseGuid, (exception != null) ? exception.Message : string.Empty, (exception != null) ? exception.InnerException : null);
			}
			filterToRestrictionList.TrimExcess();
			rpcPagingInfo = localRpcPagingInfo;
			sortByRestriction = localSortByRestriction;
			return filterToRestrictionList;
		}

		private StoreSession OpenMailboxSession(ExchangePrincipal exchangePrincipal)
		{
			this.CheckIfAborted();
			if (this.MailboxSessionCache == null)
			{
				if (this.disposingCallStack != null)
				{
					string text = string.Format("Stack Details of the thread that called dispose: {0} {1}", Environment.NewLine, this.disposingCallStack.ToString());
					Factory.Current.LocalTaskTracer.TraceInformation<Guid, string>(0, 0L, "Correlation Id:{0}. The mailbox session cache is null, most likely the Client/Search has been aborted/disposed. {1}", this.queryCorrelationId, text);
					Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoverySearchFailure, null, new object[]
					{
						this.searchCriteria.IsPreviewSearch ? "preview" : "keyword statistics",
						this.queryCorrelationId,
						exchangePrincipal.MailboxInfo.MailboxGuid,
						this.databaseGuid,
						this.databaseServerFqdn,
						text
					});
				}
				throw new DiscoverySearchAbortedException(this.queryCorrelationId, this.databaseGuid, this.databaseServerFqdn);
			}
			return this.MailboxSessionCache.Get(exchangePrincipal);
		}

		private byte[] GetFolderRestrictionForMailbox(int mailboxIndex, ExRpcAdmin rpcAdmin)
		{
			byte[] serializedFolderRestriction = null;
			Util.HandleGenericMailboxFailureForMailboxOperation(delegate
			{
				MailboxInfo mailboxInfo = this.Mailboxes[mailboxIndex];
				List<QueryFilter> list = new List<QueryFilter>();
				StoreSession storeSession = this.OpenMailboxSession(mailboxInfo.ExchangePrincipal);
				this.UpdateMailboxStats(storeSession, mailboxInfo.Folder);
				if (!string.IsNullOrEmpty(mailboxInfo.Folder) && storeSession.IsPublicFolderSession)
				{
					VersionedId versionedId = VersionedId.Deserialize(mailboxInfo.Folder);
					byte[] longTermFolderId = versionedId.ObjectId.LongTermFolderId;
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.MapiStoreEntryId, longTermFolderId));
				}
				else if (!storeSession.IsPublicFolderSession)
				{
					List<StoreId> excludedFolderRestrictionListForMailbox = this.GetExcludedFolderRestrictionListForMailbox(storeSession);
					foreach (StoreId id in excludedFolderRestrictionListForMailbox)
					{
						this.CheckIfAborted();
						StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
						byte[] longTermIdFromId = storeSession.IdConverter.GetLongTermIdFromId(storeObjectId);
						list.Add(new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.MapiStoreEntryId, longTermIdFromId));
					}
				}
				if (list.Count > 0)
				{
					QueryFilter queryFilter = new AndFilter(list.ToArray());
					serializedFolderRestriction = this.CreateRestriction(storeSession, rpcAdmin, queryFilter);
				}
			}, delegate(LocalizedException ex)
			{
				Factory.Current.LocalTaskTracer.TraceError<Guid, Guid, string>(0L, "Enumerating folder's for creating the folder restriction failed on mailbox {0} on database {1}, with error {2}", this.Mailboxes[mailboxIndex].MailboxGuid, this.Mailboxes[mailboxIndex].MdbGuid, ex.ToString());
				if (ex != null)
				{
					Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoverySearchFailure, null, new object[]
					{
						this.searchCriteria.IsPreviewSearch ? "preview" : "keyword statistics",
						this.queryCorrelationId.ToString(),
						this.Mailboxes[mailboxIndex].MailboxGuid.ToString(),
						this.databaseGuid.ToString(),
						this.databaseServerFqdn,
						ex
					});
				}
			});
			return serializedFolderRestriction;
		}

		private byte[] CreateRestriction(StoreSession session, ExRpcAdmin rpcAdmin, QueryFilter queryFilter)
		{
			this.CheckIfAborted();
			Factory.Current.LocalTaskTracer.TraceInformation(0, 0L, "Correlation Id:{0}. Creating restriction for query:{1} using Mailbox:{2} on database:{3}", new object[]
			{
				this.queryCorrelationId,
				queryFilter.ToString(),
				session.MailboxGuid,
				this.databaseServerFqdn
			});
			Restriction restriction = FilterRestrictionConverter.CreateRestriction(session, ExTimeZone.UtcTimeZone, session.Mailbox.MapiStore, queryFilter);
			Factory.Current.LocalTaskTracer.TraceInformation(0, 0L, "Correlation Id:{0}. Serializing restriction for query:{1} using Mailbox:{2} on database:{3}", new object[]
			{
				this.queryCorrelationId,
				queryFilter.ToString(),
				session.MailboxGuid,
				this.databaseServerFqdn
			});
			byte[] byteArray = new byte[0];
			Exception ex = null;
			if (!this.ExecuteExRpcCall(delegate(object[] args)
			{
				byteArray = rpcAdmin.SerializeAndFormatRestriction(restriction);
			}, new object[]
			{
				session.MailboxGuid
			}, out ex))
			{
				Factory.Current.LocalTaskTracer.TraceError<string, string>(0, 0L, "Creating SRestriction from QueryFilter:'{0}' failed, root cause:{1}", queryFilter.ToString(), (ex != null) ? ex.Message : string.Empty);
				if (ex != null)
				{
					Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoverySearchFailure, null, new object[]
					{
						this.searchCriteria.IsPreviewSearch ? "preview" : "keyword statistics",
						this.queryCorrelationId.ToString(),
						session.MailboxGuid.ToString(),
						this.databaseGuid.ToString(),
						this.databaseServerFqdn,
						ex
					});
				}
			}
			return byteArray;
		}

		private Sorting CreateRPCSortOrder()
		{
			if (this.PagingInfo.Direction != PageDirection.Next)
			{
				if (!this.PagingInfo.AscendingSort)
				{
					return Sorting.Ascending;
				}
				return Sorting.Descending;
			}
			else
			{
				if (!this.PagingInfo.AscendingSort)
				{
					return Sorting.Descending;
				}
				return Sorting.Ascending;
			}
		}

		private ExPerformanceCounter GetRpcSearchLatencyPerfCounter(SearchType searchType, int searchTimeInMilliSeconds)
		{
			return MultiMailboxSearch.GetSearchLatencyPerfCounter(searchType, searchTimeInMilliSeconds);
		}

		private bool ExecuteExRpcCall(MultiMailboxSearchClient.ExRpcAdminMethodDelegate methodDelegate, object[] args, out Exception exception)
		{
			exception = null;
			try
			{
				this.CheckIfAborted();
				methodDelegate(args);
			}
			catch (MapiExceptionMultiMailboxSearchInvalidRestriction)
			{
				throw new DiscoverySearchInvalidQuery();
			}
			catch (MapiExceptionNetworkError mapiExceptionNetworkError)
			{
				throw new DiscoverySearchFailed(this.databaseGuid, mapiExceptionNetworkError.ErrorCode, mapiExceptionNetworkError);
			}
			catch (MapiExceptionMultiMailboxSearchMailboxNotFound)
			{
				throw new SearchMailboxNotFound(this.databaseServerFqdn, this.MailboxDatabaseGuid.ToString(), (args != null && args.Length > 0) ? args[0].ToString() : string.Empty);
			}
			catch (MapiExceptionInvalidMultiMailboxKeywordStatsRequest innerException)
			{
				throw new DiscoverySearchInvalidKeywordStatsRequest(this.databaseGuid, this.databaseServerFqdn, innerException);
			}
			catch (MapiExceptionMultiMailboxKeywordStatsTimeOut)
			{
				exception = new DiscoveryKeywordStatsSearchTimedOut(this.mailboxes.Length, this.databaseGuid, (args != null && args.Length > 1) ? args[1].ToString() : string.Empty);
			}
			catch (MapiExceptionInvalidMultiMailboxSearchRequest innerException2)
			{
				throw new DiscoverySearchInvalidSearchRequest(this.databaseGuid, this.databaseServerFqdn, innerException2);
			}
			catch (MapiExceptionMultiMailboxSearchNonFullTextSearch)
			{
				throw new DiscoverySearchNonFullTextQuery(this.searchCriteria.SearchType, this.searchCriteria.QueryString);
			}
			catch (MapiExceptionMultiMailboxSearchInvalidSortBy)
			{
				throw new DiscoverySearchInvalidSortSpecification((this.PagingInfo.SortColumn != null) ? this.PagingInfo.SortColumn.Name : string.Empty);
			}
			catch (MapiExceptionMultiMailboxSearchNonFullTextSortBy)
			{
				throw new SearchNonFullTextSortSpecification(this.PagingInfo.SortColumn.Name);
			}
			catch (MapiExceptionMultiMailboxSearchInvalidPagination)
			{
				throw new DiscoverySearchInvalidPagination();
			}
			catch (MapiExceptionMultiMailboxSearchNonFullTextPropertyInPagination)
			{
				throw new DiscoverySearchNonFullTextPaginationProperty((this.PagingInfo.SortValue != null) ? this.pagingInfo.SortValue.SortColumn.Name : string.Empty);
			}
			catch (MapiExceptionMaxMultiMailboxSearchExceeded)
			{
				exception = new DiscoverySearchMaxSearchesExceeded(this.databaseGuid);
			}
			catch (MapiExceptionMultiMailboxSearchOperationFailed mapiExceptionMultiMailboxSearchOperationFailed)
			{
				exception = new DiscoverySearchCIFailure(this.databaseGuid, this.databaseServerFqdn, mapiExceptionMultiMailboxSearchOperationFailed.LowLevelError, mapiExceptionMultiMailboxSearchOperationFailed);
			}
			catch (MapiExceptionMultiMailboxSearchTimeOut)
			{
				exception = new DiscoverySearchTimedOut(this.mailboxes.Length, this.databaseGuid, this.searchCriteria.QueryString);
			}
			catch (StorageTransientException innerException3)
			{
				exception = new SearchTransientException(this.searchCriteria.IsPreviewSearch ? SearchType.Preview : SearchType.Statistics, innerException3);
			}
			catch (StoragePermanentException ex)
			{
				exception = new DiscoverySearchFailed(this.databaseGuid, ex.ErrorCode, ex);
			}
			catch (MapiExceptionCallFailed mapiExceptionCallFailed)
			{
				exception = new DiscoverySearchFailed(this.databaseGuid, mapiExceptionCallFailed.LowLevelError, mapiExceptionCallFailed);
			}
			catch (MapiPermanentException ex2)
			{
				exception = new DiscoverySearchFailed(this.databaseGuid, ex2.LowLevelError, ex2);
			}
			catch (MapiRetryableException innerException4)
			{
				exception = new SearchTransientException(this.searchCriteria.IsPreviewSearch ? SearchType.Preview : SearchType.Statistics, innerException4);
			}
			finally
			{
				if (exception != null)
				{
					Factory.Current.LocalTaskTracer.TraceError(this.GetHashCode(), 0L, exception.ToString());
				}
			}
			return exception == null;
		}

		private ExRpcAdmin InitRpcAdminConnection()
		{
			ExRpcAdmin rpcAdmin = null;
			Exception ex = null;
			this.ExecuteExRpcCall(delegate(object[] args)
			{
				rpcAdmin = ExRpcAdmin.Create("Client=EDiscoverySearch", this.databaseServerFqdn, null, null, null);
			}, new object[0], out ex);
			if (ex != null)
			{
				if (ex != null)
				{
					Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoverySearchFailure, null, new object[]
					{
						this.searchCriteria.IsPreviewSearch ? "preview" : "keyword statistics",
						this.queryCorrelationId.ToString(),
						Guid.Empty.ToString(),
						this.databaseGuid.ToString(),
						this.databaseServerFqdn,
						ex
					});
				}
				throw ex;
			}
			if (rpcAdmin == null)
			{
				throw new SearchTransientException(this.searchCriteria.SearchType, new DiscoverySearchFailed(this.databaseGuid, -1));
			}
			return rpcAdmin;
		}

		private void ExecutePreviewSearch(int refinerResultTrimCount, ExRpcAdmin rpcAdmin, PagingInfo rpcPagingInfo, byte[] sortByRestriction, List<KeyValuePair<string, byte[]>> queryFilterList, Sorting rpcSortOrder, int currentMailboxIndex, ref ResultAggregator resultAggregator, ref long totalTimeTaken, ref long totalFastTime, ref long totalStoreTime)
		{
			this.CheckIfAborted();
			Exception ex = null;
			MultiMailboxSearchRequest multiMailboxSearchRequest = new MultiMailboxSearchRequest();
			Stopwatch stopwatch = Stopwatch.StartNew();
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			multiMailboxSearchRequest.MailboxInfos = this.CreateRpcMailInfo(currentMailboxIndex, rpcAdmin);
			string mailbox = multiMailboxSearchRequest.MailboxInfos[0].MailboxGuid.ToString();
			Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Executing eDiscovery preview search for query:{1} on Mailbox:{2} ({3} of {4} mailboxes) in Database:{5}", new object[]
			{
				this.queryCorrelationId,
				this.Criteria.QueryString,
				multiMailboxSearchRequest.MailboxInfos[0].MailboxGuid,
				currentMailboxIndex + 1,
				this.Mailboxes.Length,
				this.databaseGuid
			});
			this.RestrictionTime += stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
			this.UpdateSearchStatistics(mailbox, "RTIME", stopwatch.ElapsedMilliseconds - elapsedMilliseconds);
			elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			multiMailboxSearchRequest.SortingOrder = rpcSortOrder;
			multiMailboxSearchRequest.Paging = rpcPagingInfo;
			multiMailboxSearchRequest.Query = this.searchCriteria.QueryString;
			multiMailboxSearchRequest.RefinersEnabled = (refinerResultTrimCount > 0);
			multiMailboxSearchRequest.RefinerResultsTrimCount = refinerResultTrimCount;
			multiMailboxSearchRequest.SortCriteria = sortByRestriction;
			multiMailboxSearchRequest.CorrelationId = this.queryCorrelationId;
			if (queryFilterList != null && queryFilterList.Count > 0 && queryFilterList[0].Value != null && queryFilterList[0].Value.Length > 0)
			{
				multiMailboxSearchRequest.Restriction = queryFilterList[0].Value;
			}
			byte[] requestByteArray = MultiMailboxSearchRequest.Serialize(multiMailboxSearchRequest);
			byte[] responseByteArray = null;
			try
			{
				if (!this.ExecuteExRpcCall(delegate(object[] args)
				{
					responseByteArray = rpcAdmin.MultiMailboxSearch(this.databaseGuid, requestByteArray);
				}, new object[]
				{
					this.mailboxes[currentMailboxIndex].MailboxGuid
				}, out ex))
				{
					Factory.Current.LocalTaskTracer.TraceInformation<Guid, string, string>(0, 0L, "eDiscovery preview search failed for mailbox '{0}' on database '{1}'. Root cause: {2}", this.mailboxes[currentMailboxIndex].MailboxGuid, this.databaseServerFqdn, (ex != null) ? ex.ToString() : string.Empty);
					if (ex != null)
					{
						ExEventLog.EventTuple tuple;
						object[] messageArgs;
						if (ex is DiscoverySearchCIFailure)
						{
							tuple = InfoWorkerEventLogConstants.Tuple_DiscoverySearchCIFailure;
							messageArgs = new object[]
							{
								"preview",
								this.queryCorrelationId.ToString(),
								this.Mailboxes[currentMailboxIndex].MailboxGuid.ToString(),
								this.databaseGuid.ToString(),
								this.databaseServerFqdn,
								false,
								ex.InnerException
							};
						}
						else
						{
							tuple = InfoWorkerEventLogConstants.Tuple_DiscoverySearchFailure;
							messageArgs = new object[]
							{
								"preview",
								this.queryCorrelationId.ToString(),
								this.Mailboxes[currentMailboxIndex].MailboxGuid.ToString(),
								this.databaseGuid.ToString(),
								this.databaseServerFqdn,
								ex
							};
						}
						Factory.Current.EventLog.LogEvent(tuple, null, messageArgs);
					}
				}
			}
			finally
			{
				stopwatch.Stop();
				resultAggregator.ProtocolLog.Add("LongestMailboxFastSearchTime", stopwatch.ElapsedMilliseconds);
				totalFastTime = stopwatch.ElapsedMilliseconds;
				long num = stopwatch.ElapsedMilliseconds;
				Factory.Current.LocalTaskTracer.TracePerformance(this.GetHashCode(), 0L, "Correlation Id:{0}. Fast search time for mailbox:{1} in database:{2} was {3}ms.", new object[]
				{
					this.queryCorrelationId,
					this.Mailboxes[currentMailboxIndex].MailboxGuid,
					this.databaseServerFqdn,
					num
				});
				this.FastTime += stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
				this.UpdateSearchStatistics(mailbox, "FAST", stopwatch.ElapsedMilliseconds - elapsedMilliseconds);
				elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				stopwatch.Restart();
				elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				MultiMailboxSearchResponse multiMailboxSearchResponse = MultiMailboxSearchResponse.DeSerialize(responseByteArray);
				AggregatedSearchTaskResult aggregatedSearchTaskResult = null;
				if (multiMailboxSearchResponse != null && multiMailboxSearchResponse.Results != null)
				{
					aggregatedSearchTaskResult = this.ProcessRpcSearchResponse(multiMailboxSearchResponse, this.Mailboxes[currentMailboxIndex]);
					this.UpdateSearchStatistics(mailbox, "FASTCOUNT", multiMailboxSearchResponse.Count);
					this.ReturnedFastItems += multiMailboxSearchResponse.Count;
					if (aggregatedSearchTaskResult != null)
					{
						this.UpdateSearchStatistics(mailbox, "STORECOUNT", aggregatedSearchTaskResult.TotalResultCount);
						this.UpdateSearchStatistics(mailbox, "STORESIZE", aggregatedSearchTaskResult.TotalResultSize.ToKB());
						this.ReturnedStoreSize += (long)aggregatedSearchTaskResult.TotalResultSize.ToKB();
						this.ReturnedStoreItems += (long)aggregatedSearchTaskResult.TotalResultCount;
					}
				}
				if (ex != null)
				{
					resultAggregator.PreviewErrors.Add(new Pair<MailboxInfo, Exception>(this.mailboxes[currentMailboxIndex], ex));
				}
				stopwatch.Stop();
				totalTimeTaken += stopwatch.ElapsedMilliseconds;
				totalStoreTime += stopwatch.ElapsedMilliseconds;
				num += stopwatch.ElapsedMilliseconds;
				resultAggregator.ProtocolLog.Add("LongestMailboxStoreTime", stopwatch.ElapsedMilliseconds);
				resultAggregator.ProtocolLog.Add("LongestMailboxSearchTime", totalTimeTaken);
				resultAggregator.MergeSearchResult(aggregatedSearchTaskResult);
				PerformanceCounters.AveragePreviewSearchPerMailboxProcessingTime.IncrementBy(num);
				PerformanceCounters.AveragePreviewSearchPerMailboxProcessingTimeBase.Increment();
				Factory.Current.LocalTaskTracer.TracePerformance(this.GetHashCode(), 0L, "Correlation Id:{0}. Reading properties from store for mailbox:{1} in database:{2} took {3} ms.", new object[]
				{
					this.queryCorrelationId,
					this.Mailboxes[currentMailboxIndex].MailboxGuid,
					this.databaseServerFqdn,
					stopwatch.ElapsedMilliseconds
				});
				this.StoreTime += stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
				this.UpdateSearchStatistics(mailbox, "STORE", stopwatch.ElapsedMilliseconds - elapsedMilliseconds);
				elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			}
		}

		private void ExecuteKeywordHits(string[] keywordsToComputeStatsFor, ExRpcAdmin rpcAdmin, List<KeyValuePair<string, byte[]>> queryFilterRestrictions, int currentMailboxIndex, ref ResultAggregator resultAggregator, ref long totalTimeTaken)
		{
			this.CheckIfAborted();
			byte[] responseByteArray = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			MultiMailboxKeywordStatsRequest multiMailboxKeywordStatsRequest = new MultiMailboxKeywordStatsRequest();
			multiMailboxKeywordStatsRequest.MailboxInfos = this.CreateRpcMailInfo(currentMailboxIndex, rpcAdmin);
			Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Executing eDiscovery keyword stats search for {1}, on Mailbox:{2} ({3} of {4} mailboxes) in Database:{5}", new object[]
			{
				this.queryCorrelationId,
				queryFilterRestrictions.Count,
				multiMailboxKeywordStatsRequest.MailboxInfos[0].MailboxGuid,
				currentMailboxIndex + 1,
				this.Mailboxes.Length,
				this.databaseGuid
			});
			multiMailboxKeywordStatsRequest.Keywords = queryFilterRestrictions;
			multiMailboxKeywordStatsRequest.CorrelationId = this.queryCorrelationId;
			byte[] requestByteArray = MultiMailboxKeywordStatsRequest.Serialize(multiMailboxKeywordStatsRequest);
			Exception ex = null;
			try
			{
				if (!this.ExecuteExRpcCall(delegate(object[] args)
				{
					responseByteArray = rpcAdmin.GetMultiMailboxSearchKeywordStats(this.databaseGuid, requestByteArray);
				}, new object[]
				{
					this.mailboxes[currentMailboxIndex].MailboxGuid,
					string.Join(" OR ", keywordsToComputeStatsFor)
				}, out ex))
				{
					Factory.Current.LocalTaskTracer.TraceInformation(0, 0L, "Correlation Id:{0}. eDiscovery keyword stats search failed for mailbox '{1}' on database '{2}'. Root cause: {3}", new object[]
					{
						this.queryCorrelationId,
						this.mailboxes[currentMailboxIndex].MailboxGuid,
						this.databaseServerFqdn,
						(ex != null) ? ex.ToString() : string.Empty
					});
					if (ex != null)
					{
						ExEventLog.EventTuple tuple;
						object[] messageArgs;
						if (ex is DiscoverySearchCIFailure)
						{
							tuple = InfoWorkerEventLogConstants.Tuple_DiscoverySearchCIFailure;
							messageArgs = new object[]
							{
								"keyword statistics",
								this.queryCorrelationId.ToString(),
								this.Mailboxes[currentMailboxIndex].MailboxGuid.ToString(),
								this.databaseGuid.ToString(),
								this.databaseServerFqdn,
								false,
								ex.InnerException
							};
						}
						else
						{
							tuple = InfoWorkerEventLogConstants.Tuple_DiscoverySearchFailure;
							messageArgs = new object[]
							{
								"keyword statistics",
								this.queryCorrelationId.ToString(),
								this.Mailboxes[currentMailboxIndex].MailboxGuid.ToString(),
								this.databaseGuid.ToString(),
								this.databaseServerFqdn,
								ex
							};
						}
						Factory.Current.EventLog.LogEvent(tuple, null, messageArgs);
					}
				}
			}
			finally
			{
				stopwatch.Stop();
				totalTimeTaken += stopwatch.ElapsedMilliseconds;
				PerformanceCounters.AverageStatisticsSearchPerMailboxProcessingTime.IncrementBy(stopwatch.ElapsedMilliseconds);
				PerformanceCounters.AverageStatisticsSearchPerMailboxProcessingTimeBase.Increment();
				Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. eDiscovery MultiMailbox Keyword Stats on mailbox:{1} in database:{2} took {3} ms.", new object[]
				{
					this.queryCorrelationId,
					this.Mailboxes[currentMailboxIndex].MailboxGuid,
					this.databaseServerFqdn,
					stopwatch.ElapsedMilliseconds
				});
			}
			AggregatedSearchTaskResult aggregator = null;
			if (ex == null)
			{
				MultiMailboxKeywordStatsResponse multiMailboxKeywordStatsResponse = MultiMailboxKeywordStatsResponse.DeSerialize(responseByteArray);
				List<IKeywordHit> list = this.ProcessKeywordHitRpcResponse((MultiMailboxKeywordStatsResult[])multiMailboxKeywordStatsResponse.Results);
				if (list != null)
				{
					aggregator = new AggregatedSearchTaskResult(Factory.Current.CreateMailboxInfoList(new MailboxInfo[]
					{
						this.Mailboxes[currentMailboxIndex]
					}), list);
				}
			}
			else
			{
				aggregator = new AggregatedSearchTaskResult(Factory.Current.CreateMailboxInfoList(new MailboxInfo[]
				{
					this.Mailboxes[currentMailboxIndex]
				}), new List<string>(keywordsToComputeStatsFor), ex);
			}
			resultAggregator.MergeSearchResult(aggregator);
		}

		private List<object[]> ReadRowsFromQueryResult(QueryResult queryResult, int rowsToFetch, Guid mailboxId)
		{
			Util.ThrowOnNull(queryResult, "queryResult");
			if (rowsToFetch < 0)
			{
				throw new ArgumentException("Invalid row count specified.");
			}
			int num = Math.Min(rowsToFetch, 100);
			Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Fetching Preview properties from mailbox:{1} for {2} items using batch size:{3}", new object[]
			{
				this.queryCorrelationId,
				mailboxId,
				rowsToFetch,
				num
			});
			List<object[]> list = new List<object[]>(rowsToFetch);
			this.CheckIfAborted();
			object[][] rows;
			while ((rows = queryResult.GetRows(num)) != null && rows.Length > 0)
			{
				list.AddRange(rows);
				this.CheckIfAborted();
			}
			return list;
		}

		private void CheckIfAborted()
		{
			if (Interlocked.Read(ref this.aborted) == 1L)
			{
				Factory.Current.LocalTaskTracer.TraceInformation<Guid, Guid, string>(this.GetHashCode(), 0L, "Correlation Id:{0}. MMS client has been aborted for database:{1} in server:{2}", this.queryCorrelationId, this.databaseGuid, this.databaseServerFqdn);
				throw new DiscoverySearchAbortedException(this.queryCorrelationId, this.databaseGuid, this.databaseServerFqdn);
			}
		}

		private SortedResultPage CreatePreviewItemsFromStore(Guid mailboxId, List<KeyValuePair<int, long>> messageIdPairs, MailboxInfo mailboxInfo)
		{
			SortedResultPage result = null;
			List<object[]> list = new List<object[]>(messageIdPairs.Count);
			List<ComparisonFilter> list2 = new List<ComparisonFilter>(messageIdPairs.Count);
			foreach (KeyValuePair<int, long> keyValuePair in messageIdPairs)
			{
				list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.DocumentId, keyValuePair.Key));
			}
			Factory.Current.GeneralTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Opening the Mailbox({1}) on database:{2}, to fetch the preview properties for {3} results.", new object[]
			{
				this.queryCorrelationId,
				mailboxInfo.MailboxGuid,
				mailboxInfo.IsPrimary ? mailboxInfo.MdbGuid : mailboxInfo.ArchiveDatabase,
				list2.Count
			});
			this.CheckIfAborted();
			Folder folder = null;
			ItemQueryType queryFlags = ItemQueryType.None;
			StoreSession storeSession = this.OpenMailboxSession(mailboxInfo.ExchangePrincipal);
			if (!string.IsNullOrEmpty(mailboxInfo.Folder) && storeSession.IsPublicFolderSession)
			{
				VersionedId folderId = VersionedId.Deserialize(mailboxInfo.Folder);
				folder = Folder.Bind(storeSession, folderId);
			}
			else if (!storeSession.IsPublicFolderSession)
			{
				folder = Folder.Bind(storeSession, DefaultFolderType.Configuration, new PropertyDefinition[]
				{
					FolderSchema.Id
				});
				queryFlags = ItemQueryType.DocumentIdView;
			}
			if (folder != null)
			{
				using (folder)
				{
					QueryFilter queryFilter = new OrFilter(list2.ToArray());
					using (QueryResult queryResult = folder.ItemQuery(queryFlags, queryFilter, null, this.pagingInfo.DataColumns))
					{
						list = this.ReadRowsFromQueryResult(queryResult, messageIdPairs.Count, mailboxId);
					}
				}
			}
			if (list != null && list.Count > 0)
			{
				if (list.Count != list2.Count)
				{
					Factory.Current.GeneralTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Expected to read preview properties for {1} docids from mailbox:{2} on database {3}, but the read actually returned for only {4} docids.", new object[]
					{
						this.queryCorrelationId,
						list2.Count,
						mailboxInfo.MailboxGuid,
						mailboxInfo.IsPrimary ? mailboxInfo.MdbGuid : mailboxInfo.ArchiveDatabase,
						list.Count
					});
				}
				this.CheckIfAborted();
				PreviewItem[] array = Util.CreateSearchPreviewItems(mailboxInfo, list.ToArray(), messageIdPairs, storeSession, this.pagingInfo);
				Array.Sort<PreviewItem>(array, new PreviewItemComparer(this.pagingInfo.AscendingSort));
				result = new SortedResultPage(array, this.pagingInfo);
			}
			else
			{
				Factory.Current.LocalTaskTracer.TraceDebug((long)this.GetHashCode(), "Correlation Id:{0}. Got {1} hits from CI in mailbox {2} in {3} database, but ItemQuery for the hits yielded 0 results", new object[]
				{
					this.queryCorrelationId,
					messageIdPairs.Count,
					mailboxId,
					this.databaseServerFqdn
				});
			}
			return result;
		}

		private StoreSession CreateSessionForCache(ExchangePrincipal mailboxIdentity)
		{
			MailboxInfo mailboxInfo = this.Mailboxes.FirstOrDefault((MailboxInfo t) => t.ExchangePrincipal == mailboxIdentity);
			if (mailboxInfo != null && mailboxInfo.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
			{
				return SearchUtils.OpenSession(mailboxIdentity, this.sessionCache.ExecutingUserGenericIdentity, true);
			}
			return SearchUtils.OpenSession(mailboxIdentity, this.sessionCache.ExecutingUserGenericIdentity, false);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.previewSearchFailures != null)
				{
					this.previewSearchFailures.Clear();
				}
				if (this.MailboxSessionCache != null)
				{
					this.sessionCache.Dispose();
					this.sessionCache = null;
				}
				if (this.disposingCallStack == null)
				{
					this.disposingCallStack = new StackTrace(true);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MultiMailboxSearchClient>(this);
		}

		private void UpdateMailboxStats(StoreSession storeSession, string folder)
		{
			if (storeSession != null)
			{
				MailboxSession mailboxSession = storeSession as MailboxSession;
				if (mailboxSession != null)
				{
					MailboxSession.MailboxItemCountsAndSizes mailboxItemCountsAndSizes = mailboxSession.ReadMailboxCountsAndSizes();
					string mailbox = mailboxSession.MailboxGuid.ToString();
					this.UpdateSearchStatistics(mailbox, "CNT", mailboxItemCountsAndSizes.ItemCount);
					this.UpdateSearchStatistics(mailbox, "DELCNT", mailboxItemCountsAndSizes.DeletedItemCount);
					this.UpdateSearchStatistics(mailbox, "SIZE", mailboxItemCountsAndSizes.TotalItemSize);
					this.UpdateSearchStatistics(mailbox, "DELSIZE", mailboxItemCountsAndSizes.TotalDeletedItemSize);
					this.UpdateSearchStatistics(mailbox, "TYPE", "MAILBOX");
					if (mailboxItemCountsAndSizes.DeletedItemCount != null)
					{
						this.TotalItems += (long)mailboxItemCountsAndSizes.DeletedItemCount.Value;
					}
					if (mailboxItemCountsAndSizes.ItemCount != null)
					{
						this.TotalItems += (long)mailboxItemCountsAndSizes.ItemCount.Value;
					}
					if (mailboxItemCountsAndSizes.TotalDeletedItemSize != null)
					{
						this.TotalSize += mailboxItemCountsAndSizes.TotalDeletedItemSize.Value;
					}
					if (mailboxItemCountsAndSizes.TotalItemSize != null)
					{
						this.TotalSize += mailboxItemCountsAndSizes.TotalItemSize.Value;
						return;
					}
				}
				else if (storeSession.IsPublicFolderSession)
				{
					this.UpdateSearchStatistics(folder ?? storeSession.MailboxGuid.ToString(), "TYPE", "PF");
				}
			}
		}

		private void UpdateSearchStatistics(string mailbox, string statistic, object value)
		{
			if (this.SearchStatistics == null)
			{
				this.SearchStatistics = new Dictionary<string, Dictionary<string, string>>();
			}
			if (!this.SearchStatistics.ContainsKey(mailbox))
			{
				this.SearchStatistics[mailbox] = new Dictionary<string, string>();
			}
			this.SearchStatistics[mailbox][statistic] = ((value != null) ? value.ToString() : null);
		}

		private const string SenderRefinerEntryName = "from";

		private const int QUERYROWS_BATCH_SIZE = 100;

		private readonly Guid databaseGuid;

		private readonly MailboxInfo[] mailboxes;

		private readonly PagingInfo pagingInfo;

		private readonly CallerInfo executingUserIdentity;

		private readonly SearchCriteria searchCriteria;

		private string databaseServerFqdn = string.Empty;

		private readonly Guid queryCorrelationId;

		private List<Pair<MailboxInfo, Exception>> previewSearchFailures;

		private MailboxSessionCache sessionCache;

		private long aborted;

		private StackTrace disposingCallStack;

		private delegate void ExRpcAdminMethodDelegate(object[] args);
	}
}
