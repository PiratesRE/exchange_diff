using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal class NegativeAnchorMailboxCache
	{
		internal NegativeAnchorMailboxCache(TimeSpan cacheAbsoluteTimeout, TimeSpan gen1Timeout, TimeSpan gen2Timeout)
		{
			this.cacheAbsoluteTimeout = cacheAbsoluteTimeout;
			this.gen1Timeout = gen1Timeout;
			this.gen2Timeout = gen2Timeout;
			this.innerCache = new ExactTimeoutCache<string, NegativeAnchorMailboxCacheEntry>(delegate(string k, NegativeAnchorMailboxCacheEntry v, RemoveReason r)
			{
				this.UpdateCacheSizeCounter();
			}, null, null, NegativeAnchorMailboxCache.NegativeAnchorMailboxCacheSize.Value, false);
		}

		private NegativeAnchorMailboxCache() : this(NegativeAnchorMailboxCache.CacheAbsoluteTimeout.Value, NegativeAnchorMailboxCache.Gen1Timeout.Value, NegativeAnchorMailboxCache.Gen2Timeout.Value)
		{
		}

		public static NegativeAnchorMailboxCache Instance
		{
			get
			{
				return NegativeAnchorMailboxCache.StaticInstance;
			}
		}

		public void Add(string key, NegativeAnchorMailboxCacheEntry entry)
		{
			TimeSpan timeSpan = this.cacheAbsoluteTimeout;
			NegativeAnchorMailboxCacheEntry negativeAnchorMailboxCacheEntry;
			if (!this.TryGet(key, false, out negativeAnchorMailboxCacheEntry))
			{
				entry.StartTime = DateTime.UtcNow;
				entry.Generation = NegativeAnchorMailboxCacheEntry.CacheGeneration.One;
				this.Add(key, entry, timeSpan, true);
				return;
			}
			double num;
			NegativeAnchorMailboxCacheEntry.CacheGeneration generation;
			if (!this.IsDueForRefresh(negativeAnchorMailboxCacheEntry, out num, out generation))
			{
				return;
			}
			if (timeSpan.TotalSeconds > num)
			{
				negativeAnchorMailboxCacheEntry.Generation = generation;
				this.Add(key, negativeAnchorMailboxCacheEntry, timeSpan - TimeSpan.FromSeconds(num), false);
			}
		}

		public bool TryGet(string key, out NegativeAnchorMailboxCacheEntry entry)
		{
			if (!this.TryGet(key, true, out entry))
			{
				return false;
			}
			double num;
			NegativeAnchorMailboxCacheEntry.CacheGeneration cacheGeneration;
			if (this.IsDueForRefresh(entry, out num, out cacheGeneration))
			{
				return false;
			}
			PerfCounters.HttpProxyCacheCountersInstance.NegativeAnchorMailboxLocalCacheHitsRate.Increment();
			return true;
		}

		public void Remove(string key)
		{
			this.innerCache.Remove(key);
		}

		private bool IsDueForRefresh(NegativeAnchorMailboxCacheEntry entry, out double timeElapsedInSeconds, out NegativeAnchorMailboxCacheEntry.CacheGeneration nextGeneration)
		{
			timeElapsedInSeconds = 0.0;
			nextGeneration = NegativeAnchorMailboxCacheEntry.CacheGeneration.Max;
			if (entry.Generation == NegativeAnchorMailboxCacheEntry.CacheGeneration.Max)
			{
				return false;
			}
			timeElapsedInSeconds = (DateTime.UtcNow - entry.StartTime).TotalSeconds;
			if (timeElapsedInSeconds > this.gen2Timeout.TotalSeconds)
			{
				nextGeneration = NegativeAnchorMailboxCacheEntry.CacheGeneration.Max;
				return true;
			}
			if (timeElapsedInSeconds > this.gen1Timeout.TotalSeconds)
			{
				nextGeneration = NegativeAnchorMailboxCacheEntry.CacheGeneration.Two;
				return entry.Generation == NegativeAnchorMailboxCacheEntry.CacheGeneration.One;
			}
			return false;
		}

		private bool TryGet(string key, bool updatePerfCounter, out NegativeAnchorMailboxCacheEntry entry)
		{
			if (updatePerfCounter)
			{
				PerfCounters.HttpProxyCacheCountersInstance.NegativeAnchorMailboxLocalCacheHitsRateBase.Increment();
			}
			entry = null;
			return this.innerCache.TryGetValue(key, out entry);
		}

		private void Add(string key, NegativeAnchorMailboxCacheEntry entry, TimeSpan timeout, bool updatePerfCounter)
		{
			this.innerCache.TryInsertAbsolute(key, entry, timeout);
			if (updatePerfCounter)
			{
				this.UpdateCacheSizeCounter();
			}
		}

		private void UpdateCacheSizeCounter()
		{
			PerfCounters.HttpProxyCacheCountersInstance.NegativeAnchorMailboxCacheSize.RawValue = (long)this.innerCache.Count;
		}

		private static readonly TimeSpanAppSettingsEntry CacheAbsoluteTimeout = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("NegativeAnchorMailboxCacheAbsoluteTimeout"), TimeSpanUnit.Seconds, TimeSpan.FromSeconds(86400.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry Gen1Timeout = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("NegativeAnchorMailboxCacheG1Timeout"), TimeSpanUnit.Seconds, TimeSpan.FromSeconds(300.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry Gen2Timeout = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("NegativeAnchorMailboxCacheG2Timeout"), TimeSpanUnit.Seconds, TimeSpan.FromSeconds(2100.0), ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry NegativeAnchorMailboxCacheSize = new IntAppSettingsEntry(HttpProxySettings.Prefix("NegativeAnchorMailboxCacheSize"), 4000, ExTraceGlobals.VerboseTracer);

		private static readonly NegativeAnchorMailboxCache StaticInstance = new NegativeAnchorMailboxCache();

		private readonly ExactTimeoutCache<string, NegativeAnchorMailboxCacheEntry> innerCache;

		private readonly TimeSpan cacheAbsoluteTimeout;

		private readonly TimeSpan gen1Timeout;

		private readonly TimeSpan gen2Timeout;
	}
}
