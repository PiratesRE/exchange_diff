using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ThrottlingService;

namespace Microsoft.Exchange.Data.ThrottlingService.Client
{
	internal sealed class ThrottlingClientPerformanceCountersImpl : IThrottlingClientPerformanceCounters
	{
		public ThrottlingClientPerformanceCountersImpl(string instanceName)
		{
			if (string.IsNullOrEmpty(instanceName))
			{
				throw new ArgumentException("Throttling Service Client PerfCounters InstanceName name cannot be null or empty", "throttlingServiceClientPerfCounterInstanceName");
			}
			this.percentageCounterRequestsSucceeded = new SlidingPercentageCounter(ThrottlingClientPerformanceCountersImpl.SlidingCounterInterval, ThrottlingClientPerformanceCountersImpl.SlidingCounterPrecision);
			this.percentageCounterRequestsDenied = new SlidingPercentageCounter(ThrottlingClientPerformanceCountersImpl.SlidingCounterInterval, ThrottlingClientPerformanceCountersImpl.SlidingCounterPrecision);
			this.percentageCounterRequestsBypassed = new SlidingPercentageCounter(ThrottlingClientPerformanceCountersImpl.SlidingCounterInterval, ThrottlingClientPerformanceCountersImpl.SlidingCounterPrecision);
			this.averageCountersForRequestInterval = new SlidingPercentageCounter(ThrottlingClientPerformanceCountersImpl.SlidingCounterInterval, ThrottlingClientPerformanceCountersImpl.SlidingCounterPrecision);
			try
			{
				this.perfCountersInstance = ThrottlingServiceClientPerformanceCounters.GetInstance(instanceName);
			}
			catch (InvalidOperationException arg)
			{
				ThrottlingClientPerformanceCountersImpl.tracer.TraceError<string, InvalidOperationException>(0L, "Failed to initialize performance counters instance '{0}': {1}", instanceName, arg);
				this.perfCountersInstance = null;
			}
		}

		public void AddRequestStatus(ThrottlingRpcResult result)
		{
			this.AddRequestStatusToCounters(result, -1L);
		}

		public void AddRequestStatus(ThrottlingRpcResult result, long requestTimeMsec)
		{
			if (requestTimeMsec < 0L)
			{
				throw new ArgumentException("Request time must be greater than or equal to zero.", "requestTimeMsec");
			}
			this.AddRequestStatusToCounters(result, requestTimeMsec);
		}

		private void AddRequestStatusToCounters(ThrottlingRpcResult result, long requestTimeMsec)
		{
			if (this.perfCountersInstance != null)
			{
				switch (result)
				{
				case ThrottlingRpcResult.Allowed:
					this.percentageCounterRequestsSucceeded.AddNumerator(1L);
					break;
				case ThrottlingRpcResult.Bypassed:
				case ThrottlingRpcResult.Failed:
					this.percentageCounterRequestsBypassed.AddNumerator(1L);
					break;
				case ThrottlingRpcResult.Denied:
					this.percentageCounterRequestsDenied.AddNumerator(1L);
					break;
				}
				this.percentageCounterRequestsSucceeded.AddDenominator(1L);
				this.percentageCounterRequestsDenied.AddDenominator(1L);
				this.percentageCounterRequestsBypassed.AddDenominator(1L);
				this.perfCountersInstance.SuccessfulSubmissionRequests.RawValue = (long)this.percentageCounterRequestsSucceeded.GetSlidingPercentage();
				this.perfCountersInstance.BypassedSubmissionRequests.RawValue = (long)this.percentageCounterRequestsBypassed.GetSlidingPercentage();
				this.perfCountersInstance.DeniedSubmissionRequest.RawValue = (long)this.percentageCounterRequestsDenied.GetSlidingPercentage();
				if (requestTimeMsec >= 0L)
				{
					this.averageCountersForRequestInterval.AddNumerator(requestTimeMsec);
					this.averageCountersForRequestInterval.AddDenominator(1L);
					this.perfCountersInstance.AverageSubmissionRequestTime.RawValue = (long)this.averageCountersForRequestInterval.GetSlidingPercentage() / 100L;
				}
			}
		}

		private static readonly TimeSpan SlidingCounterInterval = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan SlidingCounterPrecision = TimeSpan.FromSeconds(1.0);

		private static Trace tracer = ExTraceGlobals.ThrottlingClientTracer;

		private SlidingPercentageCounter percentageCounterRequestsSucceeded;

		private SlidingPercentageCounter percentageCounterRequestsDenied;

		private SlidingPercentageCounter percentageCounterRequestsBypassed;

		private SlidingPercentageCounter averageCountersForRequestInterval;

		private ThrottlingServiceClientPerformanceCountersInstance perfCountersInstance;
	}
}
