using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ADHealthReporter
	{
		public ADHealthReporter(TimeSpan redSuppression, TimeSpan greenSuppression)
		{
			this.maxGreenFreq = greenSuppression;
			this.maxRedFreq = redSuppression;
		}

		public ADHealthReporter() : this(TimeSpan.FromMinutes(2.0), TimeSpan.FromMinutes(5.0))
		{
		}

		public void RaiseRedEvent(string failureMsg)
		{
			if (ExDateTime.Now - this.lastRedEventRaisedTime > this.maxRedFreq || this.lastGreenEventRaisedTime > this.lastRedEventRaisedTime)
			{
				string text = string.Format("ADHealthReporter: AD access is failing: {0}", failureMsg);
				EventNotificationItem eventNotificationItem = new EventNotificationItem("MSExchangeRepl", "MonitoringADConfigManager", "ADConfigQueryStatus", text, text, ResultSeverityLevel.Critical);
				eventNotificationItem.Publish(false);
				this.lastRedEventRaisedTime = ExDateTime.Now;
			}
		}

		public void RaiseGreenEvent()
		{
			if (ExDateTime.Now - this.lastGreenEventRaisedTime > this.maxGreenFreq)
			{
				string text = "ADHealthReporter: AD access is healthy.";
				EventNotificationItem eventNotificationItem = new EventNotificationItem("MSExchangeRepl", "MonitoringADConfigManager", "ADConfigQueryStatus", text, text, ResultSeverityLevel.Informational);
				eventNotificationItem.Publish(false);
				this.lastGreenEventRaisedTime = ExDateTime.Now;
			}
		}

		public const string NotificationItemServiceName = "MSExchangeRepl";

		public const string NotificationItemComponentName = "MonitoringADConfigManager";

		public const string NotificationItemTag = "ADConfigQueryStatus";

		private ExDateTime lastGreenEventRaisedTime = ExDateTime.MinValue;

		private ExDateTime lastRedEventRaisedTime = ExDateTime.MinValue;

		private readonly TimeSpan maxGreenFreq;

		private readonly TimeSpan maxRedFreq;
	}
}
