using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncTransportResourceMonitor : SyncResourceMonitor
	{
		public SyncTransportResourceMonitor(SyncLogSession syncLogSession, int maxPendingMessages) : base(syncLogSession, null, SyncResourceMonitorType.ServerTransportQueue)
		{
			this.maxPendingMessages = maxPendingMessages;
		}

		protected int CurrentPendingMessagesCount { get; set; }

		internal void AddLoad(int newMessages)
		{
			int num;
			lock (this.syncLock)
			{
				num = (this.CurrentPendingMessagesCount += newMessages);
			}
			base.SyncLogSession.LogVerbose((TSLID)1115UL, "AddLoad: CurrentPendingMessagesCount: {0}, added: {1}", new object[]
			{
				num,
				newMessages
			});
		}

		internal void RemoveLoad(int countOfMessagesNoLongerPending)
		{
			int num;
			lock (this.syncLock)
			{
				num = this.CurrentPendingMessagesCount - countOfMessagesNoLongerPending;
				this.CurrentPendingMessagesCount = Math.Max(0, num);
			}
			base.SyncLogSession.LogVerbose((TSLID)333UL, "RemoveLoad: CurrentPendingMessagesCount: {0}, removed: {1}", new object[]
			{
				num,
				countOfMessagesNoLongerPending
			});
		}

		protected override ResourceLoad GetResourceLoad(AggregationWorkItem workItem)
		{
			double num = (double)this.CurrentPendingMessagesCount / (double)this.maxPendingMessages;
			if (num > 1.0)
			{
				num = ResourceLoad.Critical.LoadRatio;
			}
			return new ResourceLoad(num, null, null);
		}

		protected override IResourceLoadMonitor CreateResourceHealthMonitor(ResourceKey resourceKey)
		{
			return null;
		}

		private readonly object syncLock = new object();

		private readonly int maxPendingMessages;
	}
}
