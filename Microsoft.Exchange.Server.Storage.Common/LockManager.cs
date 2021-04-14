using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class LockManager
	{
		internal static void Initialize()
		{
			LockManager.trackAllLockAcquisition = ConfigurationSchema.TrackAllLockAcquisition.Value;
		}

		public static TimeSpan CrashingThresholdTimeout
		{
			get
			{
				return LockManager.crashingThresholdTimeout.Value;
			}
		}

		public static void SetExternalConditionValidator(Func<LockManager.LockType, TimeSpan, bool> externalConditionValidator)
		{
			LockManager.externalConditionValidator = externalConditionValidator;
		}

		private static LockManager.NamedLockObjectsPartition[] CreateNamedLockObjectsPartitionArray()
		{
			LockManager.NamedLockObjectsPartition[] array = new LockManager.NamedLockObjectsPartition[40];
			for (int i = 0; i < 40; i++)
			{
				array[i] = new LockManager.NamedLockObjectsPartition();
			}
			return array;
		}

		internal static TimeSpan StaleLockCleanupInterval
		{
			get
			{
				return LockManager.staleLockCleanupInterval;
			}
			set
			{
				LockManager.staleLockCleanupInterval = value;
			}
		}

		internal static int StaleLockCleanupSkipCount
		{
			get
			{
				return LockManager.staleLockCleanupSkipCount;
			}
			set
			{
				LockManager.staleLockCleanupSkipCount = value;
			}
		}

		public static LockManager.NamedLockFrame Lock(ILockName lockName, LockManager.LockType lockType)
		{
			return LockManager.Lock(lockName, lockType, null);
		}

		public static LockManager.NamedLockFrame Lock(ILockName lockName, LockManager.LockType lockType, ILockStatistics lockStats)
		{
			LockManager.NamedLockObject namedLockObject = lockName.CachedLockObject;
			if (namedLockObject == null || !namedLockObject.TryAddRef())
			{
				namedLockObject = (lockName.CachedLockObject = LockManager.GetLockObject(lockName, true));
			}
			return new LockManager.NamedLockFrame(namedLockObject, lockType, lockStats);
		}

		[Obsolete("Locking by string name is now obsolete and should not be used.")]
		public static LockManager.ObjectLockFrame Lock(string lockName, LockManager.LockType lockType)
		{
			return default(LockManager.ObjectLockFrame);
		}

		public static LockManager.ObjectLockFrame Lock(object lockObject, LockManager.LockType lockType)
		{
			return new LockManager.ObjectLockFrame(lockObject, lockType, null);
		}

		public static LockManager.ObjectLockFrame Lock(object lockObject, LockManager.LockType lockType, ILockStatistics lockStats)
		{
			return new LockManager.ObjectLockFrame(lockObject, lockType, lockStats);
		}

		public static LockManager.ObjectLockFrame Lock(object lockObject)
		{
			return new LockManager.ObjectLockFrame(lockObject, LockManager.LockType.LeafMonitorLock, null);
		}

		public static LockManager.ObjectLockFrame Lock(object lockObject, ILockStatistics lockStats)
		{
			return new LockManager.ObjectLockFrame(lockObject, LockManager.LockType.LeafMonitorLock, lockStats);
		}

		public static void GetLock(ILockName lockName, LockManager.LockType lockType)
		{
			LockManager.GetLock(lockName, lockType, null);
		}

		public static void GetLock(ILockName lockName, LockManager.LockType lockType, ILockStatistics lockStats)
		{
			LockManager.NamedLockObject namedLockObject = lockName.CachedLockObject;
			if (namedLockObject == null || !namedLockObject.TryAddRef())
			{
				namedLockObject = (lockName.CachedLockObject = LockManager.GetLockObject(lockName, true));
			}
			LockManager.GetNamedLockImpl(namedLockObject, lockType, LockManager.InfiniteTimeout, lockStats);
		}

		public static bool HasContention(ILockName lockName)
		{
			return LockManager.simulateContention.Value || LockManager.GetWaitingCount(lockName) != 0;
		}

		public static int GetWaitingCount(ILockName lockName)
		{
			LockManager.NamedLockObject namedLockObject = lockName.CachedLockObject;
			int waitingCount;
			try
			{
				if (namedLockObject == null || !namedLockObject.TryAddRef())
				{
					namedLockObject = (lockName.CachedLockObject = LockManager.GetLockObject(lockName, true));
				}
				waitingCount = namedLockObject.WaitingCount;
			}
			finally
			{
				namedLockObject.ReleaseRef();
			}
			return waitingCount;
		}

		public static IDisposable SimulateContentionForTest()
		{
			return LockManager.simulateContention.SetTestHook(true);
		}

		public static IDisposable SetCrashingThresholdTimeoutTestHook(TimeSpan newTimeout)
		{
			return LockManager.crashingThresholdTimeout.SetTestHook(newTimeout);
		}

		[Obsolete("Locking by string name is now obsolete and should not be used.")]
		public static void GetLock(string lockName, LockManager.LockType lockType)
		{
		}

		public static void GetLock(object lockObject, LockManager.LockType lockType)
		{
			LockManager.GetLock(lockObject, lockType, null);
		}

		public static void GetLock(object lockObject, LockManager.LockType lockType, ILockStatistics lockStats)
		{
			LockManager.GetObjectLockImpl(lockObject, lockType, LockManager.InfiniteTimeout, lockStats);
		}

		public static bool TryGetLock(ILockName lockName, LockManager.LockType lockType)
		{
			return LockManager.TryGetLock(lockName, lockType, TimeSpan.FromMilliseconds(0.0), null);
		}

		public static bool TryGetLock(ILockName lockName, LockManager.LockType lockType, ILockStatistics lockStats)
		{
			return LockManager.TryGetLock(lockName, lockType, TimeSpan.FromMilliseconds(0.0), lockStats);
		}

		public static bool TryGetLock(ILockName lockName, LockManager.LockType lockType, TimeSpan timeout, ILockStatistics lockStats)
		{
			LockManager.NamedLockObject namedLockObject = lockName.CachedLockObject;
			if (namedLockObject == null || !namedLockObject.TryAddRef())
			{
				namedLockObject = (lockName.CachedLockObject = LockManager.GetLockObject(lockName, true));
			}
			return LockManager.GetNamedLockImpl(namedLockObject, lockType, timeout, lockStats);
		}

		public static bool TryGetLock(object lockObject, LockManager.LockType lockType)
		{
			return LockManager.TryGetLock(lockObject, lockType, null);
		}

		public static bool TryGetLock(object lockObject, LockManager.LockType lockType, ILockStatistics lockStats)
		{
			return LockManager.TryGetLock(lockObject, lockType, TimeSpan.FromMilliseconds(0.0), lockStats);
		}

		public static bool TryGetLock(object lockObject, LockManager.LockType lockType, TimeSpan timeout, ILockStatistics lockStats)
		{
			return LockManager.GetObjectLockImpl(lockObject, lockType, timeout, lockStats);
		}

		public static void ReleaseLock(ILockName lockName, LockManager.LockType lockType)
		{
			LockManager.NamedLockObject namedLockObject = lockName.CachedLockObject;
			if (namedLockObject == null)
			{
				namedLockObject = (lockName.CachedLockObject = LockManager.GetLockObject(lockName, false));
			}
			LockManager.ReleaseNamedLockImpl(namedLockObject, lockType);
		}

		[Obsolete("Locking by string name is now obsolete and should not be used.")]
		public static void ReleaseLock(string lockName, LockManager.LockType lockType)
		{
		}

		public static void ReleaseLock(object lockObject, LockManager.LockType lockType)
		{
			LockManager.ReleaseObjectLockImpl(lockObject, lockType);
		}

		public static void ReleaseAnyLock(LockManager.LockType lockType)
		{
			List<LockManager.LockHeldEntry> locksHeld = LockManager.LocksHeld;
			Globals.AssertRetail(locksHeld != null && locksHeld.Count > 0, "Releasing a lock the user does not hold");
			object lockObject = locksHeld[locksHeld.Count - 1].LockObject;
			LockManager.NamedLockObject namedLockObject = lockObject as LockManager.NamedLockObject;
			if (namedLockObject != null)
			{
				LockManager.ReleaseNamedLockImpl(namedLockObject, lockType);
				return;
			}
			LockManager.ReleaseObjectLockImpl(lockObject, lockType);
		}

		public static bool TestLock(ILockName lockName, LockManager.LockType lockType)
		{
			return LockManager.TestLockImpl(lockName, lockType);
		}

		[Obsolete("Locking by string name is now obsolete and should not be used.")]
		public static bool TestLock(string lockName, LockManager.LockType lockType)
		{
			return false;
		}

		public static bool TestLock(object lockObject, LockManager.LockType lockType)
		{
			return LockManager.TestLockImpl(lockObject, lockType);
		}

		[Conditional("DEBUG")]
		public static void AssertLockHeld(ILockName lockName, LockManager.LockType lockType)
		{
			LockManager.TestLockImpl(lockName, lockType);
		}

		[Obsolete("Locking by string name is now obsolete and should not be used.")]
		[Conditional("DEBUG")]
		public static void AssertLockHeld(string lockName, LockManager.LockType lockType)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertLockHeld(object lockObject, LockManager.LockType lockType)
		{
			LockManager.TestLockImpl(lockObject, lockType);
		}

		[Conditional("DEBUG")]
		public static void AssertLockNotHeld(ILockName lockName, LockManager.LockType lockType)
		{
			LockManager.TestLockImpl(lockName, lockType);
		}

		[Conditional("DEBUG")]
		[Obsolete("Locking by string name is now obsolete and should not be used.")]
		public static void AssertLockNotHeld(string lockName, LockManager.LockType lockType)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertLockNotHeld(object lockObject, LockManager.LockType lockType)
		{
			LockManager.TestLockImpl(lockObject, lockType);
		}

		[Conditional("DEBUG")]
		public static void AssertNoLocksHeld(LockManager.LockType lockType)
		{
		}

		public static bool IsLockHeld(LockManager.LockType lockType)
		{
			List<LockManager.LockHeldEntry> locksHeld = LockManager.LocksHeld;
			if (locksHeld != null)
			{
				for (int i = 0; i < locksHeld.Count; i++)
				{
					if (locksHeld[i].LockType == lockType)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void AssertNoLocksHeld()
		{
			List<LockManager.LockHeldEntry> locksHeld = LockManager.LocksHeld;
			Globals.AssertRetail(locksHeld == null || locksHeld.Count == 0, "Thread still holds some locks");
		}

		private static bool ValidateExternalCondition(LockManager.LockType lockType, TimeSpan timeout)
		{
			return LockManager.externalConditionValidator == null || LockManager.externalConditionValidator(lockType, timeout);
		}

		private static bool GetNamedLockImpl(LockManager.NamedLockObject lockObject, LockManager.LockType lockType, TimeSpan timeout, ILockStatistics lockStats)
		{
			LockManager.LockLevel lockLevel = LockManager.LockLevelFromLockType(lockType);
			List<LockManager.LockHeldEntry> list = LockManager.LocksHeld;
			if (list == null)
			{
				list = (LockManager.LocksHeld = new List<LockManager.LockHeldEntry>(8));
			}
			if (list.Count != 0)
			{
				LockManager.LockHeldEntry lockHeldEntry = list[list.Count - 1];
				LockManager.LockLevel lockLevel2 = LockManager.LockLevelFromLockType(lockHeldEntry.LockType);
				if (lockLevel < lockLevel2 || (lockLevel == lockLevel2 && (LockManager.LockKindFromLockType(lockType) != LockManager.LockKindFromLockType(lockHeldEntry.LockType) || !(lockHeldEntry.LockObject is LockManager.NamedLockObject) || lockObject.LockName.CompareTo(((LockManager.NamedLockObject)lockHeldEntry.LockObject).LockName) <= 0)))
				{
					Globals.AssertRetail(false, string.Format("Lock Hierarchy violation: Taking {0} violates {1}", lockType, lockHeldEntry.LockType));
				}
			}
			bool flag = false;
			if (timeout == LockManager.InfiniteTimeout || timeout >= LockManager.CrashingThresholdTimeout)
			{
				timeout = LockManager.CrashingThresholdTimeout;
				flag = true;
			}
			bool flag2 = lockObject.TryGetLock(lockType, timeout, lockStats);
			if (flag2)
			{
				list.Add(new LockManager.LockHeldEntry(lockType, lockObject));
			}
			else
			{
				try
				{
					if (flag)
					{
						throw new InvalidOperationException("Waiting time reached CrashingThresholdTimeout");
					}
				}
				finally
				{
					lockObject.ReleaseRef();
				}
			}
			return flag2;
		}

		private static bool GetObjectLockImpl(object lockObject, LockManager.LockType lockType, TimeSpan timeout, ILockStatistics lockStats)
		{
			LockManager.LockLevel lockLevel = LockManager.LockLevelFromLockType(lockType);
			List<LockManager.LockHeldEntry> list = LockManager.LocksHeld;
			if (list == null)
			{
				list = (LockManager.LocksHeld = new List<LockManager.LockHeldEntry>(8));
			}
			if (list.Count != 0 && lockLevel <= LockManager.LockLevelFromLockType(list[list.Count - 1].LockType))
			{
				Globals.AssertRetail(false, string.Format("Lock Hierarchy violation: Taking {0} violates {1}", lockType, list[list.Count - 1].LockType));
			}
			bool flag = Monitor.TryEnter(lockObject);
			bool flag2 = false;
			TimeSpan timeSpentWaiting = TimeSpan.Zero;
			bool flag3 = false;
			if (!flag)
			{
				flag2 = true;
				if (timeout != TimeSpan.Zero)
				{
					if (timeout == LockManager.InfiniteTimeout || timeout >= LockManager.CrashingThresholdTimeout)
					{
						timeout = LockManager.CrashingThresholdTimeout;
						flag3 = true;
					}
					StopwatchStamp stamp = StopwatchStamp.GetStamp();
					flag = Monitor.TryEnter(lockObject, timeout);
					timeSpentWaiting = stamp.ElapsedTime;
				}
			}
			if ((LockManager.trackAllLockAcquisition || flag2) && lockStats != null)
			{
				lockStats.OnAfterLockAcquisition(lockType, flag, flag2, null, timeSpentWaiting);
			}
			if (flag)
			{
				list.Add(new LockManager.LockHeldEntry(lockType, lockObject));
			}
			else if (flag3)
			{
				throw new InvalidOperationException("Waiting time reached CrashingThresholdTimeout");
			}
			return flag;
		}

		private static void ReleaseNamedLockImpl(LockManager.NamedLockObject lockObject, LockManager.LockType lockType)
		{
			LockManager.LockLevelFromLockType(lockType);
			List<LockManager.LockHeldEntry> locksHeld = LockManager.LocksHeld;
			if (locksHeld == null || locksHeld.Count == 0)
			{
				Globals.AssertRetail(false, string.Format("Releasing a lock {0} the user does not hold", lockType));
			}
			if (locksHeld[locksHeld.Count - 1].LockType != lockType || !object.ReferenceEquals(lockObject, locksHeld[locksHeld.Count - 1].LockObject))
			{
				Globals.AssertRetail(false, string.Format("Lock Hierarchy violation: Releasing {0} violates {1}", lockType, locksHeld[locksHeld.Count - 1].LockType));
			}
			lockObject.ReleaseLock(LockManager.LockKindFromLockType(lockType));
			lockObject.ReleaseRef();
			locksHeld.RemoveAt(locksHeld.Count - 1);
			int num = (lockObject.LockName.GetHashCode() & int.MaxValue) % 40;
			LockManager.NamedLockObjectsPartition namedLockObjectsPartition = LockManager.NamedLockObjects[num];
			if (!namedLockObjectsPartition.ShouldSkipCleanup())
			{
				LockManager.CleanupUnusedNamedLocks(namedLockObjectsPartition);
			}
		}

		private static void ReleaseObjectLockImpl(object lockObject, LockManager.LockType lockType)
		{
			LockManager.LockLevelFromLockType(lockType);
			List<LockManager.LockHeldEntry> locksHeld = LockManager.LocksHeld;
			if (locksHeld == null || locksHeld.Count == 0)
			{
				Globals.AssertRetail(false, string.Format("Releasing a lock {0} the user does not hold", lockType));
			}
			if (locksHeld[locksHeld.Count - 1].LockType != lockType || !object.ReferenceEquals(locksHeld[locksHeld.Count - 1].LockObject, lockObject))
			{
				Globals.AssertRetail(false, string.Format("Lock Hierarchy violation: Releasing {0} violates {1}", lockType, locksHeld[locksHeld.Count - 1].LockType));
			}
			Monitor.Exit(lockObject);
			locksHeld.RemoveAt(locksHeld.Count - 1);
		}

		private static bool TestLockImpl(ILockName lockName, LockManager.LockType lockType)
		{
			LockManager.LockLevelFromLockType(lockType);
			List<LockManager.LockHeldEntry> locksHeld = LockManager.LocksHeld;
			if (locksHeld != null)
			{
				for (int i = 0; i < locksHeld.Count; i++)
				{
					if (locksHeld[i].LockType == lockType && lockName.Equals(((LockManager.NamedLockObject)locksHeld[i].LockObject).LockName))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool TestLockImpl(object lockObject, LockManager.LockType lockType)
		{
			LockManager.LockLevelFromLockType(lockType);
			List<LockManager.LockHeldEntry> locksHeld = LockManager.LocksHeld;
			if (locksHeld != null)
			{
				for (int i = 0; i < locksHeld.Count; i++)
				{
					if (locksHeld[i].LockType == lockType && object.ReferenceEquals(locksHeld[i].LockObject, lockObject))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static LockManager.NamedLockObject GetLockObject(ILockName lockName, bool addref)
		{
			int num = (lockName.GetHashCode() & int.MaxValue) % 40;
			LockManager.NamedLockObjectsPartition namedLockObjectsPartition = LockManager.NamedLockObjects[num];
			LockManager.NamedLockObject namedLockObject;
			lock (namedLockObjectsPartition)
			{
				if (!namedLockObjectsPartition.LockObjectsDictionary.TryGetValue(lockName, out namedLockObject))
				{
					ILockName lockNameToCache = lockName.GetLockNameToCache();
					namedLockObject = new LockManager.NamedLockObject(lockNameToCache);
					namedLockObjectsPartition.LockObjectsDictionary.Add(lockNameToCache, namedLockObject);
				}
				if (addref)
				{
					namedLockObject.TryAddRef();
				}
			}
			return namedLockObject;
		}

		internal static LockManager.LockType GetLockType(LockManager.LockKind lockKind, LockManager.LockLevel lockLevel)
		{
			return (LockManager.LockType)(lockKind | (LockManager.LockKind)lockLevel);
		}

		internal static LockManager.LockKind LockKindFromLockType(LockManager.LockType lockType)
		{
			return (LockManager.LockKind)(lockType & (LockManager.LockType)96);
		}

		internal static LockManager.LockLevel LockLevelFromLockType(LockManager.LockType lockType)
		{
			return (LockManager.LockLevel)(lockType & (LockManager.LockType)31);
		}

		private static void CleanupUnusedNamedLocks(LockManager.NamedLockObjectsPartition partition)
		{
			try
			{
				DateTime cutoffTime;
				if (Monitor.TryEnter(partition, 0) && partition.TimeToRunCleanup(out cutoffTime))
				{
					List<LockManager.NamedLockObject> list = null;
					foreach (LockManager.NamedLockObject namedLockObject in partition.LockObjectsDictionary.Values)
					{
						if (namedLockObject.CanDispose(cutoffTime))
						{
							if (list == null)
							{
								list = new List<LockManager.NamedLockObject>(10);
							}
							list.Add(namedLockObject);
						}
					}
					if (list != null)
					{
						foreach (LockManager.NamedLockObject namedLockObject2 in list)
						{
							if (namedLockObject2.TryDispose())
							{
								partition.LockObjectsDictionary.Remove(namedLockObject2.LockName);
							}
						}
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(partition))
				{
					Monitor.Exit(partition);
				}
			}
		}

		private const int EstNumberOfObjects = 4000;

		private const int NumberOfObjectHashes = 40;

		private const int EstNumberOfLocksPerThread = 8;

		internal const int LockLevelBitCount = 5;

		internal const int LockKindBitCount = 2;

		public static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1.0);

		private static Hookable<TimeSpan> crashingThresholdTimeout = Hookable<TimeSpan>.Create(false, DefaultSettings.Get.LockManagerCrashingThresholdTimeout);

		private static int staleLockCleanupSkipCount = 1000;

		private static TimeSpan staleLockCleanupInterval = TimeSpan.FromMinutes(10.0);

		private static Hookable<bool> simulateContention = Hookable<bool>.Create(false, false);

		private static Func<LockManager.LockType, TimeSpan, bool> externalConditionValidator = null;

		private static bool trackAllLockAcquisition;

		[ThreadStatic]
		internal static List<LockManager.LockHeldEntry> LocksHeld;

		internal static LockManager.NamedLockObjectsPartition[] NamedLockObjects = LockManager.CreateNamedLockObjectsPartitionArray();

		private static long[] lockTimes = new long[128];

		private static Microsoft.Exchange.Diagnostics.Trace lockWaitTimeTracer = ExTraceGlobals.LockWaitTimeTracer;

		public enum LockLevel
		{
			First,
			AdminRpcInterface,
			Session,
			Database,
			Mailbox,
			UserInformation,
			User,
			LogicalIndexCache,
			LogicalIndex,
			PerUserCache,
			PerUser,
			ChangeNumberAndIdCounters,
			MailboxComponents,
			PhysicalIndexCache,
			WatermarkTable,
			WatermarkConsumer,
			MailboxStateCache,
			NotificationContext,
			EventCounterBounds,
			TaskList,
			Task,
			GlobalsTableRowUpdate,
			BlockModeReplicationEmitter,
			BlockModeSender,
			Leaf,
			Breadcrumbs,
			Last = 25
		}

		internal enum LockKind
		{
			Monitor,
			Exclusive = 32,
			Shared = 64,
			Last
		}

		public enum LockType
		{
			AdminRpcInterfaceExclusive = 33,
			AdminRpcInterfaceShared = 65,
			Session = 2,
			UserInformationExclusive = 37,
			UserInformationShared = 69,
			MailboxExclusive = 36,
			MailboxShared = 68,
			UserExclusive = 38,
			UserShared = 70,
			PerUserCacheShared = 73,
			PerUserCacheExclusive = 41,
			PerUserShared = 74,
			PerUserExclusive = 42,
			LogicalIndexCacheShared = 71,
			LogicalIndexCacheExclusive = 39,
			LogicalIndexShared = 72,
			LogicalIndexExclusive = 40,
			ChangeNumberAndIdCountersShared = 75,
			ChangeNumberAndIdCountersExclusive = 43,
			MailboxComponentsShared = 76,
			MailboxComponentsExclusive = 44,
			DatabaseExclusive = 35,
			DatabaseShared = 67,
			PhysicalIndexCache = 13,
			WatermarkTableExclusive = 46,
			WatermarkTableShared = 78,
			WatermarkConsumer = 15,
			MailboxStateCache,
			NotificationContext,
			EventCounterBounds,
			TaskList,
			Task,
			GlobalsTableRowUpdate,
			BlockModeReplicationEmitter,
			BlockModeSender,
			LeafMonitorLock,
			Breadcrumbs
		}

		internal struct LockHeldEntry
		{
			public LockHeldEntry(LockManager.LockType lockType, object lockObject)
			{
				this.LockType = lockType;
				this.LockObject = lockObject;
			}

			public LockManager.LockType LockType;

			public object LockObject;
		}

		public class NamedLockObject
		{
			internal NamedLockObject(ILockName lockName)
			{
				this.lockName = lockName;
			}

			internal ILockName LockName
			{
				get
				{
					return this.lockName;
				}
			}

			internal int RefCount
			{
				get
				{
					int num = this.refCount;
					if (num != -1)
					{
						return num & 65535;
					}
					return -1;
				}
			}

			internal DateTime LastUsed
			{
				get
				{
					return this.lastUsed;
				}
			}

			internal int WaitingCount
			{
				get
				{
					return this.waitingCount;
				}
			}

			internal bool TryAddRef()
			{
				for (;;)
				{
					int num = this.RefCount;
					if (num < 0)
					{
						return false;
					}
					int num2 = num + 1;
					if (num2 < 65535)
					{
						if (num == Interlocked.CompareExchange(ref this.refCount, num2, num))
						{
							break;
						}
					}
					else
					{
						Globals.AssertRetail(false, "lock object refcount overflow");
					}
				}
				return true;
			}

			internal void ReleaseRef()
			{
				int num = Interlocked.Decrement(ref this.refCount);
				Globals.AssertRetail((num & -65536) == 0, "At this point we should have only ref counter bits");
				if (num <= 0)
				{
					Globals.AssertRetail(num == 0, "lock object refcounting problem - refcount goes negative");
					if (this.rwLock != null && Interlocked.CompareExchange(ref this.refCount, 16711680, 0) == 0)
					{
						ReaderWriterLockSlim readerWriterLockSlim = this.rwLock;
						this.rwLock = null;
						int num2 = Interlocked.Exchange(ref this.refCount, 0);
						Globals.AssertRetail(num2 == 16711680, "How could it be different?");
						if (readerWriterLockSlim != null && !ConcurrentLookAside<ReaderWriterLockSlim>.Pool.Put(readerWriterLockSlim))
						{
							readerWriterLockSlim.Dispose();
						}
					}
					this.lastUsed = DateTime.UtcNow;
				}
			}

			internal bool CanDispose(DateTime cutoffTime)
			{
				return this.RefCount == 0 && this.lastUsed <= cutoffTime;
			}

			internal bool TryDispose()
			{
				if (Interlocked.CompareExchange(ref this.refCount, -1, 0) == 0)
				{
					if (this.rwLock != null)
					{
						if (!ConcurrentLookAside<ReaderWriterLockSlim>.Pool.Put(this.rwLock))
						{
							this.rwLock.Dispose();
						}
						this.rwLock = null;
					}
					return true;
				}
				return false;
			}

			internal bool TryGetLock(LockManager.LockType lockType, TimeSpan timeout, ILockStatistics lockStats)
			{
				ILockStatistics lockStatistics = this.mostRecentOwner;
				LockManager.LockKind lockKind = LockManager.LockKindFromLockType(lockType);
				TimeSpan timeSpentWaiting = TimeSpan.Zero;
				bool flag = false;
				bool flag2 = false;
				LockManager.LockKind lockKind2 = lockKind;
				if (lockKind2 != LockManager.LockKind.Monitor)
				{
					if (lockKind2 != LockManager.LockKind.Exclusive)
					{
						if (lockKind2 == LockManager.LockKind.Shared)
						{
							flag = this.GetReaderWriterLock().TryEnterReadLock(0);
							if (!flag)
							{
								flag2 = true;
								if (timeout != TimeSpan.Zero)
								{
									StopwatchStamp stamp = StopwatchStamp.GetStamp();
									Interlocked.Increment(ref this.waitingCount);
									try
									{
										if (lockStatistics == null)
										{
											lockStatistics = this.mostRecentOwner;
										}
										flag = this.GetReaderWriterLock().TryEnterReadLock(timeout);
									}
									finally
									{
										Interlocked.Decrement(ref this.waitingCount);
										timeSpentWaiting = stamp.ElapsedTime;
									}
								}
							}
							if (flag)
							{
								this.mostRecentOwner = lockStats;
							}
						}
					}
					else
					{
						flag = this.GetReaderWriterLock().TryEnterWriteLock(0);
						if (!flag)
						{
							flag2 = true;
							if (timeout != TimeSpan.Zero)
							{
								StopwatchStamp stamp2 = StopwatchStamp.GetStamp();
								Interlocked.Increment(ref this.waitingCount);
								try
								{
									if (lockStatistics == null)
									{
										lockStatistics = this.mostRecentOwner;
									}
									flag = this.GetReaderWriterLock().TryEnterWriteLock(timeout);
								}
								finally
								{
									Interlocked.Decrement(ref this.waitingCount);
									timeSpentWaiting = stamp2.ElapsedTime;
								}
							}
						}
						if (flag)
						{
							this.mostRecentOwner = lockStats;
						}
					}
				}
				else
				{
					flag = Monitor.TryEnter(this);
					if (!flag)
					{
						flag2 = true;
						if (timeout != TimeSpan.Zero)
						{
							StopwatchStamp stamp3 = StopwatchStamp.GetStamp();
							Interlocked.Increment(ref this.waitingCount);
							try
							{
								if (lockStatistics == null)
								{
									lockStatistics = this.mostRecentOwner;
								}
								flag = Monitor.TryEnter(this, timeout);
							}
							finally
							{
								Interlocked.Decrement(ref this.waitingCount);
								timeSpentWaiting = stamp3.ElapsedTime;
							}
						}
					}
					if (flag)
					{
						this.mostRecentOwner = lockStats;
					}
				}
				if ((LockManager.trackAllLockAcquisition || flag2) && lockStats != null)
				{
					lockStats.OnAfterLockAcquisition(lockType, flag, flag2, lockStatistics, timeSpentWaiting);
				}
				if (flag)
				{
					this.lastOwnerThreadId = Environment.CurrentManagedThreadId;
				}
				return flag;
			}

			internal void ReleaseLock(LockManager.LockKind lockKind)
			{
				bool flag = true;
				if (lockKind != LockManager.LockKind.Monitor)
				{
					if (lockKind != LockManager.LockKind.Exclusive)
					{
						if (lockKind == LockManager.LockKind.Shared)
						{
							this.rwLock.ExitReadLock();
							flag = (this.rwLock.CurrentReadCount == 0);
						}
					}
					else
					{
						this.rwLock.ExitWriteLock();
					}
				}
				else
				{
					Monitor.Exit(this);
				}
				if (flag)
				{
					this.mostRecentOwner = null;
				}
			}

			public override string ToString()
			{
				return this.lockName.ToString();
			}

			private ReaderWriterLockSlim GetReaderWriterLock()
			{
				if (this.rwLock == null)
				{
					ReaderWriterLockSlim readerWriterLockSlim = ConcurrentLookAside<ReaderWriterLockSlim>.Pool.Get();
					if (readerWriterLockSlim == null)
					{
						readerWriterLockSlim = new ReaderWriterLockSlim();
					}
					if (Interlocked.CompareExchange<ReaderWriterLockSlim>(ref this.rwLock, readerWriterLockSlim, null) != null && !ConcurrentLookAside<ReaderWriterLockSlim>.Pool.Put(readerWriterLockSlim))
					{
						readerWriterLockSlim.Dispose();
					}
				}
				return this.rwLock;
			}

			private const int RefCounterMask = 65535;

			private const int LockReleaserFlag = 16711680;

			private readonly ILockName lockName;

			private ReaderWriterLockSlim rwLock;

			private int refCount;

			private int lastOwnerThreadId;

			private DateTime lastUsed;

			private int waitingCount;

			private ILockStatistics mostRecentOwner;
		}

		public struct ObjectLockFrame : IDisposable
		{
			internal ObjectLockFrame(object lockObject, LockManager.LockType lockType, ILockStatistics lockStats)
			{
				LockManager.GetObjectLockImpl(lockObject, lockType, LockManager.InfiniteTimeout, lockStats);
				this.lockObject = lockObject;
				this.lockType = lockType;
			}

			public void Dispose()
			{
				if (this.lockObject != null)
				{
					LockManager.ReleaseObjectLockImpl(this.lockObject, this.lockType);
				}
			}

			private object lockObject;

			private LockManager.LockType lockType;
		}

		public struct NamedLockFrame : IDisposable
		{
			internal NamedLockFrame(LockManager.NamedLockObject lockObject, LockManager.LockType lockType, ILockStatistics lockStats)
			{
				LockManager.GetNamedLockImpl(lockObject, lockType, LockManager.InfiniteTimeout, lockStats);
				this.lockObject = lockObject;
				this.lockType = lockType;
			}

			public void Dispose()
			{
				if (this.lockObject != null)
				{
					LockManager.ReleaseNamedLockImpl(this.lockObject, this.lockType);
				}
			}

			private LockManager.NamedLockObject lockObject;

			private LockManager.LockType lockType;
		}

		internal class NamedLockObjectsPartition
		{
			public NamedLockObjectsPartition()
			{
				this.cleanupSkipCounter = 0;
				this.lastCleanupTime = DateTime.UtcNow;
				this.lockObjectsDictionary = new Dictionary<ILockName, LockManager.NamedLockObject>(100);
			}

			public Dictionary<ILockName, LockManager.NamedLockObject> LockObjectsDictionary
			{
				get
				{
					return this.lockObjectsDictionary;
				}
			}

			public bool ShouldSkipCleanup()
			{
				return 0 != ++this.cleanupSkipCounter % LockManager.StaleLockCleanupSkipCount;
			}

			public bool TimeToRunCleanup(out DateTime freeLocksUsedBeforeTime)
			{
				DateTime utcNow = DateTime.UtcNow;
				freeLocksUsedBeforeTime = utcNow.Add(-LockManager.StaleLockCleanupInterval);
				if (this.lastCleanupTime <= freeLocksUsedBeforeTime)
				{
					this.lastCleanupTime = utcNow;
					return true;
				}
				return false;
			}

			private int cleanupSkipCounter;

			private DateTime lastCleanupTime;

			private Dictionary<ILockName, LockManager.NamedLockObject> lockObjectsDictionary;
		}
	}
}
