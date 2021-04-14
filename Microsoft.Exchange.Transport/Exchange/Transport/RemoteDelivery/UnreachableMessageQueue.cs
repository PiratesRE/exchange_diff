using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal sealed class UnreachableMessageQueue : RemoteMessageQueue
	{
		private UnreachableMessageQueue(RoutedQueueBase queueStorage) : base(queueStorage, PriorityBehaviour.IgnorePriority, null)
		{
			this.queuingPerfCountersInstance = QueueManager.GetTotalPerfCounters();
			this.currentRoutingTablesTimestamp = DateTime.MinValue;
		}

		public static UnreachableMessageQueue Instance
		{
			get
			{
				return UnreachableMessageQueue.instance;
			}
		}

		public override NextHopSolutionKey Key
		{
			get
			{
				return NextHopSolutionKey.Unreachable;
			}
		}

		protected override LatencyComponent LatencyComponent
		{
			get
			{
				return LatencyComponent.UnreachableQueue;
			}
		}

		public static void CreateInstance()
		{
			if (UnreachableMessageQueue.instance != null)
			{
				throw new InvalidOperationException("Unreachable queue already created");
			}
			RoutedQueueBase orAddQueue = Components.MessagingDatabase.GetOrAddQueue(NextHopSolutionKey.Unreachable);
			UnreachableMessageQueue.instance = new UnreachableMessageQueue(orAddQueue);
		}

		public static void LoadInstance(RoutedQueueBase queueStorage)
		{
			if (UnreachableMessageQueue.instance != null)
			{
				throw new InvalidOperationException("Unreachable queue already created");
			}
			UnreachableMessageQueue.instance = new UnreachableMessageQueue(queueStorage);
		}

		public new void Enqueue(IQueueItem item)
		{
			if (this.queuingPerfCountersInstance != null)
			{
				this.queuingPerfCountersInstance.UnreachableQueueLength.Increment();
			}
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			LatencyTracker.BeginTrackLatency(this.LatencyComponent, routedMailItem.LatencyTracker);
			base.Enqueue(item);
		}

		public void RoutingTablesChangedHandler(IMailRouter eventSource, DateTime newRoutingTablesTimestamp, bool routesChanged)
		{
			if (newRoutingTablesTimestamp < this.currentRoutingTablesTimestamp)
			{
				return;
			}
			this.currentRoutingTablesTimestamp = newRoutingTablesTimestamp;
			if (routesChanged)
			{
				this.Resubmit(ResubmitReason.ConfigUpdate, null);
			}
		}

		protected override SmtpResponse GetItemExpiredResponse(RoutedMailItem routedMailItem)
		{
			UnreachableReason unreachableReasons = routedMailItem.UnreachableReasons;
			return AckReason.UnreachableMessageExpired(StatusCodeConverter.UnreachableReasonToString(unreachableReasons, CultureInfo.InvariantCulture, ";"));
		}

		protected override void ItemRemoved(IQueueItem item)
		{
			base.ItemRemoved(item);
			if (this.queuingPerfCountersInstance != null)
			{
				this.queuingPerfCountersInstance.UnreachableQueueLength.Decrement();
			}
			LatencyTracker.EndTrackLatency(this.LatencyComponent, ((RoutedMailItem)item).LatencyTracker);
		}

		protected override bool ShouldDequeueForResubmit(IQueueItem queueItem)
		{
			if (!base.ShouldDequeueForResubmit(queueItem))
			{
				return false;
			}
			RoutedMailItem routedMailItem = (RoutedMailItem)queueItem;
			DateTime routingTimeStamp = routedMailItem.RoutingTimeStamp;
			if (routingTimeStamp != DateTime.MinValue)
			{
				return this.resubmitReason == ResubmitReason.Admin || this.resubmitReason == ResubmitReason.Redirect || routingTimeStamp < this.currentRoutingTablesTimestamp;
			}
			throw new InvalidOperationException("Message does not have routing time-stamp");
		}

		public override int Resubmit(ResubmitReason resubmitReason, Action<TransportMailItem> updateBeforeResubmit = null)
		{
			if (resubmitReason == ResubmitReason.Inactivity)
			{
				throw new ArgumentException("Invalid resubmit reason: " + resubmitReason);
			}
			if (this.Suspended)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<ResubmitReason>((long)this.GetHashCode(), "A resubmit request for the unreachable queue due to reason '{0}' was not performed because the queue is frozen.", resubmitReason);
				return 0;
			}
			ExTraceGlobals.QueuingTracer.TraceDebug<ResubmitReason>((long)this.GetHashCode(), "Performing resubmit request for the unreachable queue due to reason '{0}'", resubmitReason);
			int num = base.Resubmit(resubmitReason, updateBeforeResubmit);
			if (num > 0 && resubmitReason == ResubmitReason.ConfigUpdate)
			{
				QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ResubmitDueToConfigUpdate, null, new object[]
				{
					num,
					DataStrings.UnreachableQueueNextHopDomain
				});
			}
			return num;
		}

		public override bool IsInterestingQueueToLog()
		{
			return base.IsInterestingQueueToLog() || base.TotalCount > 0;
		}

		private static UnreachableMessageQueue instance;

		private QueuingPerfCountersInstance queuingPerfCountersInstance;

		private DateTime currentRoutingTablesTimestamp;
	}
}
