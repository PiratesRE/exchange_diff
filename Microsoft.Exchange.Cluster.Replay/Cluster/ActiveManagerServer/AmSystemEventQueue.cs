using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemEventQueue
	{
		internal AmSystemEventQueue()
		{
			this.m_queue = new Queue<AmEvtBase>();
			this.m_highPriorityQueue = new Queue<AmEvtBase>();
			this.ArrivalEvent = new AutoResetEvent(false);
			this.IsEnabled = true;
		}

		internal AutoResetEvent ArrivalEvent { get; private set; }

		internal bool IsEnabled { get; set; }

		internal bool IsEmpty
		{
			get
			{
				bool result;
				lock (this.m_locker)
				{
					result = (this.m_highPriorityQueue.Count == 0 && this.m_queue.Count == 0);
				}
				return result;
			}
		}

		internal void Cancel(bool isStopAcceptingEvents, bool isClearHighPriority)
		{
			AmTrace.Debug("Cancelling all system events (stopAccceptingEvents={0}, clearhighprio={1})", new object[]
			{
				isStopAcceptingEvents,
				isClearHighPriority
			});
			lock (this.m_locker)
			{
				if (isStopAcceptingEvents)
				{
					this.IsEnabled = false;
				}
				if (isClearHighPriority)
				{
					this.m_highPriorityQueue.Clear();
				}
				this.m_queue.Clear();
			}
		}

		internal void Stop()
		{
			this.Cancel(true, true);
			this.ArrivalEvent.Set();
		}

		internal bool Enqueue(AmEvtBase evt, bool isHighPriority)
		{
			if (!this.IsEnabled)
			{
				return false;
			}
			bool result;
			lock (this.m_locker)
			{
				if (isHighPriority)
				{
					this.m_highPriorityQueue.Enqueue(evt);
				}
				else
				{
					this.m_queue.Enqueue(evt);
				}
				this.ArrivalEvent.Set();
				result = true;
			}
			return result;
		}

		internal AmEvtBase Dequeue()
		{
			AmEvtBase result = null;
			lock (this.m_locker)
			{
				if (this.m_highPriorityQueue.Count > 0)
				{
					result = this.m_highPriorityQueue.Dequeue();
				}
				else if (this.m_queue.Count > 0)
				{
					result = this.m_queue.Dequeue();
				}
			}
			return result;
		}

		private object m_locker = new object();

		private Queue<AmEvtBase> m_queue;

		private Queue<AmEvtBase> m_highPriorityQueue;
	}
}
