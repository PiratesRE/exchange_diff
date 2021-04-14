using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;
using Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.Pop;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Pop3SyncStorageProviderState : SyncStorageProviderState
	{
		internal Pop3SyncStorageProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery) : this(subscription, syncLogSession, underRecovery, AggregationConfiguration.Instance.GetMaxDownloadItemsPerConnection(subscription.AggregationType), (long)AggregationConfiguration.Instance.GetMaxDownloadSizePerConnection(subscription.AggregationType).ToBytes(), AggregationConfiguration.Instance.MaxDownloadSizePerItem, AggregationConfiguration.Instance.RemoteConnectionTimeout, new EventHandler<DownloadCompleteEventArgs>(FrameworkPerfCounterHandler.Instance.OnPop3RetrieveMessageCompletion))
		{
		}

		internal Pop3SyncStorageProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery, int maxDownloadItemsPerConnection, long maxDownloadSizePerConnection, long maxDownloadSizePerItem, int connectionTimeout, EventHandler<DownloadCompleteEventArgs> downloadCompleted) : base(subscription, syncLogSession, underRecovery, downloadCompleted)
		{
			this.maxDownloadItemsPerConnection = maxDownloadItemsPerConnection;
			this.maxDownloadSizePerConnection = maxDownloadSizePerConnection;
			this.maxDownloadSizePerItem = maxDownloadSizePerItem;
			this.pop3Client = Pop3Client.FromSubscription(this.PopSubscription, connectionTimeout, syncLogSession, new EventHandler<DownloadCompleteEventArgs>(base.InternalOnDownloadCompletion), new EventHandler<RoundtripCompleteEventArgs>(this.OnRoundtripComplete));
		}

		internal PopAggregationSubscription PopSubscription
		{
			get
			{
				base.CheckDisposed();
				return (PopAggregationSubscription)base.Subscription;
			}
		}

		internal Pop3Client Pop3Client
		{
			get
			{
				base.CheckDisposed();
				return this.pop3Client;
			}
		}

		internal long MaxDownloadSizePerConnection
		{
			get
			{
				base.CheckDisposed();
				return this.maxDownloadSizePerConnection;
			}
		}

		internal long MaxDownloadSizePerItem
		{
			get
			{
				base.CheckDisposed();
				return this.maxDownloadSizePerItem;
			}
		}

		internal int MaxDownloadItemsPerConnection
		{
			get
			{
				base.CheckDisposed();
				return this.maxDownloadItemsPerConnection;
			}
		}

		internal Pop3ResultData Pop3ResultData
		{
			get
			{
				base.CheckDisposed();
				return this.pop3ResultData;
			}
			set
			{
				base.CheckDisposed();
				this.pop3ResultData = value;
			}
		}

		internal int EmailsWereAdded
		{
			get
			{
				base.CheckDisposed();
				return this.emailsWereAdded;
			}
			set
			{
				base.CheckDisposed();
				this.emailsWereAdded = value;
			}
		}

		internal int EmailsYetToCome
		{
			get
			{
				base.CheckDisposed();
				return this.emailsYetToCome;
			}
			set
			{
				base.CheckDisposed();
				this.emailsYetToCome = value;
			}
		}

		internal Exception DeletionError
		{
			get
			{
				base.CheckDisposed();
				return this.deletionError;
			}
			set
			{
				base.CheckDisposed();
				this.deletionError = value;
			}
		}

		internal PopBookmark PendingDeletionItems
		{
			get
			{
				if (this.pendingDeletionItems == null)
				{
					string encoded;
					base.StateStorage.TryGetPropertyValue("PendingDeletionItems", out encoded);
					this.pendingDeletionItems = PopBookmark.Parse(encoded);
				}
				return this.pendingDeletionItems;
			}
		}

		internal string SyncWatermark
		{
			get
			{
				base.CheckDisposed();
				string result;
				this.PopWatermark.Load(out result);
				return result;
			}
		}

		internal StringWatermark PopWatermark
		{
			get
			{
				base.CheckDisposed();
				return (StringWatermark)base.BaseWatermark;
			}
		}

		internal bool UniqueIdsResultDataLoaded
		{
			get
			{
				base.CheckDisposed();
				return this.uniqueIdsResultDataLoaded;
			}
		}

		internal AsyncOperationResult<Pop3ResultData> CachedGetUniqueIdsResult
		{
			get
			{
				base.CheckDisposed();
				return this.cachedGetUniqueIdsResult;
			}
		}

		internal void CacheNewWatermark(string newWatermark)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("newWatermark", newWatermark);
			this.newSyncWatermark = newWatermark;
		}

		internal void UpdateSubscriptionWithCurrentWaterMark()
		{
			base.CheckDisposed();
			this.PopWatermark.Save(this.newSyncWatermark);
		}

		internal void CacheGetUniqueIdsResult(AsyncOperationResult<Pop3ResultData> getUniqueIdsResult)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("getUniqueIdsResult", getUniqueIdsResult);
			this.cachedGetUniqueIdsResult = getUniqueIdsResult;
			this.uniqueIdsResultDataLoaded = true;
		}

		internal void PersistPendingDeletionItems()
		{
			if (this.pendingDeletionItems != null && this.pendingDeletionItems.HasChanged)
			{
				string value = this.pendingDeletionItems.ToString();
				if (base.StateStorage.ContainsProperty("PendingDeletionItems"))
				{
					base.StateStorage.ChangePropertyValue("PendingDeletionItems", value);
				}
				else
				{
					base.StateStorage.AddProperty("PendingDeletionItems", value);
				}
				this.pendingDeletionItems.HasChanged = false;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.pop3Client != null)
			{
				this.pop3Client.Dispose();
				this.pop3Client = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Pop3SyncStorageProviderState>(this);
		}

		private const string PendingDeletionItemsPropertyName = "PendingDeletionItems";

		private readonly long maxDownloadSizePerConnection;

		private readonly long maxDownloadSizePerItem;

		private readonly int maxDownloadItemsPerConnection;

		private Pop3ResultData pop3ResultData;

		private int emailsYetToCome;

		private int emailsWereAdded;

		private Pop3Client pop3Client;

		private PopBookmark pendingDeletionItems;

		private Exception deletionError;

		private bool uniqueIdsResultDataLoaded;

		private AsyncOperationResult<Pop3ResultData> cachedGetUniqueIdsResult;

		private string newSyncWatermark = string.Empty;
	}
}
