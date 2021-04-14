using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class StoreNumberOfActiveBackgroundTasksTrigger : PerInstanceTrigger
	{
		public StoreNumberOfActiveBackgroundTasksTrigger(IJob job) : base(job, "MSExchangeIS Store\\(.+?\\)\\\\Number of active background tasks", new PerfLogCounterTrigger.TriggerConfiguration("StoreNumberOfActiveBackgroundTasksTrigger", double.NaN, 15.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(5.0), 0), new HashSet<DiagnosticMeasurement>(), StoreNumberOfActiveBackgroundTasksTrigger.excludedInstances)
		{
		}

		private const double NumberOfActiveBackgroundTasksThreshold = 15.0;

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"_Total"
		};
	}
}
