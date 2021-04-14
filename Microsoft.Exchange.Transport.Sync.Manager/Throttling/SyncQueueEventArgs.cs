using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncQueueEventArgs : EventArgs
	{
		private SyncQueueEventArgs(Guid databaseGuid, TimeSpan syncInterval, int numberOfItemsChanged, TimeSpan dispatchLagTime)
		{
			this.databaseGuid = databaseGuid;
			this.syncInterval = syncInterval;
			this.numberOfItemsChanged = numberOfItemsChanged;
			this.dispatchLagTime = dispatchLagTime;
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public TimeSpan SyncInterval
		{
			get
			{
				return this.syncInterval;
			}
		}

		public int NumberOfItemsChanged
		{
			get
			{
				return this.numberOfItemsChanged;
			}
		}

		public TimeSpan DispatchLagTime
		{
			get
			{
				return this.dispatchLagTime;
			}
		}

		public static SyncQueueEventArgs CreateReportSyncQueueDispatchLagTimeEventArgs(TimeSpan dispatchLagTime)
		{
			SyncUtilities.ThrowIfArgumentLessThanZero("dispatchLagTime", dispatchLagTime);
			return new SyncQueueEventArgs(Guid.Empty, TimeSpan.MinValue, 0, dispatchLagTime);
		}

		public static SyncQueueEventArgs CreateSubscriptionAddedEventArgs(Guid databaseGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			return new SyncQueueEventArgs(databaseGuid, TimeSpan.MinValue, 1, TimeSpan.MinValue);
		}

		public static SyncQueueEventArgs CreateSubscriptionRemovedEventArgs(Guid databaseGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			return new SyncQueueEventArgs(databaseGuid, TimeSpan.MinValue, -1, TimeSpan.MinValue);
		}

		public static SyncQueueEventArgs CreateSubscriptionEnqueuedEventArgs(Guid databaseGuid, TimeSpan syncInterval)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentLessThanZero("syncInterval", syncInterval);
			return new SyncQueueEventArgs(databaseGuid, syncInterval, 1, TimeSpan.MinValue);
		}

		public static SyncQueueEventArgs CreateSubscriptionDequeuedEventArgs(Guid databaseGuid, TimeSpan syncInterval)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentLessThanZero("syncInterval", syncInterval);
			return new SyncQueueEventArgs(databaseGuid, syncInterval, -1, TimeSpan.MinValue);
		}

		private readonly Guid databaseGuid;

		private readonly TimeSpan syncInterval;

		private readonly int numberOfItemsChanged;

		private readonly TimeSpan dispatchLagTime;
	}
}
