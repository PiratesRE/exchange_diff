using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Mserve.Perf;

namespace Microsoft.Exchange.Net.Mserve
{
	internal class MservePerfCounters
	{
		static MservePerfCounters()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				MservePerfCounters.processProcessName = currentProcess.ProcessName;
			}
		}

		internal void UpdateRequestPerfCountersForMserveWebService(int readCount, int addCount, int delCount)
		{
			this.counters.ReadRequestsInMserveWebService.IncrementBy((long)readCount);
			this.counters.AddRequestsInMserveWebService.IncrementBy((long)addCount);
			this.counters.DeleteRequestsInMserveWebService.IncrementBy((long)delCount);
			int num = readCount + addCount + delCount;
			this.counters.TotalRequestsInMserveWebService.IncrementBy((long)num);
			MservePerfCounters.percentageCounterFailuresInMserveWebService.AddDenominator((long)num);
			this.counters.PercentageFailuresInMserveWebService.RawValue = (long)MservePerfCounters.percentageCounterFailuresInMserveWebService.GetSlidingPercentage();
			MservePerfCounters.percentageCounterRequestsInMserveWebService.AddNumerator((long)num);
			MservePerfCounters.percentageCounterRequestsInMserveWebService.AddDenominator((long)num);
			MservePerfCounters.percentageCounterRequestsInCacheService.AddDenominator((long)num);
			this.counters.PercentageRequestsInMserveWebService.RawValue = (long)MservePerfCounters.percentageCounterRequestsInMserveWebService.GetSlidingPercentage();
			this.counters.PercentageRequestsInMserveCacheService.RawValue = (long)MservePerfCounters.percentageCounterRequestsInCacheService.GetSlidingPercentage();
		}

		internal void UpdateFailurePerfCountersForMserveWebService(int failureCount)
		{
			this.counters.TotalFailuresInMserveWebService.IncrementBy((long)failureCount);
			MservePerfCounters.percentageCounterFailuresInMserveWebService.AddNumerator((long)failureCount);
			this.counters.PercentageFailuresInMserveWebService.RawValue = (long)MservePerfCounters.percentageCounterFailuresInMserveWebService.GetSlidingPercentage();
		}

		internal void UpdateRequestPerfCountersForMserveCacheService(int readCount, int addCount, int delCount)
		{
			this.counters.ReadRequestsInMserveCacheService.IncrementBy((long)readCount);
			int num = readCount + addCount + delCount;
			this.counters.TotalRequestsInMserveCacheService.IncrementBy((long)num);
			MservePerfCounters.percentageCounterFailuresInCacheService.AddDenominator((long)num);
			this.counters.PercentageFailuresInMserveCacheService.RawValue = (long)MservePerfCounters.percentageCounterFailuresInCacheService.GetSlidingPercentage();
			MservePerfCounters.percentageCounterRequestsInCacheService.AddNumerator((long)num);
			MservePerfCounters.percentageCounterRequestsInCacheService.AddDenominator((long)num);
			MservePerfCounters.percentageCounterRequestsInMserveWebService.AddDenominator((long)num);
			this.counters.PercentageRequestsInMserveCacheService.RawValue = (long)MservePerfCounters.percentageCounterRequestsInCacheService.GetSlidingPercentage();
			this.counters.PercentageRequestsInMserveWebService.RawValue = (long)MservePerfCounters.percentageCounterRequestsInMserveWebService.GetSlidingPercentage();
		}

		internal void UpdateFailurePerfCountersForMservCacheService(int failureCount)
		{
			this.counters.TotalFailuresInMserveCacheService.IncrementBy((long)failureCount);
			MservePerfCounters.percentageCounterFailuresInCacheService.AddNumerator((long)failureCount);
			this.counters.PercentageFailuresInMserveCacheService.RawValue = (long)MservePerfCounters.percentageCounterFailuresInCacheService.GetSlidingPercentage();
		}

		internal void UpdateTotalFailuresPerfCounters(int failureCount)
		{
			this.counters.TotalFailuresInMserveService.IncrementBy((long)failureCount);
			MservePerfCounters.percentageCounterTotalFailuresInMserveService.AddNumerator((long)failureCount);
			this.counters.PercentageTotalFailuresInMserveService.RawValue = (long)MservePerfCounters.percentageCounterTotalFailuresInMserveService.GetSlidingPercentage();
		}

		internal void UpdateTotalRequestPerfCounters(int totalCount)
		{
			this.counters.TotalRequestsInMserveService.IncrementBy((long)totalCount);
			MservePerfCounters.percentageCounterTotalFailuresInMserveService.AddDenominator((long)totalCount);
			this.counters.PercentageTotalFailuresInMserveService.RawValue = (long)MservePerfCounters.percentageCounterTotalFailuresInMserveService.GetSlidingPercentage();
		}

		private static readonly string processProcessName;

		private readonly MserveWebServiceCountersInstance counters = MserveWebServiceCounters.GetInstance(MservePerfCounters.processProcessName);

		private static readonly TimeSpan SlidingCounterInterval = TimeSpan.FromMinutes(60.0);

		private static readonly TimeSpan SlidingCounterPrecision = TimeSpan.FromSeconds(1.0);

		private static SlidingPercentageCounter percentageCounterFailuresInCacheService = new SlidingPercentageCounter(MservePerfCounters.SlidingCounterInterval, MservePerfCounters.SlidingCounterPrecision);

		private static SlidingPercentageCounter percentageCounterRequestsInCacheService = new SlidingPercentageCounter(MservePerfCounters.SlidingCounterInterval, MservePerfCounters.SlidingCounterPrecision);

		private static SlidingPercentageCounter percentageCounterFailuresInMserveWebService = new SlidingPercentageCounter(MservePerfCounters.SlidingCounterInterval, MservePerfCounters.SlidingCounterPrecision);

		private static SlidingPercentageCounter percentageCounterRequestsInMserveWebService = new SlidingPercentageCounter(MservePerfCounters.SlidingCounterInterval, MservePerfCounters.SlidingCounterPrecision);

		private static SlidingPercentageCounter percentageCounterTotalFailuresInMserveService = new SlidingPercentageCounter(MservePerfCounters.SlidingCounterInterval, MservePerfCounters.SlidingCounterPrecision);
	}
}
