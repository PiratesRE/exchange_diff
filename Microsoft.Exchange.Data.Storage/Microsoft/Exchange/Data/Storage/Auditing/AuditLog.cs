using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AuditLog : IAuditLog
	{
		public AuditLog(StoreSession storeSession, StoreId logFolderId, DateTime logRangeStart, DateTime logRangeEnd, int itemCount, Func<IAuditLogRecord, MessageItem, int> recordFormatter)
		{
			this.storeSession = storeSession;
			this.LogFolderId = logFolderId;
			this.EstimatedLogStartTime = logRangeStart;
			this.EstimatedLogEndTime = logRangeEnd;
			this.EstimatedItemCount = itemCount;
			this.recordFormatter = recordFormatter;
		}

		public DateTime EstimatedLogStartTime { get; private set; }

		public DateTime EstimatedLogEndTime { get; private set; }

		public bool IsAsynchronous
		{
			get
			{
				return false;
			}
		}

		public StoreId LogFolderId { get; private set; }

		public StoreSession Session
		{
			get
			{
				return this.storeSession;
			}
		}

		public int EstimatedItemCount { get; private set; }

		public IEnumerable<T> FindAuditRecords<T>(IAuditRecordStrategy<T> strategy)
		{
			using (Folder logFolder = Folder.Bind(this.storeSession, this.LogFolderId))
			{
				foreach (T result in AuditLog.InternalFindAuditRecords<T>(logFolder, strategy))
				{
					yield return result;
				}
			}
			yield break;
		}

		public int WriteAuditRecord(IAuditLogRecord auditRecord)
		{
			if (this.recordFormatter == null)
			{
				throw new InvalidOperationException("Audit log is not configured properly.");
			}
			int result;
			using (MessageItem messageItem = MessageItem.Create(this.Session, this.LogFolderId))
			{
				result = this.recordFormatter(auditRecord, messageItem);
				messageItem.Save(SaveMode.NoConflictResolutionForceSave);
			}
			return result;
		}

		public IAuditQueryContext<TFilter> CreateAuditQueryContext<TFilter>()
		{
			if (typeof(TFilter) != typeof(QueryFilter))
			{
				throw new NotSupportedException();
			}
			return (IAuditQueryContext<TFilter>)new AuditLog.AuditLogQueryContext(this);
		}

		private static IEnumerable<T> InternalFindAuditRecords<T>(Folder logFolder, IAuditRecordStrategy<T> strategy)
		{
			using (QueryResult queryResults = logFolder.ItemQuery(ItemQueryType.None, null, strategy.QuerySortOrder, strategy.Columns))
			{
				queryResults.SeekToOffset(SeekReference.OriginBeginning, 0);
				bool theEnd = false;
				while (!theEnd)
				{
					object[][] rows = queryResults.GetRows(1000);
					if (rows.Length <= 0)
					{
						break;
					}
					foreach (object[] row in rows)
					{
						AuditLog.RowPropertyBag rowAsPropertyBag = new AuditLog.RowPropertyBag(strategy.Columns, row);
						bool stopNow;
						bool match = strategy.RecordFilter(rowAsPropertyBag, out stopNow);
						if (stopNow)
						{
							theEnd = true;
							break;
						}
						if (match)
						{
							yield return strategy.Convert(rowAsPropertyBag);
						}
					}
				}
			}
			yield break;
		}

		private const int ItemQueryBatchSize = 1000;

		private StoreSession storeSession;

		private Func<IAuditLogRecord, MessageItem, int> recordFormatter;

		private class RowPropertyBag : IReadOnlyPropertyBag
		{
			public RowPropertyBag(PropertyDefinition[] columns, object[] values)
			{
				this.columns = columns;
				this.row = values;
			}

			public object this[PropertyDefinition propertyDefinition]
			{
				get
				{
					StorePropertyDefinition other = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
					for (int i = 0; i < this.columns.Length; i++)
					{
						StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(this.columns[i]);
						if (storePropertyDefinition.CompareTo(other) == 0)
						{
							return this.row[i];
						}
					}
					return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
				}
			}

			public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
			{
				throw new NotSupportedException();
			}

			private PropertyDefinition[] columns;

			private object[] row;
		}

		private class AuditLogQueryContext : DisposableObject, IAuditQueryContext<QueryFilter>, IDisposable
		{
			public AuditLogQueryContext(AuditLog auditLog)
			{
				this.auditLog = auditLog;
				this.pendingAsyncResult = null;
			}

			public IAsyncResult BeginAuditLogQuery(QueryFilter queryFilter, int maximumResultsCount)
			{
				if (this.pendingAsyncResult != null)
				{
					throw new InvalidOperationException("Asynchronous query is already pending.");
				}
				StoreId storeId = null;
				IAsyncResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					Folder disposable;
					if (queryFilter != null)
					{
						SearchFolder searchFolder = SearchFolder.Create(this.auditLog.Session, this.auditLog.Session.GetDefaultFolderId(DefaultFolderType.SearchFolders), "SearchAuditMailboxFolder" + Guid.NewGuid().ToString(), CreateMode.OpenIfExists);
						disposeGuard.Add<SearchFolder>(searchFolder);
						searchFolder.Save();
						searchFolder.Load();
						storeId = searchFolder.Id;
						result = searchFolder.BeginApplyOneTimeSearch(new SearchFolderCriteria(queryFilter, new StoreId[]
						{
							this.auditLog.LogFolderId
						})
						{
							DeepTraversal = false,
							UseCiForComplexQueries = true,
							FailNonContentIndexedSearch = true,
							MaximumResultsCount = new int?(maximumResultsCount)
						}, null, null);
						disposable = searchFolder;
					}
					else
					{
						disposable = Folder.Bind(this.auditLog.Session, this.auditLog.LogFolderId);
						disposeGuard.Add<Folder>(disposable);
						result = new CompletedAsyncResult();
					}
					disposeGuard.Success();
					this.pendingAsyncResult = result;
					this.folder = disposable;
					this.folderIdToDelete = storeId;
				}
				return result;
			}

			public IEnumerable<T> EndAuditLogQuery<T>(IAsyncResult asyncResult, IAuditQueryStrategy<T> queryStrategy)
			{
				SearchFolder searchFolder = this.folder as SearchFolder;
				if (searchFolder != null)
				{
					SearchFolderCriteria searchCriteria = searchFolder.GetSearchCriteria();
					if ((searchCriteria.SearchState & SearchState.FailNonContentIndexedSearch) == SearchState.FailNonContentIndexedSearch && (searchCriteria.SearchState & SearchState.Failed) == SearchState.Failed)
					{
						throw queryStrategy.GetQueryFailedException();
					}
				}
				AuditLog.AuditLogQueryContext.QueryRecordStrategy<T> strategy = new AuditLog.AuditLogQueryContext.QueryRecordStrategy<T>(queryStrategy);
				return AuditLog.InternalFindAuditRecords<T>(this.folder, strategy);
			}

			protected override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<AuditLog.AuditLogQueryContext>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					if (this.folder != null)
					{
						this.folder.Dispose();
						this.folder = null;
					}
					if (this.folderIdToDelete != null)
					{
						this.auditLog.Session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
						{
							this.folderIdToDelete
						});
					}
				}
				base.InternalDispose(disposing);
			}

			private static readonly SortBy[] SortBySentTime = new SortBy[]
			{
				new SortBy(ItemSchema.SentTime, SortOrder.Descending)
			};

			private static readonly PropertyDefinition[] QueryProperties = new PropertyDefinition[]
			{
				ItemSchema.Id,
				ItemSchema.SentTime
			};

			private AuditLog auditLog;

			private Folder folder;

			private StoreId folderIdToDelete;

			private IAsyncResult pendingAsyncResult;

			private class QueryRecordStrategy<T> : IAuditRecordStrategy<T>
			{
				public QueryRecordStrategy(IAuditQueryStrategy<T> queryStrategy)
				{
					this.queryStrategy = queryStrategy;
				}

				public SortBy[] QuerySortOrder
				{
					get
					{
						return AuditLog.AuditLogQueryContext.SortBySentTime;
					}
				}

				public PropertyDefinition[] Columns
				{
					get
					{
						return AuditLog.AuditLogQueryContext.QueryProperties;
					}
				}

				public bool RecordFilter(IReadOnlyPropertyBag propertyBag, out bool stopNow)
				{
					stopNow = false;
					return propertyBag[ItemSchema.Id] is StoreId && this.queryStrategy.RecordFilter(propertyBag, out stopNow);
				}

				public T Convert(IReadOnlyPropertyBag propertyBag)
				{
					return this.queryStrategy.Convert(propertyBag);
				}

				private IAuditQueryStrategy<T> queryStrategy;
			}
		}
	}
}
