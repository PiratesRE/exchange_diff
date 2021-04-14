using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport
{
	internal class ConditionBasedQueue
	{
		public ConditionBasedQueue()
		{
			this.lockedList = new Dictionary<WaitCondition, ConditionBasedQueue.LinkedListPair>();
			this.activeList = new ConditionBasedQueue.LinkedListPair(null, null);
			this.activeLock = new object();
			this.lockedLock = new object();
		}

		public int ActiveCount
		{
			get
			{
				return this.activeCount;
			}
		}

		public int TotalCount
		{
			get
			{
				return this.lockedCount + this.activeCount;
			}
		}

		public long OldestItem
		{
			get
			{
				if (!this.activeList.IsEmpty)
				{
					return this.activeList.Head.CreatedAt;
				}
				return DateTime.UtcNow.Ticks;
			}
		}

		public void Lock(IQueueItem item, WaitCondition condition, int dehydrateThreshold, Action<IQueueItem> dehydrateItem)
		{
			QueueNode node = new QueueNode(item);
			lock (this.lockedLock)
			{
				ConditionBasedQueue.Add(this.lockedList, condition, node, dehydrateThreshold, dehydrateItem);
				this.lockedCount++;
			}
		}

		public void RelockAll(IList<IQueueItem> items, string lockReason, ItemRelocked itemRelocked)
		{
			lock (this.activeLock)
			{
				lock (this.lockedLock)
				{
					this.lockedCount += this.activeCount;
					this.activeCount = 0;
					Dictionary<WaitCondition, ConditionBasedQueue.LinkedListPair> dictionary = new Dictionary<WaitCondition, ConditionBasedQueue.LinkedListPair>();
					if (items != null)
					{
						foreach (IQueueItem item in items)
						{
							WaitCondition condition;
							itemRelocked(item, lockReason, out condition);
							QueueNode node = new QueueNode(item);
							ConditionBasedQueue.Add(dictionary, condition, node, 0, null);
							this.lockedCount++;
						}
					}
					for (QueueNode queueNode = this.activeList.RemoveFirst(); queueNode != null; queueNode = this.activeList.RemoveFirst())
					{
						WaitCondition condition;
						itemRelocked(queueNode.Value, lockReason, out condition);
						ConditionBasedQueue.Add(dictionary, condition, queueNode, 0, null);
					}
					foreach (KeyValuePair<WaitCondition, ConditionBasedQueue.LinkedListPair> keyValuePair in dictionary)
					{
						ConditionBasedQueue.LinkedListPair list;
						if (!this.lockedList.TryGetValue(keyValuePair.Key, out list))
						{
							this.lockedList.Add(keyValuePair.Key, keyValuePair.Value);
						}
						else
						{
							keyValuePair.Value.Append(list);
							this.lockedList[keyValuePair.Key] = keyValuePair.Value;
						}
					}
				}
			}
		}

		public QueueItemList DequeueAll(Predicate<IQueueItem> match, bool lockedOnly)
		{
			QueueItemList queueItemList = new QueueItemList();
			if (!lockedOnly)
			{
				lock (this.activeLock)
				{
					queueItemList = ConditionBasedQueue.DequeueAll(match, this.activeList);
					this.activeCount -= queueItemList.Count;
				}
			}
			lock (this.lockedList)
			{
				List<WaitCondition> list = new List<WaitCondition>();
				foreach (KeyValuePair<WaitCondition, ConditionBasedQueue.LinkedListPair> keyValuePair in this.lockedList)
				{
					ConditionBasedQueue.LinkedListPair value = keyValuePair.Value;
					QueueItemList queueItemList2 = ConditionBasedQueue.DequeueAll(match, value);
					if (value.IsEmpty)
					{
						list.Add(keyValuePair.Key);
					}
					this.lockedCount -= queueItemList2.Count;
					queueItemList.Concat(queueItemList2);
				}
				if (list.Count > 0)
				{
					foreach (WaitCondition key in list)
					{
						this.lockedList.Remove(key);
					}
				}
			}
			return queueItemList;
		}

		public IQueueItem Dequeue()
		{
			IQueueItem result;
			lock (this.activeLock)
			{
				QueueNode queueNode = this.activeList.RemoveFirst();
				if (queueNode == null)
				{
					if (this.activeCount != 0)
					{
						throw new InvalidOperationException("We have active items that we have lost");
					}
					result = null;
				}
				else
				{
					this.activeCount--;
					result = queueNode.Value;
				}
			}
			return result;
		}

		public IQueueItem DequeueItem(DequeueMatch match, bool iterateLocked, out bool matchFound)
		{
			matchFound = false;
			lock (this.activeLock)
			{
				QueueNode queueNode;
				if (ConditionBasedQueue.DequeueItem(match, this.activeList, out queueNode))
				{
					matchFound = true;
					if (queueNode != null)
					{
						this.activeCount--;
						return queueNode.Value;
					}
					return null;
				}
			}
			if (iterateLocked)
			{
				lock (this.lockedLock)
				{
					IQueueItem result = null;
					List<WaitCondition> list = new List<WaitCondition>();
					foreach (KeyValuePair<WaitCondition, ConditionBasedQueue.LinkedListPair> keyValuePair in this.lockedList)
					{
						ConditionBasedQueue.LinkedListPair value = keyValuePair.Value;
						QueueNode queueNode;
						matchFound = ConditionBasedQueue.DequeueItem(match, value, out queueNode);
						if (matchFound)
						{
							if (value.IsEmpty)
							{
								list.Add(keyValuePair.Key);
							}
							if (queueNode != null)
							{
								this.lockedCount--;
								result = queueNode.Value;
								break;
							}
							break;
						}
					}
					if (list.Count > 0)
					{
						foreach (WaitCondition key in list)
						{
							this.lockedList.Remove(key);
						}
					}
					return result;
				}
			}
			return null;
		}

		public void ForEach(Action<IQueueItem> action, bool iterateLocked)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bool flag;
			this.DequeueItem(delegate(IQueueItem item)
			{
				MessageQueue.RunUnderPoisonContext(item, action);
				return DequeueMatchResult.Continue;
			}, iterateLocked, out flag);
		}

		public void ForEach<T>(Action<IQueueItem, T> action, T state, bool iterateLocked)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bool flag;
			this.DequeueItem(delegate(IQueueItem item)
			{
				MessageQueue.RunUnderPoisonContext<T>(item, state, action);
				return DequeueMatchResult.Continue;
			}, iterateLocked, out flag);
		}

		public bool ActivateOne(WaitCondition condition, AccessToken token, ItemUnlocked itemUnlocked)
		{
			lock (this.activeLock)
			{
				QueueNode queueNode;
				lock (this.lockedLock)
				{
					ConditionBasedQueue.LinkedListPair linkedListPair;
					if (!this.lockedList.TryGetValue(condition, out linkedListPair))
					{
						return false;
					}
					for (;;)
					{
						queueNode = linkedListPair.RemoveFirst();
						if (queueNode == null)
						{
							break;
						}
						this.lockedCount--;
						if (itemUnlocked(queueNode.Value, token))
						{
							goto Block_7;
						}
					}
					this.lockedList.Remove(condition);
					return false;
					Block_7:;
				}
				this.activeList.Append(queueNode);
				this.activeCount++;
			}
			return true;
		}

		internal XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("conditionQueue");
			xelement.Add(new XElement("UnlockedCount", this.ActiveCount));
			xelement.Add(new XElement("Locked", this.lockedCount));
			lock (this.lockedLock)
			{
				XElement xelement2 = new XElement("LockedList");
				foreach (KeyValuePair<WaitCondition, ConditionBasedQueue.LinkedListPair> keyValuePair in this.lockedList)
				{
					XElement xelement3 = new XElement("condition");
					xelement3.Add(new XElement("condition", keyValuePair.Key.ToString()));
					xelement3.Add(new XElement("lockedCount", keyValuePair.Value.Count));
					xelement3.Add(new XElement("dehydrated", keyValuePair.Value.Dehydrated));
					xelement3.Add(new XElement("lastProcessedTime", keyValuePair.Value.LastDequeueTime));
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			return xelement;
		}

		private static void Add(Dictionary<WaitCondition, ConditionBasedQueue.LinkedListPair> lockedList, WaitCondition condition, QueueNode node, int dehydrateThreshold, Action<IQueueItem> dehydrateDelegate)
		{
			ConditionBasedQueue.LinkedListPair linkedListPair;
			if (!lockedList.TryGetValue(condition, out linkedListPair))
			{
				linkedListPair = new ConditionBasedQueue.LinkedListPair(node, null);
				lockedList[condition] = linkedListPair;
			}
			else
			{
				linkedListPair.Append(node);
			}
			if (dehydrateThreshold <= 0 || dehydrateDelegate == null || linkedListPair.Count <= dehydrateThreshold)
			{
				linkedListPair.Dehydrated = false;
				return;
			}
			if (!linkedListPair.Dehydrated)
			{
				QueueNode queueNode;
				ConditionBasedQueue.DequeueItem(delegate(IQueueItem item)
				{
					dehydrateDelegate(item);
					return DequeueMatchResult.Continue;
				}, linkedListPair, out queueNode);
				linkedListPair.Dehydrated = true;
				return;
			}
			dehydrateDelegate(node.Value);
		}

		private static bool DequeueItem(DequeueMatch match, ConditionBasedQueue.LinkedListPair list, out QueueNode matchedItem)
		{
			QueueNode queueNode = list.Head;
			QueueNode previous = null;
			bool flag = false;
			matchedItem = null;
			while (queueNode != null && !flag)
			{
				switch (match(queueNode.Value))
				{
				case DequeueMatchResult.Break:
					flag = true;
					break;
				case DequeueMatchResult.DequeueAndBreak:
					list.RemoveCurrent(previous, queueNode);
					matchedItem = queueNode;
					flag = true;
					break;
				case DequeueMatchResult.Continue:
					previous = queueNode;
					queueNode = (QueueNode)queueNode.Next;
					break;
				default:
					throw new InvalidOperationException("Invalid return value from match()");
				}
			}
			return flag;
		}

		private static QueueItemList DequeueAll(Predicate<IQueueItem> match, ConditionBasedQueue.LinkedListPair list)
		{
			QueueItemList queueItemList = new QueueItemList();
			QueueNode queueNode = list.Head;
			QueueNode previous = null;
			while (queueNode != null)
			{
				if (match(queueNode.Value))
				{
					list.RemoveCurrent(previous, queueNode);
					queueItemList.Add(queueNode.Value);
				}
				else
				{
					previous = queueNode;
				}
				queueNode = (QueueNode)queueNode.Next;
			}
			return queueItemList;
		}

		private ConditionBasedQueue.LinkedListPair activeList;

		private Dictionary<WaitCondition, ConditionBasedQueue.LinkedListPair> lockedList;

		private int lockedCount;

		private int activeCount;

		private object activeLock;

		private object lockedLock;

		private class LinkedListPair
		{
			public LinkedListPair(QueueNode head, QueueNode tail)
			{
				this.Head = head;
				this.Tail = tail;
				this.count = ((this.Head != null) ? 1 : 0) + ((this.Tail != null) ? 1 : 0);
			}

			public bool IsEmpty
			{
				get
				{
					return this.Head == null;
				}
			}

			public int Count
			{
				get
				{
					return this.count;
				}
			}

			public bool Dehydrated
			{
				get
				{
					return this.dehydrated;
				}
				set
				{
					this.dehydrated = value;
				}
			}

			public DateTime LastDequeueTime
			{
				get
				{
					return this.lastDequeueTime;
				}
			}

			public QueueNode Head { get; set; }

			public QueueNode Tail { get; set; }

			public void Append(QueueNode end)
			{
				if (end == null)
				{
					throw new ArgumentNullException("end");
				}
				this.count++;
				if (this.Head == null)
				{
					this.Head = end;
					return;
				}
				if (this.Tail == null)
				{
					this.Head.Next = end;
					this.Tail = end;
					return;
				}
				this.Tail.Next = end;
				this.Tail = end;
			}

			public void Append(ConditionBasedQueue.LinkedListPair list2)
			{
				if (list2 == null)
				{
					throw new ArgumentNullException("list2");
				}
				if (list2.IsEmpty)
				{
					return;
				}
				if (this.IsEmpty)
				{
					this.Head = list2.Head;
					this.Tail = list2.Tail;
					this.count = list2.Count;
					return;
				}
				this.count += list2.Count;
				QueueNode tail = list2.Tail;
				if (list2.Tail == null)
				{
					tail = list2.Head;
				}
				if (this.Tail != null)
				{
					this.Tail.Next = list2.Head;
				}
				else
				{
					this.Head.Next = list2.Head;
				}
				this.Tail = tail;
			}

			public void Prepend(QueueNode start)
			{
				if (start == null)
				{
					throw new ArgumentNullException("start");
				}
				this.count++;
				if (this.Tail == null)
				{
					this.Tail = this.Head;
				}
				start.Next = this.Head;
				this.Head = start;
			}

			public QueueNode RemoveFirst()
			{
				if (this.IsEmpty)
				{
					return null;
				}
				this.count--;
				QueueNode head = this.Head;
				if (this.Tail == null)
				{
					this.Head = null;
				}
				else
				{
					this.Head = (QueueNode)this.Head.Next;
					if (this.Head == this.Tail)
					{
						this.Tail = null;
					}
				}
				this.lastDequeueTime = DateTime.UtcNow;
				head.Next = null;
				return head;
			}

			public void RemoveCurrent(QueueNode previous, QueueNode current)
			{
				this.count--;
				if (previous == null)
				{
					this.Head = (QueueNode)current.Next;
					if (!this.IsEmpty && this.Head.Next == null)
					{
						this.Tail = null;
						return;
					}
				}
				else if (current == this.Tail)
				{
					previous.Next = null;
					if (previous == this.Head)
					{
						this.Tail = null;
						return;
					}
					this.Tail = previous;
					return;
				}
				else
				{
					previous.Next = current.Next;
				}
			}

			private int count;

			private bool dehydrated;

			private DateTime lastDequeueTime = DateTime.UtcNow;
		}
	}
}
