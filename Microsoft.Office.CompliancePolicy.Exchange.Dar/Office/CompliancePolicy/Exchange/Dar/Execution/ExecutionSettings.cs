using System;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution
{
	internal class ExecutionSettings
	{
		public ExecutionSettings()
		{
			this.CacheSize = 100;
			this.CacheExpiryPeriod = new TimeSpan(0, 30, 0);
			this.PageSize = 0;
			this.MaxTaskExecutionTime = new TimeSpan(1, 0, 0);
			this.MaxTasksQueued = 100;
			this.MaxThreadCount = 100;
			this.MaxTasksDelayCached = 100;
			this.MaxDelayCacheTime = TimeSpan.FromHours(1.0);
		}

		public int CacheSize { get; private set; }

		public TimeSpan CacheExpiryPeriod { get; private set; }

		public int PageSize { get; private set; }

		public TimeSpan MaxTaskExecutionTime { get; private set; }

		public int MaxThreadCount { get; private set; }

		public int MaxTasksQueued { get; private set; }

		public int MaxTasksDelayCached { get; private set; }

		public TimeSpan MaxDelayCacheTime { get; private set; }
	}
}
