using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionInformation : EventArgs, ISubscriptionInformation
	{
		internal SubscriptionInformation()
		{
		}

		internal SubscriptionInformation(SubscriptionCacheManager cacheManager, SubscriptionCacheEntry cacheEntry)
		{
			SyncUtilities.ThrowIfArgumentNull("cacheManager", cacheManager);
			SyncUtilities.ThrowIfArgumentNull("cacheEntry", cacheEntry);
			this.databaseGuid = cacheManager.DatabaseGuid;
			this.cacheEntry = cacheEntry;
		}

		public virtual Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public virtual Guid MailboxGuid
		{
			get
			{
				return this.cacheEntry.MailboxGuid;
			}
		}

		public virtual Guid SubscriptionGuid
		{
			get
			{
				return this.cacheEntry.SubscriptionGuid;
			}
		}

		public Guid TenantGuid
		{
			get
			{
				return this.cacheEntry.TenantGuid;
			}
		}

		public Guid ExternalDirectoryOrgId
		{
			get
			{
				return this.cacheEntry.ExternalDirectoryOrgId;
			}
		}

		public string IncomingServerName
		{
			get
			{
				return this.cacheEntry.IncomingServerName;
			}
		}

		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.cacheEntry.SubscriptionType;
			}
		}

		public AggregationType AggregationType
		{
			get
			{
				return this.cacheEntry.AggregationType;
			}
		}

		public virtual bool Disabled
		{
			get
			{
				return this.cacheEntry.Disabled;
			}
		}

		public SyncPhase SyncPhase
		{
			get
			{
				return this.cacheEntry.SyncPhase;
			}
		}

		public virtual ExDateTime? LastSuccessfulDispatchTime
		{
			get
			{
				return this.cacheEntry.LastSuccessfulDispatchTime;
			}
		}

		public string HubServerDispatched
		{
			get
			{
				return this.cacheEntry.HubServerDispatched;
			}
		}

		public string LastHubServerDispatched
		{
			get
			{
				return this.cacheEntry.LastHubServerDispatched;
			}
		}

		public bool SupportsSerialization
		{
			get
			{
				return true;
			}
		}

		public SerializedSubscription SerializedSubscription
		{
			get
			{
				return this.cacheEntry.SerializedSubscription;
			}
		}

		public ExDateTime? LastSyncCompletedTime
		{
			get
			{
				return this.cacheEntry.LastSyncCompletedTime;
			}
		}

		public virtual bool IsMigration
		{
			get
			{
				return this.cacheEntry.IsMigration;
			}
		}

		internal ExDateTime? FirstOutstandingDispatchTime
		{
			get
			{
				return this.cacheEntry.FirstOutstandingDispatchTime;
			}
		}

		internal bool RecoverySyncEnabled
		{
			get
			{
				return this.cacheEntry.RecoverySyncEnabled;
			}
		}

		internal StoreObjectId SubscriptionMessageId
		{
			get
			{
				return this.cacheEntry.SubscriptionMessageId;
			}
		}

		internal string UserLegacyDn
		{
			get
			{
				return this.cacheEntry.UserLegacyDn;
			}
		}

		internal string Diagnostics
		{
			get
			{
				return this.cacheEntry.Diagnostics;
			}
			set
			{
				this.cacheEntry.Diagnostics = value;
			}
		}

		internal string SyncWatermark
		{
			get
			{
				return this.cacheEntry.SyncWatermark;
			}
		}

		public override string ToString()
		{
			return this.cacheEntry.ToString();
		}

		internal bool TrySave(SyncLogSession syncLogSession)
		{
			SubscriptionCacheManager cacheManager = DataAccessLayer.GetCacheManager(this.databaseGuid);
			if (cacheManager == null)
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)224UL, this.SubscriptionGuid, this.MailboxGuid, "Failed to save cache message as cache manager is not found.", new object[0]);
				return false;
			}
			if (syncLogSession != null)
			{
				this.cacheEntry.Diagnostics = syncLogSession.GetBlackBoxText();
			}
			else
			{
				this.cacheEntry.Diagnostics = null;
			}
			bool result;
			try
			{
				cacheManager.UpdateCacheMessage(this.cacheEntry);
				result = true;
			}
			catch (CacheTransientException)
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)225UL, this.SubscriptionGuid, this.MailboxGuid, "Failed to save cache message due to transient exception.", new object[0]);
				result = false;
			}
			catch (CachePermanentException)
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)226UL, this.SubscriptionGuid, this.MailboxGuid, "Failed to save cache message due to permanent exception.", new object[0]);
				result = false;
			}
			return result;
		}

		internal void MarkOutstandingDispatch(ExDateTime dispatchTime, string hubServerName)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("hubServerName", hubServerName);
			if (this.cacheEntry.FirstOutstandingDispatchTime == null)
			{
				this.cacheEntry.FirstOutstandingDispatchTime = new ExDateTime?(dispatchTime);
			}
			this.cacheEntry.LastSuccessfulDispatchTime = new ExDateTime?(dispatchTime);
			this.SetLastHubServer(this.cacheEntry.HubServerDispatched);
			this.cacheEntry.HubServerDispatched = hubServerName;
		}

		internal void MarkLastSuccessfulDispatch(ExDateTime? dispatchTime)
		{
			if (this.cacheEntry.LastSuccessfulDispatchTime != null)
			{
				this.cacheEntry.LastSuccessfulDispatchTime = dispatchTime;
			}
			this.cacheEntry.FirstOutstandingDispatchTime = null;
			this.cacheEntry.HubServerDispatched = null;
		}

		internal void MarkFailedDispatch(ExDateTime? lastSuccessfulDispatchTime, ExDateTime? firstOutstandingDispatchTime, string hubServerDispatched)
		{
			this.cacheEntry.LastSuccessfulDispatchTime = lastSuccessfulDispatchTime;
			this.cacheEntry.FirstOutstandingDispatchTime = firstOutstandingDispatchTime;
			this.cacheEntry.HubServerDispatched = hubServerDispatched;
		}

		internal void MarkSyncCompletion(bool disableSubscription, SyncPhase? syncPhase, SerializedSubscription serializedSubscription, string syncWatermark)
		{
			this.cacheEntry.Disabled = disableSubscription;
			this.cacheEntry.RecoverySyncEnabled = false;
			this.cacheEntry.LastSyncCompletedTime = new ExDateTime?(ExDateTime.UtcNow);
			if (serializedSubscription != null)
			{
				this.cacheEntry.SerializedSubscription = serializedSubscription;
			}
			if (syncWatermark != null)
			{
				this.cacheEntry.SyncWatermark = syncWatermark;
			}
			this.SetLastHubServer(this.cacheEntry.HubServerDispatched);
			this.cacheEntry.HubServerDispatched = null;
			this.cacheEntry.FirstOutstandingDispatchTime = null;
			if (syncPhase != null)
			{
				this.cacheEntry.UpdateSyncPhase(syncPhase.Value);
			}
		}

		internal void MarkSyncTimeOut()
		{
			this.cacheEntry.RecoverySyncEnabled = true;
		}

		internal void UpdateSyncPhase(SyncPhase syncPhase)
		{
			this.cacheEntry.UpdateSyncPhase(syncPhase);
		}

		internal bool Validate(AggregationSubscription actualSubscription, Guid actualUserMailboxGuid, bool fix, out string inconsistencyInfo)
		{
			return this.cacheEntry.Validate(actualSubscription, actualUserMailboxGuid, fix, out inconsistencyInfo);
		}

		private void SetLastHubServer(string lastHubServer)
		{
			if (!string.IsNullOrEmpty(lastHubServer))
			{
				this.cacheEntry.LastHubServerDispatched = lastHubServer;
			}
		}

		private readonly SubscriptionCacheEntry cacheEntry;

		private readonly Guid databaseGuid;
	}
}
