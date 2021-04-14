using System;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class LogicalDiskFreeMegabytesTrigger : PerfLogCounterTrigger
	{
		public LogicalDiskFreeMegabytesTrigger(IJob job) : base(job, "LogicalDisk\\(D:\\)\\\\Free Megabytes", new PerfLogCounterTrigger.TriggerConfiguration("LogicalDiskFreeMegabytesTrigger", double.NaN, Configuration.GetConfigDouble("DirectoryLogicalDiskFreeMegabytesTriggerThreshold", double.MinValue, double.MaxValue, 51200.0), TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(15.0), 1))
		{
		}

		protected override void OnThresholdEvent(PerfLogLine line, PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
		}

		private const string ThresholdConfigAttributeName = "DirectoryLogicalDiskFreeMegabytesTriggerThreshold";

		private const double DefaultTriggerThreshold = 51200.0;

		private const string PerfCounterName = "LogicalDisk\\(D:\\)\\\\Free Megabytes";

		private const string TriggerPrefix = "LogicalDiskFreeMegabytesTrigger";
	}
}
