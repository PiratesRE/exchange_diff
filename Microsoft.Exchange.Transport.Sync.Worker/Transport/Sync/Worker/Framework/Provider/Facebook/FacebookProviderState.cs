using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Facebook;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.Facebook
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FacebookProviderState : SyncStorageProviderState
	{
		public IFacebookClient Client { get; private set; }

		public string CurrentWatermark { get; set; }

		public List<FacebookUser> CloudUpdates { get; set; }

		public int MaxDownloadItems { get; set; }

		public bool MoreItemsAvailable { get; set; }

		internal FacebookProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, IFacebookClient client) : this(subscription, syncLogSession, client, AggregationConfiguration.Instance.GetMaxDownloadItemsPerConnection(subscription.AggregationType))
		{
		}

		internal FacebookProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, IFacebookClient client, int maxDownloadItems) : base(subscription, syncLogSession, false, new EventHandler<DownloadCompleteEventArgs>(FrameworkPerfCounterHandler.Instance.OnFacebookSyncDownloadCompletion))
		{
			SyncUtilities.ThrowIfArgumentNull("client", client);
			this.Client = client;
			this.MaxDownloadItems = maxDownloadItems;
			this.Client.SubscribeDownloadCompletedEvent(new EventHandler<DownloadCompleteEventArgs>(base.InternalOnDownloadCompletion));
		}

		internal void TriggerRequestEvent()
		{
			base.CheckDisposed();
			FrameworkPerfCounterHandler.Instance.OnFacebookRequest(this, null);
		}

		internal void TriggerRequestEventWithNoChanges()
		{
			base.CheckDisposed();
			FrameworkPerfCounterHandler.Instance.OnFacebookRequestWithNoChanges(this, null);
		}

		internal void TriggerContactDownloadEvent()
		{
			base.CheckDisposed();
			FrameworkPerfCounterHandler.Instance.OnFacebookContactDownload(this, null);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FacebookProviderState>(this);
		}
	}
}
