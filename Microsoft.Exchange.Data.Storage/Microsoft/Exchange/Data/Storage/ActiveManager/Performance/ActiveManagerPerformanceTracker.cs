using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.Storage.ActiveManager.Performance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct ActiveManagerPerformanceTracker : IDisposable
	{
		public ActiveManagerPerformanceTracker(string marker, IPerformanceDataLogger logger)
		{
			ArgumentValidator.ThrowIfNull("marker", marker);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.marker = marker;
			this.logger = logger;
			this.initialSnapshots = new PerformanceData[ActiveManagerPerformanceData.Providers.Length];
			for (int i = 0; i < ActiveManagerPerformanceData.Providers.Length; i++)
			{
				this.initialSnapshots[i] = ActiveManagerPerformanceData.Providers[i].Provider.TakeSnapshot(true);
			}
		}

		public void Dispose()
		{
			for (int i = 0; i < ActiveManagerPerformanceData.Providers.Length; i++)
			{
				PerformanceData pd = ActiveManagerPerformanceData.Providers[i].Provider.TakeSnapshot(false);
				PerformanceData performanceData = pd - this.initialSnapshots[i];
				this.logger.Log(this.marker, ActiveManagerPerformanceData.Providers[i].LogCount, performanceData.Count);
				this.logger.Log(this.marker, ActiveManagerPerformanceData.Providers[i].LogLatency, performanceData.Latency);
			}
		}

		private readonly string marker;

		private readonly IPerformanceDataLogger logger;

		private readonly PerformanceData[] initialSnapshots;
	}
}
