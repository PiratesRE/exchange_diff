using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.LinkedIn;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.LinkedIn
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LinkedInSyncStorageProviderState : SyncStorageProviderState
	{
		internal LinkedInSyncStorageProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, LinkedInAppConfig linkedInConfig, ILinkedInWebClient webClient) : this(subscription, syncLogSession, linkedInConfig, webClient, AggregationConfiguration.Instance.GetMaxDownloadItemsPerConnection(subscription.AggregationType))
		{
		}

		internal LinkedInSyncStorageProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, LinkedInAppConfig linkedInConfig, ILinkedInWebClient webClient, int maxDownloadItemsPerConnection) : base(subscription, syncLogSession, false, new EventHandler<DownloadCompleteEventArgs>(FrameworkPerfCounterHandler.Instance.OnLinkedInSyncDownloadCompletion))
		{
			SyncUtilities.ThrowIfArgumentNull("linkedInConfig", linkedInConfig);
			this.config = linkedInConfig;
			this.client = webClient;
			this.maxDownloadItemsPerConnection = maxDownloadItemsPerConnection;
		}

		internal LinkedInAppConfig Config
		{
			get
			{
				base.CheckDisposed();
				return this.config;
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

		internal ILinkedInWebClient LinkedInClient
		{
			get
			{
				base.CheckDisposed();
				if (this.Config != null)
				{
					return this.client;
				}
				return null;
			}
		}

		internal string CurrentWatermark
		{
			get
			{
				base.CheckDisposed();
				return this.currentWatermark;
			}
			set
			{
				base.CheckDisposed();
				this.currentWatermark = value;
			}
		}

		internal string SyncWatermark
		{
			get
			{
				base.CheckDisposed();
				StringWatermark stringWatermark = (StringWatermark)base.BaseWatermark;
				string result;
				stringWatermark.Load(out result);
				return result;
			}
		}

		internal LinkedInConnections CloudData
		{
			get
			{
				base.CheckDisposed();
				return this.cloudData;
			}
			set
			{
				base.CheckDisposed();
				this.cloudData = value;
			}
		}

		internal bool MoreItemsAvailable
		{
			get
			{
				base.CheckDisposed();
				return this.moreItemsAvailable;
			}
			set
			{
				base.CheckDisposed();
				this.moreItemsAvailable = value;
			}
		}

		internal void TriggerRequestEvent()
		{
			base.CheckDisposed();
			FrameworkPerfCounterHandler.Instance.OnLinkedInRequest();
		}

		internal void TriggerContactDownloadEvent()
		{
			base.CheckDisposed();
			FrameworkPerfCounterHandler.Instance.OnLinkedInContactDownload();
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LinkedInSyncStorageProviderState>(this);
		}

		private readonly LinkedInAppConfig config;

		private readonly ILinkedInWebClient client;

		private readonly int maxDownloadItemsPerConnection;

		private bool moreItemsAvailable;

		private LinkedInConnections cloudData;

		private string currentWatermark;
	}
}
