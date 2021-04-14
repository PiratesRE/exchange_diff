using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal interface ICacheTracer<K>
	{
		void Accessed(K key, CachableItem value, AccessStatus accessStatus, DateTime timestamp);

		void Flushed(long cacheSize, DateTime timestamp);

		void ItemAdded(K key, CachableItem value, DateTime timestamp);

		void ItemRemoved(K key, CachableItem value, CacheItemRemovedReason removeReason, DateTime timestamp);

		void TraceException(string details, Exception exception);
	}
}
