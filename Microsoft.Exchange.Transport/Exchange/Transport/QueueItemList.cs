using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport
{
	internal class QueueItemList : ICollection<IQueueItem>, IEnumerable<IQueueItem>, IEnumerable
	{
		public bool IsEmpty
		{
			get
			{
				return this.head == null;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(IQueueItem item)
		{
			this.Add(new Node<IQueueItem>(item));
		}

		public void Add(Node<IQueueItem> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (this.IsEmpty)
			{
				this.head = node;
				this.tail = node;
			}
			else
			{
				this.tail.Next = node;
				this.tail = node;
			}
			this.count++;
			node.Next = null;
		}

		public void Clear()
		{
			this.head = null;
			this.tail = null;
			this.count = 0;
		}

		public bool Contains(IQueueItem item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(IQueueItem[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(IQueueItem item)
		{
			throw new NotImplementedException();
		}

		public void Concat(QueueItemList list)
		{
			if (list.IsEmpty)
			{
				return;
			}
			if (this.IsEmpty)
			{
				this.head = list.head;
				this.tail = list.tail;
			}
			else
			{
				this.tail.Next = list.head;
				this.tail = list.tail;
			}
			this.count += list.count;
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

		public IEnumerator<IQueueItem> GetEnumerator()
		{
			for (Node<IQueueItem> crt = this.head; crt != null; crt = crt.Next)
			{
				yield return crt.Value;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int count;

		private Node<IQueueItem> head;

		private Node<IQueueItem> tail;
	}
}
