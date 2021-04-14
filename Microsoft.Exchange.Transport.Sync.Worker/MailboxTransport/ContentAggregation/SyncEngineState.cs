using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncEngineState : DisposeTrackableBase
	{
		internal SyncEngineState(ISubscriptionInformationLoader subscriptionInformationLoader, SyncLogSession syncLogSession, bool originalIsUnderRecoveryFlag, SyncPoisonStatus subscriptionPoisonStatus, SyncHealthData syncHealthData, string subscriptionPoisonCallstack, string legacyDn, MailSubmitter mailSubmitter, SyncMode syncMode, SyncStorageProviderConnectionStatistics connectionStatistics, ISyncWorkerData mailboxServerSubscription, IRemoteServerHealthChecker remoteServerHealthChecker)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformationLoader", subscriptionInformationLoader);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("syncHealthData", syncHealthData);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("legacyDn", legacyDn);
			SyncUtilities.ThrowIfArgumentNull("mailSubmitter", mailSubmitter);
			SyncUtilities.ThrowIfArgumentNull("connectionStatistics", connectionStatistics);
			SyncUtilities.ThrowIfArgumentNull("remoteServerHealthChecker", remoteServerHealthChecker);
			this.mailboxServerSubscription = mailboxServerSubscription;
			if (this.mailboxServerSubscription != null)
			{
				this.OnSubscriptionLoad(mailboxServerSubscription);
			}
			this.subscriptionInformationLoader = subscriptionInformationLoader;
			this.connectionStatistics = connectionStatistics;
			this.syncMailboxSession = new SyncMailboxSession(syncLogSession);
			this.syncLogSession = syncLogSession;
			this.startSyncTime = ExDateTime.UtcNow;
			this.originalIsUnderRecoveryFlag = originalIsUnderRecoveryFlag;
			this.subscriptionPoisonStatus = subscriptionPoisonStatus;
			this.syncHealthData = syncHealthData;
			this.subscriptionPoisonCallstack = subscriptionPoisonCallstack;
			this.legacyDn = legacyDn;
			this.mailSubmitter = mailSubmitter;
			this.syncMode = syncMode;
			this.remoteServerHealthChecker = remoteServerHealthChecker;
		}

		internal SyncProgress PreviousSyncProgress
		{
			get
			{
				return this.previousSyncProgress.Value;
			}
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				base.CheckDisposed();
				return this.organizationId;
			}
		}

		internal ISyncWorkerData MailboxServerSubscription
		{
			get
			{
				base.CheckDisposed();
				return this.mailboxServerSubscription;
			}
		}

		internal ISyncWorkerData UserMailboxSubscription
		{
			get
			{
				base.CheckDisposed();
				return this.userMailboxSubscription;
			}
		}

		internal SyncMailboxSession SyncMailboxSession
		{
			get
			{
				base.CheckDisposed();
				return this.syncMailboxSession;
			}
		}

		internal NativeSyncStorageProvider NativeProvider
		{
			get
			{
				base.CheckDisposed();
				return this.nativeProvider;
			}
		}

		internal ISyncStorageProvider CloudProvider
		{
			get
			{
				base.CheckDisposed();
				return this.cloudProvider;
			}
		}

		internal IStateStorage StateStorage
		{
			get
			{
				base.CheckDisposed();
				return this.stateStorage;
			}
		}

		internal AsyncOperationResult<SyncProviderResultData> LastSyncOperationResult
		{
			get
			{
				base.CheckDisposed();
				return this.lastSyncOperationResult;
			}
			set
			{
				base.CheckDisposed();
				this.lastSyncOperationResult = value;
			}
		}

		internal SyncEngineStep CurrentStep
		{
			get
			{
				base.CheckDisposed();
				return this.currentStep;
			}
			set
			{
				base.CheckDisposed();
				this.currentStep = value;
			}
		}

		internal SyncEngineStep? ContinutationSyncStep
		{
			get
			{
				base.CheckDisposed();
				return this.continuationSyncStep;
			}
			set
			{
				base.CheckDisposed();
				if (value == SyncEngineStep.PreSyncStepInEnumerateChangesMode || value == SyncEngineStep.PreSyncStepInCheckForChangesMode)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Setting Invalid ContinutationSyncStep:{0}, it should not be a PreSyncStep.", new object[]
					{
						value
					}), "ContinutationSyncStep");
				}
				this.continuationSyncStep = value;
			}
		}

		internal SyncLogSession SyncLogSession
		{
			get
			{
				base.CheckDisposed();
				return this.syncLogSession;
			}
		}

		internal ExDateTime StartSyncTime
		{
			get
			{
				base.CheckDisposed();
				return this.startSyncTime;
			}
		}

		internal NativeSyncStorageProviderState NativeProviderState
		{
			get
			{
				base.CheckDisposed();
				return this.nativeProviderState;
			}
		}

		internal SyncStorageProviderState CloudProviderState
		{
			get
			{
				base.CheckDisposed();
				return this.cloudProviderState;
			}
		}

		internal SyncStorageProviderConnectionStatistics ConnectionStatistics
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics;
			}
		}

		internal bool OriginalIsUnderRecoveryFlag
		{
			get
			{
				base.CheckDisposed();
				return this.originalIsUnderRecoveryFlag;
			}
		}

		internal bool UnderRetry
		{
			get
			{
				base.CheckDisposed();
				return this.underRetry;
			}
		}

		internal void SetSyncUnderRetry()
		{
			base.CheckDisposed();
			this.underRetry = true;
		}

		internal bool MoreItemsAvailable
		{
			get
			{
				base.CheckDisposed();
				return this.moreItemsAvailable;
			}
		}

		internal SyncPhase SyncPhaseBeforeSync
		{
			get
			{
				base.CheckDisposed();
				return this.syncPhaseBeforeSync;
			}
		}

		internal AggregationStatus UserMailboxSubscriptionStatusBeforeSync
		{
			get
			{
				base.CheckDisposed();
				return this.userMailboxSubscriptionStatusBeforeSync;
			}
		}

		public bool SubscriptionNotificationSent
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionNotificationSent;
			}
		}

		internal int CloudItemsSynced
		{
			get
			{
				base.CheckDisposed();
				return this.cloudItemsSynced;
			}
			set
			{
				base.CheckDisposed();
				this.cloudItemsSynced = value;
			}
		}

		internal string UpdatedSyncWatermark
		{
			get
			{
				base.CheckDisposed();
				return this.updatedSyncWatermark;
			}
			set
			{
				base.CheckDisposed();
				SyncUtilities.ThrowIfArgumentNull("UpdatedSyncWatermark", value);
				this.updatedSyncWatermark = value;
			}
		}

		internal SyncHealthData SyncHealthData
		{
			get
			{
				base.CheckDisposed();
				return this.syncHealthData;
			}
		}

		internal object SyncRoot
		{
			get
			{
				base.CheckDisposed();
				return this.syncRoot;
			}
		}

		internal int TotalSuccessfulRoundtrips
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.TotalSuccessfulRoundtrips;
			}
		}

		internal TimeSpan AverageSuccessfulRoundtripTime
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.AverageSuccessfulRoundtripTime;
			}
		}

		internal int TotalUnsuccessfulRoundtrips
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.TotalUnsuccessfulRoundtrips;
			}
		}

		internal TimeSpan AverageUnsuccessfulRoundtripTime
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.AverageUnsuccessfulRoundtripTime;
			}
		}

		internal TimeSpan AverageBackoffTime
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.AverageBackoffTime;
			}
		}

		internal ThrottlingStatistics ThrottlingStatistics
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.ThrottlingStatistics;
			}
		}

		internal bool TryCancel
		{
			get
			{
				base.CheckDisposed();
				return this.tryCancel;
			}
		}

		internal SyncPoisonStatus SubscriptionPoisonStatus
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionPoisonStatus;
			}
		}

		internal string SubscriptionPoisonCallstack
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionPoisonCallstack;
			}
		}

		internal string LegacyDN
		{
			get
			{
				base.CheckDisposed();
				return this.legacyDn;
			}
		}

		internal bool HasTransientItemLevelErrors
		{
			get
			{
				base.CheckDisposed();
				return this.hasTransientItemLevelErrors;
			}
			set
			{
				base.CheckDisposed();
				this.hasTransientItemLevelErrors = value;
			}
		}

		internal bool WasSyncInterrupted
		{
			get
			{
				base.CheckDisposed();
				return this.syncInterrupted;
			}
		}

		internal ISubscriptionInformationLoader SubscriptionInformationLoader
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionInformationLoader;
			}
		}

		public SyncMode SyncMode
		{
			get
			{
				return this.syncMode;
			}
		}

		public void SwitchToEnumerateChangeMode()
		{
			this.syncMode = SyncMode.EnumerateChangesMode;
		}

		public bool WasAttemptMadeToOpenMailboxSession
		{
			get
			{
				return this.syncMode == SyncMode.EnumerateChangesMode;
			}
		}

		public IRemoteServerHealthChecker RemoteServerHealthChecker
		{
			get
			{
				return this.remoteServerHealthChecker;
			}
		}

		public void SetUserMailboxSubscription(ISyncWorkerData newSubscription)
		{
			SyncUtilities.ThrowIfArgumentNull("newSubscription", newSubscription);
			this.PreserveNewestDatesFromMailboxServerSubscriptionOn(newSubscription);
			this.userMailboxSubscriptionCachedLastSyncTime = newSubscription.LastSyncTime;
			this.userMailboxSubscriptionStatusBeforeSync = newSubscription.Status;
			this.userMailboxSubscription = newSubscription;
			this.OnSubscriptionLoad(newSubscription);
		}

		public void SetNativeProvider(NativeSyncStorageProvider nativeSyncStorageProvider)
		{
			SyncUtilities.ThrowIfArgumentNull("nativeSyncStorageProvider", nativeSyncStorageProvider);
			this.nativeProvider = nativeSyncStorageProvider;
		}

		public void SetCloudProvider(ISyncStorageProvider cloudSyncStorageProvider)
		{
			SyncUtilities.ThrowIfArgumentNull("cloudSyncStorageProvider", cloudSyncStorageProvider);
			this.cloudProvider = cloudSyncStorageProvider;
		}

		public void SetPreviousSyncProgress(SyncProgress previousSyncProgress)
		{
			if (this.previousSyncProgress != null)
			{
				throw new InvalidOperationException("previousSyncProgress already has a value, but SetPreviousSyncProgress must only be called once for the lifetime of SyncEngineState");
			}
			this.previousSyncProgress = new SyncProgress?(previousSyncProgress);
		}

		private void PreserveNewestDatesFromMailboxServerSubscriptionOn(ISyncWorkerData newSubscription)
		{
			if (this.mailboxServerSubscription == null || this.mailboxServerSubscription.LastSyncTime == null)
			{
				return;
			}
			if (newSubscription.LastSyncTime == null)
			{
				this.CopyDatesFromExistingSubscriptionInto(newSubscription);
				return;
			}
			if (this.mailboxServerSubscription.LastSyncTime.Value >= newSubscription.LastSyncTime.Value)
			{
				this.CopyDatesFromExistingSubscriptionInto(newSubscription);
			}
		}

		private void CopyDatesFromExistingSubscriptionInto(ISyncWorkerData newSubscription)
		{
			this.syncLogSession.LogDebugging((TSLID)1162UL, "CopyDatesFromExistingSubscriptionInto::LastSyncTime:{0},LastSuccessfulSyncTime:{1},AdjustedLastSuccessfulSyncTime:{2}", new object[]
			{
				this.mailboxServerSubscription.LastSyncTime,
				this.mailboxServerSubscription.LastSuccessfulSyncTime,
				this.mailboxServerSubscription.AdjustedLastSuccessfulSyncTime
			});
			newSubscription.LastSyncTime = this.mailboxServerSubscription.LastSyncTime;
			newSubscription.LastSuccessfulSyncTime = this.mailboxServerSubscription.LastSuccessfulSyncTime;
			newSubscription.AdjustedLastSuccessfulSyncTime = this.mailboxServerSubscription.AdjustedLastSuccessfulSyncTime;
		}

		public override string ToString()
		{
			base.CheckDisposed();
			return string.Format(CultureInfo.InvariantCulture, "Step: {0}, MoreItemsAvailable: {1}", new object[]
			{
				this.currentStep,
				this.moreItemsAvailable
			});
		}

		internal void SetMoreItemsAvailable()
		{
			base.CheckDisposed();
			this.moreItemsAvailable = true;
		}

		internal void SetTryCancel()
		{
			base.CheckDisposed();
			this.tryCancel = true;
		}

		internal void OnRoundtripComplete(object sender, RoundtripCompleteEventArgs roundtripCompleteEventArgs)
		{
			base.CheckDisposed();
			if (roundtripCompleteEventArgs == null)
			{
				throw new ArgumentNullException("roundtripCompleteEventArgs");
			}
			this.connectionStatistics.OnRoundtripComplete(sender, roundtripCompleteEventArgs);
		}

		public Exception CommitStateStorage(bool commitState)
		{
			base.CheckDisposed();
			return this.stateStorage.Commit(commitState, this.SyncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(this.OnRoundtripComplete));
		}

		internal void SetSyncInterrupted()
		{
			base.CheckDisposed();
			this.syncInterrupted = true;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.nativeProviderState != null)
				{
					this.nativeProvider.Unbind(this.nativeProviderState);
					this.nativeProviderState.Dispose();
					this.nativeProviderState = null;
				}
				if (this.cloudProviderState != null)
				{
					this.cloudProvider.Unbind(this.cloudProviderState);
					this.cloudProviderState.Dispose();
					this.cloudProviderState = null;
				}
				if (this.stateStorage != null)
				{
					this.stateStorage.Dispose();
					this.stateStorage = null;
				}
				if (this.syncMailboxSession != null)
				{
					this.syncMailboxSession.Dispose();
					this.syncMailboxSession = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncEngineState>(this);
		}

		public void SetCloudProviderState(SyncStorageProviderState newCloudProviderState)
		{
			this.cloudProviderState = newCloudProviderState;
		}

		public void SetNativeProviderState(NativeSyncStorageProviderState newNativeProviderState)
		{
			this.nativeProviderState = newNativeProviderState;
		}

		public void SetStateStorage(IStateStorage newStateStorage)
		{
			SyncUtilities.ThrowIfArgumentNull("newStateStorage", newStateStorage);
			this.stateStorage = newStateStorage;
		}

		public bool HasConnectedMailboxSession()
		{
			return this.SyncMailboxSession.MailboxSession != null && this.SyncMailboxSession.MailboxSession.IsConnected;
		}

		public MailSubmitter MailSubmitter
		{
			get
			{
				return this.mailSubmitter;
			}
		}

		public DateTime? UserMailboxSubscriptionCachedLastSyncTime
		{
			get
			{
				return this.userMailboxSubscriptionCachedLastSyncTime;
			}
		}

		internal void SetOrganizationId(OrganizationId newOrganizationId)
		{
			SyncUtilities.ThrowIfArgumentNull("newOrganizationId", newOrganizationId);
			this.organizationId = newOrganizationId;
		}

		internal void SetSubscriptionNotificationSent()
		{
			this.subscriptionNotificationSent = true;
		}

		private void OnSubscriptionLoad(ISyncWorkerData subscription)
		{
			this.syncPhaseBeforeSync = subscription.SyncPhase;
		}

		private readonly SyncLogSession syncLogSession;

		private readonly ExDateTime startSyncTime;

		private readonly SyncPoisonStatus subscriptionPoisonStatus;

		private readonly SyncHealthData syncHealthData;

		private readonly bool originalIsUnderRecoveryFlag;

		private readonly string subscriptionPoisonCallstack;

		private readonly string legacyDn;

		private readonly SyncStorageProviderConnectionStatistics connectionStatistics;

		private readonly object syncRoot = new object();

		private readonly ISubscriptionInformationLoader subscriptionInformationLoader;

		private readonly MailSubmitter mailSubmitter;

		private readonly IRemoteServerHealthChecker remoteServerHealthChecker;

		private readonly ISyncWorkerData mailboxServerSubscription;

		private ISyncWorkerData userMailboxSubscription;

		private SyncMailboxSession syncMailboxSession;

		private NativeSyncStorageProvider nativeProvider;

		private ISyncStorageProvider cloudProvider;

		private DateTime? userMailboxSubscriptionCachedLastSyncTime;

		private AsyncOperationResult<SyncProviderResultData> lastSyncOperationResult;

		private SyncEngineStep currentStep;

		private SyncEngineStep? continuationSyncStep;

		private bool underRetry;

		private NativeSyncStorageProviderState nativeProviderState;

		private SyncStorageProviderState cloudProviderState;

		private IStateStorage stateStorage;

		private bool tryCancel;

		private bool moreItemsAvailable;

		private int cloudItemsSynced;

		private bool hasTransientItemLevelErrors;

		private bool syncInterrupted;

		private OrganizationId organizationId;

		private SyncMode syncMode;

		private string updatedSyncWatermark;

		private SyncPhase syncPhaseBeforeSync;

		private AggregationStatus userMailboxSubscriptionStatusBeforeSync;

		private bool subscriptionNotificationSent;

		private SyncProgress? previousSyncProgress;
	}
}
