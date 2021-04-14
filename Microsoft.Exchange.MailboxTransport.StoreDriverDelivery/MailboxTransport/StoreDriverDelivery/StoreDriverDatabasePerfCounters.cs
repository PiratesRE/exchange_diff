using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class StoreDriverDatabasePerfCounters
	{
		public static void IncrementDeliveryAttempt(string mdbGuid, bool calculateOnly = false)
		{
			StoreDriverDatabasePerfCounters.InstanceEntry instanceEntry = StoreDriverDatabasePerfCounters.GetInstanceEntry(mdbGuid);
			if (instanceEntry != null)
			{
				if (!calculateOnly)
				{
					instanceEntry.DeliveryAttemptsCounter.AddValue(1L);
				}
				lock (MSExchangeStoreDriverDatabase.TotalInstance.DeliveryAttempts)
				{
					instanceEntry.PerfCounterInstance.DeliveryAttempts.RawValue = instanceEntry.DeliveryAttemptsCounter.CalculateAverage();
				}
			}
		}

		public static void IncrementDeliveryFailure(string mdbGuid, bool calculateOnly = false)
		{
			StoreDriverDatabasePerfCounters.InstanceEntry instanceEntry = StoreDriverDatabasePerfCounters.GetInstanceEntry(mdbGuid);
			if (instanceEntry != null)
			{
				if (!calculateOnly)
				{
					instanceEntry.DeliveryFailuresCounter.AddValue(1L);
				}
				lock (MSExchangeStoreDriverDatabase.TotalInstance.DeliveryFailures)
				{
					instanceEntry.PerfCounterInstance.DeliveryFailures.RawValue = instanceEntry.DeliveryFailuresCounter.CalculateAverage();
				}
			}
		}

		public static void AddDeliveryThreadSample(string mdbGuid, long sample)
		{
			StoreDriverDatabasePerfCounters.InstanceEntry instanceEntry = StoreDriverDatabasePerfCounters.GetInstanceEntry(mdbGuid);
			if (instanceEntry != null)
			{
				instanceEntry.DeliveryThreadsCounter.AddValue(sample);
				lock (MSExchangeStoreDriverDatabase.TotalInstance.CurrentDeliveryThreadsPerMdb)
				{
					instanceEntry.PerfCounterInstance.CurrentDeliveryThreadsPerMdb.RawValue = instanceEntry.DeliveryThreadsCounter.CalculateAverage();
				}
			}
		}

		public static void RefreshPerformanceCounters()
		{
			foreach (string mdbGuid in StoreDriverDatabasePerfCounters.PerfCountersDictionary.Keys)
			{
				StoreDriverDatabasePerfCounters.IncrementDeliveryAttempt(mdbGuid, true);
				StoreDriverDatabasePerfCounters.IncrementDeliveryFailure(mdbGuid, true);
			}
		}

		private static StoreDriverDatabasePerfCounters.InstanceEntry GetInstanceEntry(string mdbGuid)
		{
			if (string.IsNullOrEmpty(mdbGuid))
			{
				return null;
			}
			return StoreDriverDatabasePerfCounters.PerfCountersDictionary.AddIfNotExists(mdbGuid, new SynchronizedDictionary<string, StoreDriverDatabasePerfCounters.InstanceEntry>.CreationalMethod(StoreDriverDatabasePerfCounters.CreateInstanceEntry));
		}

		private static StoreDriverDatabasePerfCounters.InstanceEntry CreateInstanceEntry(string mdbGuid)
		{
			MSExchangeStoreDriverDatabaseInstance msexchangeStoreDriverDatabaseInstance = null;
			try
			{
				if (mdbGuid != null)
				{
					msexchangeStoreDriverDatabaseInstance = MSExchangeStoreDriverDatabase.GetInstance(mdbGuid);
				}
			}
			catch (InvalidOperationException arg)
			{
				StoreDriverDatabasePerfCounters.Diag.TraceError<string, InvalidOperationException>(0L, "Get StoreDriver PerfCounters Instance {0} failed due to: {1}", mdbGuid, arg);
			}
			if (msexchangeStoreDriverDatabaseInstance == null)
			{
				return null;
			}
			return new StoreDriverDatabasePerfCounters.InstanceEntry(msexchangeStoreDriverDatabaseInstance);
		}

		private static readonly Trace Diag = ExTraceGlobals.MapiDeliverTracer;

		private static readonly SynchronizedDictionary<string, StoreDriverDatabasePerfCounters.InstanceEntry> PerfCountersDictionary = new SynchronizedDictionary<string, StoreDriverDatabasePerfCounters.InstanceEntry>(100, StringComparer.OrdinalIgnoreCase);

		private static readonly TimeSpan SlidingWindowLength = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan SlidingBucketLength = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan SlidingSequenceWindowLength = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan SlidingSequenceBucketLength = TimeSpan.FromSeconds(2.0);

		private class InstanceEntry
		{
			internal InstanceEntry(MSExchangeStoreDriverDatabaseInstance perfCounterInstance)
			{
				this.PerfCounterInstance = perfCounterInstance;
				this.DeliveryAttemptsCounter = new SlidingAverageCounter(StoreDriverDatabasePerfCounters.SlidingWindowLength, StoreDriverDatabasePerfCounters.SlidingBucketLength);
				this.DeliveryFailuresCounter = new SlidingAverageCounter(StoreDriverDatabasePerfCounters.SlidingWindowLength, StoreDriverDatabasePerfCounters.SlidingBucketLength);
				this.DeliveryThreadsCounter = new AverageSlidingSequence(StoreDriverDatabasePerfCounters.SlidingSequenceWindowLength, StoreDriverDatabasePerfCounters.SlidingSequenceBucketLength);
			}

			internal MSExchangeStoreDriverDatabaseInstance PerfCounterInstance { get; private set; }

			internal SlidingAverageCounter DeliveryAttemptsCounter { get; private set; }

			internal SlidingAverageCounter DeliveryFailuresCounter { get; private set; }

			internal AverageSlidingSequence DeliveryThreadsCounter { get; private set; }
		}
	}
}
