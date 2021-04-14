using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public sealed class CASRoutingLatencyTrigger : CASTriggerBase
	{
		public CASRoutingLatencyTrigger(IJob job) : base(job, string.Format("MSExchange HttpProxy Per Site(.+?)\\\\Routing Latency {0} Percentile", Configuration.GetConfigString("CASRoutingLatencyTriggerPercentile", "80th")), new PerfLogCounterTrigger.TriggerConfiguration("CASRoutingLatencyTrigger", double.NaN, 3000.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(5.0), 0), CASRoutingLatencyTrigger.requestsPerSecondMeasurement, 0.4, CASRoutingLatencyTrigger.additionalContext, CASRoutingLatencyTrigger.excludedInstances)
		{
		}

		private const string TriggerPrefix = "CASRoutingLatencyTrigger";

		private static readonly DiagnosticMeasurement requestsPerSecondMeasurement = DiagnosticMeasurement.GetMeasure("MSExchange HttpProxy Per Site", "Proxy Requests with Latency Data/Sec");

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Unknown"
		};

		private static readonly HashSet<DiagnosticMeasurement> additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
		{
			CASRoutingLatencyTrigger.requestsPerSecondMeasurement
		};
	}
}
