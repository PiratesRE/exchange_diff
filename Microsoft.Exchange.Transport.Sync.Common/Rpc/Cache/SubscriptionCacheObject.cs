using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache
{
	[Serializable]
	public class SubscriptionCacheObject
	{
		internal SubscriptionCacheObject(Guid subscriptionGuid, StoreObjectId subscriptionMessageId, string userLegacyDn, AggregationSubscriptionType subscriptionType, AggregationType aggregationType, SyncPhase syncPhase, ExDateTime? lastSyncCompletedTime, string incomingServerName, Guid userMailboxGuid, long? serializedSubscriptionVersion, Guid tenantGuid, string hubServerDispatched, string lastHubServerDispatched, ExDateTime? firstOutstandingDispatchTime, ExDateTime? lastSuccessfulDispatchTime, bool recoverySyncEnabled, bool disabled, string diagnostics, SubscriptionCacheObjectState state, string reasonForTheState)
		{
			this.subscriptionGuid = subscriptionGuid;
			if (subscriptionMessageId != null)
			{
				this.subscriptionMessageId = subscriptionMessageId.ToString();
			}
			this.userLegacyDn = userLegacyDn;
			this.subscriptionType = subscriptionType;
			this.aggregationType = aggregationType;
			this.syncPhase = syncPhase;
			this.lastSyncCompletedTime = (DateTime?)lastSyncCompletedTime;
			this.incomingServerName = incomingServerName;
			this.userMailboxGuid = userMailboxGuid;
			this.serializedSubscriptionVersion = serializedSubscriptionVersion;
			this.tenantGuid = tenantGuid;
			this.hubServerDispatched = hubServerDispatched;
			this.lastHubServerDispatched = lastHubServerDispatched;
			this.firstOutstandingDispatchTime = (DateTime?)firstOutstandingDispatchTime;
			this.lastSuccessfulDispatchTime = (DateTime?)lastSuccessfulDispatchTime;
			this.recoverySyncEnabled = recoverySyncEnabled;
			this.disabled = disabled;
			this.diagnostics = diagnostics;
			this.state = state;
			this.reasonForTheState = reasonForTheState;
		}

		public Guid SubscriptionGuid
		{
			get
			{
				return this.subscriptionGuid;
			}
		}

		public string SubscriptionMessageId
		{
			get
			{
				return this.subscriptionMessageId;
			}
		}

		public string UserLegacyDn
		{
			get
			{
				return this.userLegacyDn;
			}
		}

		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
		}

		public AggregationType AggregationType
		{
			get
			{
				return this.aggregationType;
			}
		}

		public SyncPhase SyncPhase
		{
			get
			{
				return this.syncPhase;
			}
		}

		public DateTime? LastSyncCompletionTime
		{
			get
			{
				return this.lastSyncCompletedTime;
			}
		}

		public string IncomingServerName
		{
			get
			{
				return this.incomingServerName;
			}
		}

		public Guid UserMailboxGuid
		{
			get
			{
				return this.userMailboxGuid;
			}
		}

		public long? SerializedSubscriptionVersion
		{
			get
			{
				return this.serializedSubscriptionVersion;
			}
		}

		public Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		public string HubServerDispatched
		{
			get
			{
				return this.hubServerDispatched;
			}
		}

		public string LastHubServerDispatched
		{
			get
			{
				return this.lastHubServerDispatched;
			}
		}

		public DateTime? FirstOutstandingDispatchTime
		{
			get
			{
				return this.firstOutstandingDispatchTime;
			}
		}

		public DateTime? LastSuccessfulDispatchTime
		{
			get
			{
				return this.lastSuccessfulDispatchTime;
			}
		}

		public bool RecoverySyncEnabled
		{
			get
			{
				return this.recoverySyncEnabled;
			}
		}

		public bool Disabled
		{
			get
			{
				return this.disabled;
			}
		}

		public string Diagnostics
		{
			get
			{
				return this.diagnostics;
			}
		}

		public string State
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}; {1}", new object[]
				{
					this.state,
					this.reasonForTheState
				});
			}
		}

		internal SubscriptionCacheObjectState ObjectState
		{
			get
			{
				return this.state;
			}
		}

		internal static readonly int ApproximateCacheObjectSizeInBytes = 1024;

		private readonly Guid subscriptionGuid;

		private readonly string subscriptionMessageId;

		private readonly string userLegacyDn;

		private readonly AggregationSubscriptionType subscriptionType;

		private readonly AggregationType aggregationType;

		private readonly SyncPhase syncPhase;

		private readonly DateTime? lastSyncCompletedTime;

		private readonly string incomingServerName;

		private readonly Guid userMailboxGuid;

		private readonly long? serializedSubscriptionVersion;

		private readonly Guid tenantGuid;

		private readonly string hubServerDispatched;

		private readonly string lastHubServerDispatched;

		private readonly DateTime? firstOutstandingDispatchTime;

		private readonly DateTime? lastSuccessfulDispatchTime;

		private readonly bool recoverySyncEnabled;

		private readonly bool disabled;

		private readonly string diagnostics;

		private readonly SubscriptionCacheObjectState state;

		private readonly string reasonForTheState;
	}
}
