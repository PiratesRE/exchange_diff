using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport
{
	internal sealed class PoisonMessageQueue
	{
		private PoisonMessageQueue()
		{
			this.queuingPerfCountersInstance = QueueManager.GetTotalPerfCounters();
		}

		public static PoisonMessageQueue Instance
		{
			get
			{
				return PoisonMessageQueue.instance;
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.items.Count == 0;
			}
		}

		public TransportMailItem this[long mailItemId]
		{
			get
			{
				TransportMailItem result;
				lock (this.items)
				{
					this.items.TryGetValue(mailItemId, out result);
				}
				return result;
			}
		}

		public void Enqueue(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				ExTraceGlobals.PoisonTracer.TraceError(0L, "Skipping Enqueue. MailItem is null.");
				return;
			}
			bool flag = false;
			lock (this.items)
			{
				if (!this.items.ContainsKey(mailItem.RecordId))
				{
					this.items.Add(mailItem.RecordId, mailItem);
					flag = true;
				}
				else
				{
					ExTraceGlobals.PoisonTracer.TraceError<long>(0L, "Queue already contains the mailitem with id {0}.", mailItem.RecordId);
				}
			}
			if (flag)
			{
				LatencyTracker.BeginTrackLatency(LatencyComponent.PoisonQueue, mailItem.LatencyTracker);
				if (this.queuingPerfCountersInstance != null)
				{
					this.queuingPerfCountersInstance.PoisonQueueLength.Increment();
				}
			}
		}

		public TransportMailItem Extract(long mailItemId)
		{
			TransportMailItem transportMailItem = null;
			lock (this.items)
			{
				if (this.items.TryGetValue(mailItemId, out transportMailItem))
				{
					this.items.Remove(mailItemId);
					if (this.queuingPerfCountersInstance != null)
					{
						this.queuingPerfCountersInstance.PoisonQueueLength.Decrement();
					}
				}
			}
			if (transportMailItem != null)
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.PoisonQueue, transportMailItem.LatencyTracker);
			}
			return transportMailItem;
		}

		public void VisitMailItems(Func<TransportMailItem, bool> visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			lock (this.items)
			{
				foreach (TransportMailItem arg in this.items.Values)
				{
					if (!visitor(arg))
					{
						break;
					}
				}
			}
		}

		private static PoisonMessageQueue instance = new PoisonMessageQueue();

		private Dictionary<long, TransportMailItem> items = new Dictionary<long, TransportMailItem>(10);

		private QueuingPerfCountersInstance queuingPerfCountersInstance;
	}
}
