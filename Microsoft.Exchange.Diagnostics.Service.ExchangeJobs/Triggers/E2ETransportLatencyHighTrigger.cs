using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class E2ETransportLatencyHighTrigger : TransportOverThresholdGatedTrigger
	{
		public E2ETransportLatencyHighTrigger(IJob job) : base(job, "MSExchangeTransport End To End Latency\\(total - high\\)\\\\Percentile80$", 50.0, new PerfLogCounterTrigger.TriggerConfiguration("E2ETransportLatencyHighTrigger", 45.0, 90.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), 0), E2ETransportLatencyHighTrigger.additionalContext, E2ETransportLatencyHighTrigger.excludedInstances)
		{
		}

		protected override DiagnosticMeasurement AdditionalDiagnosticMeasurement(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			return DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, E2ETransportLatencyHighTrigger.gatingCounter.ObjectName, E2ETransportLatencyHighTrigger.gatingCounter.CounterName, E2ETransportLatencyHighTrigger.gatingCounter.InstanceName);
		}

		private static readonly DiagnosticMeasurement gatingCounter = DiagnosticMeasurement.GetMeasure("MSExchangeTransport End To End Latency", "Percentile80Samples", "Total - High");

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<DiagnosticMeasurement> additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
		{
			E2ETransportLatencyHighTrigger.gatingCounter
		};
	}
}
