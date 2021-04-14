using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LazyIndexing;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public class LogicalIndexCache : LockableMailboxComponent, IComponentData
	{
		internal LogicalIndexCache(Context context, MailboxState mailboxState)
		{
			this.mailboxLockName = mailboxState;
			if (LogicalIndexCache.folderCachePerfCounters == null)
			{
				StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(context.Database);
				LogicalIndexCache.folderCachePerfCounters = new CachePerformanceCounters<StorePerDatabasePerformanceCountersInstance>(() => databaseInstance, (StorePerDatabasePerformanceCountersInstance instance) => instance.LogicalIndexSize, (StorePerDatabasePerformanceCountersInstance instance) => instance.RateOfLogicalIndexLookups, (StorePerDatabasePerformanceCountersInstance instance) => instance.RateOfLogicalIndexMisses, (StorePerDatabasePerformanceCountersInstance instance) => instance.RateOfLogicalIndexHits, (StorePerDatabasePerformanceCountersInstance instance) => instance.RateOfLogicalIndexInserts, (StorePerDatabasePerformanceCountersInstance instance) => instance.RateOfLogicalIndexDeletes, (StorePerDatabasePerformanceCountersInstance instance) => instance.SizeOfLogicalIndexExpirationQueue);
			}
			this.folderIdToFolderIndexCache = new SingleKeyCache<ExchangeId, LogicalIndexCache.FolderIndexCache>(new LRU2WithTimeToLiveExpirationPolicy<ExchangeId>(LogicalIndexCache.NumberOfCachedFoldersPerMailbox, LogicalIndexCache.TimeToLive, false), LogicalIndexCache.folderCachePerfCounters);
			IMailboxContext mailboxContext = context.GetMailboxContext(mailboxState.MailboxNumber);
			if (mailboxContext.GetCreatedByMove(context))
			{
				this.updateIndexDirectly = true;
			}
			this.mailboxCreationTime = mailboxContext.GetCreationTime(context);
			this.InitializeCache(context);
		}

		public MailboxLockNameBase MailboxLockName
		{
			get
			{
				return this.mailboxLockName;
			}
		}

		public long EstimatedOldestMaintenanceRecord
		{
			get
			{
				return this.estimatedOldestMaintenanceRecord;
			}
		}

		public long EstimatedNewestMaintenanceRecord
		{
			get
			{
				return this.estimatedNewestMaintenanceRecord;
			}
		}

		public override MailboxComponentId MailboxComponentId
		{
			get
			{
				return MailboxComponentId.LogicalIndexCache;
			}
		}

		public override Guid DatabaseGuid
		{
			get
			{
				return this.mailboxLockName.DatabaseGuid;
			}
		}

		public override int MailboxPartitionNumber
		{
			get
			{
				return this.mailboxLockName.MailboxPartitionNumber;
			}
		}

		public override LockManager.LockType ReaderLockType
		{
			get
			{
				return LockManager.LockType.LogicalIndexCacheShared;
			}
		}

		public override LockManager.LockType WriterLockType
		{
			get
			{
				return LockManager.LockType.LogicalIndexCacheExclusive;
			}
		}

		internal static LogicalIndexCache.ApplyMaintenanceSettings ApplyMaintenanceParameters
		{
			get
			{
				return LogicalIndexCache.applyMaintenanceParameters.Value;
			}
		}

		internal static IDisposable SetLogicalIndexCleanupChunkSizeForTest(int chunkSize)
		{
			return LogicalIndexCache.logicalIndexCleanupChunkSize.SetTestHook(chunkSize);
		}

		internal static IDisposable SetMarkMailboxForLogicalIndexCleanupTestHook(Func<MailboxState, bool, bool> testDelegate)
		{
			return LogicalIndexCache.markMailboxForLogicalIndexCleanupTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetForceMailboxLogicalIndexCleanupTestHook(Func<MailboxState, bool, bool> testDelegate)
		{
			return LogicalIndexCache.forceMailboxLogicalIndexCleanupTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetCleanupOneLogicalIndexTestHook(Action<MailboxState, LogicalIndex, bool> testDelegate)
		{
			return LogicalIndexCache.cleanupOneLogicalIndexTestHook.SetTestHook(testDelegate);
		}

		internal bool UpdateIndexDirectly
		{
			get
			{
				return this.updateIndexDirectly;
			}
		}

		internal SingleKeyCache<ExchangeId, LogicalIndexCache.FolderIndexCache> FolderIdToFolderIndexCacheForTest
		{
			get
			{
				return this.folderIdToFolderIndexCache;
			}
		}

		public static void Initialize()
		{
			LogicalIndexCache.NumberOfCachedFoldersPerMailbox = ConfigurationSchema.LogicalIndexCacheSize.Value;
			LogicalIndexCache.TimeToLive = ConfigurationSchema.LogicalIndexCacheTimeToLive.Value;
			LogicalIndexCache.maxIdleCleanupPeriod = ConfigurationSchema.MaxIdleCleanupPeriod.Value;
			LogicalIndexCache.applyMaintenanceParameters = Hookable<LogicalIndexCache.ApplyMaintenanceSettings>.Create(false, new LogicalIndexCache.ApplyMaintenanceSettings
			{
				StopMaintenanceThreshold = ConfigurationSchema.StopMaintenanceThreshold.Value,
				WlmMaintenanceThreshold = ConfigurationSchema.WlmMaintenanceThreshold.Value,
				NumberOfRecordsToMaintain = ConfigurationSchema.NumberOfRecordsToMaintain.Value,
				NumberOfRecordsToReadFromMaintenanceTable = ConfigurationSchema.NumberOfRecordsToReadFromMaintenanceTable.Value,
				MaintenanceTimePeriodToKeep = ConfigurationSchema.MaintenanceTimePeriodToKeep.Value,
				WlmMinNumberOfChunksToProceed = ConfigurationSchema.WlmMinNumberOfChunksToProceed.Value
			});
			if (LogicalIndexCache.logicalIndexCacheDataSlot == -1)
			{
				LogicalIndexCache.logicalIndexCacheDataSlot = MailboxState.AllocateComponentDataSlot(false);
				LogicalIndexCache.logicalIndexCleanupMaintenance = MaintenanceHandler.RegisterMailboxMaintenance(LogicalIndexCache.LogicalIndexCleanupMaintenanceId, RequiredMaintenanceResourceType.Store, false, new MaintenanceHandler.MailboxMaintenanceDelegate(LogicalIndexCache.CleanupLogicalIndexes), "LogicalIndexCache.CleanupLogicalIndexes");
				LogicalIndexCache.markLogicalIndexForCleanupMaintenance = MaintenanceHandler.RegisterDatabaseMaintenance(LogicalIndexCache.MarkLogicalIndexForCleanupMaintenanceId, RequiredMaintenanceResourceType.Store, new MaintenanceHandler.DatabaseMaintenanceDelegate(LogicalIndexCache.MarkLogicalIndicesForCleanup), "LogicalIndexCache.MarkLogicalIndicesForCleanup");
				LogicalIndexCache.applyingMaintenanceTableMaintenance = MaintenanceHandler.RegisterMailboxMaintenance(LogicalIndexCache.ApplyingMaintenanceTableMaintenanceId, RequiredMaintenanceResourceType.Store, true, new MaintenanceHandler.MailboxMaintenanceDelegate(LogicalIndexCache.ApplyMaintenanceTable), "LogicalIndexCache.ApplyMaintenanceTable");
				Mailbox.RegisterOnDisconnectAction(delegate(Context context, Mailbox mailbox)
				{
					LogicalIndexCache cacheForMailboxDoNotCreate = LogicalIndexCache.GetCacheForMailboxDoNotCreate(context, mailbox.SharedState);
					if (cacheForMailboxDoNotCreate != null)
					{
						cacheForMailboxDoNotCreate.OnMailboxDisconnect(context, mailbox);
					}
				});
			}
		}

		public static void MountedEventHandler(Context context)
		{
			if (context.Database.PhysicalDatabase.DatabaseType != DatabaseType.Sql)
			{
				LogicalIndexCache.markLogicalIndexForCleanupMaintenance.ScheduleMarkForMaintenance(context, TimeSpan.FromDays(1.0));
			}
		}

		public static IEnumerable<LogicalIndex> GetIndicesForFolder(Context context, Mailbox mailbox, ExchangeId folderId)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			return cacheForFolder.Values;
		}

		public static bool IsCacheAvailable(Context context, Mailbox mailbox)
		{
			return LogicalIndexCache.GetCacheForMailboxDoNotCreate(context, mailbox.SharedState) != null;
		}

		public static void DiscardCacheForMailbox(Context context, MailboxState mailboxState)
		{
			LogicalIndexCache logicalIndexCache = mailboxState.GetComponentData(LogicalIndexCache.logicalIndexCacheDataSlot) as LogicalIndexCache;
			if (logicalIndexCache != null)
			{
				if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.PseudoIndexTracer.TraceDebug(0L, "Discarding logical index cache for mailbox " + mailboxState.MailboxNumber);
				}
				if (((IComponentData)logicalIndexCache).DoCleanup(context))
				{
					mailboxState.SetComponentData(LogicalIndexCache.logicalIndexCacheDataSlot, null);
				}
			}
		}

		public static List<IIndex> GetIndexesInScope(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table, bool matchingOnly, bool existingOnly)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			if (context.IsSharedMailboxOperation)
			{
				using (context.MailboxComponentReadOperation(cacheForMailbox))
				{
					bool flag;
					List<IIndex> indexesInScopeDoNotCreate = LogicalIndexCache.GetIndexesInScopeDoNotCreate(context, mailbox, folderId, cacheForFolder, indexType, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, matchingOnly, out flag);
					if (flag || existingOnly)
					{
						return indexesInScopeDoNotCreate;
					}
				}
			}
			List<IIndex> result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame2 = context.MailboxComponentWriteOperation(cacheForMailbox))
			{
				bool flag2;
				List<IIndex> list = LogicalIndexCache.GetIndexesInScopeDoNotCreate(context, mailbox, folderId, cacheForFolder, indexType, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, matchingOnly, out flag2);
				if (flag2 || existingOnly)
				{
					result = list;
				}
				else
				{
					if ((matchingOnly || list != null) && (indexType == LogicalIndexType.Messages || indexType == LogicalIndexType.Conversations || indexType == LogicalIndexType.SearchFolderMessages))
					{
						cacheForFolder.ConsolidateIndexes(context, indexType, 0, conditionalIndexColumn, conditionalIndexValue, ref sortOrder, ref nonKeyColumns, list);
					}
					LogicalIndex item = cacheForFolder.CreateIndex(context, mailbox.SharedState, indexType, 0, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, false);
					if (list == null)
					{
						list = new List<IIndex>(1);
					}
					list.Insert(0, item);
					mailboxComponentOperationFrame2.Success();
					result = list;
				}
			}
			return result;
		}

		public static List<IIndex> GetIndexesInScope(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, Table table)
		{
			return LogicalIndexCache.GetIndexesInScope(context, mailbox, folderId, indexType, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, null, table, false, false);
		}

		public static LogicalIndex GetIndexToUse(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table)
		{
			List<IIndex> indexesInScope = LogicalIndexCache.GetIndexesInScope(context, mailbox, folderId, indexType, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, true, false);
			return (LogicalIndex)indexesInScope[0];
		}

		public static LogicalIndex GetIndexToUseDoNotCreate(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table)
		{
			List<IIndex> indexesInScope = LogicalIndexCache.GetIndexesInScope(context, mailbox, folderId, indexType, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, true, true);
			if (indexesInScope != null)
			{
				return (LogicalIndex)indexesInScope[0];
			}
			return null;
		}

		public static LogicalIndex FindIndex(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, int indexSignature)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			LogicalIndex result;
			using (context.MailboxComponentReadOperation(cacheForMailbox))
			{
				result = LogicalIndexCache.FindIndexInternal(context, mailbox, folderId, cacheForFolder, indexType, indexSignature);
			}
			return result;
		}

		public static LogicalIndex CreateIndex(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, int indexSignature, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table, bool markCurrent)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			LogicalIndex result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(cacheForMailbox))
			{
				LogicalIndex logicalIndex = cacheForFolder.CreateIndex(context, mailbox.SharedState, indexType, indexSignature, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, markCurrent);
				mailboxComponentOperationFrame.Success();
				result = logicalIndex;
			}
			return result;
		}

		public static void TrackIndexUpdate(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, LogicalIndex.LogicalOperation operation, IColumnValueBag updatedPropBag)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			foreach (LogicalIndex logicalIndex in cacheForFolder.Values)
			{
				if (indexType == logicalIndex.IndexType && !logicalIndex.IsStale)
				{
					logicalIndex.TrackIndexUpdate(context, folderId, updatedPropBag, operation);
				}
			}
		}

		public static bool FolderHasConversationIndex(Context context, Mailbox mailbox, ExchangeId folderId)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			bool result;
			using (context.MailboxComponentReadOperation(cacheForMailbox))
			{
				result = cacheForFolder.Values.Any((LogicalIndex logicalIndex) => LogicalIndexType.Conversations == logicalIndex.IndexType);
			}
			return result;
		}

		public static bool InvalidateIndexes(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, DateTime lastReferenceDateThreshold)
		{
			bool result = false;
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			foreach (LogicalIndex logicalIndex in cacheForFolder.Values)
			{
				if (logicalIndex.FolderId == folderId && logicalIndex.IndexType == indexType && !logicalIndex.IsStale && (conditionalIndexColumn == null || (conditionalIndexColumn == logicalIndex.ConditionalIndexColumn && conditionalIndexValue == logicalIndex.ConditionalIndexValue)) && logicalIndex.LastReferenceDate < lastReferenceDateThreshold)
				{
					logicalIndex.InvalidateIndex(context, false);
					result = true;
				}
			}
			return result;
		}

		public static bool InvalidateIndexes(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType)
		{
			return LogicalIndexCache.InvalidateIndexes(context, mailbox, folderId, indexType, null, false, DateTime.MaxValue);
		}

		public static void InvalidateIndexesForFolderPropertyChange(Context context, Mailbox mailbox, ExchangeId folderId, StorePropTag propTag)
		{
			Column item = PropertySchema.MapToColumn(mailbox.Database, ObjectType.Message, propTag);
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			foreach (LogicalIndex logicalIndex in cacheForFolder.Values)
			{
				if (logicalIndex.FolderId == folderId && (logicalIndex.IndexType == LogicalIndexType.Messages || logicalIndex.IndexType == LogicalIndexType.SearchFolderMessages || logicalIndex.IndexType == LogicalIndexType.CategoryHeaders) && !logicalIndex.IsStale && logicalIndex.Columns.Contains(item))
				{
					logicalIndex.InvalidateIndex(context, false);
				}
			}
		}

		public static bool VerifyIfIndexTypeExists(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexType indexType)
		{
			bool result = false;
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			using (context.MailboxComponentReadOperation(cacheForMailbox))
			{
				foreach (LogicalIndex logicalIndex in cacheForFolder.Values)
				{
					if (logicalIndex.FolderId == folderId && logicalIndex.IndexType == indexType && !logicalIndex.IsStale)
					{
						result = true;
					}
				}
			}
			return result;
		}

		public static LogicalIndex GetLogicalIndex(Context context, Mailbox mailbox, ExchangeId folderId, int logicalIndexNumber)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			LogicalIndex logicalIndex;
			using (context.MailboxComponentReadOperation(cacheForMailbox))
			{
				logicalIndex = cacheForFolder.GetLogicalIndex(logicalIndexNumber);
			}
			return logicalIndex;
		}

		public static void DeleteIndex(Context context, Mailbox mailbox, ExchangeId folderId, int logicalIndexNumber)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
			LogicalIndex logicalIndex = cacheForFolder.GetLogicalIndex(logicalIndexNumber);
			if (cacheForMailbox.IsIndexLockedInCache(logicalIndex))
			{
				if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.PseudoIndexTracer.TraceDebug<int>(0L, "Cannot delete index {0} locked in cache", logicalIndexNumber);
				}
				throw new StoreException((LID)62304U, ErrorCodeValue.Busy);
			}
			cacheForFolder.DeleteIndex(context, logicalIndexNumber);
		}

		public static void DeleteLogicalIndexes(Context context, Mailbox mailbox)
		{
			List<LogicalIndexCache.LogicalIndexInfo> listOfLogicalIndexes = LogicalIndexCache.GetListOfLogicalIndexes(context, mailbox.SharedState, ExchangeId.Zero);
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			foreach (LogicalIndexCache.LogicalIndexInfo logicalIndexInfo in listOfLogicalIndexes)
			{
				LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, logicalIndexInfo.FolderId);
				cacheForFolder.DeleteIndex(context, logicalIndexInfo.LogicalIndexNumber);
			}
			mailbox.RemoveMailboxEntriesFromTable(context, DatabaseSchema.PseudoIndexMaintenanceTable(mailbox.Database).Table);
			LogicalIndexCache.DiscardCacheForMailbox(context, mailbox.SharedState);
		}

		public void SetEstimatedOldestMaintenanceRecord(Context context, long value)
		{
			this.estimatedOldestMaintenanceRecord = value;
		}

		public void SetEstimatedNewestMaintenanceRecord(Context context, long value)
		{
			this.estimatedNewestMaintenanceRecord = value;
		}

		public bool IsAnyIndexLockedInCache()
		{
			bool result;
			using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
			{
				result = (this.foldersLockedInCache != null);
			}
			return result;
		}

		public bool IsIndexLockedInCache(LogicalIndex logicalIndex)
		{
			bool result;
			using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
			{
				result = (this.foldersLockedInCache != null && this.foldersLockedInCache.ContainsKey(logicalIndex.FolderId));
			}
			return result;
		}

		public void LockIndexInCache(LogicalIndex logicalIndex)
		{
			using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
			{
				if (this.foldersLockedInCache == null)
				{
					this.foldersLockedInCache = new Dictionary<ExchangeId, KeyValuePair<LogicalIndexCache.FolderIndexCache, int>>();
				}
				int num = 1;
				KeyValuePair<LogicalIndexCache.FolderIndexCache, int> keyValuePair;
				if (this.foldersLockedInCache.TryGetValue(logicalIndex.FolderId, out keyValuePair))
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(object.ReferenceEquals(keyValuePair.Key, logicalIndex.FolderCache), "Replaced folder index cache object locked in cache?");
					num = keyValuePair.Value + 1;
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(num < 1000, "Run-away index locked in cache refcount");
				}
				this.foldersLockedInCache[logicalIndex.FolderId] = new KeyValuePair<LogicalIndexCache.FolderIndexCache, int>(logicalIndex.FolderCache, num);
				if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, ExchangeId, int>(0L, "Locking index {0} for folder {1} in cache, resulting refcount {2}", logicalIndex.LogicalIndexNumber, logicalIndex.FolderId, num);
				}
			}
		}

		public void UnlockIndexInCache(LogicalIndex logicalIndex)
		{
			using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
			{
				KeyValuePair<LogicalIndexCache.FolderIndexCache, int> keyValuePair;
				if (this.foldersLockedInCache != null && this.foldersLockedInCache.TryGetValue(logicalIndex.FolderId, out keyValuePair))
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(object.ReferenceEquals(keyValuePair.Key, logicalIndex.FolderCache), "Replaced folder index cache object locked in cache?");
					int num = keyValuePair.Value - 1;
					if (num > 0)
					{
						this.foldersLockedInCache[logicalIndex.FolderId] = new KeyValuePair<LogicalIndexCache.FolderIndexCache, int>(logicalIndex.FolderCache, num);
					}
					else if (this.foldersLockedInCache.Count == 1)
					{
						this.foldersLockedInCache = null;
					}
					else
					{
						this.foldersLockedInCache.Remove(logicalIndex.FolderId);
					}
					if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, ExchangeId, int>(0L, "Unlocking index {0} for folder {1} in cache, resulting refcount {2}", logicalIndex.LogicalIndexNumber, logicalIndex.FolderId, num);
					}
				}
			}
		}

		public static void MarkLogicalIndicesForCleanup(Context context, DatabaseInfo databaseInfo, out bool completed)
		{
			MaintenanceHandler.ApplyMaintenanceToActiveAndDeletedMailboxes(context, ExecutionDiagnostics.OperationSource.LogicalIndexCleanup, new Action<Context, MailboxState>(LogicalIndexCache.CheckMailboxForLogicalIndexCleanup), out completed);
		}

		public static void CleanupLogicalIndexes(Context context, MailboxState mailboxState, out bool completed)
		{
			LogicalIndexCache.CleanupLogicalIndexes(context, mailboxState, LogicalIndexCache.maxIdleCleanupPeriod, out completed);
		}

		public static void CleanupLogicalIndexes(Context context, MailboxState mailboxState, TimeSpan maxIdleCleanup, out bool completed)
		{
			completed = true;
			if (!mailboxState.IsAccessible)
			{
				return;
			}
			LogicalIndexCache cacheForMailboxDoNotCreate = LogicalIndexCache.GetCacheForMailboxDoNotCreate(context, mailboxState);
			if (cacheForMailboxDoNotCreate != null && cacheForMailboxDoNotCreate.IsAnyIndexLockedInCache())
			{
				completed = false;
				return;
			}
			using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
			{
				List<LogicalIndexCache.LogicalIndexInfo> listOfLogicalIndexes = LogicalIndexCache.GetListOfLogicalIndexes(context, mailboxState, ExchangeId.Zero);
				completed = LogicalIndexCache.CleanupLogicalIndexes(context, mailbox, listOfLogicalIndexes, maxIdleCleanup, true);
				context.Commit();
				if (cacheForMailboxDoNotCreate != null && completed)
				{
					long minFirstUpdateRecord = cacheForMailboxDoNotCreate.GetMinFirstUpdateRecord(context);
					cacheForMailboxDoNotCreate.CleanupMaintenance(context, minFirstUpdateRecord);
				}
				mailbox.Disconnect();
			}
		}

		public static DateTime GetMostRecentFolderViewAccessTime(Context context, MailboxState mailboxState, ExchangeId folderId)
		{
			DateTime dateTime = DateTime.MinValue;
			List<LogicalIndexCache.LogicalIndexInfo> listOfLogicalIndexes = LogicalIndexCache.GetListOfLogicalIndexes(context, mailboxState, folderId);
			foreach (LogicalIndexCache.LogicalIndexInfo logicalIndexInfo in listOfLogicalIndexes)
			{
				if (logicalIndexInfo.IndexType != LogicalIndexType.SearchFolderBaseView && dateTime < logicalIndexInfo.LastReferenceDate)
				{
					dateTime = logicalIndexInfo.LastReferenceDate;
				}
			}
			return dateTime;
		}

		public override bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			if (operationType == Connection.OperationType.CreateTable || operationType == Connection.OperationType.DeleteTable)
			{
				return this.TestExclusiveLock();
			}
			if (table.Equals(DatabaseSchema.PseudoIndexControlTable(context.Database).Table) || table.Equals(DatabaseSchema.PseudoIndexMaintenanceTable(context.Database).Table) || table.Equals(DatabaseSchema.PseudoIndexDefinitionTable(context.Database).Table))
			{
				if (operationType == Connection.OperationType.Query)
				{
					return this.TestSharedLock() || this.TestExclusiveLock();
				}
				if (operationType == Connection.OperationType.Insert)
				{
					return this.TestExclusiveLock();
				}
			}
			return false;
		}

		internal static LogicalIndexCache GetCacheForMailbox(Context context, Mailbox mailbox)
		{
			LogicalIndexCache logicalIndexCache = LogicalIndexCache.GetCacheForMailboxDoNotCreate(context, mailbox.SharedState);
			if (logicalIndexCache == null)
			{
				if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.PseudoIndexTracer.TraceDebug(0L, "Creating logical index cache for mailbox " + mailbox.SharedState.MailboxNumber);
				}
				logicalIndexCache = new LogicalIndexCache(context, mailbox.SharedState);
				LogicalIndexCache logicalIndexCache2 = (LogicalIndexCache)mailbox.SharedState.CompareExchangeComponentData(LogicalIndexCache.logicalIndexCacheDataSlot, null, logicalIndexCache);
				if (logicalIndexCache2 != null)
				{
					logicalIndexCache = logicalIndexCache2;
				}
			}
			return logicalIndexCache;
		}

		internal static LogicalIndexCache.FolderIndexCache GetCacheForFolderForTest(Context context, Mailbox mailbox, ExchangeId folderId)
		{
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			return cacheForMailbox.GetCacheForFolder(context, mailbox, folderId);
		}

		internal static IDisposable SetApplyMaintenanceParametersTestHook(LogicalIndexCache.ApplyMaintenanceSettings applyMaintenanceParameters)
		{
			return LogicalIndexCache.applyMaintenanceParameters.SetTestHook(applyMaintenanceParameters);
		}

		internal static void ApplyMaintenanceTable(Context context, MailboxState mailboxState, out bool completed)
		{
			completed = false;
			try
			{
				if (context.PerfInstance != null)
				{
					context.PerfInstance.NumberOfActiveWlmLogicalIndexMaintenanceTableMaintenances.Increment();
				}
				bool flag = true;
				int num = 0;
				while (mailboxState.IsUserAccessible)
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
					{
						LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
						if (cacheForMailbox.IsAnyIndexLockedInCache())
						{
							return;
						}
						cacheForMailbox.ApplyMaintenanceChunk(context, mailbox, out completed);
					}
					num++;
					if (LockManager.HasContention(mailboxState) && num >= LogicalIndexCache.ApplyMaintenanceParameters.WlmMinNumberOfChunksToProceed)
					{
						flag = false;
					}
					if (context.Database.HasExclusiveLockContention())
					{
						flag = false;
					}
					if (flag && !completed)
					{
						ErrorCode first = context.PulseMailboxOperation();
						flag = (first == ErrorCode.NoError);
					}
					if (!flag || completed || MaintenanceHandler.ShouldStopMailboxMaintenanceTask(context, mailboxState, LogicalIndexCache.ApplyingMaintenanceTableMaintenanceId))
					{
						return;
					}
				}
				completed = true;
			}
			finally
			{
				if (context.PerfInstance != null)
				{
					context.PerfInstance.NumberOfActiveWlmLogicalIndexMaintenanceTableMaintenances.Decrement();
					if (completed)
					{
						context.PerfInstance.NumberOfMailboxesMarkedForWlmLogicalIndexMaintenanceTableMaintenance.Decrement();
					}
				}
			}
		}

		internal void CheckMaintenanceSize(Context context)
		{
			long num = this.EstimatedNewestMaintenanceRecord - this.EstimatedOldestMaintenanceRecord + 1L;
			if (num > (long)LogicalIndexCache.ApplyMaintenanceParameters.WlmMaintenanceThreshold)
			{
				Mailbox mailbox = context.PrimaryMailboxContext as Mailbox;
				if (mailbox != null && mailbox.SharedState.IsMailboxLockedExclusively())
				{
					bool flag = LogicalIndexCache.applyingMaintenanceTableMaintenance.MarkForMaintenance(context, mailbox.SharedState);
					if (flag && context.PerfInstance != null)
					{
						context.PerfInstance.NumberOfMailboxesMarkedForWlmLogicalIndexMaintenanceTableMaintenance.Increment();
					}
				}
			}
		}

		internal void ApplyMaintenanceChunk(Context context, Mailbox mailbox, out bool completed)
		{
			completed = false;
			List<LogicalIndexCache.LogicalIndexInfo> listOfLogicalIndexes = LogicalIndexCache.GetListOfLogicalIndexes(context, mailbox.SharedState, ExchangeId.Zero);
			if (!LogicalIndexCache.CleanupLogicalIndexes(context, mailbox, listOfLogicalIndexes, LogicalIndexCache.maxIdleCleanupPeriod, false))
			{
				return;
			}
			long minFirstUpdateRecord = this.GetMinFirstUpdateRecord(context);
			long num = this.EstimatedNewestMaintenanceRecord - minFirstUpdateRecord + 1L;
			if (num <= (long)LogicalIndexCache.ApplyMaintenanceParameters.StopMaintenanceThreshold)
			{
				this.CleanupMaintenance(context, minFirstUpdateRecord);
				completed = true;
				return;
			}
			Dictionary<int, LogicalIndexCache.LogicalIndexInfo> indexNumberToIndexInfo = new Dictionary<int, LogicalIndexCache.LogicalIndexInfo>(100);
			foreach (LogicalIndexCache.LogicalIndexInfo value in listOfLogicalIndexes)
			{
				indexNumberToIndexInfo.Add(value.LogicalIndexNumber, value);
			}
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			long num4 = minFirstUpdateRecord - 1L;
			Dictionary<int, Queue<LogicalIndex.MaintRecord>> dictionary = new Dictionary<int, Queue<LogicalIndex.MaintRecord>>(100);
			Queue<LogicalIndex.MaintRecord> queue = new Queue<LogicalIndex.MaintRecord>(LogicalIndexCache.ApplyMaintenanceParameters.NumberOfRecordsToMaintain);
			while (!flag)
			{
				LogicalIndex.ReadMaintenanceTable(context, this.MailboxPartitionNumber, null, num4 + 1L, queue, LogicalIndexCache.ApplyMaintenanceParameters.NumberOfRecordsToMaintain);
				if (queue.Count < LogicalIndexCache.ApplyMaintenanceParameters.NumberOfRecordsToMaintain)
				{
					flag = true;
				}
				num3 += queue.Count;
				if (num3 >= LogicalIndexCache.ApplyMaintenanceParameters.NumberOfRecordsToReadFromMaintenanceTable)
				{
					flag = true;
				}
				while (queue.Count > 0)
				{
					LogicalIndex.MaintRecord item = queue.Dequeue();
					num4 = item.UpdateRecordId;
					LogicalIndexCache.LogicalIndexInfo logicalIndexInfo;
					if (indexNumberToIndexInfo.TryGetValue(item.LogicalIndexNumber, out logicalIndexInfo) && logicalIndexInfo.FirstUpdateRecord <= item.UpdateRecordId && logicalIndexInfo.FirstUpdateRecord != -1L)
					{
						Queue<LogicalIndex.MaintRecord> queue2;
						if (!dictionary.TryGetValue(item.LogicalIndexNumber, out queue2))
						{
							queue2 = new Queue<LogicalIndex.MaintRecord>(32);
							dictionary.Add(item.LogicalIndexNumber, queue2);
						}
						queue2.Enqueue(item);
						num2++;
					}
				}
				if (num2 >= LogicalIndexCache.ApplyMaintenanceParameters.NumberOfRecordsToMaintain)
				{
					flag = true;
				}
			}
			List<LogicalIndexCache.FolderIdAndIndexNumber> list = (from indexNumber in dictionary.Keys
			select new LogicalIndexCache.FolderIdAndIndexNumber(indexNumberToIndexInfo[indexNumber].FolderId, indexNumber)).ToList<LogicalIndexCache.FolderIdAndIndexNumber>();
			list.Sort((LogicalIndexCache.FolderIdAndIndexNumber e1, LogicalIndexCache.FolderIdAndIndexNumber e2) => e1.FolderId.CompareTo(e2.FolderId));
			foreach (LogicalIndexCache.FolderIdAndIndexNumber folderIdAndIndexNumber in list)
			{
				queue = dictionary[folderIdAndIndexNumber.IndexNumber];
				LogicalIndex logicalIndex = LogicalIndexCache.GetLogicalIndex(context, mailbox, folderIdAndIndexNumber.FolderId, folderIdAndIndexNumber.IndexNumber);
				if (logicalIndex != null)
				{
					do
					{
						logicalIndex.ApplyMaintenanceChunk(context, queue, 512, true);
					}
					while (queue.Count != 0 && !logicalIndex.IsStale);
				}
			}
			PseudoIndexControlTable pseudoIndexControlTable = DatabaseSchema.PseudoIndexControlTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				this.MailboxPartitionNumber
			});
			SearchCriteriaAnd restriction = Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
			{
				Factory.CreateSearchCriteriaCompare(pseudoIndexControlTable.FirstUpdateRecord, SearchCriteriaCompare.SearchRelOp.GreaterThanEqual, Factory.CreateConstantColumn(this.EstimatedOldestMaintenanceRecord)),
				Factory.CreateSearchCriteriaCompare(pseudoIndexControlTable.FirstUpdateRecord, SearchCriteriaCompare.SearchRelOp.LessThanEqual, Factory.CreateConstantColumn(num4))
			});
			using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, pseudoIndexControlTable.Table, pseudoIndexControlTable.PseudoIndexControlPK, null, restriction, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, false), new Column[]
			{
				pseudoIndexControlTable.FirstUpdateRecord
			}, new object[]
			{
				num4 + 1L
			}, false))
			{
				updateOperator.ExecuteScalar();
			}
			LogicalIndexCache.DiscardCacheForMailbox(context, mailbox.SharedState);
			context.Commit();
			this.CleanupMaintenance(context, num4 + 1L);
		}

		private static void CheckMailboxForLogicalIndexCleanup(Context context, MailboxState mailboxState)
		{
			List<LogicalIndexCache.LogicalIndexInfo> listOfLogicalIndexes = LogicalIndexCache.GetListOfLogicalIndexes(context, mailboxState, ExchangeId.Zero);
			if (listOfLogicalIndexes.Count > 0)
			{
				bool flag = false;
				if (mailboxState.IsUserAccessible)
				{
					DateTime utcNow = DateTime.UtcNow;
					using (List<LogicalIndexCache.LogicalIndexInfo>.Enumerator enumerator = listOfLogicalIndexes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							LogicalIndexCache.LogicalIndexInfo logicalIndexInfo = enumerator.Current;
							if (utcNow - logicalIndexInfo.LastReferenceDate >= LogicalIndexCache.maxIdleCleanupPeriod)
							{
								flag = true;
								break;
							}
						}
						goto IL_73;
					}
				}
				flag = true;
				IL_73:
				if (LogicalIndexCache.markMailboxForLogicalIndexCleanupTestHook.Value != null)
				{
					flag = LogicalIndexCache.markMailboxForLogicalIndexCleanupTestHook.Value(mailboxState, flag);
				}
				if (flag)
				{
					mailboxState.AddReference();
					try
					{
						LogicalIndexCache.logicalIndexCleanupMaintenance.MarkForMaintenance(context, mailboxState);
					}
					finally
					{
						mailboxState.ReleaseReference();
					}
				}
			}
		}

		private static LogicalIndex FindIndexInternal(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexCache.FolderIndexCache folderCache, LogicalIndexType indexType, int indexSignature)
		{
			LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			return folderCache.FindIndex(indexType, indexSignature);
		}

		private static bool CleanupLogicalIndexes(Context context, Mailbox mailbox, List<LogicalIndexCache.LogicalIndexInfo> listOfIndexInfos, TimeSpan maxIdleCleanup, bool permitBringNonDeletableIndicesUpToDate)
		{
			bool result = true;
			int num = 0;
			DateTime utcNow = DateTime.UtcNow;
			bool flag = !mailbox.SharedState.IsUserAccessible;
			if (LogicalIndexCache.forceMailboxLogicalIndexCleanupTestHook.Value != null)
			{
				flag = LogicalIndexCache.forceMailboxLogicalIndexCleanupTestHook.Value(mailbox.SharedState, flag);
			}
			LogicalIndexCache cacheForMailbox = LogicalIndexCache.GetCacheForMailbox(context, mailbox);
			foreach (LogicalIndexCache.LogicalIndexInfo logicalIndexInfo in listOfIndexInfos)
			{
				if (flag || utcNow - logicalIndexInfo.LastReferenceDate >= maxIdleCleanup)
				{
					LogicalIndexCache.FolderIndexCache cacheForFolder = cacheForMailbox.GetCacheForFolder(context, mailbox, logicalIndexInfo.FolderId);
					LogicalIndex logicalIndex = cacheForFolder.GetLogicalIndex(logicalIndexInfo.LogicalIndexNumber);
					if (logicalIndex != null)
					{
						if (logicalIndex.IndexType == LogicalIndexType.SearchFolderBaseView || logicalIndex.IndexType == LogicalIndexType.ConversationDeleteHistory)
						{
							if (!permitBringNonDeletableIndicesUpToDate)
							{
								continue;
							}
							if (!logicalIndex.IsStale)
							{
								logicalIndex.ApplyMaintenanceToIndex(context, false, true, 2147483647L);
							}
						}
						else
						{
							if (LogicalIndexCache.cleanupOneLogicalIndexTestHook.Value != null)
							{
								LogicalIndexCache.cleanupOneLogicalIndexTestHook.Value(mailbox.SharedState, logicalIndex, flag);
							}
							cacheForFolder.DeleteIndex(context, logicalIndex.LogicalIndexNumber);
						}
						num++;
						if (num % LogicalIndexCache.logicalIndexCleanupChunkSize.Value == 0)
						{
							context.Commit();
							if (MaintenanceHandler.ShouldStopMailboxMaintenanceTask(context, mailbox.SharedState, LogicalIndexCache.LogicalIndexCleanupMaintenanceId))
							{
								result = false;
								break;
							}
						}
					}
				}
			}
			return result;
		}

		private static LogicalIndexCache GetCacheForMailboxDoNotCreate(Context context, MailboxState mailboxState)
		{
			return mailboxState.GetComponentData(LogicalIndexCache.logicalIndexCacheDataSlot) as LogicalIndexCache;
		}

		internal LogicalIndexCache.FolderIndexCache GetCacheForFolder(Context context, Mailbox mailbox, ExchangeId folderId)
		{
			LogicalIndexCache.FolderIndexCache folderIndexCache;
			using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
			{
				folderIndexCache = this.folderIdToFolderIndexCache.Find(folderId, false);
				KeyValuePair<LogicalIndexCache.FolderIndexCache, int> keyValuePair;
				if (folderIndexCache == null && this.foldersLockedInCache != null && this.foldersLockedInCache.TryGetValue(folderId, out keyValuePair))
				{
					folderIndexCache = keyValuePair.Key;
					this.folderIdToFolderIndexCache.Insert(folderId, folderIndexCache, false);
				}
			}
			if (folderIndexCache == null)
			{
				using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this))
				{
					using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
					{
						folderIndexCache = this.folderIdToFolderIndexCache.Find(folderId, false);
					}
					if (folderIndexCache == null)
					{
						folderIndexCache = new LogicalIndexCache.FolderIndexCache(context, this, mailbox, folderId);
						using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
						{
							this.folderIdToFolderIndexCache.Insert(folderId, folderIndexCache, false);
						}
					}
					mailboxComponentOperationFrame.Success();
				}
			}
			return folderIndexCache;
		}

		internal LogicalIndexCache.FolderIndexCache GetCacheForFolderDoNotLoad(ExchangeId folderId)
		{
			LogicalIndexCache.FolderIndexCache result;
			using (LockManager.Lock(this.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
			{
				LogicalIndexCache.FolderIndexCache folderIndexCache = this.folderIdToFolderIndexCache.Find(folderId, false);
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(folderIndexCache != null, "folder cache must be loaded");
				result = folderIndexCache;
			}
			return result;
		}

		[Conditional("DEBUG")]
		private static void ValidateNoCulturallySignificantColumns(SortOrder sortOrder)
		{
			for (int i = 0; i < sortOrder.Count; i++)
			{
			}
		}

		[Conditional("DEBUG")]
		private static void ValidateSingleMVProp(SortOrder sortOrder)
		{
			int num = 0;
			for (int i = 0; i < sortOrder.Count; i++)
			{
				PropertyColumn propertyColumn = sortOrder[i].Column as PropertyColumn;
				if (propertyColumn != null && propertyColumn.StorePropTag.IsMultiValueInstance)
				{
					num++;
				}
			}
		}

		private static List<IIndex> GetIndexesInScopeDoNotCreate(Context context, Mailbox mailbox, ExchangeId folderId, LogicalIndexCache.FolderIndexCache folderCache, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table, bool matchingOnly, out bool foundMatchingIndex)
		{
			foundMatchingIndex = false;
			List<IIndex> list = null;
			LogicalIndex logicalIndex = null;
			bool flag = false;
			foreach (LogicalIndex logicalIndex2 in folderCache.Values)
			{
				if (logicalIndex2.IndexInScope(context, folderId, indexType, conditionalIndexColumn, conditionalIndexValue) && logicalIndex2.CompatibleMvExplosionColumn(context, indexType, sortOrder) && (CultureHelper.GetLcidFromCulture(logicalIndex2.GetCulture()) == CultureHelper.GetLcidFromCulture(context.Culture) || !logicalIndex2.IsCultureSensitive()))
				{
					if (logicalIndex2.CanUseIndex(context, folderId, indexType, sortOrder, nonKeyColumns, conditionalIndexColumn, conditionalIndexValue, categorizationInfo))
					{
						foundMatchingIndex = true;
						if (logicalIndex2.IsStale)
						{
							if (!flag && (logicalIndex == null || logicalIndex.LogicalIndexNumber < logicalIndex2.LogicalIndexNumber))
							{
								logicalIndex = logicalIndex2;
							}
						}
						else if (!flag || logicalIndex.LogicalIndexNumber < logicalIndex2.LogicalIndexNumber)
						{
							flag = true;
							logicalIndex = logicalIndex2;
						}
					}
					else if (!matchingOnly && !logicalIndex2.IsStale)
					{
						if (list == null)
						{
							list = new List<IIndex>(Math.Min(10, folderCache.Count));
						}
						list.Add(logicalIndex2);
					}
				}
			}
			if (logicalIndex != null)
			{
				if (list == null)
				{
					list = new List<IIndex>(1);
				}
				list.Insert(0, logicalIndex);
			}
			return list;
		}

		private static List<LogicalIndexCache.LogicalIndexInfo> GetListOfLogicalIndexes(Context context, MailboxState mailboxState, ExchangeId folderId)
		{
			PseudoIndexControlTable pseudoIndexControlTable = DatabaseSchema.PseudoIndexControlTable(context.Database);
			StartStopKey startStopKey;
			if (folderId.IsValid)
			{
				startStopKey = new StartStopKey(true, new object[]
				{
					mailboxState.MailboxPartitionNumber,
					folderId.To26ByteArray()
				});
			}
			else
			{
				startStopKey = new StartStopKey(true, new object[]
				{
					mailboxState.MailboxPartitionNumber
				});
			}
			List<LogicalIndexCache.LogicalIndexInfo> result;
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, pseudoIndexControlTable.Table, pseudoIndexControlTable.PseudoIndexControlPK, new Column[]
			{
				pseudoIndexControlTable.FolderId,
				pseudoIndexControlTable.LogicalIndexNumber,
				pseudoIndexControlTable.LastReferenceDate,
				pseudoIndexControlTable.FirstUpdateRecord,
				pseudoIndexControlTable.IndexType
			}, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, false))
			{
				List<LogicalIndexCache.LogicalIndexInfo> list = new List<LogicalIndexCache.LogicalIndexInfo>(100);
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						byte[] binary = reader.GetBinary(pseudoIndexControlTable.FolderId);
						int @int = reader.GetInt32(pseudoIndexControlTable.LogicalIndexNumber);
						DateTime dateTime = reader.GetDateTime(pseudoIndexControlTable.LastReferenceDate);
						long int2 = reader.GetInt64(pseudoIndexControlTable.FirstUpdateRecord);
						LogicalIndexType int3 = (LogicalIndexType)reader.GetInt32(pseudoIndexControlTable.IndexType);
						list.Add(new LogicalIndexCache.LogicalIndexInfo(@int, ExchangeId.CreateFrom26ByteArray(null, null, binary), dateTime, int2, int3));
					}
				}
				result = list;
			}
			return result;
		}

		private void InitializeCache(Context context)
		{
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this))
			{
				this.RefreshMaintenanceCache(context, true, true);
				mailboxComponentOperationFrame.Success();
			}
		}

		private long GetMinFirstUpdateRecord(Context context)
		{
			long num = this.EstimatedNewestMaintenanceRecord + 1L;
			PseudoIndexControlTable pseudoIndexControlTable = DatabaseSchema.PseudoIndexControlTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				this.MailboxPartitionNumber
			});
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, pseudoIndexControlTable.Table, pseudoIndexControlTable.PseudoIndexControlPK, new Column[]
			{
				pseudoIndexControlTable.FirstUpdateRecord
			}, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, false))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						long @int = reader.GetInt64(pseudoIndexControlTable.FirstUpdateRecord);
						if (@int != -1L)
						{
							num = Math.Min(num, @int);
						}
					}
				}
			}
			return num;
		}

		private void CleanupMaintenance(Context context, long minFirstUpdateRecord)
		{
			DateTime utcNow = DateTime.UtcNow;
			double num = (utcNow > this.mailboxCreationTime) ? (utcNow - this.mailboxCreationTime).TotalDays : 0.0;
			long num2 = (long)((double)this.EstimatedNewestMaintenanceRecord / (num + 1.0));
			long num3 = (long)((double)num2 * LogicalIndexCache.ApplyMaintenanceParameters.MaintenanceTimePeriodToKeep.TotalDays);
			long num4 = Math.Min(this.EstimatedNewestMaintenanceRecord - num3, minFirstUpdateRecord);
			if (num4 > this.EstimatedOldestMaintenanceRecord)
			{
				PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable = DatabaseSchema.PseudoIndexMaintenanceTable(context.Database);
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					this.MailboxPartitionNumber
				});
				bool flag;
				do
				{
					flag = false;
					StartStopKey startKey = startStopKey;
					StartStopKey stopKey = new StartStopKey(false, new object[]
					{
						this.MailboxPartitionNumber,
						num4
					});
					using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, pseudoIndexMaintenanceTable.Table, pseudoIndexMaintenanceTable.Table.PrimaryKeyIndex, new PhysicalColumn[]
					{
						pseudoIndexMaintenanceTable.UpdateRecordNumber
					}, null, null, LogicalIndexCache.maxMaintenanceRowsToDeleteAtATime, 1, new KeyRange(startKey, stopKey), false, false))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							if (reader.Read())
							{
								flag = true;
								long @int = reader.GetInt64(pseudoIndexMaintenanceTable.UpdateRecordNumber);
								stopKey = new StartStopKey(false, new object[]
								{
									this.MailboxPartitionNumber,
									@int
								});
								startStopKey = new StartStopKey(true, new object[]
								{
									this.MailboxPartitionNumber,
									@int
								});
							}
						}
					}
					using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, pseudoIndexMaintenanceTable.Table, pseudoIndexMaintenanceTable.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startKey, stopKey), false, false), false))
					{
						deleteOperator.ExecuteScalar();
					}
					context.Commit();
				}
				while (flag);
				this.RefreshMaintenanceCache(context, true, true);
			}
		}

		private void RefreshMaintenanceCache(Context context, bool min, bool max)
		{
			PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable = DatabaseSchema.PseudoIndexMaintenanceTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				this.MailboxPartitionNumber
			});
			if (min)
			{
				long value = 0L;
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, pseudoIndexMaintenanceTable.Table, pseudoIndexMaintenanceTable.Table.PrimaryKeyIndex, new Column[]
				{
					pseudoIndexMaintenanceTable.UpdateRecordNumber
				}, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, false))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (reader.Read())
						{
							value = reader.GetInt64(pseudoIndexMaintenanceTable.UpdateRecordNumber);
						}
					}
				}
				this.SetEstimatedOldestMaintenanceRecord(context, value);
			}
			if (max)
			{
				long value2 = 0L;
				using (TableOperator tableOperator2 = Factory.CreateTableOperator(context.Culture, context, pseudoIndexMaintenanceTable.Table, pseudoIndexMaintenanceTable.Table.PrimaryKeyIndex, new Column[]
				{
					pseudoIndexMaintenanceTable.UpdateRecordNumber
				}, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), true, false))
				{
					using (Reader reader2 = tableOperator2.ExecuteReader(false))
					{
						if (reader2.Read())
						{
							value2 = reader2.GetInt64(pseudoIndexMaintenanceTable.UpdateRecordNumber);
						}
					}
				}
				this.SetEstimatedNewestMaintenanceRecord(context, value2);
			}
		}

		private void OnMailboxDisconnect(Context context, Mailbox mailbox)
		{
			if (!mailbox.SharedState.IsMailboxLockedExclusively())
			{
				return;
			}
			this.folderIdToFolderIndexCache.EvictionCheckpoint();
		}

		bool IComponentData.DoCleanup(Context context)
		{
			if (this.IsAnyIndexLockedInCache())
			{
				this.folderIdToFolderIndexCache.Reset();
				return false;
			}
			return true;
		}

		private const int AvgLogicalIndexPerMailbox = 100;

		private const int DefaultLogicalIndexCleanupChunkSize = 25;

		public static readonly Guid LogicalIndexCleanupMaintenanceId = new Guid("{818429a5-c7c8-4546-8cad-c71efaf3c219}");

		public static readonly Guid MarkLogicalIndexForCleanupMaintenanceId = new Guid("{8dda68d9-e1c4-4b97-a884-bf0ab208cf5c}");

		public static readonly Guid ApplyingMaintenanceTableMaintenanceId = new Guid("{f4946920-3356-4f2d-bfb0-e72f14af6f56}");

		internal static int NumberOfCachedFoldersPerMailbox = 24;

		internal static TimeSpan TimeToLive = TimeSpan.FromMinutes(15.0);

		private static TimeSpan maxIdleCleanupPeriod;

		private static int maxMaintenanceRowsToDeleteAtATime = 500;

		private static IMailboxMaintenance applyingMaintenanceTableMaintenance;

		private static IMailboxMaintenance logicalIndexCleanupMaintenance;

		private static IDatabaseMaintenance markLogicalIndexForCleanupMaintenance;

		private static int logicalIndexCacheDataSlot = -1;

		private static ICachePerformanceCounters folderCachePerfCounters;

		private static Hookable<LogicalIndexCache.ApplyMaintenanceSettings> applyMaintenanceParameters;

		private static Hookable<int> logicalIndexCleanupChunkSize = Hookable<int>.Create(true, 25);

		private static readonly Hookable<Func<MailboxState, bool, bool>> markMailboxForLogicalIndexCleanupTestHook = Hookable<Func<MailboxState, bool, bool>>.Create(true, null);

		private static readonly Hookable<Func<MailboxState, bool, bool>> forceMailboxLogicalIndexCleanupTestHook = Hookable<Func<MailboxState, bool, bool>>.Create(true, null);

		private static readonly Hookable<Action<MailboxState, LogicalIndex, bool>> cleanupOneLogicalIndexTestHook = Hookable<Action<MailboxState, LogicalIndex, bool>>.Create(true, null);

		private readonly bool updateIndexDirectly;

		private readonly DateTime mailboxCreationTime;

		private MailboxLockNameBase mailboxLockName;

		private SingleKeyCache<ExchangeId, LogicalIndexCache.FolderIndexCache> folderIdToFolderIndexCache;

		private Dictionary<ExchangeId, KeyValuePair<LogicalIndexCache.FolderIndexCache, int>> foldersLockedInCache;

		private long estimatedOldestMaintenanceRecord;

		private long estimatedNewestMaintenanceRecord;

		private struct DatabaseAndMailboxNumber
		{
			internal DatabaseAndMailboxNumber(StoreDatabase database, int mailboxNumber)
			{
				this.database = database;
				this.mailboxNumber = mailboxNumber;
			}

			internal StoreDatabase Database
			{
				get
				{
					return this.database;
				}
			}

			internal int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			private readonly StoreDatabase database;

			private readonly int mailboxNumber;
		}

		private struct FolderIdAndIndexNumber
		{
			internal FolderIdAndIndexNumber(ExchangeId folderId, int indexNumber)
			{
				this.folderId = folderId;
				this.indexNumber = indexNumber;
			}

			internal ExchangeId FolderId
			{
				get
				{
					return this.folderId;
				}
			}

			internal int IndexNumber
			{
				get
				{
					return this.indexNumber;
				}
			}

			private readonly ExchangeId folderId;

			private readonly int indexNumber;
		}

		private struct LogicalIndexInfo
		{
			internal LogicalIndexInfo(int logicalIndexNumber, ExchangeId folderId, DateTime lastReferenceDate, long firstUpdateRecord, LogicalIndexType indexType)
			{
				this.logicalIndexNumber = logicalIndexNumber;
				this.folderId = folderId;
				this.lastReferenceDate = lastReferenceDate;
				this.firstUpdateRecord = firstUpdateRecord;
				this.indexType = indexType;
			}

			internal int LogicalIndexNumber
			{
				get
				{
					return this.logicalIndexNumber;
				}
			}

			internal ExchangeId FolderId
			{
				get
				{
					return this.folderId;
				}
			}

			internal DateTime LastReferenceDate
			{
				get
				{
					return this.lastReferenceDate;
				}
			}

			internal long FirstUpdateRecord
			{
				get
				{
					return this.firstUpdateRecord;
				}
			}

			internal LogicalIndexType IndexType
			{
				get
				{
					return this.indexType;
				}
			}

			private int logicalIndexNumber;

			private ExchangeId folderId;

			private DateTime lastReferenceDate;

			private long firstUpdateRecord;

			private LogicalIndexType indexType;
		}

		internal class FolderIndexCache : Dictionary<int, LogicalIndex>, IStateObject
		{
			internal FolderIndexCache(Context context, LogicalIndexCache logicalIndexCache, Mailbox mailbox, ExchangeId folderId)
			{
				this.logicalIndexCache = logicalIndexCache;
				this.folderId = folderId;
				this.LoadFolderCache(context, mailbox);
			}

			internal LogicalIndexCache LogicalIndexCache
			{
				get
				{
					return this.logicalIndexCache;
				}
			}

			internal ExchangeId FolderId
			{
				get
				{
					return this.folderId;
				}
			}

			void IStateObject.OnBeforeCommit(Context context)
			{
			}

			void IStateObject.OnCommit(Context context)
			{
			}

			void IStateObject.OnAbort(Context context)
			{
				using (LockManager.Lock(this.logicalIndexCache.folderIdToFolderIndexCache, LockManager.LockType.LeafMonitorLock))
				{
					this.logicalIndexCache.folderIdToFolderIndexCache.Remove(this.folderId, false);
				}
			}

			internal LogicalIndex GetLogicalIndex(int logicalIndexNumber)
			{
				LogicalIndex result;
				if (!base.TryGetValue(logicalIndexNumber, out result))
				{
					return null;
				}
				return result;
			}

			internal LogicalIndex FindIndex(LogicalIndexType indexType, int indexSignature)
			{
				foreach (LogicalIndex logicalIndex in base.Values)
				{
					if (logicalIndex.IndexType == indexType && logicalIndex.IndexSignature == indexSignature)
					{
						return logicalIndex;
					}
				}
				return null;
			}

			internal LogicalIndex CreateIndex(Context context, MailboxState mailboxState, LogicalIndexType indexType, int indexSignature, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table, bool markCurrent)
			{
				LogicalIndex logicalIndex = LogicalIndex.CreateIndex(context, mailboxState, this, this.folderId, indexType, indexSignature, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, markCurrent);
				base[logicalIndex.LogicalIndexNumber] = logicalIndex;
				return logicalIndex;
			}

			public LogicalIndex GetIndexToUseForPopulation(Context context, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, int logicalIndexNumber)
			{
				LogicalIndex result;
				using (context.MailboxComponentReadOperation(this.logicalIndexCache))
				{
					foreach (LogicalIndex logicalIndex in base.Values)
					{
						if (logicalIndex.LogicalIndexNumber != logicalIndexNumber && logicalIndex.CanUseIndexForPopulation(context, this.folderId, indexType, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns))
						{
							return logicalIndex;
						}
					}
					result = null;
				}
				return result;
			}

			internal void ConsolidateIndexes(Context context, LogicalIndexType indexType, int indexSignature, Column conditionalIndexColumn, bool conditionalIndexValue, ref SortOrder sortOrder, ref IList<Column> nonKeyColumns, List<IIndex> indexList)
			{
				bool flag = !context.TransactionStarted;
				LogicalIndex logicalIndex;
				do
				{
					logicalIndex = null;
					foreach (LogicalIndex logicalIndex2 in base.Values)
					{
						bool flag2;
						if (!logicalIndex2.IsStale && !logicalIndex2.IsInvalidatePending && !this.LogicalIndexCache.IsIndexLockedInCache(logicalIndex2) && logicalIndex2.IndexInScope(context, this.folderId, indexType, conditionalIndexColumn, conditionalIndexValue) && (SortOrder.IsMatch(sortOrder, logicalIndex2.LogicalSortOrder, logicalIndex2.ConstantColumns, out flag2) || SortOrder.IsMatch(logicalIndex2.LogicalSortOrder, sortOrder, logicalIndex2.ConstantColumns, out flag2)) && (CultureHelper.GetLcidFromCulture(logicalIndex2.GetCulture()) == CultureHelper.GetLcidFromCulture(context.Culture) || !logicalIndex2.IsCultureSensitive()))
						{
							logicalIndex = logicalIndex2;
							break;
						}
					}
					if (logicalIndex != null)
					{
						bool flag2;
						if (!SortOrder.IsMatch(logicalIndex.LogicalSortOrder, sortOrder, logicalIndex.ConstantColumns, out flag2))
						{
							sortOrder = logicalIndex.LogicalSortOrder;
						}
						if (nonKeyColumns != null && logicalIndex.NonKeyColumns != null)
						{
							List<Column> list = new List<Column>(logicalIndex.NonKeyColumns.Count + 4);
							for (int i = 0; i < logicalIndex.NonKeyColumns.Count; i++)
							{
								Column column = logicalIndex.NonKeyColumns[i];
								if (column.MaxLength > PhysicalIndex.MaxSortColumnLength(column.Type) || !sortOrder.Contains(column))
								{
									list.Add(column);
								}
							}
							for (int j = 0; j < nonKeyColumns.Count; j++)
							{
								Column column2 = nonKeyColumns[j];
								if (!list.Contains(column2) && (column2.MaxLength > PhysicalIndex.MaxSortColumnLength(column2.Type) || !sortOrder.Contains(column2)))
								{
									list.Add(column2);
								}
							}
							nonKeyColumns = list.ToArray();
						}
						else if (logicalIndex.NonKeyColumns != null)
						{
							nonKeyColumns = logicalIndex.NonKeyColumns;
						}
						if (indexList != null)
						{
							indexList.Remove(logicalIndex);
						}
						logicalIndex.InvalidateIndex(context, true);
					}
				}
				while (logicalIndex != null);
				if (flag)
				{
					context.Commit();
				}
			}

			internal void DeleteIndex(Context context, int logicalIndexNumber)
			{
				this.DeleteIndexImpl(logicalIndexNumber, delegate(LogicalIndex logicalIndex)
				{
					logicalIndex.DeleteIndex(context);
				});
			}

			internal void DeleteIndexNoLock(Context context, int logicalIndexNumber)
			{
				this.DeleteIndexImpl(logicalIndexNumber, delegate(LogicalIndex logicalIndex)
				{
					logicalIndex.DeleteIndexNoLock(context);
				});
			}

			private void DeleteIndexImpl(int logicalIndexNumber, Action<LogicalIndex> indexDeletionAction)
			{
				LogicalIndex logicalIndex;
				if (base.TryGetValue(logicalIndexNumber, out logicalIndex))
				{
					indexDeletionAction(logicalIndex);
					base.Remove(logicalIndex.LogicalIndexNumber);
				}
			}

			private void LoadFolderCache(Context context, Mailbox mailbox)
			{
				List<LogicalIndexCache.LogicalIndexInfo> listOfLogicalIndexes = LogicalIndexCache.GetListOfLogicalIndexes(context, mailbox.SharedState, this.folderId);
				foreach (LogicalIndexCache.LogicalIndexInfo logicalIndexInfo in listOfLogicalIndexes)
				{
					LogicalIndex logicalIndex = LogicalIndex.LoadIndex(context, this, logicalIndexInfo.FolderId, logicalIndexInfo.LogicalIndexNumber, mailbox);
					base.Add(logicalIndex.LogicalIndexNumber, logicalIndex);
				}
				foreach (LogicalIndexCache.LogicalIndexInfo logicalIndexInfo2 in listOfLogicalIndexes)
				{
					LogicalIndex logicalIndex2;
					if (base.TryGetValue(logicalIndexInfo2.LogicalIndexNumber, out logicalIndex2) && !logicalIndex2.IsCompatibleWithCurrentSchema(context))
					{
						if (!context.IsStateObjectRegistered(this))
						{
							context.RegisterStateObject(this);
						}
						this.DeleteIndex(context, logicalIndex2.LogicalIndexNumber);
					}
				}
			}

			private readonly LogicalIndexCache logicalIndexCache;

			private readonly ExchangeId folderId;
		}

		internal class ApplyMaintenanceSettings
		{
			internal int StopMaintenanceThreshold
			{
				get
				{
					return this.stopMaintenanceThreshold;
				}
				set
				{
					this.stopMaintenanceThreshold = value;
				}
			}

			internal int WlmMaintenanceThreshold
			{
				get
				{
					return this.wlmMaintenanceThreshold;
				}
				set
				{
					this.wlmMaintenanceThreshold = value;
				}
			}

			internal int NumberOfRecordsToMaintain
			{
				get
				{
					return this.numberOfRecordsToMaintain;
				}
				set
				{
					this.numberOfRecordsToMaintain = value;
				}
			}

			internal int NumberOfRecordsToReadFromMaintenanceTable
			{
				get
				{
					return this.numberOfRecordsToReadFromMaintenanceTable;
				}
				set
				{
					this.numberOfRecordsToReadFromMaintenanceTable = value;
				}
			}

			internal TimeSpan MaintenanceTimePeriodToKeep
			{
				get
				{
					return this.maintenanceTimePeriodToKeep;
				}
				set
				{
					this.maintenanceTimePeriodToKeep = value;
				}
			}

			internal int WlmMinNumberOfChunksToProceed
			{
				get
				{
					return this.wlmMinNumberOfChunksToProceed;
				}
				set
				{
					this.wlmMinNumberOfChunksToProceed = value;
				}
			}

			private int stopMaintenanceThreshold;

			private int wlmMaintenanceThreshold;

			private int numberOfRecordsToMaintain;

			private int numberOfRecordsToReadFromMaintenanceTable;

			private TimeSpan maintenanceTimePeriodToKeep;

			private int wlmMinNumberOfChunksToProceed;
		}
	}
}
