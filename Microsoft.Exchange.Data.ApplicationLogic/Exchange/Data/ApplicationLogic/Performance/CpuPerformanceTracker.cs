using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.ApplicationLogic.Performance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct CpuPerformanceTracker : IDisposable
	{
		public CpuPerformanceTracker(string marker, IPerformanceDataLogger logger)
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
			this.beginThreadTimes = ThreadTimes.GetFromCurrentThread();
		}

		public void Dispose()
		{
			ThreadTimes fromCurrentThread = ThreadTimes.GetFromCurrentThread();
			this.logger.Log(this.marker, "CpuTime", fromCurrentThread.Kernel - this.beginThreadTimes.Kernel + fromCurrentThread.User - this.beginThreadTimes.User);
		}

		public const string CpuTime = "CpuTime";

		private readonly string marker;

		private readonly IPerformanceDataLogger logger;

		private readonly ThreadTimes beginThreadTimes;
	}
}
