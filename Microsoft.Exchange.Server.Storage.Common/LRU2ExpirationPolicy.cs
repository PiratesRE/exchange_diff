using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class LRU2ExpirationPolicy<TKey> : EvictionPolicy<TKey>
	{
		public LRU2ExpirationPolicy(int capacity) : base(capacity)
		{
			this.longTermQueue = new KeyQueue<TKey>(capacity * 3 / 4);
			this.shortTermQueue = new KeyQueue<TKey>(capacity / 4);
			this.memoryQueue = new KeyQueue<TKey>(capacity / 2);
		}

		public override int Count
		{
			get
			{
				return this.longTermQueue.Count + this.shortTermQueue.Count + base.Count;
			}
		}

		internal KeyQueue<TKey> LongTermQueueForTest
		{
			get
			{
				return this.longTermQueue;
			}
		}

		internal KeyQueue<TKey> ShortTermQueueForTest
		{
			get
			{
				return this.shortTermQueue;
			}
		}

		internal KeyQueue<TKey> MemoryQueueForTest
		{
			get
			{
				return this.memoryQueue;
			}
		}

		public override void EvictionCheckpoint()
		{
		}

		public override void Insert(TKey key)
		{
			if (this.Contains(key))
			{
				return;
			}
			if (this.memoryQueue.Contains(key))
			{
				if (this.longTermQueue.Count == this.longTermQueue.Capacity)
				{
					TKey tailKey = this.longTermQueue.TailKey;
					this.longTermQueue.RemoveTail();
					base.AddKeyToCleanup(tailKey);
					this.OnEnqueuedKeyForEviction(tailKey);
				}
				this.longTermQueue.AddHead(key);
				return;
			}
			if (this.shortTermQueue.Count == this.shortTermQueue.Capacity)
			{
				TKey tailKey2 = this.shortTermQueue.TailKey;
				this.shortTermQueue.RemoveTail();
				this.InsertToMemoryQueue(tailKey2);
				base.AddKeyToCleanup(tailKey2);
				this.OnEnqueuedKeyForEviction(tailKey2);
			}
			this.shortTermQueue.AddHead(key);
		}

		public override void Remove(TKey key)
		{
			if (this.longTermQueue.Contains(key))
			{
				this.longTermQueue.Remove(key);
				return;
			}
			if (this.shortTermQueue.Contains(key))
			{
				this.shortTermQueue.Remove(key);
				return;
			}
			base.RemoveKeyToCleanup(key);
		}

		public override void KeyAccess(TKey key)
		{
			if (this.longTermQueue.Contains(key))
			{
				this.longTermQueue.MoveToHead(key);
				return;
			}
			if (this.shortTermQueue.Contains(key))
			{
				return;
			}
			base.RemoveKeyToCleanup(key);
			this.Insert(key);
		}

		public override bool Contains(TKey key)
		{
			return this.longTermQueue.Contains(key) || this.shortTermQueue.Contains(key) || base.Contains(key);
		}

		public override void Reset()
		{
			this.longTermQueue.Reset();
			this.shortTermQueue.Reset();
			this.memoryQueue.Reset();
			base.Reset();
		}

		protected virtual void OnEnqueuedKeyForEviction(TKey key)
		{
		}

		protected void ForceEvictKey(TKey key)
		{
			if (this.shortTermQueue.Contains(key))
			{
				this.shortTermQueue.Remove(key);
			}
			else if (this.longTermQueue.Contains(key))
			{
				this.longTermQueue.Remove(key);
			}
			base.AddKeyToCleanup(key);
			this.OnEnqueuedKeyForEviction(key);
		}

		private void InsertToMemoryQueue(TKey lastKey)
		{
			if (this.memoryQueue.Count >= this.memoryQueue.Capacity)
			{
				this.memoryQueue.RemoveTail();
			}
			this.memoryQueue.AddHead(lastKey);
		}

		private KeyQueue<TKey> longTermQueue;

		private KeyQueue<TKey> shortTermQueue;

		private KeyQueue<TKey> memoryQueue;
	}
}
