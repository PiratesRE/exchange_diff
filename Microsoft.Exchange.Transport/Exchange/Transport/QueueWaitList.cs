using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class QueueWaitList
	{
		public QueueWaitList(NextHopSolutionKey queue, Trace tracer) : this(tracer)
		{
			QueueWaitList.QueueWeight value = new QueueWaitList.QueueWeight(queue);
			this.queues[queue] = value;
			this.messageCount++;
		}

		public QueueWaitList(Trace tracer)
		{
			this.tracer = tracer;
			this.queues = new Dictionary<NextHopSolutionKey, QueueWaitList.QueueWeight>();
		}

		public static TimeSpan MaxConditionSkewInterval { get; set; }

		public int QueueCount
		{
			get
			{
				return this.queues.Count;
			}
		}

		public int MessageCount
		{
			get
			{
				return this.messageCount;
			}
		}

		public int PendingMessageCount
		{
			get
			{
				return this.pendingMessageCount;
			}
		}

		public bool HasOlderMessages(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight))
				{
					return queueWeight.HasOlderMessages;
				}
			}
			return false;
		}

		public int GetMessageCount(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight))
				{
					return queueWeight.MessageCount;
				}
			}
			return 0;
		}

		public int GetPendingMessageCount(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight))
				{
					return queueWeight.PendingMessageCount;
				}
			}
			return 0;
		}

		public bool HasDisabledMessages(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight))
				{
					return queueWeight.HasDisabledMessages;
				}
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			lock (this.syncRoot)
			{
				stringBuilder.AppendFormat("Found {0} queues {", this.queues.Count);
				foreach (KeyValuePair<NextHopSolutionKey, QueueWaitList.QueueWeight> keyValuePair in this.queues)
				{
					stringBuilder.AppendFormat("Queue {0}: {1}\n", keyValuePair.Key.ToShortString(), keyValuePair.Value.ToString());
				}
			}
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}

		internal bool Add(NextHopSolutionKey queue)
		{
			bool result;
			lock (this.syncRoot)
			{
				if (this.state == WaitListState.Deleted)
				{
					result = false;
				}
				else
				{
					QueueWaitList.QueueWeight queueWeight;
					if (this.queues.TryGetValue(queue, out queueWeight))
					{
						queueWeight.Add();
					}
					else
					{
						queueWeight = new QueueWaitList.QueueWeight(queue);
						this.queues[queue] = queueWeight;
					}
					this.messageCount++;
					result = true;
				}
			}
			return result;
		}

		internal bool GetNextItem(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight) && queueWeight.ProvisionallyRemove())
				{
					this.messageCount--;
					this.pendingMessageCount++;
					return true;
				}
			}
			return false;
		}

		internal bool ConfirmRemove(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (!this.queues.TryGetValue(queue, out queueWeight))
				{
					throw new KeyNotFoundException("The specified queue name was not found");
				}
				if (queueWeight.ConfirmRemove())
				{
					return this.RemoveAndCleanupIfLast(queue);
				}
			}
			return false;
		}

		internal bool RemoveWaitingAndOneOutstanding(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight))
				{
					this.tracer.TraceWarning(0L, "Removing items may make the map out of sync with the queues");
					bool flag2 = queueWeight.RemoveWaitingAndOneOutstanding();
					this.messageCount = this.queues.Values.Sum((QueueWaitList.QueueWeight q) => q.MessageCount);
					this.pendingMessageCount--;
					if (flag2)
					{
						return this.RemoveAndCleanupIfLast(queue);
					}
				}
			}
			return false;
		}

		internal bool MoveToDisabled(NextHopSolutionKey queue)
		{
			bool result;
			lock (this.syncRoot)
			{
				if (this.state == WaitListState.Deleted)
				{
					result = false;
				}
				else
				{
					QueueWaitList.QueueWeight queueWeight;
					if (!this.queues.TryGetValue(queue, out queueWeight))
					{
						queueWeight = new QueueWaitList.QueueWeight(queue);
						this.queues[queue] = queueWeight;
					}
					queueWeight.MoveToDisabled();
					this.messageCount = this.queues.Values.Sum((QueueWaitList.QueueWeight q) => q.MessageCount);
					result = true;
				}
			}
			return result;
		}

		internal bool DisabledMessagesCleared(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight))
				{
					this.tracer.TraceWarning(0L, "Removing items may make the map out of sync with the queues");
					if (queueWeight.DisabledMessagesCleared())
					{
						return this.RemoveAndCleanupIfLast(queue);
					}
				}
			}
			return false;
		}

		internal bool CompleteItem(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (!this.queues.TryGetValue(queue, out queueWeight))
				{
					throw new KeyNotFoundException("The specified queue name was not found");
				}
				bool flag2 = queueWeight.CompleteItem();
				this.pendingMessageCount--;
				if (flag2)
				{
					return this.RemoveAndCleanupIfLast(queue);
				}
			}
			return false;
		}

		internal bool Cleanup(NextHopSolutionKey queue)
		{
			lock (this.syncRoot)
			{
				if (this.queues.ContainsKey(queue))
				{
					this.tracer.TraceWarning(0L, "Removing a queue that could have pending tokens");
					bool result = this.RemoveAndCleanupIfLast(queue);
					this.messageCount = this.queues.Values.Sum((QueueWaitList.QueueWeight q) => q.MessageCount);
					this.pendingMessageCount = this.queues.Values.Sum((QueueWaitList.QueueWeight q) => q.PendingMessageCount);
					return result;
				}
			}
			return this.queues.Count == 0;
		}

		internal bool CleanupItem(NextHopSolutionKey queue)
		{
			bool result;
			lock (this.syncRoot)
			{
				QueueWaitList.QueueWeight queueWeight;
				if (this.queues.TryGetValue(queue, out queueWeight))
				{
					bool flag2 = queueWeight.CleanupItem();
					this.messageCount = this.queues.Values.Sum((QueueWaitList.QueueWeight q) => q.MessageCount);
					if (flag2)
					{
						return this.RemoveAndCleanupIfLast(queue);
					}
				}
				result = (this.queues.Count == 0);
			}
			return result;
		}

		internal NextHopSolutionKey[] Clear()
		{
			if (this.queues.Count == 0)
			{
				return null;
			}
			NextHopSolutionKey[] result;
			lock (this.syncRoot)
			{
				NextHopSolutionKey[] array = new NextHopSolutionKey[this.queues.Count];
				this.queues.Keys.CopyTo(array, 0);
				this.queues.Clear();
				this.state = WaitListState.Deleted;
				this.messageCount = 0;
				this.pendingMessageCount = 0;
				result = array;
			}
			return result;
		}

		internal void Reset()
		{
			lock (this.syncRoot)
			{
				this.state = WaitListState.Live;
				this.Clear();
			}
		}

		private bool RemoveAndCleanupIfLast(NextHopSolutionKey queue)
		{
			this.queues.Remove(queue);
			if (this.queues.Count == 0)
			{
				this.state = WaitListState.Deleted;
				return true;
			}
			return false;
		}

		private Dictionary<NextHopSolutionKey, QueueWaitList.QueueWeight> queues;

		private Trace tracer;

		private object syncRoot = new object();

		private WaitListState state;

		private int messageCount;

		private int pendingMessageCount;

		private class QueueWeight
		{
			public QueueWeight(NextHopSolutionKey queue)
			{
				this.queue = queue;
				this.messageCount = 1;
			}

			public NextHopSolutionKey Name
			{
				get
				{
					return this.queue;
				}
			}

			public int MessageCount
			{
				get
				{
					return this.messageCount;
				}
			}

			public int PendingMessageCount
			{
				get
				{
					return this.outstandingItems;
				}
			}

			public bool HasOlderMessages
			{
				get
				{
					return this.messageCount > 0 || this.outstandingItems > 0;
				}
			}

			public bool HasDisabledMessages
			{
				get
				{
					return this.hasDisabledMessages;
				}
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Queue {0}: messageCount={1}, tokens={2}", this.Name, this.messageCount, this.outstandingItems);
				return stringBuilder.ToString();
			}

			internal void Add()
			{
				this.messageCount++;
			}

			internal bool ProvisionallyRemove()
			{
				if (this.messageCount == 0)
				{
					return false;
				}
				this.outstandingItems++;
				this.provisionallyRemovedItems++;
				this.messageCount--;
				return true;
			}

			internal bool ConfirmRemove()
			{
				if (this.provisionallyRemovedItems <= 0)
				{
					throw new InvalidOperationException("Cannot update an item without first provisionally removing it");
				}
				this.provisionallyRemovedItems--;
				return this.IsEmpty();
			}

			internal bool RemoveWaitingAndOneOutstanding()
			{
				this.messageCount = 0;
				this.provisionallyRemovedItems--;
				this.outstandingItems--;
				return this.IsEmpty();
			}

			internal bool CompleteItem()
			{
				if (this.outstandingItems <= 0)
				{
					throw new InvalidOperationException("There is no pending item to activate. UpdateItemActivated should be called after GetNextQueue");
				}
				this.outstandingItems--;
				return this.IsEmpty();
			}

			internal bool CleanupItem()
			{
				if (this.messageCount > 0)
				{
					this.messageCount--;
				}
				return this.IsEmpty();
			}

			internal void MoveToDisabled()
			{
				if (this.messageCount > 0)
				{
					this.messageCount--;
				}
				this.hasDisabledMessages = true;
			}

			internal bool DisabledMessagesCleared()
			{
				this.hasDisabledMessages = false;
				return this.IsEmpty();
			}

			private bool IsEmpty()
			{
				return this.outstandingItems == 0 && this.messageCount == 0 && this.provisionallyRemovedItems == 0 && !this.hasDisabledMessages;
			}

			private NextHopSolutionKey queue;

			private int messageCount;

			private int outstandingItems;

			private int provisionallyRemovedItems;

			private bool hasDisabledMessages;
		}
	}
}
