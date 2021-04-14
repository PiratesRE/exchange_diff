using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public class MonitoringPattern
	{
		public MonitoringPattern(int recurrance = 120, int monitoringInterval = 600, int monitoringThreshold = 4, int timeout = 120)
		{
			this.recurrenceInSeconds = recurrance;
			this.monitoringIntervalInSeconds = monitoringInterval;
			this.monitoringThreshold = monitoringThreshold;
			if (timeout < recurrance)
			{
				this.timeoutInSeconds = timeout;
				return;
			}
			this.timeoutInSeconds = recurrance;
		}

		public int RecurrenceInSeconds
		{
			get
			{
				return this.recurrenceInSeconds;
			}
			set
			{
				this.recurrenceInSeconds = value;
			}
		}

		public int TimeoutInSeconds
		{
			get
			{
				return this.timeoutInSeconds;
			}
			set
			{
				this.timeoutInSeconds = value;
			}
		}

		public int MonitoringIntervalInSeconds
		{
			get
			{
				return this.monitoringIntervalInSeconds;
			}
			set
			{
				this.monitoringIntervalInSeconds = value;
			}
		}

		public int MonitoringThreshold
		{
			get
			{
				return this.monitoringThreshold;
			}
			set
			{
				this.monitoringThreshold = value;
			}
		}

		private int recurrenceInSeconds = 120;

		private int timeoutInSeconds = 120;

		private int monitoringIntervalInSeconds = 300;

		private int monitoringThreshold = 2;
	}
}
