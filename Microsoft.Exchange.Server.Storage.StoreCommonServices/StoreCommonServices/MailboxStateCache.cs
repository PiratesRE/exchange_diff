using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxStateCache : IStoreDatabaseQueryTarget<MailboxStateCache.QueryableMailboxState>, IStoreQueryTargetBase<MailboxStateCache.QueryableMailboxState>
	{
		private MailboxStateCache(StoreDatabase database)
		{
			this.database = database;
			this.mailboxNumberDictionary = new Dictionary<int, MailboxState>(500);
			this.mailboxGuidDictionary = new Dictionary<Guid, MailboxState>(500);
			this.unifiedMailboxGuidDictionary = new Dictionary<Guid, MailboxState.UnifiedMailboxState>(100);
			this.deletedMailboxes = new HashSet<int>();
			this.postMailboxDisposeList = new List<MailboxState>(500);
			this.perfInstance = StorePerDatabasePerformanceCounters.GetInstance(database.MdbName);
			this.activeMailboxesEvictionPolicy = new LRU2WithTimeToLiveExpirationPolicy<int>(MailboxStateCache.expectedNumberOfActiveMailboxes.Value, MailboxStateCache.activeMailboxCacheTimeToLive, false);
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		private Dictionary<int, MailboxState> MailboxNumberDictionary
		{
			get
			{
				return this.mailboxNumberDictionary;
			}
		}

		private Dictionary<Guid, MailboxState> MailboxGuidDictionary
		{
			get
			{
				return this.mailboxGuidDictionary;
			}
		}

		private Dictionary<Guid, MailboxState.UnifiedMailboxState> UnifiedMailboxGuidDictionary
		{
			get
			{
				return this.unifiedMailboxGuidDictionary;
			}
		}

		private object LockObject
		{
			get
			{
				return this.lockObject;
			}
		}

		public static List<int> GetActiveMailboxNumbers(Context context)
		{
			MailboxStateCache mailboxStateCache = (MailboxStateCache)context.Database.ComponentData[MailboxStateCache.cacheSlot];
			if (mailboxStateCache != null)
			{
				List<int> list = new List<int>(1000);
				using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
				{
					foreach (KeyValuePair<int, MailboxState> keyValuePair in mailboxStateCache.mailboxNumberDictionary)
					{
						if (keyValuePair.Value.CurrentlyActive)
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
				return list;
			}
			return null;
		}

		public static MailboxState Get(Context context, int mailboxNumber)
		{
			MailboxStateCache mailboxStateCache = (MailboxStateCache)context.Database.ComponentData[MailboxStateCache.cacheSlot];
			MailboxState result;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
			{
				if (mailboxStateCache.MailboxNumberDictionary.TryGetValue(mailboxNumber, out result))
				{
					return result;
				}
			}
			if (mailboxStateCache.TryLoadMailboxState(context, null, new int?(mailboxNumber), out result))
			{
				return result;
			}
			return null;
		}

		public static bool TryGetLocked(Context context, int mailboxNumber, bool sharedLock, Func<MailboxState, TimeSpan> timeoutFunc, ILockStatistics lockStats, out bool timeoutReached, out MailboxState lockedMailboxState)
		{
			lockedMailboxState = null;
			timeoutReached = false;
			MailboxState mailboxState = MailboxStateCache.Get(context, mailboxNumber);
			if (mailboxState == null)
			{
				return false;
			}
			if (!mailboxState.TryGetMailboxLock(sharedLock, timeoutFunc(mailboxState), lockStats))
			{
				timeoutReached = true;
				return false;
			}
			if (!mailboxState.IsValid)
			{
				MailboxState mailboxState2 = MailboxStateCache.Get(context, mailboxNumber);
				if (mailboxState2 == null)
				{
					mailboxState.ReleaseMailboxLock(sharedLock);
					return false;
				}
				mailboxState = mailboxState2;
			}
			lockedMailboxState = mailboxState;
			return true;
		}

		public static MailboxState DangerousGetByGuidForTest(Context context, Guid mailboxGuid, bool createIfNotFound)
		{
			return MailboxStateCache.DangerousGetByGuidForTest(context, mailboxGuid, createIfNotFound ? MailboxCreation.Allow(null) : MailboxCreation.DontAllow);
		}

		public static MailboxState DangerousGetByGuidForTest(Context context, Guid mailboxGuid, MailboxCreation mailboxCreation)
		{
			bool flag = !context.Database.IsSharedLockHeld() && !context.Database.IsExclusiveLockHeld();
			MailboxState byGuid;
			try
			{
				if (flag)
				{
					context.Database.GetSharedLock();
				}
				byGuid = MailboxStateCache.GetByGuid(context, mailboxGuid, mailboxCreation);
			}
			finally
			{
				if (flag)
				{
					context.Database.ReleaseSharedLock();
				}
			}
			return byGuid;
		}

		public static bool TryGetByGuidLocked(Context context, Guid mailboxGuid, MailboxCreation mailboxCreation, bool findRemovedMailbox, bool sharedLock, Func<MailboxState, TimeSpan> timeoutFunc, ILockStatistics lockStats, out bool timeoutReached, out MailboxState lockedMailboxState)
		{
			lockedMailboxState = null;
			MailboxState mailboxState;
			for (;;)
			{
				timeoutReached = false;
				mailboxState = MailboxStateCache.GetByGuid(context, mailboxGuid, mailboxCreation);
				if (mailboxState == null)
				{
					break;
				}
				if (!mailboxState.TryGetMailboxLock(sharedLock, timeoutFunc(mailboxState), lockStats))
				{
					goto Block_2;
				}
				if (mailboxState.IsCurrent || mailboxState.IsNew)
				{
					goto IL_44;
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!object.ReferenceEquals(mailboxState, MailboxStateCache.GetByGuid(context, mailboxGuid, MailboxCreation.DontAllow)), "Mailbox state object with such status should be accessible by GUID!");
				mailboxState.ReleaseMailboxLock(sharedLock);
			}
			return false;
			Block_2:
			timeoutReached = true;
			return false;
			IL_44:
			if ((mailboxState.IsNew && !mailboxCreation.IsAllowed) || (mailboxState.IsRemoved && !findRemovedMailbox))
			{
				mailboxState.ReleaseMailboxLock(sharedLock);
				mailboxState = null;
			}
			lockedMailboxState = mailboxState;
			return lockedMailboxState != null;
		}

		public static void DeleteMailboxState(Context context, MailboxState mailboxState)
		{
			MailboxStateCache mailboxStateCache = mailboxState.MailboxStateCache;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache))
			{
				mailboxStateCache.MailboxNumberDictionary.Remove(mailboxState.MailboxNumber);
				mailboxStateCache.deletedMailboxes.Add(mailboxState.MailboxNumber);
				MailboxState mailboxState2;
				if (mailboxState.MailboxGuid != Guid.Empty && mailboxStateCache.MailboxGuidDictionary.TryGetValue(mailboxState.MailboxGuid, out mailboxState2) && mailboxState2.MailboxNumber == mailboxState.MailboxNumber)
				{
					mailboxStateCache.MailboxGuidDictionary.Remove(mailboxState.MailboxGuid);
				}
				if (mailboxState.IsNewMailboxPartition)
				{
					mailboxStateCache.unifiedMailboxGuidDictionary.Remove(mailboxState.UnifiedState.UnifiedMailboxGuid);
				}
				mailboxState.Invalidate(context);
				if (mailboxState.CurrentlyActive)
				{
					mailboxStateCache.activeMailboxesEvictionPolicy.Remove(mailboxState.MailboxNumber);
					mailboxState.CurrentlyActive = false;
					if (mailboxStateCache.perfInstance != null)
					{
						mailboxStateCache.perfInstance.ActiveMailboxes.Decrement();
					}
				}
			}
		}

		public static void MakeRoomForNewMailbox(MailboxState mailboxState)
		{
			MailboxStateCache mailboxStateCache = mailboxState.MailboxStateCache;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache))
			{
				MailboxState mailboxState2;
				if (mailboxState.MailboxGuid != Guid.Empty && mailboxStateCache.MailboxGuidDictionary.TryGetValue(mailboxState.MailboxGuid, out mailboxState2) && mailboxState2.MailboxNumber == mailboxState.MailboxNumber)
				{
					mailboxStateCache.MailboxGuidDictionary.Remove(mailboxState.MailboxGuid);
					mailboxStateCache.AddToPostDisposeCleanupList(mailboxState);
				}
			}
		}

		internal static void MakeRoomForNewPartition(Context context, MailboxState mailboxState, Guid newUnifiedMailboxGuid)
		{
			MailboxStateCache mailboxStateCache = mailboxState.MailboxStateCache;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache))
			{
				mailboxStateCache.UnifiedMailboxGuidDictionary.Remove(mailboxState.UnifiedState.UnifiedMailboxGuid);
				mailboxState.UnifiedState.SetNewUnfiedMailboxGuid(newUnifiedMailboxGuid);
				mailboxStateCache.UnifiedMailboxGuidDictionary.Add(mailboxState.UnifiedState.UnifiedMailboxGuid, mailboxState.UnifiedState);
			}
			if (mailboxState.IsNew)
			{
				MailboxStateCache.DeleteMailboxState(context, mailboxState);
				return;
			}
			MailboxStateCache.MakeRoomForNewMailbox(mailboxState);
		}

		public static MailboxState ResetMailboxState(Context context, MailboxState mailboxState, MailboxStatus newStatus)
		{
			MailboxStateCache mailboxStateCache = mailboxState.MailboxStateCache;
			MailboxState mailboxState2 = new MailboxState(mailboxStateCache, mailboxState.MailboxNumber, mailboxState.MailboxPartitionNumber, mailboxState.MailboxGuid, mailboxState.MailboxInstanceGuid, newStatus, mailboxState.GlobalIdLowWatermark, mailboxState.GlobalCnLowWatermark, mailboxState.CountersAlreadyPatched, mailboxState.Quarantined, mailboxState.LastMailboxMaintenanceTime, mailboxState.LastQuotaCheckTime, mailboxState.TenantHint, mailboxState.MailboxType, mailboxState.MailboxTypeDetail);
			mailboxState2.SetUnifiedMailboxState(mailboxState.UnifiedState);
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache))
			{
				mailboxState.Invalidate(context);
				mailboxStateCache.MailboxNumberDictionary[mailboxState.MailboxNumber] = mailboxState2;
				MailboxState mailboxState3;
				if (mailboxState.MailboxGuid != Guid.Empty && mailboxStateCache.MailboxGuidDictionary.TryGetValue(mailboxState.MailboxGuid, out mailboxState3) && mailboxState3.MailboxNumber == mailboxState.MailboxNumber)
				{
					mailboxStateCache.MailboxGuidDictionary[mailboxState.MailboxGuid] = mailboxState2;
				}
				if (mailboxState.CurrentlyActive)
				{
					mailboxStateCache.activeMailboxesEvictionPolicy.Remove(mailboxState.MailboxNumber);
					mailboxState.CurrentlyActive = false;
					if (mailboxStateCache.perfInstance != null)
					{
						mailboxStateCache.perfInstance.ActiveMailboxes.Decrement();
					}
				}
			}
			return mailboxState2;
		}

		public static bool TryGetMailboxNumber(Context context, Guid mailboxGuid, bool active, out int mailboxNumber)
		{
			MailboxState byGuid = MailboxStateCache.GetByGuid(context, mailboxGuid, MailboxCreation.DontAllow);
			if (byGuid != null && (!active || byGuid.IsUserAccessible))
			{
				mailboxNumber = byGuid.MailboxNumber;
				return true;
			}
			mailboxNumber = 0;
			return false;
		}

		public static IEnumerable<MailboxState> GetStateListSnapshot(Context context, SearchCriteria restriction)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(context.Database);
			List<int> mailboxNumbers = new List<int>();
			Column[] columnsToFetch = new Column[]
			{
				mailboxTable.MailboxNumber
			};
			bool transactionStarted = false;
			try
			{
				if (!context.DatabaseTransactionStarted && context.Database.PhysicalDatabase.DatabaseType != DatabaseType.Sql)
				{
					context.BeginTransactionIfNeeded();
					transactionStarted = true;
				}
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxTable.MailboxTablePK, columnsToFetch, restriction, null, 0, 0, KeyRange.AllRows, false, false))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						while (reader.Read())
						{
							int @int = reader.GetInt32(mailboxTable.MailboxNumber);
							mailboxNumbers.Add(@int);
						}
					}
				}
			}
			finally
			{
				if (transactionStarted)
				{
					try
					{
						context.Commit();
					}
					finally
					{
						context.Abort();
					}
				}
			}
			foreach (int mailboxNumber in mailboxNumbers)
			{
				MailboxState mailboxState = MailboxStateCache.Get(context, mailboxNumber);
				if (mailboxState != null)
				{
					yield return mailboxState;
				}
			}
			yield break;
		}

		public static void OnMailboxActivity(MailboxState mailboxState)
		{
			MailboxStateCache mailboxStateCache = mailboxState.MailboxStateCache;
			if (!mailboxState.IsValid)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow - mailboxState.LastUpdatedActiveTime < MailboxStateCache.activeMailboxesEvictionPolicyUpdateThreshold.Value && mailboxState.CurrentlyActive)
			{
				return;
			}
			int[] array = null;
			mailboxState.AddReference();
			try
			{
				using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache))
				{
					if (mailboxState.CurrentlyActive)
					{
						mailboxStateCache.activeMailboxesEvictionPolicy.KeyAccess(mailboxState.MailboxNumber);
					}
					else
					{
						mailboxStateCache.activeMailboxesEvictionPolicy.Insert(mailboxState.MailboxNumber);
						mailboxState.CurrentlyActive = true;
						if (mailboxStateCache.perfInstance != null)
						{
							mailboxStateCache.perfInstance.ActiveMailboxes.Increment();
						}
					}
					mailboxState.LastUpdatedActiveTime = utcNow;
					mailboxStateCache.activeMailboxesEvictionPolicy.EvictionCheckpoint();
					if (mailboxStateCache.activeMailboxesEvictionPolicy.CountOfKeysToCleanup >= MailboxStateCache.numberOfMailboxesToStartCleanupTask.Value)
					{
						array = mailboxStateCache.activeMailboxesEvictionPolicy.GetKeysToCleanup(true);
						foreach (int key in array)
						{
							MailboxState mailboxState2;
							if (mailboxStateCache.MailboxNumberDictionary.TryGetValue(key, out mailboxState2))
							{
								mailboxState2.CurrentlyActive = false;
								if (mailboxStateCache.perfInstance != null)
								{
									mailboxStateCache.perfInstance.ActiveMailboxes.Decrement();
								}
							}
						}
					}
				}
			}
			finally
			{
				mailboxState.ReleaseReference();
			}
			if (array != null)
			{
				SingleExecutionTask<MailboxStateCache.DatabaseAndMailboxNumbers>.CreateSingleExecutionTask(mailboxStateCache.Database.TaskList, TaskExecutionWrapper<MailboxStateCache.DatabaseAndMailboxNumbers>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.CleanupNonActiveMailboxStates, ClientType.System, mailboxStateCache.Database.MdbGuid), new TaskExecutionWrapper<MailboxStateCache.DatabaseAndMailboxNumbers>.TaskCallback<Context>(MailboxStateCache.CleanupNonActiveMailboxStates)), new MailboxStateCache.DatabaseAndMailboxNumbers(mailboxStateCache.Database, array), true);
			}
		}

		string IStoreQueryTargetBase<MailboxStateCache.QueryableMailboxState>.Name
		{
			get
			{
				return "MailboxState";
			}
		}

		Type[] IStoreQueryTargetBase<MailboxStateCache.QueryableMailboxState>.ParameterTypes
		{
			get
			{
				return Array<Type>.Empty;
			}
		}

		IEnumerable<MailboxStateCache.QueryableMailboxState> IStoreDatabaseQueryTarget<MailboxStateCache.QueryableMailboxState>.GetRows(IConnectionProvider connectionProvider, object[] parameters)
		{
			IContextProvider contextProvider = connectionProvider as IContextProvider;
			if (contextProvider == null)
			{
				throw new DiagnosticQueryException("The connection provider given is not a context provider.");
			}
			IList<MailboxStateCache.QueryableMailboxState> list = new List<MailboxStateCache.QueryableMailboxState>(30);
			MailboxStateCache mailboxStateCache = (MailboxStateCache)this.database.ComponentData[MailboxStateCache.cacheSlot];
			IList<int> list2;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache))
			{
				list2 = new List<int>(mailboxStateCache.MailboxNumberDictionary.Keys);
			}
			foreach (int mailboxNumber in list2)
			{
				bool flag;
				MailboxState mailboxState;
				if (MailboxStateCache.TryGetLocked(contextProvider.CurrentContext, mailboxNumber, true, (MailboxState state) => DefaultSettings.Get.DiagnosticQueryLockTimeout, contextProvider.CurrentContext.Diagnostics, out flag, out mailboxState))
				{
					try
					{
						mailboxState.AddReference();
						try
						{
							list.Add(new MailboxStateCache.QueryableMailboxState(mailboxState));
						}
						finally
						{
							mailboxState.ReleaseReference();
						}
						continue;
					}
					finally
					{
						mailboxState.ReleaseMailboxLock(true);
					}
				}
				if (flag)
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.UnableToLockMailbox(mailboxNumber));
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.MailboxStateNotFound(mailboxNumber));
			}
			return list;
		}

		internal static void Initialize()
		{
			MailboxStateCache.expectedNumberOfActiveMailboxes = Hookable<int>.Create(false, ConfigurationSchema.ActiveMailboxCacheSize.Value);
			MailboxStateCache.activeMailboxCacheTimeToLive = ConfigurationSchema.ActiveMailboxCacheTimeToLive.Value;
			MailboxStateCache.numberOfMailboxesToStartCleanupTask = Hookable<int>.Create(false, ConfigurationSchema.ActiveMailboxCacheCleanupThreshold.Value);
			if (MailboxStateCache.cacheSlot == -1)
			{
				MailboxStateCache.cacheSlot = StoreDatabase.AllocateComponentDataSlot();
				Mailbox.RegisterOnPostDisposeAction(new Action<Context, StoreDatabase>(MailboxStateCache.PostMailboxDisposeCleanup));
			}
		}

		internal static void MountHandler(StoreDatabase database, IConnectionProvider connectionProvider)
		{
			MailboxStateCache mailboxStateCache = new MailboxStateCache(database);
			mailboxStateCache.Configure(connectionProvider);
			database.ComponentData[MailboxStateCache.cacheSlot] = mailboxStateCache;
			StoreQueryTargets.Register<MailboxStateCache.QueryableMailboxState>(mailboxStateCache, database.PhysicalDatabase, Visibility.Public);
		}

		internal static void DismountHandler(Context context, StoreDatabase database)
		{
			if (MailboxStateCache.onDismountTestHook.Value != null)
			{
				MailboxStateCache.onDismountTestHook.Value(context, database);
			}
			MailboxStateCache.PostMailboxDisposeCleanup(context, database);
			database.ComponentData[MailboxStateCache.cacheSlot] = null;
		}

		internal static IDisposable SetOnDismountingTestHook()
		{
			return MailboxStateCache.onDismountTestHook.SetTestHook(new Action<Context, StoreDatabase>(MailboxStateCache.DismountHandlerForTest));
		}

		internal static IDisposable SetOnBeforeLoadTestHook(Action<Context> action)
		{
			return MailboxStateCache.onBeforeLoadTestHook.SetTestHook(action);
		}

		internal static IDisposable SetOnAfterLoadTestHook(Action<Context> action)
		{
			return MailboxStateCache.onAfterLoadTestHook.SetTestHook(action);
		}

		internal static IDisposable SetActiveMailboxesEvictionPolicyUpdateThreshold(TimeSpan threshold)
		{
			return MailboxStateCache.activeMailboxesEvictionPolicyUpdateThreshold.SetTestHook(threshold);
		}

		internal static IDisposable SetExpectedNumberOfActiveMailboxes(int numberOfActiveMailboxes)
		{
			return MailboxStateCache.expectedNumberOfActiveMailboxes.SetTestHook(numberOfActiveMailboxes);
		}

		internal static void CleanupNonActiveMailboxStates(Context context, MailboxStateCache.DatabaseAndMailboxNumbers databaseAndMailboxNumbers, Func<bool> shouldTaskContinue)
		{
			using (context.AssociateWithDatabase(databaseAndMailboxNumbers.Database))
			{
				if (context.Database.IsOnlineActive)
				{
					MailboxStateCache.CleanupNonActiveMailboxStates(context, databaseAndMailboxNumbers.MailboxNumbers);
				}
			}
		}

		internal static void CleanupNonActiveMailboxStates(Context context, int[] mailboxNumbers)
		{
			MailboxStateCache mailboxStateCache = (MailboxStateCache)context.Database.ComponentData[MailboxStateCache.cacheSlot];
			int i = 0;
			while (i < mailboxNumbers.Length)
			{
				int num = mailboxNumbers[i];
				bool flag;
				MailboxState mailboxState;
				if (!MailboxStateCache.TryGetLocked(context, num, false, (MailboxState state) => TimeSpan.Zero, context.Diagnostics, out flag, out mailboxState))
				{
					using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
					{
						if (!mailboxStateCache.activeMailboxesEvictionPolicy.Contains(num) && mailboxStateCache.MailboxNumberDictionary.TryGetValue(num, out mailboxState))
						{
							mailboxStateCache.activeMailboxesEvictionPolicy.AddKeyToCleanup(num);
							mailboxState.CurrentlyActive = true;
							if (mailboxStateCache.perfInstance != null)
							{
								mailboxStateCache.perfInstance.ActiveMailboxes.Increment();
							}
						}
						goto IL_F6;
					}
					goto Block_4;
				}
				goto IL_CC;
				IL_F6:
				i++;
				continue;
				Block_4:
				try
				{
					IL_CC:
					if (!mailboxState.CurrentlyActive)
					{
						mailboxState.AddReference();
						try
						{
							mailboxState.CleanupAsNonActive(context);
						}
						finally
						{
							mailboxState.ReleaseReference();
						}
					}
				}
				finally
				{
					mailboxState.ReleaseMailboxLock(false);
				}
				goto IL_F6;
			}
		}

		internal static int[] GetMailboxesForCleanupForTest(Context context, bool clearKeys)
		{
			MailboxStateCache mailboxStateCache = (MailboxStateCache)context.Database.ComponentData[MailboxStateCache.cacheSlot];
			int[] result;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
			{
				mailboxStateCache.activeMailboxesEvictionPolicy.EvictionCheckpoint();
				int[] keysToCleanup = mailboxStateCache.activeMailboxesEvictionPolicy.GetKeysToCleanup(clearKeys);
				if (clearKeys)
				{
					foreach (int key in keysToCleanup)
					{
						MailboxState mailboxState;
						if (mailboxStateCache.MailboxNumberDictionary.TryGetValue(key, out mailboxState))
						{
							mailboxState.CurrentlyActive = false;
							if (mailboxStateCache.perfInstance != null)
							{
								mailboxStateCache.perfInstance.ActiveMailboxes.Decrement();
							}
						}
					}
				}
				result = keysToCleanup;
			}
			return result;
		}

		private static void PostMailboxDisposeCleanup(Context context, StoreDatabase database)
		{
			if (database != null)
			{
				MailboxStateCache mailboxStateCache = (MailboxStateCache)database.ComponentData[MailboxStateCache.cacheSlot];
				if (mailboxStateCache == null)
				{
					return;
				}
				List<MailboxState> list = null;
				using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
				{
					for (int i = mailboxStateCache.postMailboxDisposeList.Count - 1; i >= 0; i--)
					{
						MailboxState mailboxState = mailboxStateCache.postMailboxDisposeList[i];
						if (mailboxState.IsMailboxLockedExclusively())
						{
							if (list == null)
							{
								list = new List<MailboxState>(1);
							}
							list.Add(mailboxState);
							mailboxStateCache.postMailboxDisposeList.RemoveAt(i);
						}
					}
				}
				if (list != null)
				{
					foreach (MailboxState mailboxState2 in list)
					{
						mailboxState2.CleanupDataSlots(context);
					}
				}
			}
		}

		private static void DismountHandlerForTest(Context context, StoreDatabase database)
		{
			MailboxStateCache mailboxStateCache = (MailboxStateCache)context.Database.ComponentData[MailboxStateCache.cacheSlot];
			if (mailboxStateCache != null)
			{
				List<MailboxState> list;
				using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
				{
					list = new List<MailboxState>(mailboxStateCache.mailboxNumberDictionary.Values);
				}
				foreach (MailboxState mailboxState in list)
				{
					mailboxState.GetMailboxLock(false, context.Diagnostics);
					MailboxTaskQueue mailboxTaskQueueNoCreate;
					try
					{
						mailboxState.AddReference();
						try
						{
							mailboxState.CleanupAsNonActive(context);
						}
						finally
						{
							mailboxState.ReleaseReference();
						}
						mailboxTaskQueueNoCreate = MailboxTaskQueue.GetMailboxTaskQueueNoCreate(context, mailboxState);
					}
					finally
					{
						mailboxState.ReleaseMailboxLock(false);
					}
					if (mailboxTaskQueueNoCreate != null)
					{
						mailboxTaskQueueNoCreate.DrainQueue();
					}
				}
			}
		}

		private static MailboxState GetByGuid(Context context, Guid mailboxGuid, MailboxCreation mailboxCreation)
		{
			MailboxStateCache mailboxStateCache = (MailboxStateCache)context.Database.ComponentData[MailboxStateCache.cacheSlot];
			MailboxState mailboxState;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
			{
				if (mailboxStateCache.MailboxGuidDictionary.TryGetValue(mailboxGuid, out mailboxState))
				{
					return mailboxState;
				}
			}
			if (mailboxStateCache.TryLoadMailboxState(context, new Guid?(mailboxGuid), null, out mailboxState))
			{
				return mailboxState;
			}
			if (!mailboxCreation.IsAllowed)
			{
				return null;
			}
			if (context.Database.IsReadOnly)
			{
				throw new StoreException((LID)56268U, ErrorCodeValue.NotSupported);
			}
			if (mailboxCreation.UnifiedMailboxGuid != null)
			{
				if (!DefaultSettings.Get.EnableUnifiedMailbox)
				{
					throw new StoreException((LID)54732U, ErrorCodeValue.NotSupported);
				}
				if (!UnifiedMailbox.IsReady(context, context.Database))
				{
					throw new StoreException((LID)49692U, ErrorCodeValue.InvalidParameter);
				}
			}
			MailboxState result;
			using (LockManager.Lock(mailboxStateCache.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
			{
				if (mailboxStateCache.MailboxGuidDictionary.TryGetValue(mailboxGuid, out mailboxState))
				{
					result = mailboxState;
				}
				else
				{
					int num = mailboxStateCache.nextMailboxNumber++;
					MailboxState.UnifiedMailboxState unifiedMailboxState = null;
					int mailboxPartitionNumber;
					if (mailboxCreation.UnifiedMailboxGuid != null)
					{
						if (mailboxStateCache.unifiedMailboxGuidDictionary.TryGetValue(mailboxCreation.UnifiedMailboxGuid.Value, out unifiedMailboxState))
						{
							mailboxPartitionNumber = unifiedMailboxState.MailboxPartitionNumber;
						}
						else
						{
							int num2;
							bool countersAlreadyPatched;
							if (mailboxStateCache.TryLoadMailboxPartitionNumber(context, mailboxCreation.UnifiedMailboxGuid.Value, out num2))
							{
								mailboxPartitionNumber = num2;
								countersAlreadyPatched = false;
							}
							else
							{
								if (!mailboxCreation.AllowPartitionCreation)
								{
									throw new StoreException((LID)62876U, ErrorCodeValue.NoAccess, "Partition creation is not allowed");
								}
								mailboxPartitionNumber = num;
								countersAlreadyPatched = true;
							}
							unifiedMailboxState = new MailboxState.UnifiedMailboxState(mailboxStateCache, mailboxCreation.UnifiedMailboxGuid.Value, mailboxPartitionNumber, 0UL, 0UL, countersAlreadyPatched);
							mailboxStateCache.unifiedMailboxGuidDictionary.Add(mailboxCreation.UnifiedMailboxGuid.Value, unifiedMailboxState);
						}
					}
					else
					{
						mailboxPartitionNumber = num;
					}
					mailboxState = new MailboxState(mailboxStateCache, num, mailboxPartitionNumber, mailboxGuid, Guid.NewGuid(), MailboxStatus.New, 0UL, 0UL, true, false, DateTime.MinValue, DateTime.MinValue, TenantHint.Empty, MailboxInfo.MailboxType.Private, MailboxInfo.MailboxTypeDetail.UserMailbox);
					if (unifiedMailboxState != null)
					{
						mailboxState.SetUnifiedMailboxState(unifiedMailboxState);
					}
					mailboxStateCache.MailboxNumberDictionary.Add(mailboxState.MailboxNumber, mailboxState);
					mailboxStateCache.MailboxGuidDictionary.Add(mailboxState.MailboxGuid, mailboxState);
					result = mailboxState;
				}
			}
			return result;
		}

		internal void AddToPostDisposeCleanupList(MailboxState mailboxState)
		{
			this.postMailboxDisposeList.Add(mailboxState);
		}

		private void Configure(IConnectionProvider connectionProvider)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(this.database);
			Column[] columnsToFetch = new Column[]
			{
				mailboxTable.MailboxNumber
			};
			using (TableOperator tableOperator = Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, connectionProvider, mailboxTable.Table, mailboxTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 1, KeyRange.AllRows, true, false))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (reader.Read())
					{
						int @int = reader.GetInt32(mailboxTable.MailboxNumber);
						this.nextMailboxNumber = @int + 1;
					}
				}
			}
		}

		private bool TryLoadMailboxPartitionNumber(Context context, Guid unifiedMailboxGuid, out int mailboxPartitionNumber)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(UnifiedMailbox.IsReady(context, context.Database), "This function should be called only after UnifiedMailbox upgrader is executed");
			mailboxPartitionNumber = 0;
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				unifiedMailboxGuid
			});
			bool result;
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxTable.UnifiedMailboxGuidIndex, new Column[]
			{
				mailboxTable.MailboxPartitionNumber
			}, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (!reader.Read())
					{
						result = false;
					}
					else
					{
						mailboxPartitionNumber = reader.GetInt32(mailboxTable.MailboxPartitionNumber);
						result = true;
					}
				}
			}
			return result;
		}

		private bool TryLoadMailboxState(Context context, Guid? mailboxGuid, int? mailboxNumber, out MailboxState mailboxState)
		{
			mailboxState = null;
			if (MailboxStateCache.onBeforeLoadTestHook.Value != null)
			{
				MailboxStateCache.onBeforeLoadTestHook.Value(context);
			}
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(context.Database);
			Column column = PropertySchema.MapToColumn(context.Database, ObjectType.Mailbox, PropTag.Mailbox.MailboxType);
			Column column2 = PropertySchema.MapToColumn(context.Database, ObjectType.Mailbox, PropTag.Mailbox.MailboxTypeDetail);
			Column column3 = PropertySchema.MapToColumn(context.Database, ObjectType.Mailbox, PropTag.Mailbox.TenantHint);
			Column column4 = PropertySchema.MapToColumn(context.Database, ObjectType.Mailbox, PropTag.Mailbox.MailboxPartitionNumber);
			List<Column> list = new List<Column>(new Column[]
			{
				mailboxTable.LastQuotaNotificationTime,
				mailboxTable.DeletedOn,
				mailboxTable.MailboxGuid,
				mailboxTable.MailboxInstanceGuid,
				mailboxTable.MailboxNumber,
				mailboxTable.Status,
				column,
				column2,
				column3,
				column4
			});
			if (AddLastMaintenanceTimeToMailbox.IsReady(context, context.Database))
			{
				list.Add(mailboxTable.LastMailboxMaintenanceTime);
			}
			if (UnifiedMailbox.IsReady(context, context.Database))
			{
				list.Add(mailboxTable.UnifiedMailboxGuid);
			}
			StartStopKey startStopKey;
			Index index;
			if (mailboxGuid != null)
			{
				startStopKey = new StartStopKey(true, new object[]
				{
					mailboxGuid.Value
				});
				index = mailboxTable.MailboxGuidIndex;
			}
			else
			{
				startStopKey = new StartStopKey(true, new object[]
				{
					mailboxNumber.Value
				});
				index = mailboxTable.MailboxTablePK;
			}
			Guid? guid = null;
			bool flag = false;
			MailboxState mailboxState2;
			try
			{
				if (!context.DatabaseTransactionStarted && context.Database.PhysicalDatabase.DatabaseType != DatabaseType.Sql)
				{
					context.BeginTransactionIfNeeded();
					flag = true;
				}
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, index, list, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (!reader.Read())
						{
							return false;
						}
						int @int = reader.GetInt32(mailboxTable.MailboxNumber);
						Guid? nullableGuid = reader.GetNullableGuid(mailboxTable.MailboxGuid);
						Guid? nullableGuid2 = reader.GetNullableGuid(mailboxTable.MailboxInstanceGuid);
						reader.GetNullableDateTime(mailboxTable.DeletedOn);
						MailboxStatus int2 = (MailboxStatus)reader.GetInt16(mailboxTable.Status);
						TenantHint tenantHint = new TenantHint(reader.GetBinary(column3));
						int? nullableInt = reader.GetNullableInt32(column);
						int? nullableInt2 = reader.GetNullableInt32(column2);
						MailboxInfo.MailboxType valueOrDefault = (MailboxInfo.MailboxType)nullableInt.GetValueOrDefault(0);
						MailboxInfo.MailboxTypeDetail valueOrDefault2 = (MailboxInfo.MailboxTypeDetail)nullableInt2.GetValueOrDefault(1);
						DateTime dateTime = reader.GetDateTime(mailboxTable.LastQuotaNotificationTime);
						int? nullableInt3 = reader.GetNullableInt32(column4);
						DateTime lastMailboxMaintenanceTime;
						if (AddLastMaintenanceTimeToMailbox.IsReady(context, context.Database))
						{
							lastMailboxMaintenanceTime = reader.GetNullableDateTime(mailboxTable.LastMailboxMaintenanceTime).GetValueOrDefault(DateTime.MinValue);
						}
						else
						{
							lastMailboxMaintenanceTime = dateTime;
						}
						if (UnifiedMailbox.IsReady(context, context.Database))
						{
							guid = reader.GetNullableGuid(mailboxTable.UnifiedMailboxGuid);
						}
						mailboxState2 = new MailboxState(this, @int, (nullableInt3 != null) ? nullableInt3.Value : @int, nullableGuid.GetValueOrDefault(Guid.Empty), nullableGuid2.GetValueOrDefault(Guid.Empty), int2, 0UL, 0UL, false, false, lastMailboxMaintenanceTime, dateTime, tenantHint, valueOrDefault, valueOrDefault2);
					}
				}
			}
			finally
			{
				if (flag)
				{
					try
					{
						context.Commit();
					}
					finally
					{
						context.Abort();
					}
				}
			}
			if (MailboxStateCache.onAfterLoadTestHook.Value != null)
			{
				MailboxStateCache.onAfterLoadTestHook.Value(context);
			}
			bool result;
			using (LockManager.Lock(this.LockObject, LockManager.LockType.MailboxStateCache, context.Diagnostics))
			{
				if (this.deletedMailboxes.Contains(mailboxState2.MailboxNumber))
				{
					result = false;
				}
				else
				{
					MailboxState mailboxState3;
					if (mailboxGuid != null && this.MailboxGuidDictionary.TryGetValue(mailboxState2.MailboxGuid, out mailboxState3))
					{
						mailboxState = mailboxState3;
					}
					else if (this.MailboxNumberDictionary.TryGetValue(mailboxState2.MailboxNumber, out mailboxState3))
					{
						if (mailboxGuid != null)
						{
							return false;
						}
						mailboxState = mailboxState3;
					}
					else
					{
						if (guid != null)
						{
							Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(UnifiedMailbox.IsReady(context, context.Database), "We should reach this code only after UnifiedMailbox upgrader is executed");
							MailboxState.UnifiedMailboxState unifiedMailboxState = null;
							if (!this.unifiedMailboxGuidDictionary.TryGetValue(guid.Value, out unifiedMailboxState))
							{
								unifiedMailboxState = new MailboxState.UnifiedMailboxState(this, guid.Value, mailboxState2.MailboxPartitionNumber, mailboxState2.GlobalIdLowWatermark, mailboxState2.GlobalCnLowWatermark, mailboxState2.CountersAlreadyPatched);
								this.unifiedMailboxGuidDictionary.Add(guid.Value, unifiedMailboxState);
							}
							mailboxState2.SetUnifiedMailboxState(unifiedMailboxState);
						}
						this.mailboxNumberDictionary.Add(mailboxState2.MailboxNumber, mailboxState2);
						if (mailboxState2.MailboxGuid != Guid.Empty)
						{
							this.mailboxGuidDictionary.Add(mailboxState2.MailboxGuid, mailboxState2);
						}
						mailboxState = mailboxState2;
					}
					result = true;
				}
			}
			return result;
		}

		private static int cacheSlot = -1;

		private static Hookable<int> expectedNumberOfActiveMailboxes = Hookable<int>.Create(false, 1000);

		private static TimeSpan activeMailboxCacheTimeToLive = TimeSpan.FromMinutes(30.0);

		private static Hookable<int> numberOfMailboxesToStartCleanupTask = Hookable<int>.Create(false, 5);

		private static Hookable<TimeSpan> activeMailboxesEvictionPolicyUpdateThreshold = Hookable<TimeSpan>.Create(false, TimeSpan.FromMinutes(0.0));

		private static Hookable<Action<Context, StoreDatabase>> onDismountTestHook = Hookable<Action<Context, StoreDatabase>>.Create(false, null);

		private static Hookable<Action<Context>> onBeforeLoadTestHook = Hookable<Action<Context>>.Create(false, null);

		private static Hookable<Action<Context>> onAfterLoadTestHook = Hookable<Action<Context>>.Create(false, null);

		private readonly StoreDatabase database;

		private readonly EvictionPolicy<int> activeMailboxesEvictionPolicy;

		private Dictionary<int, MailboxState> mailboxNumberDictionary;

		private Dictionary<Guid, MailboxState> mailboxGuidDictionary;

		private Dictionary<Guid, MailboxState.UnifiedMailboxState> unifiedMailboxGuidDictionary;

		private List<MailboxState> postMailboxDisposeList;

		private object lockObject = new object();

		private readonly StorePerDatabasePerformanceCountersInstance perfInstance;

		private int nextMailboxNumber = 100;

		private HashSet<int> deletedMailboxes;

		internal struct DatabaseAndMailboxNumbers
		{
			internal DatabaseAndMailboxNumbers(StoreDatabase database, int[] mailboxNumbers)
			{
				this.database = database;
				this.mailboxNumbers = mailboxNumbers;
			}

			internal StoreDatabase Database
			{
				get
				{
					return this.database;
				}
			}

			internal int[] MailboxNumbers
			{
				get
				{
					return this.mailboxNumbers;
				}
			}

			private readonly StoreDatabase database;

			private readonly int[] mailboxNumbers;
		}

		internal class QueryableMailboxState
		{
			internal QueryableMailboxState(MailboxState state)
			{
				this.CountersAlreadyPatched = state.CountersAlreadyPatched;
				this.CurrentlyActive = state.CurrentlyActive;
				this.DatabaseGuid = state.DatabaseGuid;
				this.GlobalCnLowWatermark = state.GlobalCnLowWatermark;
				this.GlobalIdLowWatermark = state.GlobalIdLowWatermark;
				this.IsAccessible = state.IsAccessible;
				this.IsCurrent = state.IsCurrent;
				this.IsDeleted = state.IsDeleted;
				this.IsDisabled = state.IsDisabled;
				this.IsDisconnected = state.IsDisconnected;
				this.IsHardDeleted = state.IsHardDeleted;
				this.IsNew = state.IsNew;
				this.IsRemoved = state.IsRemoved;
				this.IsSoftDeleted = state.IsSoftDeleted;
				this.IsTombstone = state.IsTombstone;
				this.IsUserAccessible = state.IsUserAccessible;
				this.IsValid = state.IsValid;
				this.LastUpdatedActiveTime = state.LastUpdatedActiveTime;
				this.MailboxGuid = state.MailboxGuid;
				this.MailboxInstanceGuid = state.MailboxInstanceGuid;
				this.MailboxNumber = state.MailboxNumber;
				this.MailboxType = state.MailboxType;
				this.Quarantined = state.Quarantined;
				this.Status = state.Status;
				this.TenantHint = state.TenantHint;
			}

			public bool CountersAlreadyPatched { get; private set; }

			public bool CurrentlyActive { get; private set; }

			public Guid DatabaseGuid { get; private set; }

			public ulong GlobalCnLowWatermark { get; private set; }

			public ulong GlobalIdLowWatermark { get; private set; }

			public bool IsAccessible { get; private set; }

			public bool IsCurrent { get; private set; }

			public bool IsDeleted { get; private set; }

			public bool IsDisabled { get; private set; }

			public bool IsDisconnected { get; private set; }

			public bool IsHardDeleted { get; private set; }

			public bool IsNew { get; private set; }

			public bool IsRemoved { get; private set; }

			public bool IsSoftDeleted { get; private set; }

			public bool IsTombstone { get; private set; }

			public bool IsUserAccessible { get; private set; }

			public bool IsValid { get; private set; }

			public DateTime LastUpdatedActiveTime { get; private set; }

			public Guid MailboxGuid { get; private set; }

			public Guid MailboxInstanceGuid { get; private set; }

			public int MailboxNumber { get; private set; }

			public MailboxInfo.MailboxType MailboxType { get; private set; }

			public bool Quarantined { get; private set; }

			public MailboxStatus Status { get; private set; }

			public TenantHint TenantHint { get; private set; }
		}
	}
}
