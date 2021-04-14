using System;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class DraPendingReplicationTrigger : PerfLogCounterTrigger
	{
		public DraPendingReplicationTrigger(IJob job) : base(job, "DirectoryServices\\(NTDS\\)\\\\DRA Pending Replication Operations", new PerfLogCounterTrigger.TriggerConfiguration("DraPendingReplicationTrigger", double.NaN, Configuration.GetConfigDouble("DirectoryDRAPendingReplicationTriggerThreshold", double.MinValue, double.MaxValue, 50.0), TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(30.0), 0))
		{
		}

		protected override void OnThresholdEvent(PerfLogLine line, PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
		}

		private const string ThresholdConfigAttributeName = "DirectoryDRAPendingReplicationTriggerThreshold";

		private const double DefaultTriggerThreshold = 50.0;

		private const string PerfCounterName = "DirectoryServices\\(NTDS\\)\\\\DRA Pending Replication Operations";

		private const string TriggerPrefix = "DraPendingReplicationTrigger";
	}
}
