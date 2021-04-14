using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.Performance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct ADPerformanceTracker : IDisposable
	{
		public ADPerformanceTracker(string marker, IPerformanceDataLogger logger)
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
			this.startSnapshot = PerformanceContext.Current.TakeSnapshot(true);
		}

		public void Dispose()
		{
			PerformanceData performanceData = PerformanceContext.Current.TakeSnapshot(false) - this.startSnapshot;
			this.logger.Log(this.marker, "LdapLatency", performanceData.Latency);
			this.logger.Log(this.marker, "LdapCount", performanceData.Count);
		}

		public const string LdapCount = "LdapCount";

		public const string LdapLatency = "LdapLatency";

		private readonly string marker;

		private readonly IPerformanceDataLogger logger;

		private readonly PerformanceData startSnapshot;
	}
}
