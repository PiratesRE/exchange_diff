using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal class PercentilePerfCounters
	{
		internal static void UpdateRoutingLatencyPerfCounter(string siteName, double value)
		{
			if (PerfCounters.HttpProxyCountersInstance.TotalProxyRequests.RawValue < 5L)
			{
				return;
			}
			value = ((value >= 0.0) ? value : 0.0);
			IPercentileCounter percentileCounter = PercentilePerfCounters.GetPercentileCounter(siteName);
			percentileCounter.AddValue((long)Convert.ToInt32(value));
			HttpProxyPerSiteCountersInstance httpProxyPerSiteCountersInstance = PerfCounters.GetHttpProxyPerSiteCountersInstance(siteName);
			PercentilePerfCounters.UpdateCounterInstance(httpProxyPerSiteCountersInstance.RoutingLatency90thPercentile, percentileCounter, 90U);
			PercentilePerfCounters.UpdateCounterInstance(httpProxyPerSiteCountersInstance.RoutingLatency95thPercentile, percentileCounter, 95U);
			PercentilePerfCounters.UpdateCounterInstance(httpProxyPerSiteCountersInstance.RoutingLatency99thPercentile, percentileCounter, 99U);
		}

		private static void UpdateCounterInstance(ExPerformanceCounter perfCounter, IPercentileCounter percentileCounter, uint percentile)
		{
			perfCounter.RawValue = percentileCounter.PercentileQuery(percentile);
		}

		private static IPercentileCounter GetPercentileCounter(string siteName)
		{
			if (!PercentilePerfCounters.siteToLatencyPercentileCounters.ContainsKey(siteName))
			{
				lock (PercentilePerfCounters.siteToLatencyPercentileCounters)
				{
					if (!PercentilePerfCounters.siteToLatencyPercentileCounters.ContainsKey(siteName))
					{
						PercentilePerfCounters.siteToLatencyPercentileCounters.Add(siteName, new PercentileCounter(PercentilePerfCounters.ExpiryInternal, PercentilePerfCounters.GranularityInterval, (long)PercentilePerfCounters.ValueGranularity, (long)PercentilePerfCounters.UpperLimit));
					}
				}
			}
			return PercentilePerfCounters.siteToLatencyPercentileCounters[siteName];
		}

		private static readonly TimeSpan ExpiryInternal = TimeSpan.FromMinutes(30.0);

		private static readonly TimeSpan GranularityInterval = TimeSpan.FromMinutes(5.0);

		private static readonly int ValueGranularity = 10;

		private static readonly int UpperLimit = 30000;

		private static Dictionary<string, IPercentileCounter> siteToLatencyPercentileCounters = new Dictionary<string, IPercentileCounter>();
	}
}
