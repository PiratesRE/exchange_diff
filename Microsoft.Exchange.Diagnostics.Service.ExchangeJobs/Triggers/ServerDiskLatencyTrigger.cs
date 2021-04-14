using System;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class ServerDiskLatencyTrigger : PerfLogCounterTrigger
	{
		public ServerDiskLatencyTrigger(IJob job) : base(job, "Logical Disk\\(_Total\\)\\\\Avg\\. Disk sec/Write", new PerfLogCounterTrigger.TriggerConfiguration("ServerDiskLatencyTrigger", double.NaN, 0.01, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromHours(24.0), 0))
		{
		}

		protected override void OnThresholdEvent(PerfLogLine line, PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
		}

		private const double TriggerThreshold = 0.01;

		private const string PerfCounterName = "Logical Disk\\(_Total\\)\\\\Avg\\. Disk sec/Write";

		private const string TriggerPrefix = "ServerDiskLatencyTrigger";
	}
}
