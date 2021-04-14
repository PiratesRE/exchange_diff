using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncEngineResultData
	{
		private SyncEngineResultData(ExDateTime startSyncTime, int cloudItemsSynced, bool cloudMoreItemsAvailable, bool disableSubscription, bool invalidState, string updatedSyncWatermark, ISyncWorkerData updatedSubscription, SyncPhase syncPhaseBeforeSync, bool deleteSubscription)
		{
			this.startSyncTime = startSyncTime;
			this.cloudItemsSynced = cloudItemsSynced;
			this.cloudMoreItemsAvailable = cloudMoreItemsAvailable;
			this.disableSubscription = disableSubscription;
			this.invalidState = invalidState;
			this.updatedSyncWatermark = updatedSyncWatermark;
			this.updatedSubscription = updatedSubscription;
			this.syncPhaseBeforeSync = syncPhaseBeforeSync;
			this.deleteSubscription = deleteSubscription;
		}

		internal SyncEngineResultData(ExDateTime startSyncTime, int cloudItemsSynced, bool cloudMoreItemsAvailable, bool disableSubscription, bool invalidState, string updatedSyncWatermark, ISyncWorkerData updatedSubscription, SyncPhase syncPhaseBeforeSync) : this(startSyncTime, cloudItemsSynced, cloudMoreItemsAvailable, disableSubscription, invalidState, updatedSyncWatermark, updatedSubscription, syncPhaseBeforeSync, false)
		{
		}

		internal SyncEngineResultData(ExDateTime startSyncTime, bool deleteSubscription) : this(startSyncTime, 0, false, false, false, null, null, SyncPhase.Initial, deleteSubscription)
		{
		}

		internal int CloudItemsSynced
		{
			get
			{
				return this.cloudItemsSynced;
			}
		}

		internal bool CloudMoreItemsAvailable
		{
			get
			{
				return this.cloudMoreItemsAvailable;
			}
		}

		internal ExDateTime StartSyncTime
		{
			get
			{
				return this.startSyncTime;
			}
		}

		public bool DisableSubscription
		{
			get
			{
				return this.disableSubscription;
			}
		}

		public bool InvalidState
		{
			get
			{
				return this.invalidState;
			}
		}

		internal string UpdatedSyncWatermark
		{
			get
			{
				return this.updatedSyncWatermark;
			}
		}

		internal ISyncWorkerData UpdatedSubscription
		{
			get
			{
				return this.updatedSubscription;
			}
		}

		internal SyncPhase SyncPhaseBeforeSync
		{
			get
			{
				return this.syncPhaseBeforeSync;
			}
		}

		internal bool DeleteSubscription
		{
			get
			{
				return this.deleteSubscription;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "CloudItemsSynced:{0},CloudMoreItemsAvailable:{1},StartSyncTime:{2},disableSubscription:{3},invalidState:{4},updatedSyncWatermark:{5},updatedSubscription:{6},syncPhaseBeforeSync:{7},deleteSubscription:{8}.", new object[]
			{
				this.cloudItemsSynced,
				this.cloudMoreItemsAvailable,
				this.startSyncTime,
				this.disableSubscription,
				this.invalidState,
				this.updatedSyncWatermark ?? "<null>",
				this.updatedSubscription,
				this.syncPhaseBeforeSync,
				this.deleteSubscription
			});
		}

		private readonly int cloudItemsSynced;

		private readonly bool cloudMoreItemsAvailable;

		private readonly ExDateTime startSyncTime;

		private readonly bool disableSubscription;

		private readonly bool invalidState;

		private readonly string updatedSyncWatermark;

		private readonly ISyncWorkerData updatedSubscription;

		private readonly SyncPhase syncPhaseBeforeSync;

		private readonly bool deleteSubscription;
	}
}
