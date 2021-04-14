using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public sealed class CASRoutingFailureTrigger : CASTriggerBase
	{
		public CASRoutingFailureTrigger(IJob job) : base(job, "MSExchange HttpProxy Per Site(.+?)\\\\Routing Failure Percentage", new PerfLogCounterTrigger.TriggerConfiguration("CASRoutingFailureTrigger", double.NaN, 50.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(5.0), 0), CASRoutingFailureTrigger.requestsPerSecondMeasurement, 0.4, CASRoutingFailureTrigger.additionalContext, CASRoutingFailureTrigger.excludedInstances)
		{
		}

		private static readonly DiagnosticMeasurement requestsPerSecondMeasurement = DiagnosticMeasurement.GetMeasure("MSExchange HttpProxy Per Site", "Failed Requests/Sec");

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<DiagnosticMeasurement> additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
		{
			CASRoutingFailureTrigger.requestsPerSecondMeasurement
		};
	}
}
