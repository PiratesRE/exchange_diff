using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data
{
	public class BucketizedTimeCounter : ITimerCounter, IDisposable
	{
		public BucketizedTimeCounter(Func<string, ExPerformanceCounter> getInstance, SortedSet<TimeSpan> divisions, bool autoStart = true)
		{
			if (getInstance == null)
			{
				throw new ArgumentNullException("getInstance");
			}
			if (divisions == null || divisions.Count == 0)
			{
				throw new ArgumentNullException("divisions");
			}
			if (divisions.Any((TimeSpan d) => d == TimeSpan.Zero))
			{
				throw new ArgumentException("TimeSpan.Zero may not be used as a bucket division.");
			}
			this.getInstance = getInstance;
			this.divisions = divisions;
			if (autoStart)
			{
				this.Start();
			}
		}

		public void Start()
		{
			this.stopwatch = Stopwatch.StartNew();
		}

		public long Stop()
		{
			long elapsedTicks = this.stopwatch.ElapsedTicks;
			this.AddSample(this.stopwatch.Elapsed);
			this.stopwatch = null;
			return elapsedTicks;
		}

		void IDisposable.Dispose()
		{
			if (this.stopwatch != null)
			{
				this.Stop();
			}
		}

		internal void AddSample(TimeSpan elapsed)
		{
			TimeSpan timeSpan = this.divisions.LastOrDefault((TimeSpan l) => l <= elapsed);
			TimeSpan timeSpan2 = this.divisions.FirstOrDefault((TimeSpan u) => u > elapsed);
			if (timeSpan == TimeSpan.Zero && timeSpan2 != TimeSpan.Zero)
			{
				this.getInstance(string.Format("< {0}", timeSpan2)).Increment();
				return;
			}
			if (timeSpan != TimeSpan.Zero && timeSpan2 != TimeSpan.Zero)
			{
				this.getInstance(string.Format("[{0}, {1})", timeSpan, timeSpan2)).Increment();
				return;
			}
			if (timeSpan != TimeSpan.Zero && timeSpan2 == TimeSpan.Zero)
			{
				this.getInstance(string.Format(">= {0}", timeSpan)).Increment();
			}
		}

		private Stopwatch stopwatch;

		private Func<string, ExPerformanceCounter> getInstance;

		private SortedSet<TimeSpan> divisions;
	}
}
