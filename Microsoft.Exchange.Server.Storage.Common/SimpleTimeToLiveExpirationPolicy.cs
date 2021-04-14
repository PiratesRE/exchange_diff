using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class SimpleTimeToLiveExpirationPolicy<TKey> : EvictionPolicy<TKey>
	{
		public SimpleTimeToLiveExpirationPolicy(int initialCapacity, TimeSpan timeToLive, bool ageEviction) : base(initialCapacity)
		{
			this.timeToLive = timeToLive;
			this.ageEviction = ageEviction;
			this.keyTimeQueue = new KeyTimeQueue<TKey>(initialCapacity);
		}

		public override int Count
		{
			get
			{
				return this.keyTimeQueue.Count + base.Count;
			}
		}

		public override void EvictionCheckpoint()
		{
			this.EvictionCheckpointAtDateTime(DateTime.UtcNow);
		}

		public override void Insert(TKey key)
		{
			this.InsertAtDateTime(key, DateTime.UtcNow);
		}

		public override void Remove(TKey key)
		{
			if (this.keyTimeQueue.Contains(key))
			{
				this.keyTimeQueue.Remove(key);
				return;
			}
			base.RemoveKeyToCleanup(key);
		}

		public override void KeyAccess(TKey key)
		{
			if (this.ageEviction)
			{
				return;
			}
			if (base.ContainsKeyToCleanup(key))
			{
				base.RemoveKeyToCleanup(key);
				this.keyTimeQueue.AddHead(key);
				return;
			}
			this.keyTimeQueue.BumpToHead(key);
		}

		public override bool Contains(TKey key)
		{
			return this.keyTimeQueue.Contains(key) || base.Contains(key);
		}

		public override void Reset()
		{
			this.keyTimeQueue.Reset();
			base.Reset();
		}

		internal void InsertAtDateTime(TKey key, DateTime datetimeInsertion)
		{
			if (!this.keyTimeQueue.Contains(key))
			{
				this.keyTimeQueue.AddHeadWithDateTime(key, datetimeInsertion);
			}
		}

		internal void EvictionCheckpointAtDateTime(DateTime datetimeEviction)
		{
			DateTime t = datetimeEviction - this.timeToLive;
			while (this.keyTimeQueue.Count != 0 && this.keyTimeQueue.TailKeyTime < t)
			{
				TKey tailKey = this.keyTimeQueue.TailKey;
				this.keyTimeQueue.RemoveTail();
				base.AddKeyToCleanup(tailKey);
			}
		}

		private readonly TimeSpan timeToLive;

		private readonly bool ageEviction;

		private KeyTimeQueue<TKey> keyTimeQueue;
	}
}
