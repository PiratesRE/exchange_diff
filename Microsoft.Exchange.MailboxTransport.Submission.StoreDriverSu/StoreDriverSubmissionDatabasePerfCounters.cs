using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.MailboxTransport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class StoreDriverSubmissionDatabasePerfCounters
	{
		public static void IncrementSubmissionAttempt(string mdbName, bool calculateOnly = false)
		{
			StoreDriverSubmissionDatabasePerfCounters.InstanceEntry instanceEntry = StoreDriverSubmissionDatabasePerfCounters.GetInstanceEntry(mdbName);
			if (instanceEntry != null)
			{
				if (!calculateOnly)
				{
					instanceEntry.SubmissionAttemptsCounter.AddValue(1L);
				}
				lock (MSExchangeStoreDriverSubmissionDatabase.TotalInstance.SubmissionAttempts)
				{
					instanceEntry.PerfCounterInstance.SubmissionAttempts.RawValue = instanceEntry.SubmissionAttemptsCounter.CalculateAverage();
				}
			}
		}

		public static void IncrementSubmissionFailure(string mdbName, bool calculateOnly = false)
		{
			StoreDriverSubmissionDatabasePerfCounters.InstanceEntry instanceEntry = StoreDriverSubmissionDatabasePerfCounters.GetInstanceEntry(mdbName);
			if (instanceEntry != null)
			{
				if (!calculateOnly)
				{
					instanceEntry.SubmissionFailuresCounter.AddValue(1L);
				}
				lock (MSExchangeStoreDriverSubmissionDatabase.TotalInstance.SubmissionFailures)
				{
					instanceEntry.PerfCounterInstance.SubmissionFailures.RawValue = instanceEntry.SubmissionFailuresCounter.CalculateAverage();
				}
			}
		}

		public static void IncrementSkippedSubmission(string mdbName, bool calculateOnly = false)
		{
			StoreDriverSubmissionDatabasePerfCounters.InstanceEntry instanceEntry = StoreDriverSubmissionDatabasePerfCounters.GetInstanceEntry(mdbName);
			if (instanceEntry != null)
			{
				if (!calculateOnly)
				{
					instanceEntry.SkippedSubmissionsCounter.AddValue(1L);
				}
				lock (MSExchangeStoreDriverSubmissionDatabase.TotalInstance.SkippedSubmissions)
				{
					instanceEntry.PerfCounterInstance.SkippedSubmissions.RawValue = instanceEntry.SkippedSubmissionsCounter.CalculateAverage();
				}
			}
		}

		public static void RefreshPerformanceCounters()
		{
			foreach (string mdbName in StoreDriverSubmissionDatabasePerfCounters.PerfCountersDictionary.Keys)
			{
				StoreDriverSubmissionDatabasePerfCounters.IncrementSubmissionAttempt(mdbName, true);
				StoreDriverSubmissionDatabasePerfCounters.IncrementSubmissionFailure(mdbName, true);
				StoreDriverSubmissionDatabasePerfCounters.IncrementSkippedSubmission(mdbName, true);
			}
		}

		private static StoreDriverSubmissionDatabasePerfCounters.InstanceEntry GetInstanceEntry(string mdbName)
		{
			if (string.IsNullOrEmpty(mdbName))
			{
				return null;
			}
			return StoreDriverSubmissionDatabasePerfCounters.PerfCountersDictionary.AddIfNotExists(mdbName, new SynchronizedDictionary<string, StoreDriverSubmissionDatabasePerfCounters.InstanceEntry>.CreationalMethod(StoreDriverSubmissionDatabasePerfCounters.CreateInstanceEntry));
		}

		private static StoreDriverSubmissionDatabasePerfCounters.InstanceEntry CreateInstanceEntry(string mdbName)
		{
			MSExchangeStoreDriverSubmissionDatabaseInstance msexchangeStoreDriverSubmissionDatabaseInstance = null;
			try
			{
				if (mdbName != null)
				{
					msexchangeStoreDriverSubmissionDatabaseInstance = MSExchangeStoreDriverSubmissionDatabase.GetInstance(mdbName);
				}
			}
			catch (InvalidOperationException arg)
			{
				TraceHelper.StoreDriverSubmissionTracer.TraceFail<string, InvalidOperationException>(TraceHelper.MessageProbeActivityId, 0L, "Get StoreDriverSubmission PerfCounters Instance {0} failed due to: {1}", mdbName, arg);
			}
			if (msexchangeStoreDriverSubmissionDatabaseInstance == null)
			{
				return null;
			}
			return new StoreDriverSubmissionDatabasePerfCounters.InstanceEntry(msexchangeStoreDriverSubmissionDatabaseInstance);
		}

		private static readonly Trace diag = ExTraceGlobals.MapiStoreDriverSubmissionTracer;

		private static readonly SynchronizedDictionary<string, StoreDriverSubmissionDatabasePerfCounters.InstanceEntry> PerfCountersDictionary = new SynchronizedDictionary<string, StoreDriverSubmissionDatabasePerfCounters.InstanceEntry>(100, StringComparer.OrdinalIgnoreCase);

		private static readonly TimeSpan SlidingWindowLength = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan SlidingBucketLength = TimeSpan.FromMinutes(1.0);

		private class InstanceEntry
		{
			internal InstanceEntry(MSExchangeStoreDriverSubmissionDatabaseInstance perfCounterInstance)
			{
				this.PerfCounterInstance = perfCounterInstance;
				this.SubmissionAttemptsCounter = new SlidingAverageCounter(StoreDriverSubmissionDatabasePerfCounters.SlidingWindowLength, StoreDriverSubmissionDatabasePerfCounters.SlidingBucketLength);
				this.SubmissionFailuresCounter = new SlidingAverageCounter(StoreDriverSubmissionDatabasePerfCounters.SlidingWindowLength, StoreDriverSubmissionDatabasePerfCounters.SlidingBucketLength);
				this.SkippedSubmissionsCounter = new SlidingAverageCounter(StoreDriverSubmissionDatabasePerfCounters.SlidingWindowLength, StoreDriverSubmissionDatabasePerfCounters.SlidingBucketLength);
			}

			internal MSExchangeStoreDriverSubmissionDatabaseInstance PerfCounterInstance { get; private set; }

			internal SlidingAverageCounter SubmissionAttemptsCounter { get; private set; }

			internal SlidingAverageCounter SubmissionFailuresCounter { get; private set; }

			internal SlidingAverageCounter SkippedSubmissionsCounter { get; private set; }
		}
	}
}
