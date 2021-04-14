using System;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class OutStandingATQRequestsTrigger : PerfLogCounterTrigger
	{
		public OutStandingATQRequestsTrigger(IJob job) : base(job, "DirectoryServices\\(NTDS\\)\\\\ATQ Outstanding Queued Requests", new PerfLogCounterTrigger.TriggerConfiguration("OutStandingATQRequestsTrigger", double.NaN, Configuration.GetConfigDouble("DirectoryOutstandingATQRequestsTriggerThreshold", double.MinValue, double.MaxValue, 2000.0), TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(15.0), 0))
		{
		}

		protected override void OnThresholdEvent(PerfLogLine line, PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
		}

		private const string ThresholdConfigAttributeName = "DirectoryOutstandingATQRequestsTriggerThreshold";

		private const double DefaultTriggerThreshold = 2000.0;

		private const string PerfCounterName = "DirectoryServices\\(NTDS\\)\\\\ATQ Outstanding Queued Requests";

		private const string TriggerPrefix = "OutStandingATQRequestsTrigger";
	}
}
