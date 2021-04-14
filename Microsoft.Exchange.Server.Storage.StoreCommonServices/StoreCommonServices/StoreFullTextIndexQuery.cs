using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FullTextIndex;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class StoreFullTextIndexQuery : ICustomParameter, IRefillableTableContents, IEnumerable<FullTextIndexRow>, IEnumerable, IHasCustomToStringImplementation
	{
		internal StoreFullTextIndexQuery(SearchCriteria criteria, CultureInfo culture)
		{
			this.criteria = criteria;
			this.culture = culture;
		}

		internal FqlQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = this.criteria.ToFql(FqlQueryGenerator.Options.Default, this.culture);
				}
				return this.query;
			}
		}

		bool IRefillableTableContents.CanRefill
		{
			get
			{
				return !this.done;
			}
		}

		void IRefillableTableContents.MarkChunkConsumed()
		{
			this.rows = null;
		}

		internal IList<FullTextIndexRow> Rows
		{
			get
			{
				return this.rows;
			}
		}

		internal bool NeedsRefill
		{
			get
			{
				return !this.Executed && !this.done;
			}
		}

		internal bool Executed
		{
			get
			{
				return this.rows != null;
			}
		}

		internal bool Done
		{
			get
			{
				return this.done;
			}
		}

		internal bool Failed
		{
			get
			{
				return this.failed;
			}
		}

		private static void UpdateQueryResultStatistics(StorePerDatabasePerformanceCountersInstance perDatabasePerformanceCounters, TimeSpan totalTime, long numberOfResults)
		{
			perDatabasePerformanceCounters.TotalSuccessfulSearches.Increment();
			perDatabasePerformanceCounters.SearchQueryResultRate.IncrementBy(numberOfResults);
			perDatabasePerformanceCounters.AverageSearchResultSize.IncrementBy(numberOfResults);
			perDatabasePerformanceCounters.AverageSearchResultSizeBase.Increment();
			StoreFullTextIndexQuery.GetCorrespondingLatencyCounter(perDatabasePerformanceCounters, totalTime).Increment();
		}

		private static ExPerformanceCounter GetCorrespondingLatencyCounter(StorePerDatabasePerformanceCountersInstance perDatabasePerformanceCounters, TimeSpan totalTime)
		{
			long num = (long)totalTime.TotalMilliseconds;
			if (num < 500L)
			{
				return perDatabasePerformanceCounters.TotalSearchesBelow500msec;
			}
			if (num < 2000L)
			{
				return perDatabasePerformanceCounters.TotalSearchesBetween500msecTo2sec;
			}
			if (num <= 10000L)
			{
				return perDatabasePerformanceCounters.TotalSearchesBetween2To10sec;
			}
			if (num <= 60000L)
			{
				return perDatabasePerformanceCounters.TotalSearchesBetween10SecTo60Sec;
			}
			return perDatabasePerformanceCounters.TotalSearchesQueriesGreaterThan60Seconds;
		}

		private static PagingImsFlowExecutor.QueryLoggingContext CreateLoggingContext(Guid databaseGuid, int mailboxNumber, FqlQuery query, Guid correlationId, int clientType, string clientAction, int pageSize, StorePerDatabasePerformanceCountersInstance perfCounters, SearchExecutionDiagnostics diagnostics)
		{
			bool isTracingEnabled = ExTraceGlobals.FullTextIndexTracer.IsTraceEnabled(TraceType.DebugTrace);
			Stopwatch stopWatch = new Stopwatch();
			return new PagingImsFlowExecutor.QueryLoggingContext(delegate()
			{
				perfCounters.TotalSearchesInProgress.Increment();
				perfCounters.TotalSearches.Increment();
				diagnostics.OnIssueFastRequest(pageSize, query);
				if (isTracingEnabled)
				{
					ExTraceGlobals.FullTextIndexTracer.TraceDebug(0L, "Mailbox {0}, Client Type={1}, Action={2}: Calling out to FAST (query: '{3}').", new object[]
					{
						mailboxNumber,
						clientType,
						clientAction,
						query
					});
				}
				stopWatch.Reset();
				stopWatch.Start();
			}, delegate(int rowCount)
			{
				stopWatch.Stop();
				perfCounters.TotalSearchesInProgress.Decrement();
				StoreFullTextIndexQuery.UpdateQueryResultStatistics(perfCounters, stopWatch.ToTimeSpan(), (long)rowCount);
				diagnostics.OnReceiveFastResponse(rowCount, stopWatch.ToTimeSpan());
				if (isTracingEnabled)
				{
					ExTraceGlobals.FullTextIndexTracer.TraceDebug(0L, "Mailbox {0}: Call to FAST returned {1} hits in {2}ms (query: '{3}').", new object[]
					{
						mailboxNumber,
						rowCount,
						stopWatch.ElapsedMilliseconds,
						query
					});
				}
			}, delegate(string errorMessage)
			{
				stopWatch.Stop();
				perfCounters.TotalSearchesInProgress.Decrement();
				diagnostics.OnReceiveFastError(errorMessage, stopWatch.ToTimeSpan());
				if (isTracingEnabled)
				{
					ExTraceGlobals.FullTextIndexTracer.TraceDebug(0L, "Mailbox {0}: Exception raised calling out to FAST (query: '{1}') in {2} ms: {3}", new object[]
					{
						mailboxNumber,
						query,
						stopWatch.ElapsedMilliseconds,
						errorMessage
					});
				}
			});
		}

		public IEnumerator<FullTextIndexRow> GetEnumerator()
		{
			if (this.rows == null)
			{
				return null;
			}
			return this.rows.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void ExecuteAll(Context context, MailboxState mailboxState, SearchExecutionDiagnostics diagnostics)
		{
			try
			{
				IFullTextIndexQuery fullTextIndexQuery = StoreFullTextIndexHelper.FtiQueryCreator.Value();
				PagingImsFlowExecutor.QueryLoggingContext loggingContext = StoreFullTextIndexQuery.CreateLoggingContext(mailboxState.DatabaseGuid, mailboxState.MailboxNumber, this.Query, diagnostics.CorrelationId, (int)context.ClientType, context.Diagnostics.ClientActionString, fullTextIndexQuery.GetPageSize(), PerformanceCounterFactory.GetDatabaseInstance(mailboxState.Database), diagnostics);
				this.rows = fullTextIndexQuery.ExecuteFullTextIndexQuery(mailboxState.DatabaseGuid, mailboxState.MailboxGuid, mailboxState.MailboxNumber, this.Query.Value, context.Culture, diagnostics.CorrelationId, loggingContext);
			}
			catch (TimeoutException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FullTextIndexException, new object[]
				{
					this.Query,
					ex.ToString()
				});
				this.rows = new List<FullTextIndexRow>(0);
				this.failed = true;
			}
			catch (CommunicationException ex2)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex2);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FullTextIndexException, new object[]
				{
					this.Query,
					ex2.ToString()
				});
				this.rows = new List<FullTextIndexRow>(0);
				this.failed = true;
			}
			this.done = true;
		}

		public void ExecuteOnePage(Context context, MailboxState mailboxState, bool needConversationDocumentId, SearchExecutionDiagnostics diagnostics)
		{
			if (this.pagedQueryResults == null)
			{
				this.pagedQueryResults = new PagedQueryResults();
			}
			try
			{
				IFullTextIndexQuery fullTextIndexQuery = StoreFullTextIndexHelper.FtiQueryCreator.Value();
				PagingImsFlowExecutor.QueryLoggingContext loggingContext = StoreFullTextIndexQuery.CreateLoggingContext(mailboxState.DatabaseGuid, mailboxState.MailboxNumber, this.Query, diagnostics.CorrelationId, (int)context.ClientType, context.Diagnostics.ClientActionString, fullTextIndexQuery.GetPageSize(), PerformanceCounterFactory.GetDatabaseInstance(mailboxState.Database), diagnostics);
				this.rows = fullTextIndexQuery.ExecutePagedFullTextIndexQuery(mailboxState.DatabaseGuid, mailboxState.MailboxGuid, mailboxState.MailboxNumber, this.Query.Value, context.Culture, diagnostics.CorrelationId, needConversationDocumentId, loggingContext, this.pagedQueryResults);
			}
			catch (TimeoutException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FullTextIndexException, new object[]
				{
					this.Query,
					ex.ToString()
				});
				this.rows = new List<FullTextIndexRow>(0);
				this.failed = true;
			}
			catch (CommunicationException ex2)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex2);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FullTextIndexException, new object[]
				{
					this.Query,
					ex2.ToString()
				});
				this.rows = new List<FullTextIndexRow>(0);
				this.failed = true;
			}
			this.done = (this.rows.Count == 0);
			if (this.done)
			{
				this.pagedQueryResults.Dispose();
				this.pagedQueryResults = null;
			}
		}

		public void Cleanup()
		{
			if (this.pagedQueryResults != null)
			{
				this.pagedQueryResults.Dispose();
				this.pagedQueryResults = null;
			}
			this.done = false;
			this.failed = false;
			this.rows = null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(120);
			this.AppendAsString(stringBuilder);
			return stringBuilder.ToString();
		}

		public void AppendAsString(StringBuilder sb)
		{
			sb.Append("FQL:[");
			sb.Append(this.Query);
			sb.Append("], ");
			sb.Append("SearchCriteria:[");
			this.criteria.AppendToString(sb, StringFormatOptions.IncludeDetails);
			sb.Append("]");
		}

		private readonly CultureInfo culture;

		private readonly SearchCriteria criteria;

		private FqlQuery query;

		private List<FullTextIndexRow> rows;

		private PagedQueryResults pagedQueryResults;

		private bool done;

		private bool failed;
	}
}
