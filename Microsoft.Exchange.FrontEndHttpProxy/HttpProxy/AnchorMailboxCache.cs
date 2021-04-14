using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy
{
	internal class AnchorMailboxCache
	{
		private AnchorMailboxCache()
		{
			this.innerCache = new ExactTimeoutCache<string, AnchorMailboxCacheEntry>(delegate(string k, AnchorMailboxCacheEntry v, RemoveReason r)
			{
				this.UpdateCacheSizeCounter();
			}, null, null, AnchorMailboxCache.AnchorMailboxCacheSize.Value, false);
			if (HttpProxySettings.AnchorMailboxSharedCacheEnabled.Value)
			{
				this.sharedCacheClient = new SharedCacheClient(WellKnownSharedCache.AnchorMailboxCache, "AnchorMailboxCache_" + HttpProxyGlobals.ProtocolType, HttpProxySettings.GlobalSharedCacheRpcTimeout.Value, ConcurrencyGuards.SharedCache);
			}
		}

		public static AnchorMailboxCache Instance
		{
			get
			{
				if (AnchorMailboxCache.instance == null)
				{
					lock (AnchorMailboxCache.staticLock)
					{
						if (AnchorMailboxCache.instance == null)
						{
							AnchorMailboxCache.instance = new AnchorMailboxCache();
						}
					}
				}
				return AnchorMailboxCache.instance;
			}
		}

		private static TimeSpan AnchorMailboxCacheAbsoluteTimeout
		{
			get
			{
				if (HttpProxySettings.AnchorMailboxSharedCacheEnabled.Value)
				{
					return AnchorMailboxCache.CacheAbsoluteTimeoutWithSharedCache.Value;
				}
				return AnchorMailboxCache.CacheAbsoluteTimeoutInMemoryCache.Value;
			}
		}

		private static TimeSpan AnchorMailboxCacheSlidingTimeout
		{
			get
			{
				if (HttpProxySettings.AnchorMailboxSharedCacheEnabled.Value)
				{
					return AnchorMailboxCache.CacheSlidingTimeoutWithSharedCache.Value;
				}
				return AnchorMailboxCache.CacheSlidingTimeoutInMemoryCache.Value;
			}
		}

		public bool TryGet(string key, IRequestContext requestContext, out AnchorMailboxCacheEntry entry)
		{
			PerfCounters.HttpProxyCacheCountersInstance.AnchorMailboxLocalCacheHitsRateBase.Increment();
			PerfCounters.HttpProxyCacheCountersInstance.AnchorMailboxOverallCacheHitsRateBase.Increment();
			entry = null;
			if (this.innerCache.TryGetValue(key, out entry))
			{
				PerfCounters.HttpProxyCacheCountersInstance.AnchorMailboxLocalCacheHitsRate.Increment();
			}
			else if (HttpProxySettings.AnchorMailboxSharedCacheEnabled.Value)
			{
				long latency = 0L;
				string diagInfo = null;
				AnchorMailboxCacheEntry sharedCacheEntry = null;
				bool latency2 = LatencyTracker.GetLatency<bool>(() => this.sharedCacheClient.TryGet<AnchorMailboxCacheEntry>(key, requestContext.ActivityId, out sharedCacheEntry, out diagInfo), out latency);
				requestContext.LogSharedCacheCall(latency, diagInfo);
				if (latency2)
				{
					this.Add(key, sharedCacheEntry, requestContext, false);
					entry = sharedCacheEntry;
				}
			}
			if (entry != null)
			{
				PerfCounters.HttpProxyCacheCountersInstance.AnchorMailboxOverallCacheHitsRate.Increment();
				return true;
			}
			return false;
		}

		public void Add(string key, AnchorMailboxCacheEntry entry, IRequestContext requestContext)
		{
			this.Add(key, entry, requestContext, true);
		}

		public void Add(string key, AnchorMailboxCacheEntry entry, IRequestContext requestContext, bool addToSharedCache)
		{
			this.Add(key, entry, DateTime.UtcNow, requestContext, addToSharedCache);
		}

		public void Add(string key, AnchorMailboxCacheEntry entry, DateTime valueTimestamp, IRequestContext requestContext, bool addToSharedCache)
		{
			this.innerCache.TryInsertLimitedSliding(key, entry, AnchorMailboxCache.AnchorMailboxCacheAbsoluteTimeout, AnchorMailboxCache.AnchorMailboxCacheSlidingTimeout);
			if (HttpProxySettings.AnchorMailboxSharedCacheEnabled.Value && addToSharedCache)
			{
				long latency = 0L;
				string diagInfo = null;
				LatencyTracker.GetLatency<bool>(() => this.sharedCacheClient.TryInsert(key, entry, valueTimestamp, requestContext.ActivityId, out diagInfo), out latency);
				requestContext.LogSharedCacheCall(latency, diagInfo);
			}
			this.UpdateCacheSizeCounter();
		}

		public void Remove(string key, IRequestContext requestContext)
		{
			this.innerCache.Remove(key);
			if (HttpProxySettings.AnchorMailboxSharedCacheEnabled.Value)
			{
				long latency = 0L;
				string diagInfo = null;
				LatencyTracker.GetLatency<bool>(() => this.sharedCacheClient.TryRemove(key, requestContext.ActivityId, out diagInfo), out latency);
				requestContext.LogSharedCacheCall(latency, diagInfo);
			}
		}

		private void UpdateCacheSizeCounter()
		{
			if (AnchorMailboxCache.AnchorMailboxCacheSizeCounterUpdateEnabled.Value)
			{
				PerfCounters.HttpProxyCacheCountersInstance.AnchorMailboxCacheSize.RawValue = (long)this.innerCache.Count;
			}
		}

		private static readonly TimeSpanAppSettingsEntry CacheAbsoluteTimeoutInMemoryCache = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("AnchorMailboxCache.InMemoryOnly.AbsoluteTimeout"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(60.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry CacheAbsoluteTimeoutWithSharedCache = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("AnchorMailboxCache.WithSharedCache.AbsoluteTimeout"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(6.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry CacheSlidingTimeoutInMemoryCache = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("AnchorMailboxCache.InMemoryOnly.SlidingTimeout"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(30.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry CacheSlidingTimeoutWithSharedCache = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("AnchorMailboxCache.WithSharedCache.SlidingTimeout"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(3.0), ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry AnchorMailboxCacheSize = new IntAppSettingsEntry(HttpProxySettings.Prefix("AnchorMailboxCache.InMemoryMaxSize"), 100000, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry AnchorMailboxCacheSizeCounterUpdateEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("AnchorMailboxCacheSizeCounterUpdateEnabled"), true, ExTraceGlobals.VerboseTracer);

		private static AnchorMailboxCache instance;

		private static object staticLock = new object();

		private readonly ExactTimeoutCache<string, AnchorMailboxCacheEntry> innerCache;

		private SharedCacheClient sharedCacheClient;
	}
}
