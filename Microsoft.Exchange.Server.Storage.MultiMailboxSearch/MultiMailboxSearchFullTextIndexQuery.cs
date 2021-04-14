using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MultiMailboxSearch;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.MultiMailboxSearch
{
	internal class MultiMailboxSearchFullTextIndexQuery : IMultiMailboxSearchFullTextIndexQuery
	{
		public List<string> RefinersList { get; set; }

		public List<string> ExtraFieldsList { get; set; }

		public Guid QueryCorrelationId { get; set; }

		internal static string MailboxGuidFastPropertyName
		{
			get
			{
				if (string.IsNullOrEmpty(MultiMailboxSearchFullTextIndexQuery.mailboxGuidFastPropertyName))
				{
					ExTraceGlobals.FullTextIndexTracer.TraceInformation(60988, 0L, "Getting the Fast Property Name for the MailboxGuid Store Property.");
					FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo = null;
					FullTextIndexSchema.Current.IsPropertyInFullTextIndex(PropTag.Message.MailboxGuid.PropInfo.PropName, Guid.Empty, out fullTextIndexInfo);
					ExTraceGlobals.FullTextIndexTracer.TraceInformation<string>(36412, 0L, "The Fast Property Name for the MailboxGuid Store Property is {0}", fullTextIndexInfo.FastPropertyName);
					MultiMailboxSearchFullTextIndexQuery.mailboxGuidFastPropertyName = fullTextIndexInfo.FastPropertyName;
				}
				return MultiMailboxSearchFullTextIndexQuery.mailboxGuidFastPropertyName;
			}
		}

		internal static string SizeFastPropertyName
		{
			get
			{
				if (string.IsNullOrEmpty(MultiMailboxSearchFullTextIndexQuery.sizeFastPropertyName))
				{
					ExTraceGlobals.FullTextIndexTracer.TraceInformation(52112, 0L, "Getting the Fast Property Name for the Size Store Property.");
					FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo = null;
					FullTextIndexSchema.Current.IsPropertyInFullTextIndex(PropTag.Message.MessageSize.PropInfo.PropName, Guid.Empty, out fullTextIndexInfo);
					ExTraceGlobals.FullTextIndexTracer.TraceInformation<string>(45968, 0L, "The Fast Property Name for the Size Store Property is {0}", fullTextIndexInfo.FastPropertyName);
					MultiMailboxSearchFullTextIndexQuery.sizeFastPropertyName = fullTextIndexInfo.FastPropertyName;
				}
				return MultiMailboxSearchFullTextIndexQuery.sizeFastPropertyName;
			}
		}

		protected bool IsAborted
		{
			get
			{
				return Interlocked.CompareExchange(ref this.isAborted, 0, 0) == 1;
			}
		}

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
				if (MultiMailboxSearchFullTextIndexQuery.serviceProxyWrapperInstance == null)
				{
					string hostName = MultiMailboxSearchFullTextIndexQuery.SearchConfig.HostName;
					int queryServicePort = MultiMailboxSearchFullTextIndexQuery.SearchConfig.QueryServicePort;
					int fastMmsImsChannelPoolSize = MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastMmsImsChannelPoolSize;
					TimeSpan fastImsMmsSendTimeout = MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastImsMmsSendTimeout;
					TimeSpan fastImsMmsReceiveTimeout = MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastImsMmsReceiveTimeout;
					int fastMmsImsRetryCount = MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastMmsImsRetryCount;
					long num = (long)MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastIMSMaxReceivedMessageSize;
					int fastIMSMaxStringContentLength = MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastIMSMaxStringContentLength;
					TimeSpan fastProxyCacheTimeout = MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastProxyCacheTimeout;
					Hookable<PagingImsFlowExecutor> value = Hookable<PagingImsFlowExecutor>.Create(true, new PagingImsFlowExecutor(hostName, queryServicePort, fastMmsImsChannelPoolSize, fastImsMmsSendTimeout, fastImsMmsSendTimeout, fastImsMmsReceiveTimeout, fastProxyCacheTimeout, num, fastIMSMaxStringContentLength, fastMmsImsRetryCount));
					Interlocked.CompareExchange<Hookable<PagingImsFlowExecutor>>(ref MultiMailboxSearchFullTextIndexQuery.serviceProxyWrapperInstance, value, null);
				}
				return MultiMailboxSearchFullTextIndexQuery.serviceProxyWrapperInstance;
			}
		}

		private static int PageSize
		{
			get
			{
				if (MultiMailboxSearchFullTextIndexQuery.pageSize == -1)
				{
					int fastQueryResultTrimHits = MultiMailboxSearchFullTextIndexQuery.SearchConfig.FastQueryResultTrimHits;
					Interlocked.CompareExchange(ref MultiMailboxSearchFullTextIndexQuery.pageSize, fastQueryResultTrimHits, -1);
				}
				return MultiMailboxSearchFullTextIndexQuery.pageSize;
			}
		}

		public static IDisposable SetPagingImsFlowExecutorTestHook(PagingImsFlowExecutor testHook)
		{
			return MultiMailboxSearchFullTextIndexQuery.ServiceProxyWrapper.SetTestHook(testHook);
		}

		public virtual KeywordStatsResultRow ExecuteFullTextKeywordHitsQuery(Guid databaseGuid, Guid mailboxGuid, string query)
		{
			bool flag = ExTraceGlobals.FullTextIndexTracer.IsTraceEnabled(TraceType.PerformanceTrace);
			Stopwatch stopwatch = flag ? Stopwatch.StartNew() : null;
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Entering MultiMailboxSearchFullTextIndexQuery.ExecuteFullTextKeywordHitsQuery");
			string imsFlowName = FullTextIndexSchema.GetImsFlowName(databaseGuid);
			ExTraceGlobals.FullTextIndexTracer.TraceInformation<Guid, string, Guid>(50748, 0L, "Correlation Id:{0}. Executing flow {1} on database {2}.", this.QueryCorrelationId, imsFlowName, databaseGuid);
			long count;
			Dictionary<string, List<RefinersResultRow>> refinersResult;
			try
			{
				using (PagingImsFlowExecutor.QueryExecutionContext queryExecutionContext = MultiMailboxSearchFullTextIndexQuery.ServiceProxyWrapper.Value.ExecuteWithNoResults(imsFlowName, mailboxGuid, this.QueryCorrelationId, query, CultureInfo.InvariantCulture, this.GenerateParameters(null)))
				{
					this.CheckIsAborted();
					count = MultiMailboxSearchFullTextIndexQuery.ServiceProxyWrapper.Value.ReadHitCount(queryExecutionContext);
					this.CheckIsAborted();
					refinersResult = MultiMailboxSearchFullTextIndexQuery.ReadRefinerInformation(queryExecutionContext);
				}
			}
			finally
			{
				if (flag)
				{
					stopwatch.Stop();
					ExTraceGlobals.FullTextIndexTracer.TracePerformance(0L, "Correlation Id:{0}. FullTextIndexQuery.ExecuteFullTextKeywordHitsQuery for mailbox {1} took {2}ms for query '{3}'", new object[]
					{
						this.QueryCorrelationId,
						mailboxGuid,
						stopwatch.ElapsedMilliseconds,
						query
					});
				}
			}
			double size = MultiMailboxSearchFullTextIndexQuery.FetchSizeFromRefiners(refinersResult, this.QueryCorrelationId);
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.ExecuteFullTextKeywordHitsQuery");
			return new KeywordStatsResultRow(query, count, size);
		}

		public virtual IList<FullTextIndexRow> ExecuteFullTextIndexQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, int maxResults, string sortSpec, out KeywordStatsResultRow keywordStatsResult, out Dictionary<string, List<RefinersResultRow>> refinersOutput)
		{
			bool flag = ExTraceGlobals.FullTextIndexTracer.IsTraceEnabled(TraceType.PerformanceTrace);
			Stopwatch stopwatch = flag ? Stopwatch.StartNew() : null;
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Entering MultiMailboxSearchFullTextIndexQuery.ExecuteFullTextIndexQuery");
			ExTraceGlobals.FullTextIndexTracer.TraceInformation(62352, 0L, "Correlation Id:{0}. Invoking FullTextIndexQuery.ExecuteFullTextIndexQuery for the following query {1}, with PageSize: {2}, SortCriteria: {3}, MaxHits: {4}", new object[]
			{
				this.QueryCorrelationId,
				query,
				MultiMailboxSearchFullTextIndexQuery.PageSize,
				sortSpec,
				maxResults
			});
			string imsFlowName = FullTextIndexSchema.GetImsFlowName(databaseGuid);
			ExTraceGlobals.FullTextIndexTracer.TraceInformation(58256, 0L, "Correlation Id:{0}. Executing flow {1} on database {2} for mailbox {3}", new object[]
			{
				this.QueryCorrelationId,
				imsFlowName,
				databaseGuid,
				mailboxGuid
			});
			long num = 0L;
			refinersOutput = null;
			keywordStatsResult = null;
			List<FullTextIndexRow> list = new List<FullTextIndexRow>(0);
			bool flag2 = true;
			foreach (KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]> keyValuePair in MultiMailboxSearchFullTextIndexQuery.ServiceProxyWrapper.Value.Execute(imsFlowName, mailboxGuid, this.QueryCorrelationId, query, 0L, CultureInfo.InvariantCulture, this.GenerateParameters(sortSpec), Math.Min(MultiMailboxSearchFullTextIndexQuery.PageSize, maxResults), null))
			{
				if (flag2)
				{
					flag2 = false;
					this.CheckIsAborted();
					num = MultiMailboxSearchFullTextIndexQuery.ServiceProxyWrapper.Value.ReadHitCount(keyValuePair.Key);
					this.CheckIsAborted();
					refinersOutput = MultiMailboxSearchFullTextIndexQuery.ReadRefinerInformation(keyValuePair.Key);
				}
				MultiMailboxSearchFullTextIndexQuery.ProcessSearchResults(list, keyValuePair.Value, imsFlowName, mailboxGuid, mailboxNumber, maxResults, this.QueryCorrelationId);
				if (list.Count >= maxResults)
				{
					break;
				}
				this.CheckIsAborted();
			}
			double num2 = MultiMailboxSearchFullTextIndexQuery.FetchSizeFromRefiners(refinersOutput, this.QueryCorrelationId);
			keywordStatsResult = new KeywordStatsResultRow(query, num, num2);
			if (flag)
			{
				stopwatch.Stop();
				ExTraceGlobals.FullTextIndexTracer.TracePerformance(0L, "Correlation Id:{0}. FullTextIndexQuery.ExecuteFullTextIndexQuery for mailbox {1} completed in {2}ms for query '{3}'", new object[]
				{
					this.QueryCorrelationId,
					mailboxGuid,
					stopwatch.ElapsedMilliseconds,
					query
				});
			}
			ExTraceGlobals.FullTextIndexTracer.TraceInformation(37776, 0L, "Correlation Id:{0}. FullTextIndexQuery.ExecuteFullTextIndexQuery for the following query {1} yielded {2} results, size: {3} bytes", new object[]
			{
				this.QueryCorrelationId,
				query,
				num,
				num2
			});
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.ExecuteFullTextIndexQuery");
			return list;
		}

		public virtual void Abort()
		{
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Entering MultiMailboxSearchFullTextIndexQuery.Abort");
			Interlocked.Exchange(ref this.isAborted, 1);
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.Abort");
		}

		private static Dictionary<string, List<RefinersResultRow>> ReadRefinerInformation(PagingImsFlowExecutor.QueryExecutionContext context)
		{
			IReadOnlyCollection<RefinerResult> readOnlyCollection = MultiMailboxSearchFullTextIndexQuery.ServiceProxyWrapper.Value.ReadRefiners(context);
			Dictionary<string, List<RefinersResultRow>> dictionary = new Dictionary<string, List<RefinersResultRow>>(readOnlyCollection.Count);
			foreach (RefinerResult refinerResult in readOnlyCollection)
			{
				string name = refinerResult.Name;
				List<RefinersResultRow> list = new List<RefinersResultRow>(4);
				dictionary.Add(name, list);
				foreach (RefinerEntry refinerEntry in refinerResult.Entries)
				{
					list.Add(RefinersResultRow.NewRow(refinerEntry.Name, refinerEntry.Count, refinerResult.Sum, refinerResult.Min, refinerResult.Max));
				}
			}
			return dictionary;
		}

		private static void ProcessSearchResults(List<FullTextIndexRow> rows, SearchResultItem[] items, string flowName, Guid mailboxGuid, int mailboxNumber, int maxResults, Guid queryCorrelationId)
		{
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Entering MultiMailboxSearchFullTextIndexQuery.ProcessSearchResults");
			if (items == null || items.Length <= 0)
			{
				MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.ProcessSearchResults");
				return;
			}
			ExTraceGlobals.FullTextIndexTracer.TraceInformation(47248, 0L, "Correlation Id:{0}. Flow {1} for Mailbox {2}, returned {3} items. Processing them.", new object[]
			{
				queryCorrelationId,
				flowName,
				mailboxGuid,
				items.Length
			});
			ExTraceGlobals.FullTextIndexTracer.TraceInformation<Guid, string>(33936, 0L, "Correlation Id:{0}. Finding the mailboxguid field index in the results for flow {1}", queryCorrelationId, flowName);
			SearchResultItem searchResultItem = items[0];
			rows.Capacity = Math.Min(maxResults, rows.Count + items.Length);
			foreach (SearchResultItem searchResultItem2 in items)
			{
				long num = (long)searchResultItem2.Fields[1].Value;
				int mailboxNumber2 = IndexId.GetMailboxNumber(num);
				if (mailboxNumber == mailboxNumber2)
				{
					rows.Add(FullTextIndexRow.Parse(mailboxGuid, num));
				}
				if (rows.Count == maxResults)
				{
					return;
				}
			}
		}

		private static int GetMailboxGuidFieldIndexFromFastResults(SearchResultItem firstItem)
		{
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Entering MultiMailboxSearchFullTextIndexQuery.GetMailboxGuidFieldIndexFromFastResults");
			int result = -1;
			if (firstItem != null && firstItem.Fields.Count == 3 && firstItem.Fields[2] != null && !string.IsNullOrEmpty(firstItem.Fields[2].Name) && firstItem.Fields[2].Name.Equals("other", StringComparison.OrdinalIgnoreCase))
			{
				SearchResultItem searchResultItem = firstItem.Fields[2].Value as SearchResultItem;
				if (searchResultItem != null && searchResultItem.Fields != null && searchResultItem.Fields.Count > 0)
				{
					for (int i = 0; i < searchResultItem.Fields.Count; i++)
					{
						if (searchResultItem.Fields[i].Name.Equals(MultiMailboxSearchFullTextIndexQuery.MailboxGuidFastPropertyName, StringComparison.OrdinalIgnoreCase))
						{
							result = i;
							break;
						}
					}
				}
			}
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.GetMailboxGuidFieldIndexFromFastResults");
			return result;
		}

		private static void TraceFunction(string message)
		{
			ExTraceGlobals.FullTextIndexTracer.TraceFunction(46608, 0L, message);
		}

		private static double FetchSizeFromRefiners(Dictionary<string, List<RefinersResultRow>> refinersResult, Guid queryCorrelationId)
		{
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Entering MultiMailboxSearchFullTextIndexQuery.FetchSizeFromRefiners");
			double result = 0.0;
			if (refinersResult == null || refinersResult.Count == 0)
			{
				ExTraceGlobals.FullTextIndexTracer.TraceInformation<Guid>(33680, 0L, "Correlation Id:{0}. Refiner result is null or empty", queryCorrelationId);
				MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.FetchSizeFromRefiners");
				return result;
			}
			List<RefinersResultRow> list = null;
			if (refinersResult.TryGetValue(MultiMailboxSearchFullTextIndexQuery.SizeFastPropertyName, out list) && list != null && list.Count > 0)
			{
				ExTraceGlobals.FullTextIndexTracer.TraceInformation<Guid>(50064, 0L, "Correlation Id:{0}. Found size refiner result in the refiner results.", queryCorrelationId);
				result = list[0].Sum;
				refinersResult.Remove(MultiMailboxSearchFullTextIndexQuery.SizeFastPropertyName);
			}
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.FetchSizeFromRefiners");
			return result;
		}

		private void CheckIsAborted()
		{
			if (this.IsAborted)
			{
				throw new TimeoutException("Search was aborted.");
			}
		}

		private AdditionalParameters GenerateParameters(string sortSpec)
		{
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Entering MultiMailboxSearchFullTextIndexQuery.GenerateParameters");
			AdditionalParameters additionalParameters = new AdditionalParameters();
			if (!string.IsNullOrEmpty(sortSpec))
			{
				additionalParameters.Sort = sortSpec;
			}
			if (this.RefinersList != null && this.RefinersList.Count > 0)
			{
				ExTraceGlobals.FullTextIndexTracer.TraceInformation<Guid>(54160, 0L, "Correlation Id:{0}. Found non empty refiner list adding it to the IMS input flow", this.QueryCorrelationId);
				additionalParameters.Refiners = this.RefinersList;
			}
			if (this.ExtraFieldsList != null && this.ExtraFieldsList.Count > 0)
			{
				ExTraceGlobals.FullTextIndexTracer.TraceInformation<Guid>(41872, 0L, "Correlation Id:{0}. Found non empty extra fields list adding it to the IMS input flow", this.QueryCorrelationId);
				additionalParameters.ExtraFields = this.ExtraFieldsList;
			}
			MultiMailboxSearchFullTextIndexQuery.TraceFunction("Exiting MultiMailboxSearchFullTextIndexQuery.GenerateParameters");
			return additionalParameters;
		}

		private const int FastSearchResultsDocumentIdFieldIndex = 1;

		private const int FastSearchResultsOtherFieldIndex = 2;

		private const string FastSearchResultsOtherFieldName = "other";

		private static int pageSize = -1;

		private static string sizeFastPropertyName;

		private static string mailboxGuidFastPropertyName;

		private static Hookable<PagingImsFlowExecutor> serviceProxyWrapperInstance;

		private int isAborted;
	}
}
