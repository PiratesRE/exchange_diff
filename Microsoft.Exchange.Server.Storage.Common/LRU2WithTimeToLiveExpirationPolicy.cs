using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class LRU2WithTimeToLiveExpirationPolicy<TKey> : LRU2ExpirationPolicy<TKey>
	{
		public LRU2WithTimeToLiveExpirationPolicy(int capacity, TimeSpan timeToLive, bool ageEviction) : base(capacity)
		{
			this.timeToLive = timeToLive;
			this.ageEviction = ageEviction;
			this.keyTimeQueue = new KeyTimeQueue<TKey>(capacity);
		}

		public override void EvictionCheckpoint()
		{
			base.EvictionCheckpoint();
			DateTime t = DateTime.UtcNow - this.timeToLive;
			while (this.keyTimeQueue.Count != 0 && this.keyTimeQueue.TailKeyTime < t)
			{
				base.ForceEvictKey(this.keyTimeQueue.TailKey);
			}
		}

		public override void Insert(TKey key)
		{
			base.Insert(key);
			this.keyTimeQueue.AddHead(key);
		}

		public override void Remove(TKey key)
		{
			if (this.keyTimeQueue.Contains(key))
			{
				this.keyTimeQueue.Remove(key);
			}
			base.Remove(key);
		}

		public override void KeyAccess(TKey key)
		{
			if (this.ageEviction)
			{
				if (!base.ContainsKeyToCleanup(key))
				{
					base.KeyAccess(key);
					return;
				}
			}
			else
			{
				base.KeyAccess(key);
				this.keyTimeQueue.BumpToHead(key);
			}
		}

		public override void Reset()
		{
			this.keyTimeQueue.Reset();
			base.Reset();
		}

		protected override void OnEnqueuedKeyForEviction(TKey key)
		{
			this.keyTimeQueue.Remove(key);
			base.OnEnqueuedKeyForEviction(key);
		}

		private readonly TimeSpan timeToLive;

		private readonly bool ageEviction;

		private KeyTimeQueue<TKey> keyTimeQueue;
	}
}
