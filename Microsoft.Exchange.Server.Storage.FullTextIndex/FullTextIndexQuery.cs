using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	internal class FullTextIndexQuery : IFullTextIndexQuery
	{
		internal FullTextIndexQuery() : this(FullTextIndexQuery.SearchConfig.FastQueryResultTrimHits)
		{
		}

		internal FullTextIndexQuery(int pageSize)
		{
			this.pageSize = pageSize;
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
				if (FullTextIndexQuery.serviceProxyWrapperInstance == null)
				{
					string hostName = FullTextIndexQuery.SearchConfig.HostName;
					int queryServicePort = FullTextIndexQuery.SearchConfig.QueryServicePort;
					int fastImsChannelPoolSize = FullTextIndexQuery.SearchConfig.FastImsChannelPoolSize;
					TimeSpan fastImsOpenTimeout = FullTextIndexQuery.SearchConfig.FastImsOpenTimeout;
					TimeSpan fastImsSendTimeout = FullTextIndexQuery.SearchConfig.FastImsSendTimeout;
					TimeSpan fastImsReceiveTimeout = FullTextIndexQuery.SearchConfig.FastImsReceiveTimeout;
					int fastSearchRetryCount = FullTextIndexQuery.SearchConfig.FastSearchRetryCount;
					long num = (long)FullTextIndexQuery.SearchConfig.FastIMSMaxReceivedMessageSize;
					int fastIMSMaxStringContentLength = FullTextIndexQuery.SearchConfig.FastIMSMaxStringContentLength;
					TimeSpan fastProxyCacheTimeout = FullTextIndexQuery.SearchConfig.FastProxyCacheTimeout;
					Hookable<PagingImsFlowExecutor> value = Hookable<PagingImsFlowExecutor>.Create(true, new PagingImsFlowExecutor(hostName, queryServicePort, fastImsChannelPoolSize, fastImsOpenTimeout, fastImsSendTimeout, fastImsReceiveTimeout, fastProxyCacheTimeout, num, fastIMSMaxStringContentLength, fastSearchRetryCount));
					Interlocked.CompareExchange<Hookable<PagingImsFlowExecutor>>(ref FullTextIndexQuery.serviceProxyWrapperInstance, value, null);
				}
				return FullTextIndexQuery.serviceProxyWrapperInstance;
			}
		}

		public List<FullTextIndexRow> ExecuteFullTextIndexQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, CultureInfo culture, Guid correlationId, PagingImsFlowExecutor.QueryLoggingContext loggingContext)
		{
			List<FullTextIndexRow> list = new List<FullTextIndexRow>(0);
			IEnumerable<SearchResultItem[]> enumerable = FullTextIndexQuery.ServiceProxyWrapper.Value.ExecuteSimple(FullTextIndexSchema.GetImsFlowName(databaseGuid), mailboxGuid, correlationId, query, 0L, culture, FullTextIndexQuery.AdditionalQueryParameters, this.pageSize, loggingContext);
			foreach (SearchResultItem[] page in enumerable)
			{
				FullTextIndexQuery.CacheOnePageOfResults(page, list, mailboxNumber);
			}
			return list;
		}

		public IEnumerable<FullTextDiagnosticRow> ExecuteDiagnosticQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, CultureInfo culture, Guid correlationId, string sortOrder, ICollection<string> additionalColumns, PagingImsFlowExecutor.QueryLoggingContext loggingContext)
		{
			AdditionalParameters additionalParameters = new AdditionalParameters
			{
				Sort = sortOrder,
				ExtraFields = new List<string>(additionalColumns)
			};
			IEnumerable<SearchResultItem[]> pagedResults = FullTextIndexQuery.ServiceProxyWrapper.Value.ExecuteSimple(FullTextIndexSchema.GetImsFlowName(databaseGuid), mailboxGuid, correlationId, query, 0L, culture, additionalParameters, this.pageSize, loggingContext);
			return FullTextDiagnosticRow.Parse(pagedResults);
		}

		public List<FullTextIndexRow> ExecutePagedFullTextIndexQuery(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, string query, CultureInfo culture, Guid correlationId, bool needConversationId, PagingImsFlowExecutor.QueryLoggingContext loggingContext, PagedQueryResults pagedQueryResults)
		{
			List<FullTextIndexRow> list = new List<FullTextIndexRow>(0);
			if (!pagedQueryResults.IsInitialized)
			{
				pagedQueryResults.Initialize(FullTextIndexQuery.ServiceProxyWrapper.Value.ExecuteSimple(FullTextIndexSchema.GetImsFlowName(databaseGuid), mailboxGuid, correlationId, query, 0L, culture, needConversationId ? FullTextIndexQuery.AdditionalQueryParametersConversations : FullTextIndexQuery.AdditionalQueryParameters, this.pageSize, loggingContext).GetEnumerator());
			}
			if (pagedQueryResults.PagedResults.MoveNext())
			{
				SearchResultItem[] page = (SearchResultItem[])pagedQueryResults.PagedResults.Current;
				FullTextIndexQuery.CacheOnePageOfResults(page, list, mailboxNumber);
			}
			return list;
		}

		public int GetPageSize()
		{
			return this.pageSize;
		}

		internal static IDisposable SetPagingImsFlowExecutorTestHook(PagingImsFlowExecutor testHook)
		{
			return FullTextIndexQuery.ServiceProxyWrapper.SetTestHook(testHook);
		}

		private static void CacheOnePageOfResults(SearchResultItem[] page, List<FullTextIndexRow> rows, int mailboxNumber)
		{
			rows.Capacity = rows.Count + page.Length;
			foreach (SearchResultItem searchResultItem in page)
			{
				long num = (long)searchResultItem.Fields[1].Value;
				if (IndexId.GetMailboxNumber(num) == mailboxNumber)
				{
					SearchResultItem searchResultItem2 = searchResultItem.Fields[2].Value as SearchResultItem;
					if (searchResultItem2 != null && searchResultItem2.Fields.Count > 0 && searchResultItem2.Fields[0].Name == FastIndexSystemSchema.ConversationId.Name && searchResultItem2.Fields[0].Value != null)
					{
						int conversationId = (int)((long)searchResultItem2.Fields[0].Value);
						rows.Add(FullTextIndexRow.Parse(num, conversationId));
					}
					else
					{
						rows.Add(FullTextIndexRow.Parse(num));
					}
				}
			}
		}

		private const int FastSearchResultsDocumentIdFieldIndex = 1;

		private const int FastSearchResultsOtherFieldIndex = 2;

		private static readonly AdditionalParameters AdditionalQueryParameters = new AdditionalParameters
		{
			Sort = "-received"
		};

		private static readonly AdditionalParameters AdditionalQueryParametersConversations = new AdditionalParameters
		{
			Sort = "-received",
			ExtraFields = new List<string>
			{
				"conversationid"
			}
		};

		private static Hookable<PagingImsFlowExecutor> serviceProxyWrapperInstance;

		private readonly int pageSize;
	}
}
