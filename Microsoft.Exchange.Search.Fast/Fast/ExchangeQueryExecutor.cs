using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.EventLog;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal sealed class ExchangeQueryExecutor : IDisposeTrackable, IDisposable
	{
		internal ExchangeQueryExecutor(ISearchServiceConfig config, string imsFlowName) : this(config.HostName, config.QueryServicePort, imsFlowName, false, 1, config.QueryOperationTimeout, config.QueryProxyCacheTimeout)
		{
		}

		internal ExchangeQueryExecutor(string hostName, int queryServicePort, bool readHitCount) : this(hostName, queryServicePort, null, readHitCount)
		{
		}

		internal ExchangeQueryExecutor(string hostName, int queryServicePort, string imsFlowName, bool readHitCount) : this(hostName, queryServicePort, imsFlowName, readHitCount, 1, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(5.0))
		{
		}

		internal ExchangeQueryExecutor(string hostName, int queryServicePort, string imsFlowName, bool readHitCount, int retryCount, TimeSpan operationTimeout, TimeSpan proxyCacheTimeout)
		{
			this.imsFlowName = imsFlowName;
			this.readHitCount = readHitCount;
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("ExchangeQueryExecutor", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.QueryExecutorTracer, (long)this.GetHashCode());
			this.flowExecutor = Factory.Current.CreatePagingImsFlowExecutor(hostName, queryServicePort, 2, operationTimeout, operationTimeout, operationTimeout, proxyCacheTimeout, 2147483647L, 65536, retryCount);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public long TotalHits { get; private set; }

		public static T RunUnderExceptionHandler<T>(Func<T> call, IDiagnosticsSession session, string flowName)
		{
			T result;
			try
			{
				result = call();
			}
			catch (CommunicationException ex)
			{
				session.TraceError<CommunicationException>("ExchangeQueryExecutor.CallImsFlow - operation returned exception: {0}.", ex);
				session.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_FastConnectionException, flowName, new object[]
				{
					ex
				});
				throw new OperationFailedException(ex);
			}
			catch (TimeoutException ex2)
			{
				session.TraceError<TimeoutException>("ExchangeQueryExecutor.CallImsFlow - operation returned exception: {0}.", ex2);
				session.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_FastConnectionException, flowName, new object[]
				{
					ex2
				});
				throw new OperationFailedException(ex2);
			}
			return result;
		}

		public IEnumerable<long> ExecuteQuery(Guid mailboxGuid, string query)
		{
			foreach (SearchResultItem item in this.ExecuteQueryWithFields(mailboxGuid, query))
			{
				yield return (long)item.Fields[1].Value;
			}
			yield break;
		}

		public IEnumerable<SearchResultItem> ExecuteQueryWithFields(Guid mailboxGuid, string query)
		{
			return this.ExecuteQueryWithFields(mailboxGuid, query, 0L, 1000, false, null);
		}

		public IEnumerable<SearchResultItem> ExecuteQueryWithFields(Guid mailboxGuid, string query, List<string> extraFields)
		{
			return this.ExecuteQueryWithFields(mailboxGuid, query, 0L, 1000, false, extraFields);
		}

		public IEnumerable<SearchResultItem> ExecuteQueryWithFields(Guid mailboxGuid, string query, long offset, int trimHits, bool firstPageOnly)
		{
			return this.ExecuteQueryWithFields(mailboxGuid, query, offset, trimHits, firstPageOnly, null);
		}

		public IEnumerable<SearchResultItem> ExecuteQueryWithFields(Guid mailboxGuid, string query, long offset, int trimHits, bool firstPageOnly, List<string> extraFields)
		{
			ExchangeQueryExecutor.<>c__DisplayClassb CS$<>8__locals1 = new ExchangeQueryExecutor.<>c__DisplayClassb();
			CS$<>8__locals1.mailboxGuid = mailboxGuid;
			CS$<>8__locals1.query = query;
			CS$<>8__locals1.offset = offset;
			CS$<>8__locals1.trimHits = trimHits;
			CS$<>8__locals1.<>4__this = this;
			this.TotalHits = -1L;
			bool isFirstPage = true;
			CS$<>8__locals1.parameters = new AdditionalParameters
			{
				ExtraFields = extraFields
			};
			using (IEnumerator<KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]>> pageEnumerator = this.RunUnderExceptionHandler<IEnumerable<KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]>>>(() => CS$<>8__locals1.<>4__this.flowExecutor.Execute(CS$<>8__locals1.<>4__this.imsFlowName, CS$<>8__locals1.mailboxGuid, Guid.NewGuid(), CS$<>8__locals1.query, CS$<>8__locals1.offset, CultureInfo.InvariantCulture, CS$<>8__locals1.parameters, CS$<>8__locals1.trimHits, null)).GetEnumerator())
			{
				do
				{
					if (!this.RunUnderExceptionHandler<bool>(() => pageEnumerator.MoveNext()))
					{
						break;
					}
					if (isFirstPage)
					{
						if (this.readHitCount)
						{
							this.TotalHits = this.RunUnderExceptionHandler<long>(delegate()
							{
								PagingImsFlowExecutor pagingImsFlowExecutor = CS$<>8__locals1.<>4__this.flowExecutor;
								KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]> keyValuePair2 = pageEnumerator.Current;
								return pagingImsFlowExecutor.ReadHitCount(keyValuePair2.Key);
							});
						}
						isFirstPage = false;
					}
					KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]> keyValuePair = pageEnumerator.Current;
					foreach (SearchResultItem item in keyValuePair.Value)
					{
						yield return item;
					}
				}
				while (!firstPageOnly);
			}
			yield break;
		}

		public PagedReader ExecutePagedQueryByDocumentId(string flowName, string query, Guid tenantId, IndexSystemField[] extraFieldsToReturn)
		{
			return this.ExecutePagedQueryByDocumentId(flowName, query, tenantId, extraFieldsToReturn, 0L, int.MaxValue);
		}

		public PagedReader ExecutePagedQueryByDocumentId(string flowName, string query, Guid tenantId, IndexSystemField[] extraFieldsToReturn, long startingDocId, int maxResults)
		{
			AdditionalParameters parameters = new AdditionalParameters
			{
				ExtraFields = this.GetIndexSystemFieldNamesList(extraFieldsToReturn)
			};
			PagedReader result = this.RunUnderExceptionHandler<PagedReader>(() => this.flowExecutor.ExecutePagedQueryByDocumentId(flowName, query, tenantId, parameters, startingDocId, maxResults));
			this.diagnosticsSession.TraceDebug<int>("Rows returned: {0}", this.RunUnderExceptionHandler<int>(() => result.Count));
			return result;
		}

		public long GetHitCount(string flowName, string query, Guid tenantId)
		{
			return this.RunUnderExceptionHandler<long>(delegate()
			{
				QueryParameters queryParameters = new QueryParameters(this.flowExecutor.GetLookupTimeout(), flowName, query, tenantId, Guid.NewGuid(), null);
				return this.flowExecutor.GetHitCount(queryParameters);
			});
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeQueryExecutor>(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.flowExecutor != null)
				{
					this.flowExecutor.Dispose();
					this.flowExecutor = null;
				}
			}
		}

		private List<string> GetIndexSystemFieldNamesList(IndexSystemField[] fields)
		{
			if (fields == null || fields.Length == 0)
			{
				return null;
			}
			List<string> list = new List<string>(fields.Length);
			foreach (IndexSystemField indexSystemField in fields)
			{
				if (!indexSystemField.Retrievable)
				{
					throw new ArgumentException(indexSystemField.Name + " is not Retrievable");
				}
				list.Add(indexSystemField.Name);
			}
			return list;
		}

		private T RunUnderExceptionHandler<T>(Func<T> call)
		{
			return ExchangeQueryExecutor.RunUnderExceptionHandler<T>(call, this.diagnosticsSession, this.imsFlowName);
		}

		private const int HitsPerQuery = 1000;

		private readonly string imsFlowName;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly bool readHitCount;

		private PagingImsFlowExecutor flowExecutor;

		private DisposeTracker disposeTracker;
	}
}
