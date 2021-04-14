using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class PerformanceCounterCheckSetting
	{
		public PerformanceCounterCheckSetting()
		{
			this.MinThreshold = int.MinValue;
			this.MaxThreshold = int.MaxValue;
		}

		public string ReasonToSkip;

		public int MinThreshold;

		public int MaxThreshold;

		public string CategoryName;

		public string CounterName;

		public string InstanceName;
	}
}
