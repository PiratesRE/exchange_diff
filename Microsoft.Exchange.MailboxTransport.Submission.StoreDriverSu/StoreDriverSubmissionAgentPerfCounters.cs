using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.MailboxTransport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class StoreDriverSubmissionAgentPerfCounters
	{
		public static void IncrementAgentSubmissionAttempt(string agentName)
		{
			StoreDriverSubmissionAgentPerfCounters.InstanceEntry instanceEntry = StoreDriverSubmissionAgentPerfCounters.GetInstanceEntry(agentName);
			if (instanceEntry != null)
			{
				instanceEntry.SubmissionAgentFailuresCounter.AddDenominator(1L);
			}
		}

		public static void IncrementAgentSubmissionFailure(string agentName)
		{
			StoreDriverSubmissionAgentPerfCounters.InstanceEntry instanceEntry = StoreDriverSubmissionAgentPerfCounters.GetInstanceEntry(agentName);
			if (instanceEntry != null)
			{
				instanceEntry.SubmissionAgentFailuresCounter.AddNumerator(1L);
			}
		}

		public static void RefreshAgentSubmissionPercentCounter(string agentName)
		{
			StoreDriverSubmissionAgentPerfCounters.InstanceEntry instanceEntry = StoreDriverSubmissionAgentPerfCounters.GetInstanceEntry(agentName);
			if (instanceEntry != null)
			{
				lock (MSExchangeStoreDriverSubmissionAgent.TotalInstance.SubmissionAgentFailures)
				{
					instanceEntry.PerfCounterInstance.SubmissionAgentFailures.RawValue = (long)instanceEntry.SubmissionAgentFailuresCounter.GetSlidingPercentage();
				}
			}
		}

		private static StoreDriverSubmissionAgentPerfCounters.InstanceEntry GetInstanceEntry(string agentName)
		{
			if (string.IsNullOrEmpty(agentName))
			{
				return null;
			}
			return StoreDriverSubmissionAgentPerfCounters.PerfCountersDictionary.AddIfNotExists(agentName, new SynchronizedDictionary<string, StoreDriverSubmissionAgentPerfCounters.InstanceEntry>.CreationalMethod(StoreDriverSubmissionAgentPerfCounters.CreateInstanceEntry));
		}

		private static StoreDriverSubmissionAgentPerfCounters.InstanceEntry CreateInstanceEntry(string agentName)
		{
			MSExchangeStoreDriverSubmissionAgentInstance msexchangeStoreDriverSubmissionAgentInstance = null;
			try
			{
				if (agentName != null)
				{
					msexchangeStoreDriverSubmissionAgentInstance = MSExchangeStoreDriverSubmissionAgent.GetInstance(agentName);
				}
			}
			catch (InvalidOperationException arg)
			{
				TraceHelper.StoreDriverSubmissionTracer.TraceFail<string, InvalidOperationException>(TraceHelper.MessageProbeActivityId, 0L, "Get StoreDriverSubmission agent PerfCounters Instance {0} failed due to: {1}", agentName, arg);
			}
			if (msexchangeStoreDriverSubmissionAgentInstance == null)
			{
				return null;
			}
			return new StoreDriverSubmissionAgentPerfCounters.InstanceEntry(msexchangeStoreDriverSubmissionAgentInstance);
		}

		private static readonly Trace diag = ExTraceGlobals.MapiStoreDriverSubmissionTracer;

		private static readonly SynchronizedDictionary<string, StoreDriverSubmissionAgentPerfCounters.InstanceEntry> PerfCountersDictionary = new SynchronizedDictionary<string, StoreDriverSubmissionAgentPerfCounters.InstanceEntry>(100, StringComparer.OrdinalIgnoreCase);

		private static readonly TimeSpan SlidingWindowLength = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan SlidingBucketLength = TimeSpan.FromMinutes(1.0);

		private class InstanceEntry
		{
			internal InstanceEntry(MSExchangeStoreDriverSubmissionAgentInstance perfCounterInstance)
			{
				this.PerfCounterInstance = perfCounterInstance;
				this.SubmissionAgentFailuresCounter = new SlidingPercentageCounter(StoreDriverSubmissionAgentPerfCounters.SlidingWindowLength, StoreDriverSubmissionAgentPerfCounters.SlidingBucketLength, true);
			}

			internal MSExchangeStoreDriverSubmissionAgentInstance PerfCounterInstance { get; private set; }

			internal SlidingPercentageCounter SubmissionAgentFailuresCounter { get; private set; }
		}
	}
}
