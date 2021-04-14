using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public struct StopwatchStamp
	{
		public static StopwatchStamp GetStamp()
		{
			return new StopwatchStamp(Stopwatch.GetTimestamp());
		}

		private StopwatchStamp(long startTicks)
		{
			this.startTicks = startTicks;
		}

		public TimeSpan ElapsedTime
		{
			get
			{
				return TimeSpan.FromTicks(StopwatchStamp.ToTimeSpanTicks(Stopwatch.GetTimestamp() - this.startTicks));
			}
		}

		public static long ToTimeSpanTicks(long stopwatchTicks)
		{
			return stopwatchTicks * 10000000L / StopwatchStamp.StopwatchTicksPerSecond;
		}

		private const int MillisecondsPerSecond = 1000;

		private const int MicrosecondsPerMillisecond = 1000;

		private const int TimeSpanTicksPerMicrosecond = 10;

		private const int TimeSpanTicksPerSecond = 10000000;

		public static readonly long StopwatchTicksPerSecond = Stopwatch.Frequency;

		private long startTicks;
	}
}
