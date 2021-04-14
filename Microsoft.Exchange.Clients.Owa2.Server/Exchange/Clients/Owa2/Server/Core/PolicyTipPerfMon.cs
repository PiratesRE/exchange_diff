using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class PolicyTipPerfMon
	{
		static PolicyTipPerfMon()
		{
			PolicyTipPerfMon.InitializePerfMon();
		}

		private static void InitializePerfMon()
		{
			PolicyTipPerfMon.percentServerFailures = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0), true);
			PolicyTipPerfMon.percentHighLatency = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0), true);
			PolicyTipPerfMon.averageLatency = new SlidingAverageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0));
			if (PolicyTipPerfMon.performanceCounterMaintenanceTimer == null)
			{
				PolicyTipPerfMon.performanceCounterMaintenanceTimer = new GuardedTimer(new TimerCallback(PolicyTipPerfMon.RefreshPerformanceCounters), null, TimeSpan.FromSeconds(60.0));
			}
		}

		internal static void IncrementTotalRequests()
		{
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsTotalRequests.Increment();
			PolicyTipPerfMon.percentServerFailures.AddDenominator(1L);
			PolicyTipPerfMon.percentHighLatency.AddDenominator(1L);
		}

		internal static void IncrementAllServerFailures()
		{
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsAllServerFailedRequests.Increment();
			PolicyTipPerfMon.percentServerFailures.AddNumerator(1L);
		}

		internal static void IncrementPercentHighLatency()
		{
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsHighLatencyRequests.Increment();
			PolicyTipPerfMon.percentHighLatency.AddNumerator(1L);
		}

		public static void RecordPerRequestLatency(TimeSpan timeSpan)
		{
			PolicyTipPerfMon.averageLatency.AddValue((long)timeSpan.TotalMilliseconds);
		}

		internal static void RefreshPerformanceCounters(object state)
		{
			PolicyTipPerfMon.ComputePercentServerFailures();
			PolicyTipPerfMon.ComputePercentHighLatency();
			PolicyTipPerfMon.ComputeAverageLatency();
		}

		public static void ComputeAverageLatency()
		{
			long num;
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsAverageRequestLatency.RawValue = PolicyTipPerfMon.averageLatency.CalculateAverageAcrossAllSamples(out num);
		}

		private static void ComputePercentServerFailures()
		{
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsPercentServerFailures.RawValue = (long)((int)PolicyTipPerfMon.percentServerFailures.GetSlidingPercentage());
		}

		private static void ComputePercentHighLatency()
		{
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsPercentHighLatency.RawValue = (long)((int)PolicyTipPerfMon.percentHighLatency.GetSlidingPercentage());
		}

		private static GuardedTimer performanceCounterMaintenanceTimer;

		private static SlidingPercentageCounter percentServerFailures;

		private static SlidingPercentageCounter percentHighLatency;

		private static SlidingAverageCounter averageLatency;
	}
}
