using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class LapTimer
	{
		public void Reset()
		{
			this.lastReading = TimeSpan.Zero;
			this.stopwatch.Restart();
		}

		public TimeSpan GetLapTime()
		{
			TimeSpan elapsed = this.stopwatch.Elapsed;
			TimeSpan result = elapsed - this.lastReading;
			this.lastReading = elapsed;
			return result;
		}

		public TimeSpan GetSplitTime()
		{
			return this.stopwatch.Elapsed;
		}

		private readonly Stopwatch stopwatch = Stopwatch.StartNew();

		private TimeSpan lastReading;
	}
}
