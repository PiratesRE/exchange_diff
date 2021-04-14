using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class BufferedPerformanceCounter : ExPerformanceCounter
	{
		public BufferedPerformanceCounter(string categoryName, string counterName, string instanceName, ExPerformanceCounter totalInstanceCounter, params ExPerformanceCounter[] autoUpdateCounters) : base(categoryName, counterName, instanceName, totalInstanceCounter, autoUpdateCounters)
		{
			this.accumulatedValue = 0L;
			this.currentValue = base.RawValue;
			BufferedPerformanceCounter.counters.TryAdd(this, null);
		}

		public override long RawValue
		{
			get
			{
				return this.currentValue + this.accumulatedValue;
			}
			set
			{
				this.rawValueWasSet = true;
				this.currentValue = value;
				this.accumulatedValue = 0L;
			}
		}

		public override void Close()
		{
			using (LockManager.Lock(BufferedPerformanceCounter.counters))
			{
				object obj;
				BufferedPerformanceCounter.counters.TryRemove(this, out obj);
			}
			base.Close();
		}

		public long BaseRawValueForTest
		{
			get
			{
				return base.RawValue;
			}
		}

		internal static void Initialize()
		{
			BufferedPerformanceCounter.flushTask = new RecurringTask<object>(delegate(TaskExecutionDiagnosticsProxy diagnosticsContext, object context, Func<bool> shouldCallbackContinue)
			{
				BufferedPerformanceCounter.FlushCounters();
			}, null, TimeSpan.FromSeconds(1.0), false);
			BufferedPerformanceCounter.flushTask.Start();
		}

		internal static void Terminate()
		{
			if (BufferedPerformanceCounter.flushTask != null)
			{
				BufferedPerformanceCounter.flushTask.Stop();
				BufferedPerformanceCounter.flushTask.Dispose();
				BufferedPerformanceCounter.flushTask = null;
			}
			BufferedPerformanceCounter.FlushCounters();
		}

		internal static void FlushCounters()
		{
			using (LockManager.Lock(BufferedPerformanceCounter.counters))
			{
				foreach (KeyValuePair<BufferedPerformanceCounter, object> keyValuePair in BufferedPerformanceCounter.counters)
				{
					keyValuePair.Key.Flush();
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.currentValue = 0L;
			this.accumulatedValue = 0L;
			this.rawValueWasSet = false;
		}

		public override long IncrementBy(long incrementValue)
		{
			Interlocked.Add(ref this.accumulatedValue, incrementValue);
			return this.currentValue + this.accumulatedValue;
		}

		internal void Flush()
		{
			long num = Interlocked.Exchange(ref this.accumulatedValue, 0L);
			if (this.rawValueWasSet)
			{
				long rawValue = this.currentValue + num;
				base.RawValue = rawValue;
				this.currentValue = rawValue;
				this.rawValueWasSet = false;
				return;
			}
			if (num != 0L)
			{
				this.currentValue = base.IncrementBy(num);
				return;
			}
			this.currentValue = base.RawValue;
		}

		private static ConcurrentDictionary<BufferedPerformanceCounter, object> counters = new ConcurrentDictionary<BufferedPerformanceCounter, object>();

		private static RecurringTask<object> flushTask;

		private long accumulatedValue;

		private long currentValue;

		private bool rawValueWasSet;
	}
}
