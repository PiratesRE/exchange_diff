using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SyncStorageProviderState : SyncStorageProviderStateBase
	{
		internal SyncStorageProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery, EventHandler<DownloadCompleteEventArgs> downloadCompleted) : base(subscription, syncLogSession, underRecovery)
		{
			this.DownloadCompleted += downloadCompleted;
			this.failedCloudItemsSeen = new HashSet<string>();
			this.cloudStatistics = new CloudStatistics();
		}

		internal event EventHandler<RemoteServerRoundtripCompleteEventArgs> RemoteServerRoundtripCompleteEvent
		{
			add
			{
				this.InternalRemoteServerRoundtripCompleteEvent += value;
			}
			remove
			{
				this.InternalRemoteServerRoundtripCompleteEvent -= value;
			}
		}

		private event EventHandler<DownloadCompleteEventArgs> DownloadCompleted;

		private event EventHandler<RemoteServerRoundtripCompleteEventArgs> InternalRemoteServerRoundtripCompleteEvent;

		internal ISimpleStateStorage StateStorage
		{
			get
			{
				base.CheckDisposed();
				return this.stateStorage;
			}
			set
			{
				base.CheckDisposed();
				this.stateStorage = value;
			}
		}

		internal ByteQuantifiedSize BytesDownloaded
		{
			get
			{
				base.CheckDisposed();
				return this.bytesDownloaded;
			}
		}

		internal HashSet<string> FailedCloudItemsSeen
		{
			get
			{
				base.CheckDisposed();
				return this.failedCloudItemsSeen;
			}
		}

		internal CloudStatistics CloudStatistics
		{
			get
			{
				base.CheckDisposed();
				return this.cloudStatistics;
			}
			set
			{
				base.CheckDisposed();
				this.cloudStatistics = value;
			}
		}

		public bool HasNoChangesOnCloud
		{
			get
			{
				base.CheckDisposed();
				return this.hasNoChangesOnCloud;
			}
			set
			{
				base.CheckDisposed();
				this.hasNoChangesOnCloud = value;
			}
		}

		public BaseWatermark BaseWatermark
		{
			get
			{
				base.CheckDisposed();
				return this.baseWatermark;
			}
		}

		public void SetBaseWatermark(BaseWatermark newWatermark)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("newWatermark", newWatermark);
			this.baseWatermark = newWatermark;
		}

		internal void InternalOnDownloadCompletion(object sender, DownloadCompleteEventArgs e)
		{
			this.bytesDownloaded += new ByteQuantifiedSize((ulong)e.BytesDownloaded);
			if (this.DownloadCompleted != null)
			{
				this.DownloadCompleted(sender, e);
			}
		}

		internal override void OnRoundtripComplete(object sender, RoundtripCompleteEventArgs roundtripCompleteEventArgs)
		{
			base.OnRoundtripComplete(sender, roundtripCompleteEventArgs);
			RemoteServerRoundtripCompleteEventArgs e = (RemoteServerRoundtripCompleteEventArgs)roundtripCompleteEventArgs;
			if (this.InternalRemoteServerRoundtripCompleteEvent != null)
			{
				this.InternalRemoteServerRoundtripCompleteEvent(sender, e);
			}
		}

		internal static readonly int NoItemOrFolderCount = -1;

		private readonly HashSet<string> failedCloudItemsSeen;

		private ISimpleStateStorage stateStorage;

		private ByteQuantifiedSize bytesDownloaded;

		private bool hasNoChangesOnCloud;

		private BaseWatermark baseWatermark;

		private CloudStatistics cloudStatistics;
	}
}
