using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal struct KeyQueue<TKey>
	{
		public KeyQueue(int capacity)
		{
			this.capacity = capacity;
			this.hashData = new Dictionary<TKey, LinkedListNode<TKey>>(capacity);
			this.listData = new LinkedList<TKey>();
		}

		public int Capacity
		{
			get
			{
				return this.capacity;
			}
		}

		public int Count
		{
			get
			{
				return this.hashData.Count;
			}
		}

		public TKey TailKey
		{
			get
			{
				return this.listData.Last.Value;
			}
		}

		public void AddTail(TKey key)
		{
			this.listData.AddLast(key);
			this.hashData[key] = this.listData.Last;
		}

		public void AddHead(TKey key)
		{
			this.listData.AddFirst(key);
			this.hashData[key] = this.listData.First;
		}

		public bool Contains(TKey key)
		{
			return this.hashData.ContainsKey(key);
		}

		public void Remove(TKey key)
		{
			LinkedListNode<TKey> node;
			if (this.hashData.TryGetValue(key, out node))
			{
				this.listData.Remove(node);
				this.hashData.Remove(key);
			}
		}

		public void RemoveTail()
		{
			this.hashData.Remove(this.listData.Last.Value);
			this.listData.RemoveLast();
		}

		public void MoveToHead(TKey key)
		{
			LinkedListNode<TKey> linkedListNode;
			if (this.hashData.TryGetValue(key, out linkedListNode) && linkedListNode != this.listData.First)
			{
				this.listData.Remove(linkedListNode);
				this.listData.AddFirst(linkedListNode);
			}
		}

		public void Reset()
		{
			this.hashData.Clear();
			this.listData.Clear();
		}

		private readonly int capacity;

		private Dictionary<TKey, LinkedListNode<TKey>> hashData;

		private LinkedList<TKey> listData;
	}
}
