using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal sealed class FailedItemStorage : IDisposeTrackable, IFailedItemStorage, IDisposable
	{
		public FailedItemStorage(ISearchServiceConfig config, string indexSystemName, string hostName)
		{
			Util.ThrowOnNullArgument(config, "config");
			Util.ThrowOnNullArgument(indexSystemName, "indexSystemName");
			Util.ThrowOnNullArgument(hostName, "hostName");
			this.internalImsFlow = FlowDescriptor.GetImsInternalFlowDescriptor(config, indexSystemName).DisplayName;
			this.diagnosticSession = DiagnosticsSession.CreateComponentDiagnosticsSession("FailedItemStorage", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.FailedItemStorageTracer, (long)this.GetHashCode());
			this.fastQueryExecutor = new ExchangeQueryExecutor(hostName, config.QueryServicePort, this.internalImsFlow, false, 0, config.QueryOperationTimeout, config.QueryProxyCacheTimeout);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public void Dispose()
		{
			if (!this.isDisposedFlag)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.fastQueryExecutor != null)
				{
					this.fastQueryExecutor.Dispose();
					this.fastQueryExecutor = null;
				}
				this.isDisposedFlag = true;
			}
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FailedItemStorage>(this);
		}

		public long GetFailedItemsCount(FailedItemParameters parameters)
		{
			string failuresQuery = FailedItemStorage.GetFailuresQuery(parameters);
			return this.GetCount(failuresQuery, "FailedItemStorage: GetFailedItemsCount()");
		}

		public ICollection<IFailureEntry> GetFailedItems(FailedItemParameters parameters)
		{
			string failuresQuery = FailedItemStorage.GetFailuresQuery(parameters);
			return this.GetFailures(failuresQuery, parameters.Fields, parameters.StartingIndexId, parameters.ResultLimit);
		}

		public ICollection<IDocEntry> GetDeletionPendingItems(int deletedMailboxNumber)
		{
			this.CheckDisposed();
			long num = IndexId.CreateIndexId(deletedMailboxNumber, 1);
			long num2 = IndexId.CreateIndexId(deletedMailboxNumber, int.MaxValue);
			QueryFilter value = new BetweenFilter(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.DocumentId, num), new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.DocumentId, num2));
			string text = FqlQueryBuilder.ToFqlString(value, CultureInfo.InvariantCulture);
			this.diagnosticSession.TraceDebug<string>("GetDeletionPendingItems with query: {0}", text);
			PagedReader pagedReader = this.fastQueryExecutor.ExecutePagedQueryByDocumentId(this.internalImsFlow, text, Guid.Empty, DocEntry.Schema);
			return new FailedItemStorage.DocEntryCollection(pagedReader, this.diagnosticSession, this.internalImsFlow);
		}

		public long GetItemsCount(Guid filterMailboxGuid)
		{
			QueryFilter value = (filterMailboxGuid == Guid.Empty) ? FailedItemStorage.ErrorCodeTrueFilter : new TextFilter(ItemSchema.MailboxGuid, filterMailboxGuid.ToString(), MatchOptions.ExactPhrase, MatchFlags.IgnoreCase);
			string query = FqlQueryBuilder.ToFqlString(value, CultureInfo.CurrentCulture);
			return this.GetCount(query, "FailedItemStorage: GetItemsCount()");
		}

		public long GetPermanentFailureCount()
		{
			return this.GetCount(FailedItemStorage.PermanentFailureQuery, "FailedItemStorage: GetPermanentFailureCount()");
		}

		public ICollection<IFailureEntry> GetRetriableItems(FieldSet fields)
		{
			return this.GetFailures(FailedItemStorage.TransientFailureQuery, fields, 0L, int.MaxValue);
		}

		public ICollection<long> GetPoisonDocuments()
		{
			this.CheckDisposed();
			string text = string.Format("errorcode:{0}", EvaluationErrorsHelper.MakePermanentError(EvaluationErrors.PoisonDocument));
			this.diagnosticSession.TraceDebug<string>("GetPoisonItems with query: {0}", text);
			PagedReader pagedReader = this.fastQueryExecutor.ExecutePagedQueryByDocumentId(this.internalImsFlow, text, Guid.Empty, null);
			return ExchangeQueryExecutor.RunUnderExceptionHandler<List<long>>(delegate()
			{
				List<long> list = new List<long>(pagedReader.Count);
				foreach (SearchResultItem searchResultItem in pagedReader)
				{
					foreach (IFieldHolder fieldHolder in searchResultItem.Fields)
					{
						if (fieldHolder.Name == "DocId")
						{
							list.Add((long)fieldHolder.Value);
						}
					}
				}
				return list;
			}, this.diagnosticSession, this.internalImsFlow);
		}

		internal static string GetFailuresQuery(FailedItemParameters parameters)
		{
			int evaluationError = parameters.ErrorCode ?? 200;
			List<QueryFilter> list = new List<QueryFilter>();
			QueryFilter item;
			switch (parameters.FailureMode)
			{
			case FailureMode.Transient:
				item = ((parameters.ErrorCode != null) ? new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakeRetriableError(evaluationError)) : new BetweenFilter(new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.IndexingErrorCode, 0), new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakeRetriableError(evaluationError))));
				break;
			case FailureMode.Permanent:
				item = ((parameters.ErrorCode != null) ? new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakePermanentError(evaluationError)) : new BetweenFilter(new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakePermanentError(evaluationError)), new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.IndexingErrorCode, 0)));
				break;
			case FailureMode.All:
				item = ((parameters.ErrorCode != null) ? new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakePermanentError(evaluationError)),
					new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakeRetriableError(evaluationError))
				}) : new OrFilter(new QueryFilter[]
				{
					new BetweenFilter(new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakePermanentError(evaluationError)), new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.IndexingErrorCode, 0)),
					new BetweenFilter(new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.IndexingErrorCode, 0), new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakeRetriableError(evaluationError)))
				}));
				break;
			default:
				throw new ArgumentException(string.Format("Unknown failure mode {0}", parameters.FailureMode));
			}
			list.Add(item);
			if (parameters.MailboxGuid != null)
			{
				list.Add(new TextFilter(ItemSchema.MailboxGuid, parameters.MailboxGuid.ToString(), MatchOptions.ExactPhrase, MatchFlags.Default));
			}
			if (parameters.StartDate != null && parameters.EndDate != null)
			{
				list.Add(new BetweenFilter(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.LastIndexingAttemptTime, parameters.StartDate), new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.LastIndexingAttemptTime, parameters.EndDate)));
			}
			else
			{
				if (parameters.StartDate != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.LastIndexingAttemptTime, parameters.StartDate));
				}
				if (parameters.EndDate != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.LastIndexingAttemptTime, parameters.EndDate));
				}
			}
			if (parameters.ParentEntryId != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ParentEntryId, parameters.ParentEntryId));
			}
			if (parameters.IsPartiallyProcessed != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IsPartiallyIndexed, parameters.IsPartiallyProcessed));
			}
			QueryFilter value = new AndFilter(list.ToArray());
			return FqlQueryBuilder.ToFqlString(value, parameters.Culture);
		}

		private void CheckDisposed()
		{
			if (this.isDisposedFlag)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private ICollection<IFailureEntry> GetFailures(string query, FieldSet fields, long startingIndexId, int maxResults)
		{
			this.CheckDisposed();
			this.diagnosticSession.TraceDebug<string>("Getting failures with query: {0}", query);
			IndexSystemField[] schema = FailureEntry.GetSchema(fields);
			PagedReader pagedReader = this.fastQueryExecutor.ExecutePagedQueryByDocumentId(this.internalImsFlow, query, Guid.Empty, schema, startingIndexId, maxResults);
			return new FailedItemStorage.FailureCollection(pagedReader, this.diagnosticSession, this.internalImsFlow);
		}

		private long GetCount(string query, string traceDebugString)
		{
			this.CheckDisposed();
			this.diagnosticSession.TraceDebug<string, string>("{0} with query: {1}", traceDebugString, query);
			long hitCount = this.fastQueryExecutor.GetHitCount(this.internalImsFlow, query, Guid.Empty);
			this.diagnosticSession.TraceDebug<long>("count: {0}", hitCount);
			return hitCount;
		}

		internal const string MonitoringItemsQuery = "errorcode:{0}";

		private const string DocIdField = "DocId";

		internal static readonly ComparisonFilter ErrorCodeTrueFilter = new ComparisonFilter(ComparisonOperator.NotEqual, ItemSchema.IndexingErrorCode, EvaluationErrorsHelper.MakePermanentError(EvaluationErrors.NonExistentErrorCode));

		internal static readonly string PermanentFailureQuery = string.Format("errorcode:range({0},0)", EvaluationErrorsHelper.MakePermanentError(EvaluationErrors.MaxFailureId));

		internal static readonly string TransientFailureQuery = string.Format("errorcode:range(1,{0})", EvaluationErrorsHelper.MakeRetriableError(EvaluationErrors.MaxFailureId));

		private readonly IDiagnosticsSession diagnosticSession;

		private readonly string internalImsFlow;

		private ExchangeQueryExecutor fastQueryExecutor;

		private DisposeTracker disposeTracker;

		private bool isDisposedFlag;

		private abstract class EntryCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : IDocEntry
		{
			protected EntryCollection(PagedReader pagedReader, IDiagnosticsSession diagnosticsSession, string flowName)
			{
				this.pagedReader = pagedReader;
				this.diagnosticsSession = diagnosticsSession;
				this.flowName = flowName;
			}

			public int Count
			{
				get
				{
					return ExchangeQueryExecutor.RunUnderExceptionHandler<int>(() => this.pagedReader.Count, this.diagnosticsSession, this.flowName);
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public abstract IEnumerator<T> GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public void Add(T item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(T item)
			{
				throw new NotSupportedException();
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				throw new NotSupportedException();
			}

			public bool Remove(T item)
			{
				throw new NotSupportedException();
			}

			protected IEnumerable<ISearchResultItem> GetItemsSafe()
			{
				using (IEnumerator<ISearchResultItem> enumerator = ExchangeQueryExecutor.RunUnderExceptionHandler<IEnumerator<SearchResultItem>>(new Func<IEnumerator<SearchResultItem>>(this.pagedReader.GetEnumerator), this.diagnosticsSession, this.flowName))
				{
					while (ExchangeQueryExecutor.RunUnderExceptionHandler<bool>(new Func<bool>(enumerator.MoveNext), this.diagnosticsSession, this.flowName))
					{
						yield return ExchangeQueryExecutor.RunUnderExceptionHandler<ISearchResultItem>(() => enumerator.Current, this.diagnosticsSession, this.flowName);
					}
				}
				yield break;
			}

			private readonly PagedReader pagedReader;

			private readonly IDiagnosticsSession diagnosticsSession;

			private readonly string flowName;
		}

		private sealed class DocEntryCollection : FailedItemStorage.EntryCollection<IDocEntry>
		{
			public DocEntryCollection(PagedReader pagedReader, IDiagnosticsSession diagnosticsSession, string flowName) : base(pagedReader, diagnosticsSession, flowName)
			{
			}

			public override IEnumerator<IDocEntry> GetEnumerator()
			{
				foreach (ISearchResultItem item in base.GetItemsSafe())
				{
					yield return new DocEntry(item);
				}
				yield break;
			}
		}

		private sealed class FailureCollection : FailedItemStorage.EntryCollection<IFailureEntry>
		{
			public FailureCollection(PagedReader pagedReader, IDiagnosticsSession diagnosticsSession, string flowName) : base(pagedReader, diagnosticsSession, flowName)
			{
			}

			public override IEnumerator<IFailureEntry> GetEnumerator()
			{
				foreach (ISearchResultItem item in base.GetItemsSafe())
				{
					yield return new FailureEntry(item);
				}
				yield break;
			}
		}
	}
}
