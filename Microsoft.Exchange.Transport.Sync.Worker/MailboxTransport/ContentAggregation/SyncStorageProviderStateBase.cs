using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SyncStorageProviderStateBase : DisposeTrackableBase
	{
		public int TotalSuccessfulRoundtrips
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

		public int TotalUnsuccessfulRoundtrips
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

		internal SyncStorageProviderStateBase(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.subscription = subscription;
			this.syncLogSession = syncLogSession;
			this.underRecovery = underRecovery;
			this.connectionStatistics = new SyncStorageProviderConnectionStatistics();
		}

		internal ISyncWorkerData Subscription
		{
			get
			{
				base.CheckDisposed();
				return this.subscription;
			}
			set
			{
				base.CheckDisposed();
				SyncUtilities.ThrowIfArgumentNull("subscription", value);
				this.subscription = value;
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

		internal IList<SyncChangeEntry> Changes
		{
			get
			{
				base.CheckDisposed();
				return this.changes;
			}
			set
			{
				base.CheckDisposed();
				SyncUtilities.ThrowIfArgumentNull("changeList", value);
				this.changes = value;
			}
		}

		internal SyncChangeEntry ItemBeingRetrieved
		{
			get
			{
				base.CheckDisposed();
				return this.itemBeingRetrieved;
			}
			set
			{
				base.CheckDisposed();
				this.itemBeingRetrieved = value;
			}
		}

		internal bool HasPermanentSyncErrors
		{
			get
			{
				base.CheckDisposed();
				return this.hasPermanentSyncErrors;
			}
			set
			{
				base.CheckDisposed();
				this.hasPermanentSyncErrors = value;
			}
		}

		internal bool HasTransientSyncErrors
		{
			get
			{
				base.CheckDisposed();
				return this.hasTransientSyncErrors;
			}
			set
			{
				base.CheckDisposed();
				this.hasTransientSyncErrors = value;
			}
		}

		internal ISyncStorageProviderItemRetriever ItemRetriever
		{
			get
			{
				base.CheckDisposed();
				return this.itemRetriever;
			}
			set
			{
				base.CheckDisposed();
				this.itemRetriever = value;
			}
		}

		internal object ItemRetrieverState
		{
			get
			{
				base.CheckDisposed();
				return this.itemRetrieverState;
			}
			set
			{
				base.CheckDisposed();
				this.itemRetrieverState = value;
			}
		}

		internal bool UnderRecovery
		{
			get
			{
				base.CheckDisposed();
				return this.underRecovery;
			}
			set
			{
				base.CheckDisposed();
				this.underRecovery = value;
			}
		}

		public override string ToString()
		{
			return string.Format("Change count: {0}, hasPermanentErrors: {1}, hasTransientErrors: {2}", (this.changes != null) ? this.changes.Count : 0, this.hasPermanentSyncErrors, this.hasTransientSyncErrors);
		}

		internal void Add(SyncChangeEntry entry)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("entry", entry);
			lock (this.changes)
			{
				this.changes.Add(entry);
				if (entry.Exception != null)
				{
					if (entry.Exception is TransientException)
					{
						this.hasTransientSyncErrors = true;
					}
					else if (!(entry.Exception is OperationCanceledException))
					{
						this.hasPermanentSyncErrors = true;
					}
				}
			}
		}

		internal virtual void OnRoundtripComplete(object sender, RoundtripCompleteEventArgs roundtripCompleteEventArgs)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("roundtripCompleteEventArgs", roundtripCompleteEventArgs);
			this.connectionStatistics.OnRoundtripComplete(sender, roundtripCompleteEventArgs);
		}

		private readonly SyncLogSession syncLogSession;

		private ISyncWorkerData subscription;

		private IList<SyncChangeEntry> changes;

		private bool hasPermanentSyncErrors;

		private bool hasTransientSyncErrors;

		private SyncChangeEntry itemBeingRetrieved;

		private ISyncStorageProviderItemRetriever itemRetriever;

		private object itemRetrieverState;

		private bool underRecovery;

		protected SyncStorageProviderConnectionStatistics connectionStatistics;
	}
}
