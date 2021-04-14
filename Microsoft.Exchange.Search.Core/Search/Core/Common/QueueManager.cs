using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class QueueManager<T> where T : class, IEquatable<T>
	{
		internal QueueManager(int capacity, int outstandingSize, ExPerformanceCounter stallPerfCounter)
		{
			this.capacity = capacity;
			this.outstandingSize = outstandingSize;
			this.queue = new Queue<T>(this.capacity);
			this.outstandingSet = new HashSet<T>();
			this.stallPerfCounter = stallPerfCounter;
			this.diagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("QueueManager", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.QueueManagerTracer, (long)this.GetHashCode());
		}

		public int Length
		{
			get
			{
				int result;
				lock (this.locker)
				{
					if (this.overflowItem == null)
					{
						result = this.queue.Count;
					}
					else
					{
						result = this.queue.Count + 1;
					}
				}
				return result;
			}
		}

		public int OutstandingLength
		{
			get
			{
				int count;
				lock (this.locker)
				{
					count = this.outstandingSet.Count;
				}
				return count;
			}
		}

		protected IDiagnosticsSession DiagnosticsSession
		{
			get
			{
				return this.diagnosticsSession;
			}
		}

		public bool Enqueue(T item)
		{
			lock (this.locker)
			{
				if (this.queue.Count < this.capacity)
				{
					this.PreEnqueue(item);
					this.queue.Enqueue(item);
					this.diagnosticsSession.TraceDebug<T>("Enqueue successfully item {0}", item);
					return true;
				}
			}
			this.diagnosticsSession.TraceDebug<T>("Enqueue failed for item {0}", item);
			return false;
		}

		public bool Dequeue(out IEnumerable<T> items)
		{
			items = null;
			lock (this.locker)
			{
				if (this.queue.Count == 0 && this.overflowItem == null)
				{
					this.diagnosticsSession.TraceDebug("No items to dequeue: pending queue is empty", new object[0]);
					return false;
				}
				if (this.outstandingSet.Count == this.outstandingSize)
				{
					this.diagnosticsSession.TraceDebug<int>("No items to dequeue: outstanding set is full (size={0})", this.outstandingSet.Count);
					return false;
				}
				if (this.overflowItem != null && this.outstandingSet.Contains(this.overflowItem))
				{
					this.diagnosticsSession.TraceDebug<T>("Stalled: item {0} is still being processed.", this.overflowItem);
					return false;
				}
				int num = Math.Min(this.Length, this.outstandingSize - this.outstandingSet.Count);
				List<T> list = new List<T>(num);
				if (this.overflowItem != null)
				{
					list.Add(this.overflowItem);
					this.outstandingSet.Add(this.overflowItem);
					this.overflowItem = default(T);
					num--;
					this.stallTimer.Stop();
					if (this.stallPerfCounter != null)
					{
						this.stallPerfCounter.IncrementBy(this.stallTimer.ElapsedMilliseconds);
					}
					this.stallTimer.Reset();
				}
				for (int i = 0; i < num; i++)
				{
					T item = this.queue.Dequeue();
					if (this.PostDequeue(item))
					{
						if (this.outstandingSet.Contains(item))
						{
							this.overflowItem = item;
							this.stallTimer.Start();
							break;
						}
						this.outstandingSet.Add(item);
					}
					list.Add(item);
				}
				if (list.Count > 0)
				{
					this.diagnosticsSession.TraceDebug<int, int, int>("Dequeue successfully {0} items, pending queue = {1}, outstanding set = {2}", num, this.queue.Count, this.outstandingSet.Count);
					items = list;
					return true;
				}
			}
			return false;
		}

		public bool Remove(T item)
		{
			bool flag2;
			lock (this.locker)
			{
				flag2 = this.outstandingSet.Remove(item);
			}
			this.diagnosticsSession.TraceDebug("Remove item {0}: success = {1} pending queue = {2}, outstanding set = {3}", new object[]
			{
				item,
				flag2,
				this.queue.Count,
				this.outstandingSet.Count
			});
			return flag2;
		}

		protected virtual void PreEnqueue(T item)
		{
		}

		protected virtual bool PostDequeue(T item)
		{
			return true;
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly Queue<T> queue;

		private readonly HashSet<T> outstandingSet;

		private readonly object locker = new object();

		private readonly int capacity;

		private readonly int outstandingSize;

		private T overflowItem;

		private ExPerformanceCounter stallPerfCounter;

		private Stopwatch stallTimer = new Stopwatch();
	}
}
