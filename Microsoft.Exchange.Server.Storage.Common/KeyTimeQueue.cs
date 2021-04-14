using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal struct KeyTimeQueue<TKey>
	{
		public KeyTimeQueue(int initialCapacity)
		{
			this.keyList = new LinkedList<KeyValuePair<TKey, DateTime>>();
			this.keyDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, DateTime>>>(initialCapacity);
		}

		public int Count
		{
			get
			{
				return this.keyDictionary.Count;
			}
		}

		public TKey TailKey
		{
			get
			{
				return this.keyList.Last.Value.Key;
			}
		}

		public DateTime TailKeyTime
		{
			get
			{
				return this.keyList.Last.Value.Value;
			}
		}

		public bool Contains(TKey key)
		{
			return this.keyDictionary.ContainsKey(key);
		}

		public void AddHead(TKey key)
		{
			this.AddHeadWithDateTime(key, DateTime.UtcNow);
		}

		public void Remove(TKey key)
		{
			LinkedListNode<KeyValuePair<TKey, DateTime>> node;
			if (this.keyDictionary.TryGetValue(key, out node))
			{
				this.keyList.Remove(node);
				this.keyDictionary.Remove(key);
			}
		}

		public void RemoveTail()
		{
			TKey tailKey = this.TailKey;
			this.keyDictionary.Remove(tailKey);
			this.keyList.RemoveLast();
		}

		public void BumpToHead(TKey key)
		{
			this.BumpToHeadWithDateTime(key, DateTime.UtcNow);
		}

		public void Reset()
		{
			this.keyList.Clear();
			this.keyDictionary.Clear();
		}

		internal void BumpToHeadWithDateTime(TKey key, DateTime newDateTime)
		{
			if (this.keyList.First.Value.Value > newDateTime)
			{
				newDateTime = this.keyList.First.Value.Value;
			}
			LinkedListNode<KeyValuePair<TKey, DateTime>> linkedListNode;
			if (this.keyDictionary.TryGetValue(key, out linkedListNode))
			{
				linkedListNode.Value = new KeyValuePair<TKey, DateTime>(key, newDateTime);
				if (linkedListNode != this.keyList.First)
				{
					this.keyList.Remove(linkedListNode);
					this.keyList.AddFirst(linkedListNode);
				}
			}
		}

		internal void AddHeadWithDateTime(TKey key, DateTime newDateTime)
		{
			if (this.Count != 0 && this.keyList.First.Value.Value > newDateTime)
			{
				newDateTime = this.keyList.First.Value.Value;
			}
			this.keyList.AddFirst(new KeyValuePair<TKey, DateTime>(key, newDateTime));
			this.keyDictionary[key] = this.keyList.First;
		}

		private readonly LinkedList<KeyValuePair<TKey, DateTime>> keyList;

		private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, DateTime>>> keyDictionary;
	}
}
