using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct StorePerformanceTracker : IDisposable
	{
		public StorePerformanceTracker(string marker, IPerformanceDataLogger logger)
		{
			if (string.IsNullOrEmpty(marker))
			{
				throw new ArgumentNullException("marker");
			}
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			this.marker = marker;
			this.logger = logger;
			this.startSnapshot = RpcDataProvider.Instance.TakeSnapshot(true);
		}

		public void Dispose()
		{
			PerformanceData performanceData = RpcDataProvider.Instance.TakeSnapshot(false) - this.startSnapshot;
			this.logger.Log(this.marker, "StoreRpcLatency", performanceData.Latency);
			this.logger.Log(this.marker, "StoreRpcCount", performanceData.Count);
		}

		public const string StoreRpcCount = "StoreRpcCount";

		public const string StoreRpcLatency = "StoreRpcLatency";

		private readonly string marker;

		private readonly IPerformanceDataLogger logger;

		private readonly PerformanceData startSnapshot;
	}
}
