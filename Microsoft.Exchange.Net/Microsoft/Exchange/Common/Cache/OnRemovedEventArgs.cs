using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal sealed class OnRemovedEventArgs<K, V> : EventArgs
	{
		public OnRemovedEventArgs(K key, V value, CacheItemRemovedReason removalReason)
		{
			this.Key = key;
			this.Value = value;
			this.RemovalReason = removalReason;
		}

		public K Key { get; private set; }

		public V Value { get; private set; }

		public CacheItemRemovedReason RemovalReason { get; private set; }
	}
}
