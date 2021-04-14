using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Common.Cache
{
	internal class DefaultCacheTracer<TCacheKey> : ICacheTracer<TCacheKey> where TCacheKey : class
	{
		public DefaultCacheTracer(ITracer tracer, string cacheName)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("cacheName", cacheName);
			this.cacheName = cacheName;
			this.tracer = tracer;
		}

		public static string GetAccessStatusString(AccessStatus status)
		{
			switch (status)
			{
			case AccessStatus.Hit:
				return "Hit";
			case AccessStatus.Miss:
				return "Miss";
			default:
				throw new ArgumentOutOfRangeException("status", status, "Unexpected value of status parameter. The GetAccessStatusString() method is out of sync with the AccessStatus enum definition.");
			}
		}

		public static string GetRemovedReasonString(CacheItemRemovedReason reason)
		{
			switch (reason)
			{
			case CacheItemRemovedReason.Removed:
				return "Removed";
			case CacheItemRemovedReason.Expired:
				return "Expired";
			case CacheItemRemovedReason.Scavenged:
				return "Scavenged";
			case CacheItemRemovedReason.OverWritten:
				return "OverWritten";
			case CacheItemRemovedReason.Clear:
				return "Clear";
			default:
				throw new ArgumentOutOfRangeException("reason", reason, "Unexpected value of reason parameter. The GetRemovedReasonString() method is out of sync with the CacheItemRemovedReason enum definition.");
			}
		}

		public void Flushed(long cacheSize, DateTime timestamp)
		{
			this.Trace<long>(timestamp, DefaultCacheTracer<TCacheKey>.CacheOperation.Flushed, default(TCacheKey), 0L, cacheSize);
		}

		public void Accessed(TCacheKey key, CachableItem value, AccessStatus accessStatus, DateTime timestamp)
		{
			long itemSize = (value != null) ? value.ItemSize : 0L;
			this.Trace<string>(timestamp, DefaultCacheTracer<TCacheKey>.CacheOperation.Accessed, key, itemSize, DefaultCacheTracer<TCacheKey>.GetAccessStatusString(accessStatus));
		}

		public void ItemAdded(TCacheKey key, CachableItem value, DateTime timestamp)
		{
			long itemSize = (value != null) ? value.ItemSize : 0L;
			this.Trace<string>(timestamp, DefaultCacheTracer<TCacheKey>.CacheOperation.Added, key, itemSize, string.Empty);
		}

		public void ItemRemoved(TCacheKey key, CachableItem value, CacheItemRemovedReason removeReason, DateTime timestamp)
		{
			long itemSize = (value != null) ? value.ItemSize : 0L;
			this.Trace<string>(timestamp, DefaultCacheTracer<TCacheKey>.CacheOperation.Removed, key, itemSize, DefaultCacheTracer<TCacheKey>.GetRemovedReasonString(removeReason));
		}

		public void TraceException(string details, Exception exception)
		{
			this.tracer.TraceError((long)this.GetHashCode(), string.Format("{0}: {1}, {2}, Exception: {3}", new object[]
			{
				ExDateTime.UtcNow,
				this.cacheName,
				details,
				exception
			}));
		}

		protected virtual string GetKeyString(TCacheKey key)
		{
			if (key != null)
			{
				return key.ToString();
			}
			return string.Empty;
		}

		private static string GetCacheOperationString(DefaultCacheTracer<TCacheKey>.CacheOperation operation)
		{
			switch (operation)
			{
			case DefaultCacheTracer<TCacheKey>.CacheOperation.Accessed:
				return "Accessed";
			case DefaultCacheTracer<TCacheKey>.CacheOperation.Added:
				return "Added";
			case DefaultCacheTracer<TCacheKey>.CacheOperation.Flushed:
				return "Flushed";
			case DefaultCacheTracer<TCacheKey>.CacheOperation.Removed:
				return "Removed";
			default:
				throw new ArgumentOutOfRangeException("operation", operation, "Unexpected value of operation parameter. The GetCacheOperationString() method is out of sync with the CacheOperation enum definition.");
			}
		}

		private void Trace<TRelatedInfo>(DateTime timestamp, DefaultCacheTracer<TCacheKey>.CacheOperation cacheOperation, TCacheKey key, long itemSize, TRelatedInfo relatedInfo)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "{0}: {1}, {2}, {3}, {4}, {5}", new object[]
			{
				timestamp,
				this.cacheName,
				DefaultCacheTracer<TCacheKey>.GetCacheOperationString(cacheOperation),
				this.GetKeyString(key),
				itemSize,
				relatedInfo
			});
		}

		private readonly string cacheName;

		private readonly ITracer tracer;

		private enum CacheOperation
		{
			Accessed,
			Added,
			Flushed,
			Removed
		}
	}
}
