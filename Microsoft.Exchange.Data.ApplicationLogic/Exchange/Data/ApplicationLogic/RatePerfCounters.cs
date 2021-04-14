using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal static class RatePerfCounters
	{
		public static void UpdatePerfCounterValues(object status)
		{
			lock (RatePerfCounters.lockObject)
			{
				TimeSpan threshold = new TimeSpan(3000L);
				if (ExDateTime.Compare(RatePerfCounters.lockObject.LastUpdateTime, ExDateTime.Now, threshold) != 0)
				{
					if (RatePerfCounters.exceptionPerfSamples != null)
					{
						for (int i = 0; i < RatePerfCounters.exceptionPerfSamples.Length; i++)
						{
							lock (RatePerfCounters.exceptionPerfSamples[i])
							{
								ExDateTime now = ExDateTime.Now;
								int j = RatePerfCounters.exceptionPerfSamples[i].Count;
								foreach (ExDateTime dt in RatePerfCounters.exceptionPerfSamples[i])
								{
									if (ExDateTime.Compare(dt, now, RatePerfCounters.ExceptionComputationInterval) == 0)
									{
										break;
									}
									j--;
								}
								RatePerfCounters.exceptionPerfCounters[i].RawValue = (long)j;
								while (j < RatePerfCounters.exceptionPerfSamples[i].Count)
								{
									RatePerfCounters.exceptionPerfSamples[i].Dequeue();
								}
							}
						}
					}
					if (RatePerfCounters.latencyPerfSamples != null)
					{
						for (int k = 0; k < RatePerfCounters.latencyPerfSamples.Length; k++)
						{
							lock (RatePerfCounters.latencyPerfSamples[k])
							{
								int l = 0;
								long num = 0L;
								ExDateTime now2 = ExDateTime.Now;
								foreach (RatePerfCounters.LatencySample latencySample in RatePerfCounters.latencyPerfSamples[k])
								{
									if (ExDateTime.Compare(latencySample.Timestamp, now2, RatePerfCounters.LatencyComputationInterval) == 0)
									{
										num += latencySample.Latency;
										l++;
									}
								}
								if (l > 0)
								{
									RatePerfCounters.latencyPerfCounters[k].RawValue = num / (long)l;
								}
								while (l < RatePerfCounters.latencyPerfSamples[k].Count)
								{
									RatePerfCounters.latencyPerfSamples[k].Dequeue();
								}
							}
						}
					}
					RatePerfCounters.lockObject.LastUpdateTime = ExDateTime.Now;
				}
			}
		}

		internal static bool Initialize(ExPerformanceCounter[] exceptionPerfCounters, ExPerformanceCounter[] latencyPerfCounters)
		{
			if (!RatePerfCounters.initialized)
			{
				if (exceptionPerfCounters != null)
				{
					RatePerfCounters.exceptionPerfCounters = exceptionPerfCounters;
					RatePerfCounters.exceptionPerfSamples = new Queue<ExDateTime>[exceptionPerfCounters.Length];
					for (int i = 0; i < exceptionPerfCounters.Length; i++)
					{
						RatePerfCounters.exceptionPerfSamples[i] = new Queue<ExDateTime>(100);
					}
				}
				if (latencyPerfCounters != null)
				{
					RatePerfCounters.latencyPerfCounters = latencyPerfCounters;
					RatePerfCounters.latencyPerfSamples = new Queue<RatePerfCounters.LatencySample>[latencyPerfCounters.Length];
					for (int j = 0; j < RatePerfCounters.latencyPerfSamples.Length; j++)
					{
						RatePerfCounters.latencyPerfSamples[j] = new Queue<RatePerfCounters.LatencySample>(6000);
					}
				}
				try
				{
					RatePerfCounters.timer = new Timer(new TimerCallback(RatePerfCounters.UpdatePerfCounterValues), null, 10000, 3000);
				}
				catch (Exception ex)
				{
					if (ex is ArgumentOutOfRangeException || ex is NotSupportedException)
					{
						return false;
					}
					throw;
				}
				RatePerfCounters.initialized = (RatePerfCounters.timer != null);
			}
			return RatePerfCounters.initialized;
		}

		internal static void IncrementExceptionPerfCounter(int perfCounterIndex)
		{
			if (perfCounterIndex < 0 || perfCounterIndex >= RatePerfCounters.exceptionPerfSamples.Length)
			{
				return;
			}
			lock (RatePerfCounters.exceptionPerfSamples[perfCounterIndex])
			{
				ExDateTime now = ExDateTime.Now;
				if (RatePerfCounters.exceptionPerfSamples[perfCounterIndex].Count == 100)
				{
					RatePerfCounters.exceptionPerfSamples[perfCounterIndex].Dequeue();
				}
				RatePerfCounters.exceptionPerfSamples[perfCounterIndex].Enqueue(now);
			}
		}

		internal static void IncrementLatencyPerfCounter(int latencyCounterIndex, long latency)
		{
			if (latencyCounterIndex < 0 || latencyCounterIndex >= RatePerfCounters.latencyPerfSamples.Length)
			{
				return;
			}
			lock (RatePerfCounters.latencyPerfSamples[latencyCounterIndex])
			{
				RatePerfCounters.LatencySample item = new RatePerfCounters.LatencySample(latency);
				if (RatePerfCounters.latencyPerfSamples[latencyCounterIndex].Count == 6000)
				{
					RatePerfCounters.latencyPerfSamples[latencyCounterIndex].Dequeue();
				}
				RatePerfCounters.latencyPerfSamples[latencyCounterIndex].Enqueue(item);
			}
		}

		private const int TimerUpdateInterval = 3000;

		private const int TimerStartDelay = 10000;

		private const int MaxExceptionQueueSize = 100;

		private const int MaxLatencyQueueSize = 6000;

		private static readonly TimeSpan ExceptionComputationInterval = new TimeSpan(0, 1, 0);

		private static readonly TimeSpan LatencyComputationInterval = new TimeSpan(0, 5, 0);

		private static ExPerformanceCounter[] exceptionPerfCounters;

		private static Queue<ExDateTime>[] exceptionPerfSamples;

		private static ExPerformanceCounter[] latencyPerfCounters;

		private static Queue<RatePerfCounters.LatencySample>[] latencyPerfSamples;

		private static Timer timer;

		private static bool initialized = false;

		private static RatePerfCounters.LockObject lockObject = new RatePerfCounters.LockObject();

		private struct LatencySample
		{
			public LatencySample(long latency)
			{
				this.Latency = latency;
				this.Timestamp = ExDateTime.Now;
			}

			public ExDateTime Timestamp;

			public long Latency;
		}

		private class LockObject
		{
			public ExDateTime LastUpdateTime { get; set; }
		}
	}
}
