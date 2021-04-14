using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxState : MailboxLockNameBase, INotificationSubscriptionList
	{
		public MailboxState(MailboxStateCache mailboxStateCache, int mailboxNumber, int mailboxPartitionNumber, Guid mailboxGuid, Guid mailboxInstanceGuid, MailboxStatus status, ulong globalIdLowWatermark, ulong globalCnLowWatermark, bool countersAlreadyPatched, bool quarantined, DateTime lastMailboxMaintenanceTime, DateTime lastQuotaCheckTime, TenantHint tenantHint, MailboxInfo.MailboxType mailboxType, MailboxInfo.MailboxTypeDetail mailboxTypeDetail) : base(mailboxStateCache.Database.MdbGuid, mailboxPartitionNumber)
		{
			this.mailboxStateCache = mailboxStateCache;
			this.mailboxNumber = mailboxNumber;
			this.mailboxGuid = mailboxGuid;
			this.mailboxInstanceGuid = mailboxInstanceGuid;
			this.status = status;
			this.globalIdLowWatermark = globalIdLowWatermark;
			this.globalCnLowWatermark = globalCnLowWatermark;
			this.countersAlreadyPatched = countersAlreadyPatched;
			this.quarantined = quarantined;
			this.digestCollector = mailboxStateCache.Database.ResourceDigest.CreateDigestCollector(mailboxGuid, mailboxNumber);
			this.tenantHint = tenantHint;
			this.mailboxType = mailboxType;
			this.mailboxTypeDetail = mailboxTypeDetail;
			this.lastMailboxMaintenanceTime = lastMailboxMaintenanceTime;
			this.lastQuotaCheckTime = lastQuotaCheckTime;
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public bool SupportsPerUserFeatures
		{
			get
			{
				return this.mailboxTypeDetail == MailboxInfo.MailboxTypeDetail.TeamMailbox || this.mailboxType == MailboxInfo.MailboxType.PublicFolderPrimary || this.mailboxType == MailboxInfo.MailboxType.PublicFolderSecondary;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.status != MailboxStatus.Invalid;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.status == MailboxStatus.New;
			}
		}

		public bool IsUserAccessible
		{
			get
			{
				return this.status == MailboxStatus.UserAccessible;
			}
		}

		public bool IsDisabled
		{
			get
			{
				return this.status == MailboxStatus.Disabled;
			}
		}

		public bool IsSoftDeleted
		{
			get
			{
				return this.status == MailboxStatus.SoftDeleted;
			}
		}

		public bool IsDisconnected
		{
			get
			{
				return this.IsDisabled || this.IsSoftDeleted;
			}
		}

		public bool IsAccessible
		{
			get
			{
				return this.IsUserAccessible || this.IsDisconnected;
			}
		}

		public bool IsHardDeleted
		{
			get
			{
				return this.status == MailboxStatus.HardDeleted;
			}
		}

		public bool IsTombstone
		{
			get
			{
				return this.status == MailboxStatus.Tombstone;
			}
		}

		public bool IsRemoved
		{
			get
			{
				return this.IsHardDeleted || this.IsTombstone;
			}
		}

		public bool IsCurrent
		{
			get
			{
				return this.IsAccessible || this.IsRemoved;
			}
		}

		public bool IsDeleted
		{
			get
			{
				return this.IsSoftDeleted || this.IsRemoved;
			}
		}

		public DateTime InvalidatedTime
		{
			get
			{
				return this.invalidatedTime;
			}
		}

		public MailboxStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public StoreDatabase Database
		{
			get
			{
				return this.MailboxStateCache.Database;
			}
		}

		public override Guid DatabaseGuid
		{
			get
			{
				return this.MailboxStateCache.Database.MdbGuid;
			}
		}

		public MailboxStateCache MailboxStateCache
		{
			get
			{
				return this.mailboxStateCache;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public bool IsPublicFolderMailbox
		{
			get
			{
				return this.MailboxType == MailboxInfo.MailboxType.PublicFolderPrimary || this.MailboxType == MailboxInfo.MailboxType.PublicFolderSecondary;
			}
		}

		public bool IsGroupMailbox
		{
			get
			{
				return this.MailboxTypeDetail == MailboxInfo.MailboxTypeDetail.GroupMailbox;
			}
		}

		public bool IsNewMailboxPartition
		{
			get
			{
				return this.unifiedMailboxState != null && this.IsNew && this.MailboxNumber == base.MailboxPartitionNumber;
			}
		}

		public Guid MailboxInstanceGuid
		{
			get
			{
				return this.mailboxInstanceGuid;
			}
			set
			{
				this.mailboxInstanceGuid = value;
			}
		}

		public override LockManager.NamedLockObject CachedLockObject
		{
			get
			{
				if (this.unifiedMailboxState != null)
				{
					return this.unifiedMailboxState.CachedLockObject;
				}
				return base.CachedLockObject;
			}
			set
			{
				if (this.unifiedMailboxState != null)
				{
					this.unifiedMailboxState.CachedLockObject = value;
					return;
				}
				base.CachedLockObject = value;
			}
		}

		public ulong GlobalIdLowWatermark
		{
			get
			{
				if (this.unifiedMailboxState != null)
				{
					return this.unifiedMailboxState.GlobalIdLowWatermark;
				}
				return this.globalIdLowWatermark;
			}
			set
			{
				if (this.unifiedMailboxState != null)
				{
					this.unifiedMailboxState.GlobalIdLowWatermark = value;
					return;
				}
				this.globalIdLowWatermark = value;
			}
		}

		public ulong GlobalCnLowWatermark
		{
			get
			{
				if (this.unifiedMailboxState != null)
				{
					return this.unifiedMailboxState.GlobalCnLowWatermark;
				}
				return this.globalCnLowWatermark;
			}
			set
			{
				if (this.unifiedMailboxState != null)
				{
					this.unifiedMailboxState.GlobalCnLowWatermark = value;
					return;
				}
				this.globalCnLowWatermark = value;
			}
		}

		public bool CountersAlreadyPatched
		{
			get
			{
				if (this.unifiedMailboxState != null)
				{
					return this.unifiedMailboxState.CountersAlreadyPatched;
				}
				return this.countersAlreadyPatched;
			}
			set
			{
				if (this.unifiedMailboxState != null)
				{
					this.unifiedMailboxState.CountersAlreadyPatched = value;
					return;
				}
				this.countersAlreadyPatched = value;
			}
		}

		public DateTime UtcNow
		{
			get
			{
				DeterministicTime deterministicTime = (DeterministicTime)this.GetComponentData(MailboxState.deterministicTimeSlot);
				if (deterministicTime == null)
				{
					deterministicTime = new DeterministicTime();
					DeterministicTime deterministicTime2 = (DeterministicTime)this.CompareExchangeComponentData(MailboxState.deterministicTimeSlot, null, deterministicTime);
					if (deterministicTime2 != null)
					{
						deterministicTime = deterministicTime2;
					}
				}
				DateTime utcNow;
				using (LockManager.Lock(deterministicTime))
				{
					utcNow = deterministicTime.UtcNow;
				}
				return utcNow;
			}
		}

		public bool Quarantined
		{
			get
			{
				return this.quarantined;
			}
			set
			{
				this.quarantined = value;
			}
		}

		public IDigestCollector ActivityDigestCollector
		{
			get
			{
				return this.digestCollector;
			}
		}

		public TenantHint TenantHint
		{
			get
			{
				return this.tenantHint;
			}
			set
			{
				this.tenantHint = value;
			}
		}

		public MailboxInfo.MailboxType MailboxType
		{
			get
			{
				return this.mailboxType;
			}
			set
			{
				this.mailboxType = value;
			}
		}

		public MailboxInfo.MailboxTypeDetail MailboxTypeDetail
		{
			get
			{
				return this.mailboxTypeDetail;
			}
			set
			{
				this.mailboxTypeDetail = value;
			}
		}

		public DateTime LastUpdatedActiveTime
		{
			get
			{
				MailboxState.ActiveMailboxData activeMailboxDataNoCreate = this.GetActiveMailboxDataNoCreate();
				if (activeMailboxDataNoCreate == null)
				{
					return DateTime.MinValue;
				}
				return activeMailboxDataNoCreate.LastUpdatedActiveTime;
			}
			set
			{
				MailboxState.ActiveMailboxData activeMailboxData = this.GetActiveMailboxData();
				activeMailboxData.LastUpdatedActiveTime = value;
			}
		}

		public DateTime LastQuotaCheckTime
		{
			get
			{
				return this.lastQuotaCheckTime;
			}
			set
			{
				this.lastQuotaCheckTime = value;
			}
		}

		public DateTime LastMailboxMaintenanceTime
		{
			get
			{
				return this.lastMailboxMaintenanceTime;
			}
			set
			{
				this.lastMailboxMaintenanceTime = value;
			}
		}

		public ICache PerUserCache
		{
			get
			{
				MailboxState.ActiveMailboxData activeMailboxDataNoCreate = this.GetActiveMailboxDataNoCreate();
				if (activeMailboxDataNoCreate == null)
				{
					return null;
				}
				return activeMailboxDataNoCreate.PerUserCache;
			}
			set
			{
				MailboxState.ActiveMailboxData activeMailboxData = this.GetActiveMailboxData();
				activeMailboxData.PerUserCache = value;
			}
		}

		internal bool CurrentlyActive
		{
			get
			{
				return this.currentlyActive;
			}
			set
			{
				this.currentlyActive = value;
			}
		}

		internal bool HasComponentDataForTest
		{
			get
			{
				return !this.componentData.IsEmpty && (this.unifiedMailboxState == null || !this.unifiedMailboxState.HasComponentDataForTest);
			}
		}

		internal MailboxState.UnifiedMailboxState UnifiedState
		{
			get
			{
				return this.unifiedMailboxState;
			}
		}

		public static void GetMailboxLock(Guid databaseGuid, int mailboxPartitionNumber, bool shared, ILockStatistics lockStats)
		{
			IMailboxLockName mailboxLockName = MailboxLockNameBase.GetMailboxLockName(databaseGuid, mailboxPartitionNumber);
			MailboxState.GetMailboxLock(mailboxLockName, shared, lockStats);
		}

		public static bool TryGetMailboxLock(Guid databaseGuid, int mailboxPartitionNumber, bool shared, TimeSpan timeout, ILockStatistics lockStats)
		{
			IMailboxLockName mailboxLockName = MailboxLockNameBase.GetMailboxLockName(databaseGuid, mailboxPartitionNumber);
			return MailboxState.TryGetMailboxLock(mailboxLockName, shared, timeout, lockStats);
		}

		public static void ReleaseMailboxLock(Guid databaseGuid, int mailboxPartitionNumber, bool shared)
		{
			IMailboxLockName mailboxLockName = MailboxLockNameBase.GetMailboxLockName(databaseGuid, mailboxPartitionNumber);
			MailboxState.ReleaseMailboxLock(mailboxLockName, shared);
		}

		public static int AllocateComponentDataSlot(bool privateSlot)
		{
			return MailboxState.ComponentDataStorage.AllocateSlot(privateSlot);
		}

		public override ILockName GetLockNameToCache()
		{
			return MailboxLockNameBase.GetMailboxLockName(this.DatabaseGuid, base.MailboxPartitionNumber);
		}

		public override string GetFriendlyNameForLogging()
		{
			if (this.UnifiedState == null)
			{
				return this.MailboxGuid.ToString();
			}
			return this.UnifiedState.GetFriendlyNameForLogging() + this.MailboxGuid.ToString();
		}

		public void InvalidateLogons()
		{
			this.invalidatedTime = this.UtcNow;
		}

		public void GetMailboxLock(bool shared, ILockStatistics lockStats)
		{
			MailboxState.GetMailboxLock(this, shared, lockStats);
		}

		public bool TryGetMailboxLock(bool shared, TimeSpan timeout, ILockStatistics lockStats)
		{
			return MailboxState.TryGetMailboxLock(this, shared, timeout, lockStats);
		}

		public void ReleaseMailboxLock(bool shared)
		{
			MailboxState.ReleaseMailboxLock(this, shared);
		}

		public void AddReference()
		{
			using (LockManager.Lock(this))
			{
				this.referenceCount++;
			}
		}

		public void ReleaseReference()
		{
			using (LockManager.Lock(this))
			{
				this.referenceCount--;
				if (this.referenceCount == 0)
				{
					MailboxState.NotificationSubscriptions notificationSubscriptions = (MailboxState.NotificationSubscriptions)this.componentData[MailboxState.mailboxSubscriptionListSlot];
					this.componentData[MailboxState.mailboxSubscriptionListSlot] = null;
					this.componentData[MailboxState.deterministicTimeSlot] = null;
				}
			}
		}

		public void DangerousReleaseReference()
		{
			using (LockManager.Lock(this))
			{
				this.referenceCount--;
			}
		}

		public void RegisterSubscription(NotificationSubscription subscription)
		{
			MailboxState.NotificationSubscriptions notificationSubscriptions = (MailboxState.NotificationSubscriptions)this.GetComponentData(MailboxState.mailboxSubscriptionListSlot);
			if (notificationSubscriptions == null)
			{
				notificationSubscriptions = new MailboxState.NotificationSubscriptions(10);
				MailboxState.NotificationSubscriptions notificationSubscriptions2 = (MailboxState.NotificationSubscriptions)this.CompareExchangeComponentData(MailboxState.mailboxSubscriptionListSlot, null, notificationSubscriptions);
				if (notificationSubscriptions2 != null)
				{
					notificationSubscriptions = notificationSubscriptions2;
				}
			}
			using (LockManager.Lock(this))
			{
				notificationSubscriptions.Add(subscription);
			}
		}

		public void UnregisterSubscription(NotificationSubscription subscription)
		{
			MailboxState.NotificationSubscriptions notificationSubscriptions = (MailboxState.NotificationSubscriptions)this.GetComponentData(MailboxState.mailboxSubscriptionListSlot);
			using (LockManager.Lock(this))
			{
				if (notificationSubscriptions != null)
				{
					notificationSubscriptions.Remove(subscription);
				}
			}
		}

		public void EnumerateSubscriptionsForEvent(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev, SubscriptionEnumerationCallback callback)
		{
			NotificationSubscription[] array = null;
			using (LockManager.Lock(this, transactionContext.Diagnostics))
			{
				MailboxState.NotificationSubscriptions notificationSubscriptions = (MailboxState.NotificationSubscriptions)this.GetComponentData(MailboxState.mailboxSubscriptionListSlot);
				if (notificationSubscriptions != null)
				{
					array = notificationSubscriptions.ToArray();
				}
			}
			if (this.referenceCount > 0 && array != null)
			{
				foreach (NotificationSubscription notificationSubscription in array)
				{
					if ((notificationSubscription.EventTypeValueMask & nev.EventTypeValue) != 0 && (notificationSubscription.Kind & (SubscriptionKind)phase) != (SubscriptionKind)0U)
					{
						if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.NotificationTracer.TraceDebug<NotificationEvent>(30652L, "MbxStateNotifEnumeration: {0}", nev);
						}
						callback(phase, transactionContext, notificationSubscription, nev);
					}
					else if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.NotificationTracer.TraceDebug<NotificationEvent, NotificationSubscription>(35961L, "MbxStateNotifEnumeration: skipping callback for {0}, {1}", nev, notificationSubscription);
					}
				}
			}
		}

		public void SetUserAccessible()
		{
			Globals.AssertRetail(this.status == MailboxStatus.New || this.status == MailboxStatus.Disabled, "unexpected mailbox status");
			this.status = MailboxStatus.UserAccessible;
		}

		public void SetTombstoned()
		{
			Globals.AssertRetail(this.status == MailboxStatus.HardDeleted, "unexpected mailbox status");
			this.status = MailboxStatus.Tombstone;
		}

		public object GetComponentData(int slotNumber)
		{
			if (this.unifiedMailboxState != null && !MailboxState.ComponentDataStorage.IsPrivateSlot(slotNumber))
			{
				return this.unifiedMailboxState.GetComponentData(slotNumber);
			}
			return this.componentData[slotNumber];
		}

		public void SetComponentData(int slotNumber, object value)
		{
			if (this.unifiedMailboxState != null && !MailboxState.ComponentDataStorage.IsPrivateSlot(slotNumber))
			{
				this.unifiedMailboxState.SetComponentData(slotNumber, value);
			}
			else
			{
				using (LockManager.Lock(this))
				{
					this.componentData[slotNumber] = value;
				}
			}
			if (value != null && !this.CurrentlyActive)
			{
				MailboxStateCache.OnMailboxActivity(this);
			}
		}

		public object CompareExchangeComponentData(int slotNumber, object comparand, object value)
		{
			object obj;
			if (this.unifiedMailboxState != null && !MailboxState.ComponentDataStorage.IsPrivateSlot(slotNumber))
			{
				obj = this.unifiedMailboxState.CompareExchangeComponentData(slotNumber, comparand, value);
			}
			else
			{
				obj = this.componentData.CompareExchange(slotNumber, comparand, value);
			}
			bool flag = object.ReferenceEquals(obj, comparand);
			if (flag && value != null && !this.CurrentlyActive)
			{
				MailboxStateCache.OnMailboxActivity(this);
			}
			return obj;
		}

		public void CleanupAsNonActive(Context context)
		{
			this.CleanupAsNonActive(context, true);
		}

		public void CleanupAsNonActive(Context context, bool doFlush)
		{
			if (this.PerUserCache != null)
			{
				if (doFlush && this.PerUserCache.FlushAllDirtyEntries(context))
				{
					context.Commit();
				}
				this.PerUserCache = null;
			}
			this.componentData.CleanupDataSlots(context);
			if (this.unifiedMailboxState != null)
			{
				this.unifiedMailboxState.CleanupAsNonActive(context, doFlush);
			}
			this.CachedLockObject = null;
		}

		public bool CleanupDataSlots(Context context)
		{
			this.componentData.CleanupDataSlots(context);
			bool flag = true;
			if (this.unifiedMailboxState != null)
			{
				flag = this.unifiedMailboxState.CleanupDataSlots(context);
			}
			return this.componentData.IsEmpty && flag;
		}

		internal static void Initialize()
		{
			if (MailboxState.mailboxSubscriptionListSlot == -1)
			{
				MailboxState.mailboxSubscriptionListSlot = MailboxState.AllocateComponentDataSlot(true);
			}
			if (MailboxState.deterministicTimeSlot == -1)
			{
				MailboxState.deterministicTimeSlot = MailboxState.AllocateComponentDataSlot(true);
			}
			if (MailboxState.activeMailboxDataSlot == -1)
			{
				MailboxState.activeMailboxDataSlot = MailboxState.AllocateComponentDataSlot(true);
			}
		}

		internal void Invalidate(Context context)
		{
			this.componentData.CleanupDataSlots(context);
			if (this.unifiedMailboxState != null)
			{
				this.unifiedMailboxState.CleanupDataSlots(context);
			}
			this.status = MailboxStatus.Invalid;
			this.mailboxStateCache.AddToPostDisposeCleanupList(this);
		}

		internal void SetUnifiedMailboxState(MailboxState.UnifiedMailboxState unifiedMailboxState)
		{
			this.unifiedMailboxState = unifiedMailboxState;
		}

		private static void GetMailboxLock(IMailboxLockName mailboxLockName, bool shared, ILockStatistics lockStats)
		{
			MailboxState.TryGetMailboxLock(mailboxLockName, shared, LockManager.InfiniteTimeout, lockStats);
		}

		private static bool TryGetMailboxLock(IMailboxLockName mailboxLockName, bool shared, TimeSpan timeout, ILockStatistics lockStats)
		{
			LockManager.LockType lockType = shared ? LockManager.LockType.MailboxShared : LockManager.LockType.MailboxExclusive;
			return LockManager.TryGetLock(mailboxLockName, lockType, timeout, lockStats);
		}

		private static void ReleaseMailboxLock(IMailboxLockName mailboxLockName, bool shared)
		{
			LockManager.LockType lockType = shared ? LockManager.LockType.MailboxShared : LockManager.LockType.MailboxExclusive;
			LockManager.ReleaseLock(mailboxLockName, lockType);
		}

		private MailboxState.ActiveMailboxData GetActiveMailboxDataNoCreate()
		{
			return (MailboxState.ActiveMailboxData)this.GetComponentData(MailboxState.activeMailboxDataSlot);
		}

		private MailboxState.ActiveMailboxData GetActiveMailboxData()
		{
			MailboxState.ActiveMailboxData activeMailboxData = this.GetActiveMailboxDataNoCreate();
			if (activeMailboxData == null)
			{
				activeMailboxData = new MailboxState.ActiveMailboxData(this);
				this.SetComponentData(MailboxState.activeMailboxDataSlot, activeMailboxData);
			}
			return activeMailboxData;
		}

		public const int PartitionScopeMailboxNumber = -1;

		private const int AvgNotificationSubscriptionsPerMailbox = 10;

		private static int mailboxSubscriptionListSlot = -1;

		private static int deterministicTimeSlot = -1;

		private static int activeMailboxDataSlot = -1;

		private readonly MailboxStateCache mailboxStateCache;

		private readonly Guid mailboxGuid;

		private readonly int mailboxNumber;

		private DateTime invalidatedTime;

		private DateTime lastMailboxMaintenanceTime;

		private DateTime lastQuotaCheckTime;

		private Guid mailboxInstanceGuid;

		private MailboxState.ComponentDataStorage componentData = new MailboxState.ComponentDataStorage();

		private ulong globalIdLowWatermark;

		private ulong globalCnLowWatermark;

		private bool countersAlreadyPatched;

		private bool quarantined;

		private bool currentlyActive;

		private MailboxInfo.MailboxType mailboxType;

		private MailboxInfo.MailboxTypeDetail mailboxTypeDetail;

		private MailboxStatus status;

		private int referenceCount;

		private IDigestCollector digestCollector;

		private TenantHint tenantHint;

		private MailboxState.UnifiedMailboxState unifiedMailboxState;

		internal class UnifiedMailboxState : MailboxLockNameBase
		{
			public UnifiedMailboxState(MailboxStateCache mailboxStateCache, Guid unifiedMailboxGuid, int mailboxPartitionNumber, ulong globalIdLowWatermark, ulong globalCnLowWatermark, bool countersAlreadyPatched) : base(mailboxStateCache.Database.MdbGuid, mailboxPartitionNumber)
			{
				this.mailboxStateCache = mailboxStateCache;
				this.unifiedMailboxGuid = unifiedMailboxGuid;
				this.globalIdLowWatermark = globalIdLowWatermark;
				this.globalCnLowWatermark = globalCnLowWatermark;
				this.countersAlreadyPatched = countersAlreadyPatched;
			}

			public Guid UnifiedMailboxGuid
			{
				get
				{
					return this.unifiedMailboxGuid;
				}
			}

			public override Guid DatabaseGuid
			{
				get
				{
					return this.mailboxStateCache.Database.MdbGuid;
				}
			}

			public ulong GlobalIdLowWatermark
			{
				get
				{
					return this.globalIdLowWatermark;
				}
				set
				{
					this.globalIdLowWatermark = value;
				}
			}

			public ulong GlobalCnLowWatermark
			{
				get
				{
					return this.globalCnLowWatermark;
				}
				set
				{
					this.globalCnLowWatermark = value;
				}
			}

			public bool CountersAlreadyPatched
			{
				get
				{
					return this.countersAlreadyPatched;
				}
				set
				{
					this.countersAlreadyPatched = value;
				}
			}

			public void SetNewUnfiedMailboxGuid(Guid newUnifiedMailboxGuid)
			{
				this.unifiedMailboxGuid = newUnifiedMailboxGuid;
			}

			public override ILockName GetLockNameToCache()
			{
				return MailboxLockNameBase.GetMailboxLockName(this.DatabaseGuid, base.MailboxPartitionNumber);
			}

			public override string GetFriendlyNameForLogging()
			{
				return string.Format("[{0}]", this.unifiedMailboxGuid);
			}

			public object GetComponentData(int slotNumber)
			{
				return this.componentData[slotNumber];
			}

			public void SetComponentData(int slotNumber, object value)
			{
				using (LockManager.Lock(this))
				{
					this.componentData[slotNumber] = value;
				}
			}

			public object CompareExchangeComponentData(int slotNumber, object comparand, object value)
			{
				return this.componentData.CompareExchange(slotNumber, comparand, value);
			}

			public void CleanupAsNonActive(Context context)
			{
				this.CleanupAsNonActive(context, true);
			}

			public void CleanupAsNonActive(Context context, bool doFlush)
			{
				this.componentData.CleanupDataSlots(context);
				this.CachedLockObject = null;
			}

			public bool CleanupDataSlots(Context context)
			{
				this.componentData.CleanupDataSlots(context);
				return this.componentData.IsEmpty;
			}

			internal bool HasComponentDataForTest
			{
				get
				{
					return !this.componentData.IsEmpty;
				}
			}

			private readonly MailboxStateCache mailboxStateCache;

			private Guid unifiedMailboxGuid;

			private MailboxState.ComponentDataStorage componentData = new MailboxState.ComponentDataStorage();

			private ulong globalIdLowWatermark;

			private ulong globalCnLowWatermark;

			private bool countersAlreadyPatched;
		}

		private class ComponentDataStorage : ComponentDataStorageBase
		{
			internal static bool IsPrivateSlot(int slotNumber)
			{
				return MailboxState.ComponentDataStorage.slotsRegistration[slotNumber];
			}

			internal static int AllocateSlot(bool privateSlot)
			{
				MailboxState.ComponentDataStorage.slotsRegistration.Add(privateSlot);
				return MailboxState.ComponentDataStorage.slotsRegistration.Count - 1;
			}

			internal override int SlotCount
			{
				get
				{
					return MailboxState.ComponentDataStorage.slotsRegistration.Count;
				}
			}

			private static List<bool> slotsRegistration = new List<bool>();
		}

		private class ActiveMailboxData
		{
			internal ActiveMailboxData(MailboxState mailboxState)
			{
			}

			internal DateTime LastUpdatedActiveTime
			{
				get
				{
					return this.lastUpdatedActiveTime;
				}
				set
				{
					this.lastUpdatedActiveTime = value;
				}
			}

			internal ICache PerUserCache
			{
				get
				{
					return this.perUserCache;
				}
				set
				{
					if (value != null)
					{
						Interlocked.CompareExchange<ICache>(ref this.perUserCache, value, null);
						return;
					}
					this.perUserCache = null;
				}
			}

			private DateTime lastUpdatedActiveTime;

			private ICache perUserCache;
		}

		private class NotificationSubscriptions : List<NotificationSubscription>, IComponentData
		{
			internal NotificationSubscriptions(int initialCapacity) : base(initialCapacity)
			{
			}

			bool IComponentData.DoCleanup(Context context)
			{
				return base.Count == 0;
			}
		}
	}
}
