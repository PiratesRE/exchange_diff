using System;
using System.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	public sealed class PerfDateTime
	{
		public PerfDateTime()
		{
			PerfDateTime.EnsureStopwatch();
			this.initialElapsedTicks = PerfDateTime.stopwatch.ElapsedTicks;
			this.initialDateTime = DateTime.UtcNow;
		}

		public DateTime UtcNow
		{
			get
			{
				double value = (double)(PerfDateTime.stopwatch.ElapsedTicks - this.initialElapsedTicks) / PerfDateTime.stopwatchFrequency;
				return this.initialDateTime.AddSeconds(value);
			}
		}

		private static void EnsureStopwatch()
		{
			if (PerfDateTime.stopwatch == null)
			{
				lock (PerfDateTime.stopwatchLock)
				{
					if (PerfDateTime.stopwatch == null)
					{
						PerfDateTime.stopwatch = Stopwatch.StartNew();
						PerfDateTime.stopwatchFrequency = (double)Stopwatch.Frequency;
					}
				}
			}
		}

		private static readonly object stopwatchLock = new object();

		private static Stopwatch stopwatch;

		private static double stopwatchFrequency;

		private readonly long initialElapsedTicks;

		private readonly DateTime initialDateTime;
	}
}
