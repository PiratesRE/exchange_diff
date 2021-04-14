using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CacheWithExpiration<TKey, TValue> where TValue : ILifetimeTrackable
	{
		public CacheWithExpiration(int maximumCacheSize, TimeSpan entryLifeTime, Action<TValue> cleanupDelegate)
		{
			this.cacheSize = maximumCacheSize;
			this.entryLifeTime = entryLifeTime;
			this.cleanupDelegate = cleanupDelegate;
			this.cache = new Dictionary<TKey, TValue>();
			this.cacheMutex = new object();
		}

		public bool TryGetValue(TKey key, DateTime currentTime, out TValue cachedValue)
		{
			bool result;
			lock (this.cacheMutex)
			{
				result = this.TryGetValueUnsafe(key, currentTime, out cachedValue);
			}
			return result;
		}

		public void Add(TKey key, DateTime currentTime, TValue newValue)
		{
			lock (this.cacheMutex)
			{
				this.AddUnsafe(key, currentTime, newValue);
			}
		}

		public bool TryAdd(TKey key, DateTime currentTime, TValue newValue)
		{
			bool result;
			lock (this.cacheMutex)
			{
				TValue tvalue;
				bool flag2 = this.TryGetValueUnsafe(key, currentTime, out tvalue);
				if (flag2)
				{
					result = false;
				}
				else
				{
					this.AddUnsafe(key, currentTime, newValue);
					result = true;
				}
			}
			return result;
		}

		public bool Set(TKey key, DateTime currentTime, TValue newValue)
		{
			bool result;
			lock (this.cacheMutex)
			{
				bool flag2 = this.cache.Remove(key);
				this.AddUnsafe(key, currentTime, newValue);
				result = flag2;
			}
			return result;
		}

		public bool Remove(TKey key)
		{
			bool result;
			lock (this.cacheMutex)
			{
				bool flag2 = this.cache.Remove(key);
				result = flag2;
			}
			return result;
		}

		private bool TryGetValueUnsafe(TKey key, DateTime currentTime, out TValue cachedValue)
		{
			bool result = false;
			cachedValue = default(TValue);
			TValue tvalue;
			if (this.cache.TryGetValue(key, out tvalue))
			{
				if (tvalue.CreateTime > currentTime || currentTime.Subtract(tvalue.CreateTime) > this.entryLifeTime)
				{
					this.cache.Remove(key);
					if (this.cleanupDelegate != null)
					{
						this.cleanupDelegate(tvalue);
					}
					if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.AdminAuditLogTracer.TraceDebug<TKey, DateTime, DateTime>((long)this.GetHashCode(), "Entry for key '{0}' was found in the cache, but was evicted by time. Create time=[{1}], check time=[{2}]", key, tvalue.CreateTime, currentTime);
					}
				}
				else
				{
					tvalue.LastAccessTime = currentTime;
					cachedValue = tvalue;
					result = true;
					if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.AdminAuditLogTracer.TraceDebug<TKey>((long)this.GetHashCode(), "Entry for key '{0}' was found in the cache.", key);
					}
				}
			}
			else if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AdminAuditLogTracer.TraceDebug<TKey>((long)this.GetHashCode(), "Entry for key '{0}' was not found in the cache", key);
			}
			return result;
		}

		private void AddUnsafe(TKey key, DateTime currentTime, TValue newValue)
		{
			if (this.cache.Count == this.cacheSize)
			{
				TKey key2 = this.cache.OrderBy(delegate(KeyValuePair<TKey, TValue> pair)
				{
					TValue value = pair.Value;
					return value.LastAccessTime;
				}).First<KeyValuePair<TKey, TValue>>().Key;
				TValue obj = this.cache[key2];
				this.cache.Remove(key2);
				if (this.cleanupDelegate != null)
				{
					this.cleanupDelegate(obj);
				}
				if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.AdminAuditLogTracer.TraceDebug<TKey>((long)this.GetHashCode(), "Entry for key '{0}' was evicted from the cache by size.", key);
				}
			}
			this.cache.Add(key, newValue);
			newValue.LastAccessTime = currentTime;
		}

		private readonly int cacheSize;

		private readonly TimeSpan entryLifeTime;

		private readonly Action<TValue> cleanupDelegate;

		private readonly Dictionary<TKey, TValue> cache;

		private readonly object cacheMutex;
	}
}
