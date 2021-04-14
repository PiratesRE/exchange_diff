using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class DatabaseLogicalPhysicalSizeRatioTrigger : PerInstanceTrigger
	{
		public DatabaseLogicalPhysicalSizeRatioTrigger(IJob job) : base(job, string.Format("MSExchangeIS Store\\(.+?\\)\\\\{0}", "Logical To Physical Size Ratio"), new PerfLogCounterTrigger.TriggerConfiguration("DatabaseLogicalPhysicalSizeRatioTrigger", double.NaN, 0.9, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(5.0), 1), new HashSet<DiagnosticMeasurement>(), DatabaseLogicalPhysicalSizeRatioTrigger.excludedInstances)
		{
		}

		private const double LogicalPhysicalRatioThreshold = 0.9;

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"_Total"
		};
	}
}
