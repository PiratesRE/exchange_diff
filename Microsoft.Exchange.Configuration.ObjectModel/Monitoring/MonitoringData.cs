using System;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class MonitoringData
	{
		public MonitoringData()
		{
			this.innerEvents = new MonitoringEventCollection();
			this.innerPerformanceCounters = new MonitoringPerformanceCounterCollection();
		}

		public MonitoringEventCollection Events
		{
			get
			{
				return this.innerEvents;
			}
		}

		public MonitoringPerformanceCounterCollection PerformanceCounters
		{
			get
			{
				return this.innerPerformanceCounters;
			}
		}

		private MonitoringEventCollection innerEvents;

		private MonitoringPerformanceCounterCollection innerPerformanceCounters;
	}
}
