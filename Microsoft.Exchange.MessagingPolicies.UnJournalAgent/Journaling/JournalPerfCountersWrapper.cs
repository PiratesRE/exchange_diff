using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class JournalPerfCountersWrapper : IDisposable
	{
		public JournalPerfCountersWrapper(IEnumerable<Tuple<ExPerformanceCounter, ExPerformanceCounter>> perfCounters)
		{
			this.perfCounterCollection = new Dictionary<ExPerformanceCounter, Tuple<ExPerformanceCounter, SlidingTotalCounter>>();
			foreach (Tuple<ExPerformanceCounter, ExPerformanceCounter> tuple in perfCounters)
			{
				this.perfCounterCollection.Add(tuple.Item1, new Tuple<ExPerformanceCounter, SlidingTotalCounter>(tuple.Item2, new SlidingTotalCounter(JournalPerfCountersWrapper.SlidingCounterInterval, JournalPerfCountersWrapper.SlidingCounterPrecision)));
			}
			this.refreshTimer = new GuardedTimer(new TimerCallback(this.RefreshOnTimer), null, JournalPerfCountersWrapper.RefreshInterval);
			this.ResetCounters();
		}

		public void Increment(ExPerformanceCounter perfCounter, long incrementValue = 1L)
		{
			perfCounter.IncrementBy(incrementValue);
			this.perfCounterCollection[perfCounter].Item2.AddValue(incrementValue);
			this.perfCounterCollection[perfCounter].Item1.RawValue = this.perfCounterCollection[perfCounter].Item2.Sum;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.refreshTimer.Change(-1, -1);
					this.refreshTimer.Dispose(true);
					this.ResetCounters();
				}
				this.disposed = true;
			}
		}

		private void ResetCounters()
		{
			foreach (KeyValuePair<ExPerformanceCounter, Tuple<ExPerformanceCounter, SlidingTotalCounter>> keyValuePair in this.perfCounterCollection)
			{
				keyValuePair.Value.Item1.RawValue = 0L;
			}
		}

		private void RefreshOnTimer(object state)
		{
			foreach (KeyValuePair<ExPerformanceCounter, Tuple<ExPerformanceCounter, SlidingTotalCounter>> keyValuePair in this.perfCounterCollection)
			{
				keyValuePair.Value.Item1.RawValue = keyValuePair.Value.Item2.Sum;
			}
		}

		private static readonly TimeSpan SlidingCounterInterval = TimeSpan.FromHours(1.0);

		private static readonly TimeSpan SlidingCounterPrecision = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(5.0);

		private readonly Dictionary<ExPerformanceCounter, Tuple<ExPerformanceCounter, SlidingTotalCounter>> perfCounterCollection;

		private GuardedTimer refreshTimer;

		private bool disposed;
	}
}
