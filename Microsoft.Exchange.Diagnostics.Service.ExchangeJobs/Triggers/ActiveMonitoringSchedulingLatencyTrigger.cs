using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class ActiveMonitoringSchedulingLatencyTrigger : PerInstanceTrigger
	{
		public ActiveMonitoringSchedulingLatencyTrigger(IJob job) : base(job, "MSExchangeWorkerTaskFramework\\(.+?\\)\\\\Scheduling Latency", new PerfLogCounterTrigger.TriggerConfiguration("ActiveMonitoringSchedulingLatencyTrigger", double.NaN, 100.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(60.0), TimeSpan.FromMinutes(5.0), 0), new HashSet<DiagnosticMeasurement>(), ActiveMonitoringSchedulingLatencyTrigger.excludedInstances)
		{
		}

		private const double SchedulingLatencyThreshold = 100.0;

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"_Total"
		};
	}
}
