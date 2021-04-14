using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationWorkItem : RetryableWorkItem
	{
		internal AggregationWorkItem(SyncLog syncLog, string legacyDN, StoreObjectId subscriptionMessageId, AggregationSubscriptionType subscriptionType, Guid subscriptionId, bool recoverySyncMode, Guid databaseGuid, Guid userMailboxGuid, Guid tenantGuid, AggregationType aggregationType, bool initialSync, string mailboxServer, bool isSyncNow, ISyncWorkerData subscription, string mailboxServerSyncWatermark, Guid mailboxServerGuid, SyncPhase syncPhase) : this(syncLog, legacyDN, subscriptionMessageId, subscriptionType, subscriptionId, recoverySyncMode, databaseGuid, userMailboxGuid, tenantGuid, aggregationType, initialSync, mailboxServer, isSyncNow, subscription, mailboxServerSyncWatermark, mailboxServerGuid, syncPhase, AggregationConfiguration.Instance.InitialRetryInMilliseconds, AggregationConfiguration.Instance.RetryBackoffFactor, AggregationConfiguration.Instance.MaximumNumberOfAttempts)
		{
		}

		protected AggregationWorkItem(SyncLog syncLog, string legacyDN, StoreObjectId subscriptionMessageId, AggregationSubscriptionType subscriptionType, Guid subscriptionId, bool recoverySyncMode, Guid databaseGuid, Guid userMailboxGuid, Guid tenantGuid, AggregationType aggregationType, bool initialSync, string mailboxServer, bool isSyncNow, ISyncWorkerData subscription, string mailboxServerSyncWatermark, Guid mailboxServerGuid, SyncPhase syncPhase, int initialRetryInMilliseconds, int retryBackOffFactor, int maximumRetries) : base(initialRetryInMilliseconds, retryBackOffFactor, maximumRetries)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLog", syncLog);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("legacyDN", legacyDN);
			SyncUtilities.ThrowIfArgumentNull("subscriptionMessageId", subscriptionMessageId);
			if (subscriptionType == AggregationSubscriptionType.All || subscriptionType == AggregationSubscriptionType.Unknown)
			{
				throw new ArgumentOutOfRangeException("subscriptionType", "AggregationWorkItem cannot be created for all/unknown subscription types.");
			}
			SyncUtilities.ThrowIfGuidEmpty("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("mailboxServer", mailboxServer);
			SyncUtilities.ThrowIfGuidEmpty("mailboxServerGuid", mailboxServerGuid);
			this.subscription = subscription;
			this.mailboxServerSyncWatermark = mailboxServerSyncWatermark;
			this.mailboxServerGuid = mailboxServerGuid;
			this.legacyDN = legacyDN;
			this.subscriptionMessageId = subscriptionMessageId;
			this.subscriptionType = subscriptionType;
			this.subscriptionId = subscriptionId;
			this.recoverySyncMode = recoverySyncMode;
			this.userMailboxGuid = userMailboxGuid;
			this.tenantGuid = tenantGuid;
			this.mailboxServer = mailboxServer;
			this.aggregationType = aggregationType;
			this.syncPhase = syncPhase;
			this.initialSync = initialSync;
			this.isSyncNow = isSyncNow;
			this.syncLogSession = syncLog.OpenSession(this.userMailboxGuid, this.subscriptionType, this.subscriptionId);
			this.subscriptionPoisonStatus = SyncPoisonHandler.GetPoisonStatus(this.subscriptionId, this.syncLogSession, out this.subscriptionPoisonCallstack);
			this.subscriptionPoisonContext = new SyncPoisonContext(this.subscriptionId);
			this.syncHealthData = new SyncHealthData();
			this.databaseGuid = databaseGuid;
		}

		public ExDateTime CreationTime
		{
			get
			{
				base.CheckDisposed();
				return this.creationTime;
			}
		}

		public TimeSpan Lifetime
		{
			get
			{
				base.CheckDisposed();
				return ExDateTime.UtcNow - this.CreationTime;
			}
		}

		public string LegacyDN
		{
			get
			{
				base.CheckDisposed();
				return this.legacyDN;
			}
		}

		public StoreObjectId SubscriptionMessageId
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionMessageId;
			}
		}

		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionType;
			}
		}

		public AggregationType AggregationType
		{
			get
			{
				base.CheckDisposed();
				return this.aggregationType;
			}
		}

		public bool InitialSync
		{
			get
			{
				base.CheckDisposed();
				return this.initialSync;
			}
		}

		public SyncPhase SyncPhase
		{
			get
			{
				base.CheckDisposed();
				return this.syncPhase;
			}
		}

		public bool IsSyncNow
		{
			get
			{
				base.CheckDisposed();
				return this.isSyncNow;
			}
		}

		public Guid SubscriptionId
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionId;
			}
		}

		public SyncPoisonContext SubscriptionPoisonContext
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionPoisonContext;
			}
		}

		public SyncPoisonStatus SubscriptionPoisonStatus
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionPoisonStatus;
			}
		}

		public string SubscriptionPoisonCallstack
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionPoisonCallstack;
			}
		}

		public bool IsRecoverySyncMode
		{
			get
			{
				base.CheckDisposed();
				return this.recoverySyncMode;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				base.CheckDisposed();
				return this.databaseGuid;
			}
		}

		public Guid UserMailboxGuid
		{
			get
			{
				base.CheckDisposed();
				return this.userMailboxGuid;
			}
		}

		public Guid TenantGuid
		{
			get
			{
				base.CheckDisposed();
				return this.tenantGuid;
			}
		}

		public string MailboxServer
		{
			get
			{
				base.CheckDisposed();
				return this.mailboxServer;
			}
		}

		public ISyncWorkerData Subscription
		{
			get
			{
				base.CheckDisposed();
				return this.subscription;
			}
		}

		public Guid MailboxServerGuid
		{
			get
			{
				base.CheckDisposed();
				return this.mailboxServerGuid;
			}
		}

		public SyncEngineState SyncEngineState
		{
			get
			{
				base.CheckDisposed();
				return this.syncEngineState;
			}
			set
			{
				base.CheckDisposed();
				lock (this.syncRoot)
				{
					this.syncEngineState = value;
				}
			}
		}

		public SyncLogSession SyncLogSession
		{
			get
			{
				base.CheckDisposed();
				return this.syncLogSession;
			}
		}

		public AsyncOperationResult<SyncEngineResultData> LastWorkItemResultData
		{
			get
			{
				base.CheckDisposed();
				return this.lastWorkItemResultData;
			}
			set
			{
				base.CheckDisposed();
				this.lastWorkItemResultData = value;
			}
		}

		public SyncHealthData SyncHealthData
		{
			get
			{
				base.CheckDisposed();
				return this.syncHealthData;
			}
		}

		public bool WasAttemptMadeToOpenMailboxSession
		{
			get
			{
				return this.syncEngineState != null && this.syncEngineState.WasAttemptMadeToOpenMailboxSession;
			}
		}

		public bool IsMailboxServerSyncWatermarkAvailable
		{
			get
			{
				return this.mailboxServerSyncWatermark != null;
			}
		}

		public string MailboxServerSyncWatermark
		{
			get
			{
				return this.mailboxServerSyncWatermark;
			}
		}

		public SyncStorageProviderConnectionStatistics ConnectionStatistics
		{
			get
			{
				return this.connectionStatistics;
			}
		}

		public override string ToString()
		{
			base.CheckDisposed();
			if (this.subscription == null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} : {1}.", new object[]
				{
					this.subscriptionType,
					this.legacyDN
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "{0} : {1}.", new object[]
			{
				this.subscription,
				this.syncEngineState
			});
		}

		public override int GetHashCode()
		{
			return this.SubscriptionId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			base.CheckDisposed();
			AggregationWorkItem otherWorkItem = obj as AggregationWorkItem;
			return this.Equals(otherWorkItem);
		}

		public bool Equals(AggregationWorkItem otherWorkItem)
		{
			base.CheckDisposed();
			return otherWorkItem != null && otherWorkItem.SubscriptionId == this.SubscriptionId;
		}

		public void Cancel(IAsyncResult asyncResult)
		{
			lock (this.syncRoot)
			{
				base.CheckDisposed();
				this.GetExecutionEngine().Cancel(asyncResult);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this.syncRoot)
			{
				if (disposing && this.syncEngineState != null)
				{
					this.syncEngineState.Dispose();
					this.syncEngineState = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AggregationWorkItem>(this);
		}

		protected internal virtual IExecutionEngine GetExecutionEngine()
		{
			if (!this.IsProcessedByDeleteEngine())
			{
				return SyncEngine.Instance;
			}
			return DeleteEngine.Instance;
		}

		internal bool IsProcessedByDeleteEngine()
		{
			return this.SyncPhase == SyncPhase.Delete;
		}

		internal bool IsProcessedBySyncEngine()
		{
			return this.SyncPhase == SyncPhase.Initial || this.SyncPhase == SyncPhase.Incremental;
		}

		internal XElement GetDiagnosticInfo()
		{
			return new XElement("WorkItem", new object[]
			{
				new XElement("subscriptionID", this.SubscriptionId),
				new XElement("type", this.SubscriptionType),
				new XElement("databaseGuid", this.DatabaseGuid),
				new XElement("mailboxGuid", this.UserMailboxGuid),
				new XElement("tenantGuid", this.TenantGuid),
				new XElement("lifetime", this.Lifetime.ToString()),
				new XElement("retryCount", base.CurrentRetryCount)
			});
		}

		internal virtual void ResetSyncEngineState()
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				if (this.syncEngineState != null)
				{
					this.syncEngineState.Dispose();
					this.syncEngineState = null;
				}
			}
		}

		private readonly object syncRoot = new object();

		private readonly SyncPoisonContext subscriptionPoisonContext;

		private readonly SyncHealthData syncHealthData;

		private readonly SyncPoisonStatus subscriptionPoisonStatus;

		private readonly string subscriptionPoisonCallstack;

		private readonly ExDateTime creationTime = ExDateTime.UtcNow;

		private readonly ISyncWorkerData subscription;

		private readonly SyncStorageProviderConnectionStatistics connectionStatistics = new SyncStorageProviderConnectionStatistics();

		private readonly bool isSyncNow;

		private readonly Guid mailboxServerGuid;

		private readonly string mailboxServerSyncWatermark;

		private SyncEngineState syncEngineState;

		private Guid subscriptionId;

		private string legacyDN;

		private StoreObjectId subscriptionMessageId;

		private AggregationSubscriptionType subscriptionType;

		private bool recoverySyncMode;

		private AggregationType aggregationType;

		private bool initialSync;

		private SyncPhase syncPhase;

		private Guid databaseGuid;

		private Guid userMailboxGuid;

		private string mailboxServer;

		private Guid tenantGuid;

		private SyncLogSession syncLogSession;

		private AsyncOperationResult<SyncEngineResultData> lastWorkItemResultData;
	}
}
