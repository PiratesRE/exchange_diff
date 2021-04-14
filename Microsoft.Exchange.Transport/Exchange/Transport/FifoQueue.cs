using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport
{
	internal class FifoQueue
	{
		public FifoQueue()
		{
			this.conditionQueue = new ConditionBasedQueue();
		}

		public int ActiveCount
		{
			get
			{
				return this.count + this.conditionQueue.ActiveCount;
			}
		}

		public int LockedCount
		{
			get
			{
				return this.conditionQueue.TotalCount - this.conditionQueue.ActiveCount;
			}
		}

		public int TotalCount
		{
			get
			{
				return this.count + this.conditionQueue.TotalCount;
			}
		}

		public long OldestItem
		{
			get
			{
				long num = (this.top != null) ? this.top.CreatedAt : DateTime.UtcNow.Ticks;
				if (this.conditionQueue.OldestItem < num)
				{
					num = this.conditionQueue.OldestItem;
				}
				return num;
			}
		}

		public void Enqueue(IQueueItem item)
		{
			if (item == null)
			{
				return;
			}
			QueueNode next = new QueueNode(item);
			if (this.tail == null)
			{
				this.top = next;
			}
			else
			{
				this.tail.Next = next;
			}
			this.tail = next;
			this.count++;
		}

		public IQueueItem Dequeue()
		{
			IQueueItem queueItem = this.conditionQueue.Dequeue();
			if (queueItem != null)
			{
				return queueItem;
			}
			QueueNode queueNode = this.top;
			if (queueNode == null)
			{
				return null;
			}
			this.top = (QueueNode)this.top.Next;
			if (this.top == null)
			{
				this.tail = null;
			}
			this.count--;
			queueNode.Next = null;
			return queueNode.Value;
		}

		public QueueItemList DequeueAll(Predicate<IQueueItem> match)
		{
			QueueNode queueNode = this.top;
			QueueNode queueNode2 = null;
			QueueItemList queueItemList = new QueueItemList();
			queueItemList.Concat(this.conditionQueue.DequeueAll(match, false));
			while (queueNode != null)
			{
				if (match(queueNode.Value))
				{
					QueueNode node = queueNode;
					queueNode = (QueueNode)queueNode.Next;
					if (queueNode2 == null)
					{
						this.top = queueNode;
					}
					else
					{
						queueNode2.Next = queueNode;
					}
					if (queueNode == null)
					{
						this.tail = queueNode2;
					}
					queueItemList.Add(node);
					this.count--;
				}
				else
				{
					queueNode2 = queueNode;
					queueNode = (QueueNode)queueNode.Next;
				}
			}
			return queueItemList;
		}

		public QueueItemList DequeueAllLocked(Predicate<IQueueItem> match)
		{
			return this.conditionQueue.DequeueAll(match, true);
		}

		public IQueueItem DequeueItem(DequeueMatch match, out bool matchFound)
		{
			matchFound = false;
			IQueueItem result = this.conditionQueue.DequeueItem(match, true, out matchFound);
			if (matchFound)
			{
				return result;
			}
			QueueNode queueNode = this.top;
			QueueNode queueNode2 = null;
			while (queueNode != null && !matchFound)
			{
				switch (match(queueNode.Value))
				{
				case DequeueMatchResult.Break:
					matchFound = true;
					break;
				case DequeueMatchResult.DequeueAndBreak:
					result = queueNode.Value;
					queueNode = (QueueNode)queueNode.Next;
					if (queueNode2 == null)
					{
						this.top = queueNode;
					}
					else
					{
						queueNode2.Next = queueNode;
					}
					if (queueNode == null)
					{
						this.tail = queueNode2;
					}
					this.count--;
					matchFound = true;
					break;
				case DequeueMatchResult.Continue:
					queueNode2 = queueNode;
					queueNode = (QueueNode)queueNode.Next;
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
			for (QueueNode queueNode = this.top; queueNode != null; queueNode = (QueueNode)queueNode.Next)
			{
				MessageQueue.RunUnderPoisonContext(queueNode.Value, action);
			}
			this.conditionQueue.ForEach(action, true);
		}

		public void ForEach<T>(Action<IQueueItem, T> action, T state)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			for (QueueNode queueNode = this.top; queueNode != null; queueNode = (QueueNode)queueNode.Next)
			{
				MessageQueue.RunUnderPoisonContext<T>(queueNode.Value, state, action);
			}
			this.conditionQueue.ForEach<T>(action, state, true);
		}

		public void Lock(IQueueItem item, WaitCondition condition, int dehydrateThreshold, Action<IQueueItem> dehydrateItem)
		{
			this.conditionQueue.Lock(item, condition, dehydrateThreshold, dehydrateItem);
		}

		public void RelockAll(IList<IQueueItem> items, string lockReason, ItemRelocked relockedItem)
		{
			this.conditionQueue.RelockAll(items, lockReason, relockedItem);
		}

		public bool ActivateOne(WaitCondition condition, AccessToken token, ItemUnlocked itemUnlocked)
		{
			return this.conditionQueue.ActivateOne(condition, token, itemUnlocked);
		}

		internal XElement GetDiagnosticInfo(XElement queue, bool conditionalQueuing)
		{
			queue.Add(new XElement("TotalCount", this.TotalCount));
			queue.Add(new XElement("LockedCount", this.LockedCount));
			queue.Add(new XElement("OldestItem", new DateTime(this.OldestItem).ToString()));
			if (conditionalQueuing)
			{
				queue.Add(this.conditionQueue.GetDiagnosticInfo());
			}
			return queue;
		}

		private ConditionBasedQueue conditionQueue;

		private QueueNode top;

		private QueueNode tail;

		private int count;
	}
}
