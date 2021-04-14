using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class StoreDriverDeliveryAgentPerfCounters
	{
		public static void IncrementAgentDeliveryAttempt(string agentName)
		{
			StoreDriverDeliveryAgentPerfCounters.InstanceEntry instanceEntry = StoreDriverDeliveryAgentPerfCounters.GetInstanceEntry(agentName);
			if (instanceEntry != null)
			{
				instanceEntry.DeliveryAgentFailuresCounter.AddDenominator(1L);
			}
		}

		public static void IncrementAgentDeliveryFailure(string agentName)
		{
			StoreDriverDeliveryAgentPerfCounters.InstanceEntry instanceEntry = StoreDriverDeliveryAgentPerfCounters.GetInstanceEntry(agentName);
			if (instanceEntry != null)
			{
				instanceEntry.DeliveryAgentFailuresCounter.AddNumerator(1L);
			}
		}

		public static void RefreshAgentDeliveryPercentCounter(string agentName)
		{
			StoreDriverDeliveryAgentPerfCounters.InstanceEntry instanceEntry = StoreDriverDeliveryAgentPerfCounters.GetInstanceEntry(agentName);
			if (instanceEntry != null)
			{
				lock (MSExchangeStoreDriverDeliveryAgent.TotalInstance.DeliveryAgentFailures)
				{
					instanceEntry.PerfCounterInstance.DeliveryAgentFailures.RawValue = (long)instanceEntry.DeliveryAgentFailuresCounter.GetSlidingPercentage();
				}
			}
		}

		private static StoreDriverDeliveryAgentPerfCounters.InstanceEntry GetInstanceEntry(string agentName)
		{
			if (string.IsNullOrEmpty(agentName))
			{
				return null;
			}
			return StoreDriverDeliveryAgentPerfCounters.PerfCountersDictionary.AddIfNotExists(agentName, new SynchronizedDictionary<string, StoreDriverDeliveryAgentPerfCounters.InstanceEntry>.CreationalMethod(StoreDriverDeliveryAgentPerfCounters.CreateInstanceEntry));
		}

		private static StoreDriverDeliveryAgentPerfCounters.InstanceEntry CreateInstanceEntry(string agentName)
		{
			MSExchangeStoreDriverDeliveryAgentInstance msexchangeStoreDriverDeliveryAgentInstance = null;
			try
			{
				if (agentName != null)
				{
					msexchangeStoreDriverDeliveryAgentInstance = MSExchangeStoreDriverDeliveryAgent.GetInstance(agentName);
				}
			}
			catch (InvalidOperationException arg)
			{
				TraceHelper.StoreDriverDeliveryTracer.TraceFail<string, InvalidOperationException>(TraceHelper.MessageProbeActivityId, 0L, "Get StoreDriverDelivery agent PerfCounters Instance {0} failed due to: {1}", agentName, arg);
			}
			if (msexchangeStoreDriverDeliveryAgentInstance == null)
			{
				return null;
			}
			return new StoreDriverDeliveryAgentPerfCounters.InstanceEntry(msexchangeStoreDriverDeliveryAgentInstance);
		}

		private static readonly Trace diag = ExTraceGlobals.StoreDriverDeliveryTracer;

		private static readonly SynchronizedDictionary<string, StoreDriverDeliveryAgentPerfCounters.InstanceEntry> PerfCountersDictionary = new SynchronizedDictionary<string, StoreDriverDeliveryAgentPerfCounters.InstanceEntry>(100, StringComparer.OrdinalIgnoreCase);

		private static readonly TimeSpan SlidingWindowLength = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan SlidingBucketLength = TimeSpan.FromMinutes(1.0);

		private class InstanceEntry
		{
			internal InstanceEntry(MSExchangeStoreDriverDeliveryAgentInstance perfCounterInstance)
			{
				this.PerfCounterInstance = perfCounterInstance;
				this.DeliveryAgentFailuresCounter = new SlidingPercentageCounter(StoreDriverDeliveryAgentPerfCounters.SlidingWindowLength, StoreDriverDeliveryAgentPerfCounters.SlidingBucketLength, true);
			}

			internal MSExchangeStoreDriverDeliveryAgentInstance PerfCounterInstance { get; private set; }

			internal SlidingPercentageCounter DeliveryAgentFailuresCounter { get; private set; }
		}
	}
}
