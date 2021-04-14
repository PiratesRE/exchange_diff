using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class LatencyPerformanceCounter : ILatencyPerformanceCounter
	{
		public LatencyPerformanceCounterType CounterType
		{
			get
			{
				return this.counterType;
			}
		}

		private LatencyPerformanceCounter(string instanceName, TimeSpan expiryInterval, long infinitySeconds, bool isLongRange, LatencyPerformanceCounterType type)
		{
			this.counterType = type;
			this.isLongRange = isLongRange;
			this.expiryInterval = expiryInterval;
			this.infinitySeconds = infinitySeconds;
			if (type == LatencyPerformanceCounterType.Local)
			{
				this.perfCounters = LatencyTrackerPerfCounters.GetInstance(instanceName);
			}
			else
			{
				this.endToEndPerfCounters = LatencyTrackerEndToEndPerfCounters.GetInstance(instanceName);
			}
			if (isLongRange)
			{
				this.percentileCounter = new MultiGranularityPercentileCounter(expiryInterval, TimeSpan.FromSeconds(5.0), LatencyPerformanceCounter.parameters);
				return;
			}
			this.percentileCounter = new PercentileCounter(expiryInterval, TimeSpan.FromSeconds(5.0), 1L, infinitySeconds);
		}

		public static ILatencyPerformanceCounter CreateInstance(string instanceName, TimeSpan expiryInterval, long infinitySeconds)
		{
			return LatencyPerformanceCounter.CreateInstance(instanceName, expiryInterval, infinitySeconds, false);
		}

		public static ILatencyPerformanceCounter CreateInstance(string instanceName, TimeSpan expiryInterval, long infinitySeconds, bool isLongRange)
		{
			return LatencyPerformanceCounter.CreateInstance(instanceName, expiryInterval, infinitySeconds, isLongRange, LatencyPerformanceCounterType.Local);
		}

		public static ILatencyPerformanceCounter CreateInstance(string instanceName, TimeSpan expiryInterval, long infinitySeconds, bool isLongRange, LatencyPerformanceCounterType type)
		{
			ILatencyPerformanceCounter result;
			try
			{
				result = new LatencyPerformanceCounter(instanceName, expiryInterval, infinitySeconds, isLongRange, type);
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.GeneralTracer.TraceError<string, InvalidOperationException>(0L, "Failed to initialize performance counters for component '{0}': {1}", instanceName, ex);
				ExEventLog exEventLog = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());
				exEventLog.LogEvent(TransportEventLogConstants.Tuple_PerfCountersLoadFailure, null, new object[]
				{
					LatencyPerformanceCounter.EventTag,
					instanceName,
					ex.ToString()
				});
				result = null;
			}
			return result;
		}

		public static void SetCategoryNames(string localCategoryName, string endToEndCategoryName)
		{
			LatencyTrackerPerfCounters.SetCategoryName(localCategoryName);
			LatencyTrackerEndToEndPerfCounters.SetCategoryName(endToEndCategoryName);
		}

		public void AddValue(long latencySeconds)
		{
			if (latencySeconds >= 0L)
			{
				this.percentileCounter.AddValue(latencySeconds);
				this.Update();
			}
		}

		public void AddValue(long latencySeconds, DeliveryPriority priority)
		{
			this.AddValue(latencySeconds);
		}

		public void Update()
		{
			long rawValue;
			if (this.perfCounters != null)
			{
				this.perfCounters.Percentile99.RawValue = this.percentileCounter.PercentileQuery(99.0, out rawValue);
				this.perfCounters.Percentile99Samples.RawValue = rawValue;
				this.perfCounters.Percentile95.RawValue = this.percentileCounter.PercentileQuery(95.0, out rawValue);
				this.perfCounters.Percentile95Samples.RawValue = rawValue;
				this.perfCounters.Percentile90.RawValue = this.percentileCounter.PercentileQuery(90.0, out rawValue);
				this.perfCounters.Percentile90Samples.RawValue = rawValue;
				this.perfCounters.Percentile80.RawValue = this.percentileCounter.PercentileQuery(80.0, out rawValue);
				this.perfCounters.Percentile80Samples.RawValue = rawValue;
				this.perfCounters.Percentile50.RawValue = this.percentileCounter.PercentileQuery(50.0, out rawValue);
				this.perfCounters.Percentile50Samples.RawValue = rawValue;
				return;
			}
			this.endToEndPerfCounters.Percentile99.RawValue = this.percentileCounter.PercentileQuery(99.0, out rawValue);
			this.endToEndPerfCounters.Percentile99Samples.RawValue = rawValue;
			this.endToEndPerfCounters.Percentile95.RawValue = this.percentileCounter.PercentileQuery(95.0, out rawValue);
			this.endToEndPerfCounters.Percentile95Samples.RawValue = rawValue;
			this.endToEndPerfCounters.Percentile90.RawValue = this.percentileCounter.PercentileQuery(90.0, out rawValue);
			this.endToEndPerfCounters.Percentile90Samples.RawValue = rawValue;
			this.endToEndPerfCounters.Percentile80.RawValue = this.percentileCounter.PercentileQuery(80.0, out rawValue);
			this.endToEndPerfCounters.Percentile80Samples.RawValue = rawValue;
			this.endToEndPerfCounters.Percentile50.RawValue = this.percentileCounter.PercentileQuery(50.0, out rawValue);
			this.endToEndPerfCounters.Percentile50Samples.RawValue = rawValue;
		}

		public void Reset()
		{
			if (this.isLongRange)
			{
				this.percentileCounter = new MultiGranularityPercentileCounter(this.expiryInterval, TimeSpan.FromSeconds(5.0), LatencyPerformanceCounter.parameters);
			}
			else
			{
				this.percentileCounter = new PercentileCounter(this.expiryInterval, TimeSpan.FromSeconds(5.0), 1L, this.infinitySeconds);
			}
			this.Update();
		}

		private static readonly string EventTag = "Latency Tracker";

		private static readonly MultiGranularityPercentileCounter.Param[] parameters = new MultiGranularityPercentileCounter.Param[]
		{
			new MultiGranularityPercentileCounter.Param(1L, 90L),
			new MultiGranularityPercentileCounter.Param(5L, 300L),
			new MultiGranularityPercentileCounter.Param(30L, 1800L),
			new MultiGranularityPercentileCounter.Param(300L, 18000L),
			new MultiGranularityPercentileCounter.Param(1800L, 43200L)
		};

		private LatencyTrackerPerfCountersInstance perfCounters;

		private LatencyTrackerEndToEndPerfCountersInstance endToEndPerfCounters;

		private IPercentileCounter percentileCounter;

		private LatencyPerformanceCounterType counterType;

		private readonly bool isLongRange;

		private readonly TimeSpan expiryInterval;

		private readonly long infinitySeconds;
	}
}
