using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics.Performance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct StopwatchPerformanceTracker : IDisposable
	{
		public StopwatchPerformanceTracker(string marker, IPerformanceDataLogger logger)
		{
			if (string.IsNullOrEmpty(marker))
			{
				throw new ArgumentNullException("marker");
			}
			this.marker = marker;
			this.logger = (logger ?? NullPerformanceDataLogger.Instance);
			this.stopwatch = Stopwatch.StartNew();
		}

		public void Dispose()
		{
			this.Stop();
		}

		public void Stop()
		{
			this.stopwatch.Stop();
			this.logger.Log(this.marker, "ElapsedTime", this.stopwatch.Elapsed);
		}

		public const string ElapsedTime = "ElapsedTime";

		private readonly string marker;

		private readonly IPerformanceDataLogger logger;

		private readonly Stopwatch stopwatch;
	}
}
