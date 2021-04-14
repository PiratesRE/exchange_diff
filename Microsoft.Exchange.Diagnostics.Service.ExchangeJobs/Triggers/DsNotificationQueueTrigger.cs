using System;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class DsNotificationQueueTrigger : PerfLogCounterTrigger
	{
		public DsNotificationQueueTrigger(IJob job) : base(job, "DirectoryServices\\(NTDS\\)\\\\DS Notify Queue Size", new PerfLogCounterTrigger.TriggerConfiguration("DsNotificationQueueTrigger", double.NaN, Configuration.GetConfigDouble("DirectoryDSNotificationQueueTriggerThreshold", double.MinValue, double.MaxValue, 250000.0), TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(15.0), 0))
		{
		}

		protected override void OnThresholdEvent(PerfLogLine line, PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
		}

		private const string ThresholdConfigAttributeName = "DirectoryDSNotificationQueueTriggerThreshold";

		private const double DefaultTriggerThreshold = 250000.0;

		private const string PerfCounterName = "DirectoryServices\\(NTDS\\)\\\\DS Notify Queue Size";

		private const string TriggerPrefix = "DsNotificationQueueTrigger";
	}
}
