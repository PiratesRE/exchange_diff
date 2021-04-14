using System;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class PendingGetConnectionCache : IPendingGetConnectionCache
	{
		static PendingGetConnectionCache()
		{
			foreach (ExPerformanceCounter exPerformanceCounter in PendingGetCounters.AllCounters)
			{
				exPerformanceCounter.Reset();
			}
			PendingGetConnectionCache.Instance = new PendingGetConnectionCache();
			PendingGetConnectionCache.ConnectionCacheCounter = new ItemCounter(PendingGetCounters.PendingGetConnectionCacheCount, PendingGetCounters.PendingGetConnectionCachePeak, PendingGetCounters.PendingGetConnectionCacheTotal);
		}

		private static ItemCounter ConnectionCacheCounter { get; set; }

		public IPendingGetConnection AddOrGetConnection(string connectionId)
		{
			IPendingGetConnection result;
			if (!this.TryGetConnection(connectionId, out result))
			{
				result = PendingGetConnectionCache.AddNewConnection(connectionId);
			}
			return result;
		}

		public bool TryGetConnection(string connectionId, out IPendingGetConnection connection)
		{
			AverageTimeCounterBase averageTimeCounterBase = new AverageTimeCounterBase(PendingGetCounters.TryGetConnectionAverageTime, PendingGetCounters.TryGetConnectionAverageTimeBase, true);
			connection = (HttpRuntime.Cache.Get(PendingGetConnectionCache.GetCacheKey(connectionId)) as IPendingGetConnection);
			averageTimeCounterBase.Stop();
			return connection != null;
		}

		private static IPendingGetConnection AddNewConnection(string connectionId)
		{
			AverageTimeCounterBase averageTimeCounterBase = new AverageTimeCounterBase(PendingGetCounters.AddNewConnectionAverageTime, PendingGetCounters.AddNewConnectionAverageTimeBase, true);
			PendingGetConnection pendingGetConnection = new PendingGetConnection(connectionId);
			ExTraceGlobals.PendingGetPublisherTracer.TraceDebug<string>((long)pendingGetConnection.GetHashCode(), "[AddNewConnection] Create new PendingGet session for PendingGet channel - {0}", connectionId);
			AverageTimeCounterBase connectionCachedTime = new AverageTimeCounterBase(PendingGetCounters.ConnectionCachedAverageTime, PendingGetCounters.ConnectionCachedAverageTimeBase, true);
			HttpRuntime.Cache.Insert(PendingGetConnectionCache.GetCacheKey(connectionId), pendingGetConnection, null, DateTime.MaxValue, TimeSpan.FromMinutes(120.0), CacheItemPriority.Normal, delegate(string key, object value, CacheItemRemovedReason reason)
			{
				PendingGetConnectionCache.ConnectionCacheCounter.Decrement();
				connectionCachedTime.Stop();
			});
			averageTimeCounterBase.Stop();
			PendingGetConnectionCache.ConnectionCacheCounter.Increment();
			return pendingGetConnection;
		}

		private static string GetCacheKey(string connectionId)
		{
			return "pnm__" + connectionId;
		}

		private const long DefaultPendingGetSessionTimeoutInMinutes = 120L;

		private const string PendingGetSessionKeyPrefix = "pnm__";

		public static readonly PendingGetConnectionCache Instance;
	}
}
