using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	internal class DeferredQueue
	{
		public DeferredQueue()
		{
			this.deferredCount = new int[DeferredQueue.deferReasonsCount];
		}

		public int TotalExcludedCount
		{
			get
			{
				return this.totalExcludedCount;
			}
		}

		public int TotalCount
		{
			get
			{
				return this.count + this.TotalExcludedCount;
			}
		}

		public long NextActivationTime
		{
			get
			{
				return this.nextActivation;
			}
		}

		public void UpdateNextActivationTime(long ticks)
		{
			if (ticks < this.nextActivation)
			{
				this.nextActivation = ticks;
			}
		}

		public void Enqueue(IQueueItem item)
		{
			if (item == null)
			{
				return;
			}
			this.head = new Node<IQueueItem>(item)
			{
				Next = this.head
			};
			this.IncrementCount(item);
			DateTime dateTime;
			if (item.DeferUntil == DateTime.MaxValue)
			{
				dateTime = item.Expiry;
			}
			else
			{
				dateTime = item.DeferUntil;
			}
			this.UpdateNextActivationTime(dateTime.Ticks);
		}

		public QueueItemList DequeueAll(Predicate<IQueueItem> match)
		{
			long val = DateTime.UtcNow.Ticks + 36000000000L;
			QueueItemList queueItemList = new QueueItemList();
			Node<IQueueItem> next = this.head;
			Node<IQueueItem> node = null;
			while (next != null)
			{
				long val2 = Math.Min(next.Value.DeferUntil.Ticks, next.Value.Expiry.Ticks);
				if (match(next.Value))
				{
					this.DecrementCount(next.Value);
					if (node == null)
					{
						this.head = next.Next;
					}
					else
					{
						node.Next = next.Next;
					}
					Node<IQueueItem> node2 = next;
					next = next.Next;
					queueItemList.Add(node2);
				}
				else
				{
					val = Math.Min(val, val2);
					node = next;
					next = next.Next;
				}
			}
			this.nextActivation = val;
			return queueItemList;
		}

		public IQueueItem DequeueItem(DequeueMatch match, out bool matchFound)
		{
			matchFound = false;
			IQueueItem result = null;
			Node<IQueueItem> next = this.head;
			Node<IQueueItem> node = null;
			while (next != null && !matchFound)
			{
				switch (match(next.Value))
				{
				case DequeueMatchResult.Break:
					matchFound = true;
					break;
				case DequeueMatchResult.DequeueAndBreak:
					this.DecrementCount(next.Value);
					if (node == null)
					{
						this.head = next.Next;
					}
					else
					{
						node.Next = next.Next;
					}
					result = next.Value;
					matchFound = true;
					break;
				case DequeueMatchResult.Continue:
					node = next;
					next = next.Next;
					break;
				default:
					throw new InvalidOperationException("Invalid return value from match()");
				}
			}
			return result;
		}

		public void ForEach(Action<IQueueItem> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			for (Node<IQueueItem> next = this.head; next != null; next = next.Next)
			{
				MessageQueue.RunUnderPoisonContext(next.Value, action);
			}
		}

		public void ForEach<T>(Action<IQueueItem, T> action, T state)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			for (Node<IQueueItem> next = this.head; next != null; next = next.Next)
			{
				MessageQueue.RunUnderPoisonContext<T>(next.Value, state, action);
			}
		}

		public int GetDeferredCount(DeferReason reason)
		{
			if (reason < DeferReason.None || reason >= (DeferReason)DeferredQueue.deferReasonsCount)
			{
				throw new ArgumentOutOfRangeException("reason");
			}
			return this.deferredCount[(int)reason];
		}

		private void IncrementCount(IQueueItem item)
		{
			RoutedMailItem routedMailItem = item as RoutedMailItem;
			if (routedMailItem != null && routedMailItem.DeferReason != DeferReason.None)
			{
				this.deferredCount[(int)routedMailItem.DeferReason]++;
				this.totalExcludedCount++;
				return;
			}
			this.count++;
		}

		private void DecrementCount(IQueueItem item)
		{
			RoutedMailItem routedMailItem = item as RoutedMailItem;
			if (routedMailItem != null && routedMailItem.DeferReason != DeferReason.None)
			{
				this.deferredCount[(int)routedMailItem.DeferReason]--;
				this.totalExcludedCount--;
				return;
			}
			this.count--;
		}

		private static readonly int deferReasonsCount = Enum.GetNames(typeof(DeferReason)).Length;

		private Node<IQueueItem> head;

		private long nextActivation = DateTime.UtcNow.Ticks + 36000000000L;

		private int count;

		private int[] deferredCount;

		private int totalExcludedCount;
	}
}
