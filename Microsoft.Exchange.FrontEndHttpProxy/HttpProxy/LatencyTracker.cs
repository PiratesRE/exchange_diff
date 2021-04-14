using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy
{
	internal class LatencyTracker
	{
		internal LatencyTracker()
		{
			this.latencyTrackerStopwatch.Start();
			this.glsLatencies = new List<long>(4);
			this.accountForestLatencies = new List<long>(4);
			this.resourceForestLatencies = new List<long>(4);
			this.sharedCacheLatencies = new List<long>(4);
		}

		internal string GlsLatencyBreakup
		{
			get
			{
				return LatencyTracker.GetBreakupOfLatencies(this.glsLatencies);
			}
		}

		internal long TotalGlsLatency
		{
			get
			{
				return this.glsLatencies.Sum();
			}
		}

		internal string AccountForestLatencyBreakup
		{
			get
			{
				return LatencyTracker.GetBreakupOfLatencies(this.accountForestLatencies);
			}
		}

		internal long TotalAccountForestDirectoryLatency
		{
			get
			{
				return this.accountForestLatencies.Sum();
			}
		}

		internal string ResourceForestLatencyBreakup
		{
			get
			{
				return LatencyTracker.GetBreakupOfLatencies(this.resourceForestLatencies);
			}
		}

		internal long TotalResourceForestDirectoryLatency
		{
			get
			{
				return this.resourceForestLatencies.Sum();
			}
		}

		internal long AdLatency
		{
			get
			{
				return this.TotalAccountForestDirectoryLatency + this.TotalResourceForestDirectoryLatency;
			}
		}

		internal string SharedCacheLatencyBreakup
		{
			get
			{
				return LatencyTracker.GetBreakupOfLatencies(this.sharedCacheLatencies);
			}
		}

		internal long TotalSharedCacheLatency
		{
			get
			{
				return this.sharedCacheLatencies.Sum();
			}
		}

		internal static LatencyTracker FromHttpContext(HttpContext httpContext)
		{
			return (LatencyTracker)httpContext.Items[Constants.LatencyTrackerContextKeyName];
		}

		internal static void GetLatency(Action operationToTrack, out long latency)
		{
			Stopwatch stopwatch = new Stopwatch();
			latency = 0L;
			try
			{
				stopwatch.Start();
				operationToTrack();
			}
			finally
			{
				stopwatch.Stop();
				latency = stopwatch.ElapsedMilliseconds;
			}
		}

		internal static T GetLatency<T>(Func<T> operationToTrack, out long latency)
		{
			Stopwatch stopwatch = new Stopwatch();
			latency = 0L;
			T result;
			try
			{
				stopwatch.Start();
				result = operationToTrack();
			}
			finally
			{
				stopwatch.Stop();
				latency = stopwatch.ElapsedMilliseconds;
			}
			return result;
		}

		internal void LogElapsedTime(RequestDetailsLogger logger, string latencyName)
		{
			if (HttpProxySettings.DetailedLatencyTracingEnabled.Value)
			{
				long currentLatency = this.GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(logger, latencyName, currentLatency);
			}
		}

		internal void LogElapsedTimeAsLatency(RequestDetailsLogger logger, LatencyTrackerKey trackerKey, HttpProxyMetadata protocolLogKey)
		{
			long currentLatency = this.GetCurrentLatency(trackerKey);
			if (currentLatency >= 0L)
			{
				logger.UpdateLatency(protocolLogKey, (double)currentLatency);
			}
		}

		internal void StartTracking(LatencyTrackerKey trackingKey, bool resetValue = false)
		{
			if (!this.latencyTrackerStartTimes.ContainsKey(trackingKey))
			{
				this.latencyTrackerStartTimes.Add(trackingKey, this.latencyTrackerStopwatch.ElapsedMilliseconds);
				return;
			}
			this.latencyTrackerStartTimes[trackingKey] = this.latencyTrackerStopwatch.ElapsedMilliseconds;
		}

		internal long GetCurrentLatency(LatencyTrackerKey trackingKey)
		{
			if (this.latencyTrackerStartTimes.ContainsKey(trackingKey))
			{
				return this.latencyTrackerStopwatch.ElapsedMilliseconds - this.latencyTrackerStartTimes[trackingKey];
			}
			return -1L;
		}

		internal void HandleGlsLatency(long latency)
		{
			this.glsLatencies.Add(latency);
		}

		internal void HandleGlsLatency(List<long> latencies)
		{
			this.glsLatencies.AddRange(latencies);
		}

		internal void HandleAccountLatency(long latency)
		{
			this.accountForestLatencies.Add(latency);
		}

		internal void HandleResourceLatency(long latency)
		{
			this.resourceForestLatencies.Add(latency);
		}

		internal void HandleResourceLatency(List<long> latencies)
		{
			this.resourceForestLatencies.AddRange(latencies);
		}

		internal void HandleSharedCacheLatency(long latency)
		{
			this.sharedCacheLatencies.Add(latency);
		}

		private static string GetBreakupOfLatencies(List<long> latencies)
		{
			if (latencies == null)
			{
				throw new ArgumentNullException("latencies");
			}
			StringBuilder result = new StringBuilder();
			latencies.ForEach(delegate(long latency)
			{
				result.Append(latency);
				result.Append(';');
			});
			return result.ToString();
		}

		internal const string SelectHandlerTime = "SelectHandler";

		private readonly List<long> glsLatencies;

		private readonly List<long> accountForestLatencies;

		private readonly List<long> resourceForestLatencies;

		private readonly List<long> sharedCacheLatencies;

		private Stopwatch latencyTrackerStopwatch = new Stopwatch();

		private Dictionary<LatencyTrackerKey, long> latencyTrackerStartTimes = new Dictionary<LatencyTrackerKey, long>();
	}
}
