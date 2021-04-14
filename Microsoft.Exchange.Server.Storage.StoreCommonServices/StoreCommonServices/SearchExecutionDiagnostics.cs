using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class SearchExecutionDiagnostics : ExecutionDiagnostics
	{
		private SearchExecutionDiagnostics(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, ExchangeId searchFolderId)
		{
			this.statistics = SearchExecutionDiagnostics.SearchOperationStatistics.Create();
			base.DatabaseGuid = databaseGuid;
			base.MailboxNumber = mailboxNumber;
			base.MailboxGuid = mailboxGuid;
			this.correlationId = CorrelationIdHelper.GetCorrelationId(base.MailboxNumber, searchFolderId.ToLong());
		}

		public Guid CorrelationId
		{
			get
			{
				return this.correlationId;
			}
		}

		protected override bool HasDataToLog
		{
			get
			{
				return this.IsSlowOperation || base.HasDataToLog;
			}
		}

		private bool IsSlowOperation
		{
			get
			{
				return ConfigurationSchema.SearchTraceFastOperationThreshold.Value <= this.statistics.SlowestResponse || ConfigurationSchema.SearchTraceFastTotalThreshold.Value <= this.statistics.TotalResponse || ConfigurationSchema.SearchTraceStoreOperationThreshold.Value <= this.statistics.SlowestLinked || ConfigurationSchema.SearchTraceStoreTotalThreshold.Value <= this.statistics.TotalLink || ConfigurationSchema.SearchTraceFirstLinkedThreshold.Value <= this.statistics.FirstLinked || ConfigurationSchema.SearchTraceGetRestrictionThreshold.Value <= this.searchRestrictionComputation || ConfigurationSchema.SearchTraceGetQueryPlanThreshold.Value <= this.searchQueryPlanComputation;
			}
		}

		public static SearchExecutionDiagnostics Create(Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, ExchangeId searchFolderId)
		{
			return new SearchExecutionDiagnostics(databaseGuid, mailboxGuid, mailboxNumber, searchFolderId);
		}

		public override void FormatCommonInformation(TraceContentBuilder cb, int indentLevel, Guid correlationId)
		{
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Database guid: " + base.DatabaseGuid);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Mailbox number: " + base.MailboxNumber);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client type: " + base.ClientType);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Correlation ID: " + correlationId);
		}

		internal void OnGetRestriction(object restriction)
		{
			this.clientRestriction = restriction;
		}

		internal void OnInitiateQuery(ExchangeId searchFolderId, TimeSpan searchRestrictionCalculationTime, TimeSpan searchPlanCalculationTime, bool pulseSearchPopulation, bool nestedSearchFolder, SearchState initialSearchState, SetSearchCriteriaFlags searchCriteriaFlags, int maxResultCount, ExchangeId firstScopeFolder, string friendlyFolderName, int scopeFoldersCount, int expandedScopeFoldersCount, SimpleQueryOperator.SimpleQueryOperatorDefinition planDefinition)
		{
			this.queryStartTime = DateTime.UtcNow - this.searchQueryPlanComputation;
			this.correlationId = CorrelationIdHelper.GetCorrelationId(base.MailboxNumber, searchFolderId.ToLong());
			this.searchRestrictionComputation = searchRestrictionCalculationTime;
			this.searchQueryPlanComputation = searchPlanCalculationTime;
			this.isPulsedSearchPopulation = pulseSearchPopulation;
			this.isNestedSearchFolder = nestedSearchFolder;
			this.initialSearchState = initialSearchState;
			this.searchCriteriaFlags = searchCriteriaFlags;
			this.maxCountForPulsedPopulation = maxResultCount;
			this.firstScopeFolderId = firstScopeFolder.ToString();
			this.firstScopeFolderFriendlyName = friendlyFolderName;
			this.scopeFoldersCount = scopeFoldersCount;
			this.expandedScopeFoldersCount = expandedScopeFoldersCount;
			this.planDefinition = planDefinition;
			this.storeResidual = SearchExecutionDiagnostics.FormatResidual(this.planDefinition);
			this.statistics.Clear();
		}

		internal void OnIssueFastRequest(int pageSize, FqlQuery fastQuery)
		{
			this.fastPageSize = pageSize;
			this.fastQuery = fastQuery.Value;
			this.scrubbedQuery = fastQuery.ScrubbedValue;
			this.termLength = fastQuery.TermLength;
			this.numberFastTrips++;
		}

		internal void OnReceiveFastResponse(int rowsReceivedFromFast, TimeSpan elapsedTime)
		{
			SearchExecutionDiagnostics.SearchOperationFastResponse operation = SearchExecutionDiagnostics.SearchOperationFastResponse.Create(rowsReceivedFromFast, elapsedTime);
			this.statistics.Add(operation);
		}

		internal void OnReceiveFastError(string errorMessage)
		{
			this.errorMessage = errorMessage;
			this.OnReceiveFastError(errorMessage, TimeSpan.Zero);
		}

		internal void OnReceiveFastError(string errorMessage, TimeSpan elapsedTime)
		{
			SearchExecutionDiagnostics.SearchOperationFastError operation = SearchExecutionDiagnostics.SearchOperationFastError.Create(errorMessage, elapsedTime);
			this.statistics.Add(operation);
			this.errorMessage = errorMessage;
		}

		internal void OnSearchOperation(string prefix, TimeSpan elapsedTime)
		{
			SearchExecutionDiagnostics.SearchOperation operation = SearchExecutionDiagnostics.SearchOperation.Create(prefix, elapsedTime);
			this.statistics.Add(operation);
		}

		internal void OnLinkedResults(int messagesLinked, int fullTextRowsProcessed, int totalMessagesLinked, TimeSpan elapsedTime)
		{
			SearchExecutionDiagnostics.SearchOperationLinkedRows operation = SearchExecutionDiagnostics.SearchOperationLinkedRows.Create(messagesLinked, fullTextRowsProcessed, totalMessagesLinked, elapsedTime);
			this.timeToLinkFirstResults = DateTime.UtcNow - this.queryStartTime;
			this.statistics.Add(operation);
		}

		internal void OnCompleteQuery(int totalMessagesLinked, bool linkCountReached, uint totalFullTextIndexRowsProcessed, SearchState finalSearchState, string clientActionString)
		{
			this.totalMessagesLinked = totalMessagesLinked;
			this.isMaximumLinkCountReached = linkCountReached;
			this.totalFullTextRowsProcessed = totalFullTextIndexRowsProcessed;
			this.finalSearchState = finalSearchState;
			this.clientActionString = clientActionString;
			base.DumpDiagnosticIfNeeded(ExecutionDiagnostics.GetLogger(LoggerType.FullTextIndex), LoggerManager.TraceGuids.FullTextIndexDetail, this.correlationId);
			DateTime utcNow = DateTime.UtcNow;
			FullTextIndexLogger.LogSingleLineQuery(this.correlationId, this.queryStartTime, utcNow, base.DatabaseGuid, base.MailboxGuid, base.MailboxNumber, (int)base.ClientType, this.scrubbedQuery, this.errorMessage != null, this.errorMessage, this.isMaximumLinkCountReached, this.storeResidual, this.numberFastTrips, this.isPulsedSearchPopulation, this.firstScopeFolderId, this.maxCountForPulsedPopulation, this.scopeFoldersCount, (int)this.initialSearchState, (int)this.finalSearchState, (int)this.searchCriteriaFlags, this.isNestedSearchFolder, this.statistics.FirstFastResults, this.statistics.FastResultsToLinkFirstSet, this.statistics.TotalFastResults, totalMessagesLinked, this.searchRestrictionComputation, this.searchQueryPlanComputation, this.statistics.FirstFastResultsTime, this.statistics.FastTimeToLinkResults, this.statistics.FastTrips, this.statistics.FirstRowsLinked ? ((int)this.timeToLinkFirstResults.TotalMilliseconds) : ((int)(utcNow - this.queryStartTime).TotalMilliseconds), (int)this.statistics.TotalResponse.TotalMilliseconds, this.expandedScopeFoldersCount, this.firstScopeFolderFriendlyName, this.totalFullTextRowsProcessed, this.clientActionString, this.fastQuery, this.termLength);
			this.statistics.Clear();
		}

		internal void OnBeforeSearchPopulationTaskStep()
		{
			FullTextIndexLogger.LogOtherSuboperation(base.DatabaseGuid, base.MailboxNumber, (int)base.ClientType, this.correlationId, 1);
		}

		internal void OnInsideSearchPopulationTaskStep()
		{
			FullTextIndexLogger.LogOtherSuboperation(base.DatabaseGuid, base.MailboxNumber, (int)base.ClientType, this.correlationId, 2);
		}

		internal TimeSpan GetFastTotalResponseTime()
		{
			return this.statistics.TotalResponse;
		}

		protected override void FormatThresholdInformation(TraceContentBuilder cb, int indentLevel)
		{
			long value = (long)this.statistics.SlowestResponse.TotalMilliseconds;
			long value2 = (long)this.statistics.TotalResponse.TotalMilliseconds;
			long value3 = (long)this.statistics.SlowestLinked.TotalMilliseconds;
			long value4 = (long)this.statistics.TotalLink.TotalMilliseconds;
			long value5 = (long)this.statistics.FirstLinked.TotalMilliseconds;
			long value6 = (long)this.searchRestrictionComputation.TotalMilliseconds;
			long value7 = (long)this.searchQueryPlanComputation.TotalMilliseconds;
			long threshold = (long)ConfigurationSchema.SearchTraceFastOperationThreshold.Value.TotalMilliseconds;
			long threshold2 = (long)ConfigurationSchema.SearchTraceFastTotalThreshold.Value.TotalMilliseconds;
			long threshold3 = (long)ConfigurationSchema.SearchTraceStoreOperationThreshold.Value.TotalMilliseconds;
			long threshold4 = (long)ConfigurationSchema.SearchTraceStoreTotalThreshold.Value.TotalMilliseconds;
			long threshold5 = (long)ConfigurationSchema.SearchTraceFirstLinkedThreshold.Value.TotalMilliseconds;
			long threshold6 = (long)ConfigurationSchema.SearchTraceGetRestrictionThreshold.Value.TotalMilliseconds;
			long threshold7 = (long)ConfigurationSchema.SearchTraceGetQueryPlanThreshold.Value.TotalMilliseconds;
			ExecutionDiagnostics.FormatLine(cb, 0, "Trace Thresholds:");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Slowest FTI Response", value, threshold, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Total FTI Response", value2, threshold2, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Slowest Store Linking", value3, threshold3, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Total Store Linking", value4, threshold4, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "First Messages Linked", value5, threshold5, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Restriction Computation", value6, threshold6, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Query Plan Computation", value7, threshold7, "ms");
		}

		protected override void FormatOperationInformation(TraceContentBuilder cb, int indentLevel)
		{
			ExecutionDiagnostics.FormatLine(cb, 0, "Search characteristics:");
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Pulsed: " + this.isPulsedSearchPopulation);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Nested: " + this.isNestedSearchFolder);
			if (this.planDefinition != null && this.planDefinition.MaxRows > 0)
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "IsTopN: True (" + this.planDefinition.MaxRows.ToString() + ")");
			}
			else
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "IsTopN: False");
			}
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Max count: " + this.maxCountForPulsedPopulation);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Page size: " + this.fastPageSize);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client action: " + this.clientActionString);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Search criteria flags: " + this.searchCriteriaFlags.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Initial search state: " + this.initialSearchState.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Final search state: " + this.finalSearchState.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "First scope folder ID: " + this.firstScopeFolderId);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "First scope folder Name: " + this.firstScopeFolderFriendlyName);
			long value = (long)this.searchRestrictionComputation.TotalMilliseconds;
			long value2 = (long)this.searchQueryPlanComputation.TotalMilliseconds;
			long value3 = (long)this.statistics.FirstLinked.TotalMilliseconds;
			ExecutionDiagnostics.FormatLine(cb, 0, "Execution information:");
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Scope folders top count: " + this.scopeFoldersCount.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Scope folders expanded count: " + this.expandedScopeFoldersCount.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Restriction computation time: " + SearchExecutionDiagnostics.Time.ToString(value) + " ms");
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Query plan computation time: " + SearchExecutionDiagnostics.Time.ToString(value2) + " ms");
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Time to first messages linked: " + SearchExecutionDiagnostics.Time.ToString(value3) + " ms");
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Total full text rows processed: " + this.totalFullTextRowsProcessed.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Total messages linked: " + this.totalMessagesLinked.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Maximum link count reached: " + this.isMaximumLinkCountReached.ToString());
		}

		protected override void FormatDiagnosticInformation(TraceContentBuilder cb, int indentLevel)
		{
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Search Operations:");
			foreach (SearchExecutionDiagnostics.SearchOperationBase searchOperationBase in this.statistics)
			{
				cb.Indent(indentLevel + 1);
				searchOperationBase.AppendToTraceContentBuilder(cb);
			}
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "FAST Query:");
			ExecutionDiagnostics.FormatLine(cb, indentLevel + 1, this.scrubbedQuery);
			if (this.clientRestriction != null)
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client restriction:");
				ExecutionDiagnostics.FormatLine(cb, indentLevel + 1, this.clientRestriction.ToString());
			}
			if (this.planDefinition != null)
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Query plan:");
				ExecutionDiagnostics.FormatLine(cb, indentLevel + 1, this.planDefinition.ToString(StringFormatOptions.SkipParametersData));
			}
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Store residual:");
			ExecutionDiagnostics.FormatLine(cb, indentLevel + 1, this.storeResidual);
		}

		private static string FormatResidual(SimpleQueryOperator.SimpleQueryOperatorDefinition planDefinition)
		{
			if (planDefinition != null)
			{
				SearchCriteria criteria = planDefinition.Criteria;
				if (criteria != null)
				{
					StringBuilder stringBuilder = new StringBuilder(256);
					criteria.AppendToString(stringBuilder, StringFormatOptions.SkipParametersData);
					return stringBuilder.ToString();
				}
			}
			return "NONE";
		}

		private readonly SearchExecutionDiagnostics.SearchOperationStatistics statistics;

		private Guid correlationId;

		private bool isPulsedSearchPopulation;

		private bool isNestedSearchFolder;

		private DateTime queryStartTime;

		private TimeSpan searchRestrictionComputation;

		private TimeSpan searchQueryPlanComputation;

		private TimeSpan timeToLinkFirstResults;

		private SearchState initialSearchState;

		private SetSearchCriteriaFlags searchCriteriaFlags;

		private int maxCountForPulsedPopulation;

		private string firstScopeFolderId;

		private string firstScopeFolderFriendlyName;

		private string errorMessage;

		private int scopeFoldersCount;

		private int expandedScopeFoldersCount;

		private int numberFastTrips;

		private object clientRestriction;

		private SimpleQueryOperator.SimpleQueryOperatorDefinition planDefinition;

		private int fastPageSize;

		private string fastQuery;

		private string scrubbedQuery;

		private string termLength;

		private int totalMessagesLinked;

		private bool isMaximumLinkCountReached;

		private uint totalFullTextRowsProcessed;

		private SearchState finalSearchState;

		private string clientActionString;

		private string storeResidual;

		private enum SearchOtherSuboperationType
		{
			BeforeSearchPopulationTaskStep = 1,
			InsideSearchPopulationTaskStep
		}

		public class SearchOperationStatistics : IEnumerable<SearchExecutionDiagnostics.SearchOperationBase>, IEnumerable
		{
			private SearchOperationStatistics()
			{
				this.operations = new List<SearchExecutionDiagnostics.SearchOperationBase>(10);
				this.fastTrips = new StringBuilder(50);
				this.isFirstFastResult = true;
			}

			public TimeSpan SlowestResponse
			{
				get
				{
					return this.slowestResponseTime;
				}
			}

			public TimeSpan SlowestLinked
			{
				get
				{
					return this.slowestLinkedTime;
				}
			}

			public TimeSpan TotalResponse
			{
				get
				{
					return this.totalResponseTime;
				}
			}

			public TimeSpan TotalLink
			{
				get
				{
					return this.totalLinkTime;
				}
			}

			public TimeSpan FirstLinked
			{
				get
				{
					return this.firstLinkedTime;
				}
			}

			public bool FirstRowsLinked
			{
				get
				{
					return this.firstRowsLinked;
				}
			}

			public int FirstFastResultsTime
			{
				get
				{
					return this.firstFastResultsTime;
				}
			}

			public int FirstFastResults
			{
				get
				{
					return this.firstFastResults;
				}
			}

			public int FastResultsToLinkFirstSet
			{
				get
				{
					return this.fastResultsToLinkFirstSet;
				}
			}

			public int FastTimeToLinkResults
			{
				get
				{
					return this.fastTimeToLinkResults;
				}
			}

			public string FastTrips
			{
				get
				{
					return this.fastTrips.ToString();
				}
			}

			public int TotalFastResults
			{
				get
				{
					return this.totalFastResults;
				}
			}

			public static SearchExecutionDiagnostics.SearchOperationStatistics Create()
			{
				return new SearchExecutionDiagnostics.SearchOperationStatistics();
			}

			public void Add(SearchExecutionDiagnostics.SearchOperationBase operation)
			{
				SearchExecutionDiagnostics.SearchOperationFastResponse searchOperationFastResponse = operation as SearchExecutionDiagnostics.SearchOperationFastResponse;
				SearchExecutionDiagnostics.SearchOperationLinkedRows searchOperationLinkedRows = operation as SearchExecutionDiagnostics.SearchOperationLinkedRows;
				this.operations.Add(operation);
				if (this.totalRowsLinked <= 0)
				{
					this.firstLinkedTime += operation.ElapsedTime;
				}
				if (searchOperationLinkedRows != null)
				{
					if (!this.firstRowsLinked)
					{
						this.fastTimeToLinkResults = (int)this.totalResponseTime.TotalMilliseconds;
						this.fastResultsToLinkFirstSet = this.totalFastResults;
					}
					this.totalLinkTime += operation.ElapsedTime;
					this.slowestLinkedTime = TimeSpan.FromTicks(Math.Max(this.slowestLinkedTime.Ticks, operation.ElapsedTime.Ticks));
					this.firstRowsLinked = (this.totalRowsLinked <= 0 && searchOperationLinkedRows.MessagesLinked > 0);
					this.totalRowsLinked += searchOperationLinkedRows.MessagesLinked;
				}
				if (searchOperationFastResponse != null)
				{
					this.totalResponseTime += operation.ElapsedTime;
					this.totalFastResults += searchOperationFastResponse.RowsReceived;
					this.slowestResponseTime = TimeSpan.FromTicks(Math.Max(this.slowestResponseTime.Ticks, operation.ElapsedTime.Ticks));
					this.fastTrips.AppendFormat("{0};", searchOperationFastResponse.ElapsedTime);
					if (this.isFirstFastResult)
					{
						this.isFirstFastResult = false;
						this.firstFastResultsTime = (int)searchOperationFastResponse.ElapsedTime.TotalMilliseconds;
						this.firstFastResults = searchOperationFastResponse.RowsReceived;
					}
				}
			}

			public IEnumerator<SearchExecutionDiagnostics.SearchOperationBase> GetEnumerator()
			{
				return this.operations.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.operations.GetEnumerator();
			}

			public void Clear()
			{
				this.operations.Clear();
				this.firstLinkedTime = TimeSpan.Zero;
				this.slowestResponseTime = TimeSpan.Zero;
				this.slowestLinkedTime = TimeSpan.Zero;
				this.totalResponseTime = TimeSpan.Zero;
				this.totalLinkTime = TimeSpan.Zero;
				this.firstRowsLinked = false;
				this.totalRowsLinked = 0;
			}

			private readonly IList<SearchExecutionDiagnostics.SearchOperationBase> operations;

			private TimeSpan firstLinkedTime;

			private TimeSpan slowestResponseTime;

			private TimeSpan slowestLinkedTime;

			private TimeSpan totalResponseTime;

			private TimeSpan totalLinkTime;

			private bool firstRowsLinked;

			private bool isFirstFastResult;

			private int fastTimeToLinkResults;

			private int totalRowsLinked;

			private int firstFastResultsTime;

			private int firstFastResults;

			private int fastResultsToLinkFirstSet;

			private int totalFastResults;

			private StringBuilder fastTrips;
		}

		public abstract class SearchOperationBase
		{
			protected SearchOperationBase(string prefix, TimeSpan elapsedTime)
			{
				this.prefix = prefix;
				this.elapsedTime = elapsedTime;
			}

			public TimeSpan ElapsedTime
			{
				get
				{
					return this.elapsedTime;
				}
			}

			public virtual void AppendToTraceContentBuilder(TraceContentBuilder cb)
			{
				long value = (long)this.elapsedTime.TotalMicroseconds();
				cb.Append(this.prefix);
				cb.Append(": ");
				cb.Append(SearchExecutionDiagnostics.Time.ToString(value));
				cb.Append(" us");
			}

			private readonly string prefix;

			private readonly TimeSpan elapsedTime;
		}

		public sealed class SearchOperation : SearchExecutionDiagnostics.SearchOperationBase
		{
			private SearchOperation(string prefix, TimeSpan elapsedTime) : base(prefix, elapsedTime)
			{
			}

			public static SearchExecutionDiagnostics.SearchOperation Create(string prefix, TimeSpan elapsedTime)
			{
				return new SearchExecutionDiagnostics.SearchOperation(prefix, elapsedTime);
			}

			public override void AppendToTraceContentBuilder(TraceContentBuilder cb)
			{
				base.AppendToTraceContentBuilder(cb);
				cb.AppendLine();
			}
		}

		public sealed class SearchOperationFastResponse : SearchExecutionDiagnostics.SearchOperationBase
		{
			public int RowsReceived
			{
				get
				{
					return this.rowsReceived;
				}
			}

			private SearchOperationFastResponse(int rowsReceived, TimeSpan elapsedTime) : base("FRSP", elapsedTime)
			{
				this.rowsReceived = rowsReceived;
			}

			public static SearchExecutionDiagnostics.SearchOperationFastResponse Create(int rowsReceived, TimeSpan elapsedTime)
			{
				return new SearchExecutionDiagnostics.SearchOperationFastResponse(rowsReceived, elapsedTime);
			}

			public override void AppendToTraceContentBuilder(TraceContentBuilder cb)
			{
				base.AppendToTraceContentBuilder(cb);
				cb.Append(", ");
				cb.Append(this.rowsReceived);
				cb.Append(" rows");
				cb.AppendLine();
			}

			private readonly int rowsReceived;
		}

		public sealed class SearchOperationLinkedRows : SearchExecutionDiagnostics.SearchOperationBase
		{
			private SearchOperationLinkedRows(int messagesLinked, int rowsProcessed, int totalLinked, TimeSpan elapsedTime) : base("SLNK", elapsedTime)
			{
				this.messagesLinked = messagesLinked;
				this.rowsProcessed = rowsProcessed;
				this.totalLinked = totalLinked;
			}

			public int MessagesLinked
			{
				get
				{
					return this.messagesLinked;
				}
			}

			public static SearchExecutionDiagnostics.SearchOperationLinkedRows Create(int messagesLinked, int rowsProcessed, int totalLinked, TimeSpan elapsedTime)
			{
				return new SearchExecutionDiagnostics.SearchOperationLinkedRows(messagesLinked, rowsProcessed, totalLinked, elapsedTime);
			}

			public override void AppendToTraceContentBuilder(TraceContentBuilder cb)
			{
				base.AppendToTraceContentBuilder(cb);
				cb.Append(", ");
				cb.Append(this.messagesLinked);
				cb.Append(" linked, ");
				cb.Append(this.rowsProcessed);
				cb.Append(" rows, ");
				cb.Append(this.totalLinked);
				cb.Append(" total");
				cb.AppendLine();
			}

			private readonly int messagesLinked;

			private readonly int rowsProcessed;

			private readonly int totalLinked;
		}

		public sealed class SearchOperationFastError : SearchExecutionDiagnostics.SearchOperationBase
		{
			private SearchOperationFastError(string error, TimeSpan elapsedTime) : base("FERR", elapsedTime)
			{
				this.errorMessage = error;
			}

			public static SearchExecutionDiagnostics.SearchOperationFastError Create(string error, TimeSpan elapsedTime)
			{
				return new SearchExecutionDiagnostics.SearchOperationFastError(error, elapsedTime);
			}

			public override void AppendToTraceContentBuilder(TraceContentBuilder cb)
			{
				base.AppendToTraceContentBuilder(cb);
				cb.Append(", ");
				cb.Append(this.errorMessage);
				cb.AppendLine();
			}

			private readonly string errorMessage;
		}

		private static class Time
		{
			public static string ToString(long value)
			{
				return value.ToString("N0", CultureInfo.InvariantCulture);
			}

			public static string ToString(double value)
			{
				return value.ToString("N0", CultureInfo.InvariantCulture);
			}
		}
	}
}
